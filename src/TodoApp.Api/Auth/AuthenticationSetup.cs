using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TodoApp.Api.Auth;

/// <summary>
/// OAuth2 / OpenID Connect (JWT Bearer) scaffolding. It is wired up but stays
/// OFF until <c>Authentication:Enabled</c> is set to <c>true</c> in configuration.
///
/// To turn it on for a real identity provider (Keycloak, Auth0, Entra ID, ...),
/// set in appsettings / user-secrets:
///   "Authentication": {
///     "Enabled": true,
///     "Authority": "https://your-idp/realms/todo",
///     "Audience": "todo-api"
///   }
/// Then call <c>app.UseTodoAuthentication()</c> (already wired) and the
/// MapWolverineEndpoints policy will require an authenticated user on every route.
/// </summary>
public static class AuthenticationSetup
{
    public static bool IsAuthEnabled(this IConfiguration config)
        => config.GetValue("Authentication:Enabled", false);

    public static IServiceCollection AddTodoAuthentication(
        this IServiceCollection services,
        IConfiguration config)
    {
        if (!config.IsAuthEnabled())
        {
            return services;
        }

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = config["Authentication:Authority"];
                options.Audience = config["Authentication:Audience"];
                // In production keep this true; relax only against a trusted local IdP over http.
                options.RequireHttpsMetadata = config.GetValue("Authentication:RequireHttpsMetadata", true);
            });

        services.AddAuthorization();

        return services;
    }

    public static WebApplication UseTodoAuthentication(this WebApplication app)
    {
        if (!app.Configuration.IsAuthEnabled())
        {
            return app;
        }

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
