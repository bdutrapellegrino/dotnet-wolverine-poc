using Microsoft.Extensions.DependencyInjection;

namespace TodoApp.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers infrastructure services other than the DbContext — the DbContext itself is
    /// registered by the API composition root with Wolverine's EF Core integration
    /// (AddDbContextWithWolverineIntegration), which enables the transactional outbox.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        return services;
    }
}
