using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using MW.Persistence.DependencyInjection.Options;
using MW.Persistence.DependencyInjection.Providers;
using MW.Persistence.DependencyInjection.Registrations;

namespace MW.Persistence.DependencyInjection.Extensions;

/// <summary>
/// Provides extension methods on <see cref="IServiceCollection"/> for registering
/// EF Core persistence services including DbContext, repositories, UnitOfWork, and transaction manager.
/// This is the primary entry point for persistence composition in microservices.
/// </summary>
public static class PersistenceServiceCollectionExtensions
{
    /// <summary>
    /// Registers EF Core persistence services into the dependency injection container.
    /// <para>
    /// This method registers:
    /// <list type="bullet">
    ///   <item><description>DbContext with the configured database provider (SQL Server or PostgreSQL)</description></item>
    ///   <item><description>Generic repository implementations (read, write, combined, aggregate, projection)</description></item>
    ///   <item><description>Unit of Work implementation</description></item>
    ///   <item><description>Transaction manager implementation</description></item>
    ///   <item><description>Optional EF Core interceptors</description></item>
    ///   <item><description>Optional health check for database connectivity</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// All persistence services are registered with <b>scoped</b> lifetime to align with DbContext lifecycle.
    /// </para>
    /// </summary>
    /// <typeparam name="TDbContext">The type of the EF Core DbContext to register.</typeparam>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configure">A delegate to configure <see cref="PersistenceOptions"/>.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="configure"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the connection string is not configured.</exception>
    /// <example>
    /// <code>
    /// services.AddEfCorePersistence&lt;AppDbContext&gt;(options =>
    /// {
    ///     options.ConnectionString = configuration.GetConnectionString("Default")!;
    ///     options.Provider = DatabaseProvider.PostgreSql;
    ///     options.MigrationAssembly = "MyApp.Infrastructure";
    ///     options.EnableSensitiveDataLogging = builder.Environment.IsDevelopment();
    ///     options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    ///     options.EnableHealthCheck = true;
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddEfCorePersistence<TDbContext>(
        this IServiceCollection services,
        Action<PersistenceOptions> configure)
        where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        var options = new PersistenceOptions();
        configure(options);

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
            throw new InvalidOperationException("ConnectionString must be configured in PersistenceOptions.");

        // Register DbContext with the configured provider and options
        services.AddDbContext<TDbContext>((_, builder) =>
        {
            DatabaseProviderConfigurator.Configure(builder, options);

            if (options.EnableSensitiveDataLogging)
                builder.EnableSensitiveDataLogging();

            if (options.EnableDetailedErrors)
                builder.EnableDetailedErrors();
            
            if (options.IgnoreCommandLog)
                builder.ConfigureWarnings(w =>
                    w.Ignore(RelationalEventId.CommandExecuted)
                        .Ignore(RelationalEventId.CommandExecuting));

            if (options.Interceptors.Count > 0)
                builder.AddInterceptors(options.Interceptors);
        });

        // Register DbContext as the base DbContext type so repository implementations can resolve it
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<TDbContext>());

        // Register all persistence services (repositories, UnitOfWork, transaction manager)
        PersistenceServiceRegistrar.RegisterServices<TDbContext>(services);

        // Register health check if enabled
        if (options.EnableHealthCheck)
        {
            services.AddHealthChecks()
                .AddDbContextCheck<TDbContext>(options.HealthCheckName);
        }

        return services;
    }
}
