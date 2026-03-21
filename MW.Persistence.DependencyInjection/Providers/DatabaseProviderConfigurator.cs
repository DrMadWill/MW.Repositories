using Microsoft.EntityFrameworkCore;
using MW.Persistence.DependencyInjection.Options;

namespace MW.Persistence.DependencyInjection.Providers;

/// <summary>
/// Provides database provider configuration for EF Core <see cref="DbContextOptionsBuilder"/>.
/// Maps <see cref="PersistenceOptions"/> to the appropriate provider-specific configuration.
/// </summary>
internal static class DatabaseProviderConfigurator
{
    /// <summary>
    /// Configures the <see cref="DbContextOptionsBuilder"/> with the appropriate database provider
    /// based on the specified <see cref="PersistenceOptions"/>.
    /// </summary>
    /// <param name="builder">The DbContext options builder to configure.</param>
    /// <param name="options">The persistence options containing provider and connection information.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an unsupported provider is specified.</exception>
    internal static void Configure(DbContextOptionsBuilder builder, PersistenceOptions options)
    {
        switch (options.Provider)
        {
            case DatabaseProvider.SqlServer:
                ConfigureSqlServer(builder, options);
                break;
            case DatabaseProvider.PostgreSql:
                ConfigurePostgreSql(builder, options);
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(options.Provider),
                    options.Provider,
                    "Unsupported database provider.");
        }
    }

    private static void ConfigureSqlServer(DbContextOptionsBuilder builder, PersistenceOptions options)
    {
        builder.UseSqlServer(options.ConnectionString, sqlOptions =>
        {
            if (!string.IsNullOrWhiteSpace(options.MigrationAssembly))
                sqlOptions.MigrationsAssembly(options.MigrationAssembly);
        });
    }

    private static void ConfigurePostgreSql(DbContextOptionsBuilder builder, PersistenceOptions options)
    {
        builder.UseNpgsql(options.ConnectionString, npgsqlOptions =>
        {
            if (!string.IsNullOrWhiteSpace(options.MigrationAssembly))
                npgsqlOptions.MigrationsAssembly(options.MigrationAssembly);
        });
    }
}
