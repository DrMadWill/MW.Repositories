using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MW.Persistence.DependencyInjection.Options;

/// <summary>
/// Represents the configuration options for persistence registration.
/// Used to configure DbContext, provider, and behavioral settings when calling
/// <c>AddEfCorePersistence</c>.
/// </summary>
public class PersistenceOptions
{
    /// <summary>
    /// Gets or sets the database connection string.
    /// This value is required for provider registration.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the database provider to use.
    /// Determines which EF Core provider is configured (e.g., SQL Server, PostgreSQL).
    /// </summary>
    public DatabaseProvider Provider { get; set; } = DatabaseProvider.SqlServer;

    /// <summary>
    /// Gets or sets the migration assembly name.
    /// If not specified, the DbContext assembly is used by default.
    /// </summary>
    public string? MigrationAssembly { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether sensitive data logging is enabled.
    /// When <c>true</c>, EF Core includes parameter values in log messages.
    /// Should only be enabled in development environments.
    /// </summary>
    public bool EnableSensitiveDataLogging { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether detailed error messages are enabled.
    /// When <c>true</c>, EF Core provides more detailed error information.
    /// Should only be enabled in development environments.
    /// </summary>
    public bool EnableDetailedErrors { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the DbContext health check should be registered.
    /// When <c>true</c>, an EF Core health check is added for the registered DbContext.
    /// </summary>
    public bool EnableHealthCheck { get; set; }

    /// <summary>
    /// Gets or sets the name used for the health check registration.
    /// Defaults to <c>"persistence"</c> if not specified.
    /// </summary>
    public string HealthCheckName { get; set; } = "persistence";

    /// <summary>
    /// Gets the list of EF Core interceptors to register with the DbContext.
    /// Use <see cref="AddInterceptor"/> to add interceptors.
    /// </summary>
    public IList<IInterceptor> Interceptors { get; } = new List<IInterceptor>();

    /// <summary>
    /// Gets or sets a value indicating whether EF Core command logging should be suppressed.
    /// When <c>true</c>, both <see cref="Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuted"/>
    /// and <see cref="Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuting"/>
    /// events are ignored, reducing log noise from SQL command output.
    /// </summary>
    public bool IgnoreCommandLog { get; set; }

    /// <summary>
    /// Adds an EF Core interceptor to the DbContext configuration.
    /// Interceptors can be used for auditing, logging, soft delete behavior, and domain event hooks.
    /// </summary>
    /// <param name="interceptor">The interceptor to add.</param>
    /// <returns>The current <see cref="PersistenceOptions"/> instance for chaining.</returns>
    public PersistenceOptions AddInterceptor(IInterceptor interceptor)
    {
        Interceptors.Add(interceptor ?? throw new ArgumentNullException(nameof(interceptor)));
        return this;
    }
}
