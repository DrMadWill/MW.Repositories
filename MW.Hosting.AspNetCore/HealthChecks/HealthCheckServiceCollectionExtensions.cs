using Microsoft.Extensions.DependencyInjection;

namespace MW.Hosting.AspNetCore.HealthChecks;

public static class HealthCheckServiceCollectionExtensions
{
    public static IServiceCollection AddDefaultHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks();
        return services;
    }
}
