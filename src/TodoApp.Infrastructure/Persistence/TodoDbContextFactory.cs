using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TodoApp.Infrastructure.Persistence;

/// <summary>
/// Design-time factory used by `dotnet ef` so the tooling can build the DbContext
/// without booting the whole application host (which runs JasperFx/Wolverine commands).
/// </summary>
public class TodoDbContextFactory : IDesignTimeDbContextFactory<TodoDbContext>
{
    public TodoDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("TODO_CONNECTIONSTRING")
            ?? "Host=localhost;Port=5432;Database=todo;Username=todo;Password=todo";

        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new TodoDbContext(options);
    }
}
