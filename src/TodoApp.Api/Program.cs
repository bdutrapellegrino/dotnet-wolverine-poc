using FluentValidation;
using JasperFx;
using JasperFx.Resources;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TodoApp.Api.Auth;
using TodoApp.Application;
using TodoApp.Infrastructure;
using TodoApp.Infrastructure.Persistence;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.FluentValidation;
using Wolverine.Http;
using Wolverine.Http.FluentValidation;
using Wolverine.Postgresql;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Todo")
    ?? "Host=localhost;Port=5432;Database=todo;Username=todo;Password=todo";

// --- Wolverine (CQRS / messaging runtime) ---
builder.Host.UseWolverine(opts =>
{
    // Command/query handlers live in the Application assembly, so point discovery there.
    opts.Discovery.IncludeAssembly(typeof(AssemblyMarker).Assembly);

    // Durable transactional inbox/outbox stored in Postgres.
    opts.PersistMessagesWithPostgresql(connectionString);

    // Teach Wolverine about EF Core so it can resolve the DbContext in generated handler
    // code and wrap handlers in an EF transaction.
    opts.UseEntityFrameworkCoreTransactions();

    // Auto-commit: Wolverine calls SaveChangesAsync and flushes the outbox for every handler,
    // so handlers never call SaveChanges themselves.
    opts.Policies.AutoApplyTransactions();

    // Validate commands/queries as message middleware on EVERY dispatch through IMessageBus
    // (HTTP, queue, cron, tests). The HTTP-boundary validation on the Request still gives the
    // clean 400; this guards non-HTTP entry points.
    opts.UseFluentValidation();

    // Route local (in-process) messages like TodoCompleted through the durable outbox.
    opts.Policies.UseDurableLocalQueues();
});

// --- Application services (composition root) ---
// Register the DbContext with Wolverine's integration so handlers can take it without
// triggering service location during code generation, and so the outbox can enlist it.
builder.Services.AddDbContextWithWolverineIntegration<TodoDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddInfrastructure();

// Create Wolverine's message storage tables (and other stateful resources) on startup.
builder.Services.AddResourceSetupOnStartup();

// Request validators live in the API assembly (validated at the HTTP boundary, FastEndpoints-style);
// command validators live in the Application assembly (validated on the message bus).
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddValidatorsFromAssemblyContaining<AssemblyMarker>();

// Wolverine.Http endpoint support.
builder.Services.AddWolverineHttp();

// OAuth / JWT Bearer scaffolding — stays off unless Authentication:Enabled = true.
builder.Services.AddTodoAuthentication(builder.Configuration);

// OpenAPI document + Scalar UI for exploring the API.
builder.Services.AddOpenApi();

var app = builder.Build();

// Apply EF Core migrations on startup (fine for a POC; use a migration step in real deploys).
using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<TodoDbContext>().Database.MigrateAsync();
}

app.UseTodoAuthentication();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapWolverineEndpoints(opts =>
{
    // Validate HTTP request bodies (the commands) with FluentValidation -> 400 ProblemDetails.
    opts.UseFluentValidationProblemDetailMiddleware();

    // Answer 400 ProblemDetails for unparseable query values (MVC/minimal-API parity).
    opts.RejectUnparseableQueryValues = true;

    // When OAuth is enabled, require an authenticated user on every endpoint.
    if (app.Configuration.IsAuthEnabled())
    {
        opts.RequireAuthorizeOnAll();
    }
});

app.MapGet("/", () => Results.Redirect("/scalar"));

return await app.RunJasperFxCommands(args);
