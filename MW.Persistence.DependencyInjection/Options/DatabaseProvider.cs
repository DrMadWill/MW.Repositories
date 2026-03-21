namespace MW.Persistence.DependencyInjection.Options;

/// <summary>
/// Represents the supported database providers for EF Core persistence registration.
/// </summary>
public enum DatabaseProvider
{
    /// <summary>
    /// Microsoft SQL Server provider.
    /// </summary>
    SqlServer,

    /// <summary>
    /// PostgreSQL provider via Npgsql.
    /// </summary>
    PostgreSql
}
