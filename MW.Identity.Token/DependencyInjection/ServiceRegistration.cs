using Microsoft.Extensions.DependencyInjection;
using MW.Identity.Token.Contracts;
using MW.Identity.Token.Services;

namespace MW.Identity.Token.DependencyInjection;

/// <summary>
/// Extension methods for registering identity token services in the dependency injection container.
/// </summary>
public static class ServiceRegistration
{
    /// <summary>
    /// Adds the <see cref="ICurrentUser"/> service and its dependencies to the service collection.
    /// Registers <see cref="IHttpContextAccessor"/> and <see cref="CurrentUser"/> as a scoped service.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    public static void AddUserTokenManager(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
    }
}
