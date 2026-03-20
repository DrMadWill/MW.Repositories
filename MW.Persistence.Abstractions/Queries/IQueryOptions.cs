namespace MW.Persistence.Abstractions.Queries;

/// <summary>
/// Represents reusable, intention-based query options that control non-business query behaviors.
/// Infrastructure implementations map these options to provider-specific behaviors (e.g., EF Core).
/// </summary>
public interface IQueryOptions
{
    /// <summary>
    /// Gets a value indicating whether change tracking should be disabled.
    /// When <c>true</c>, the query results are not tracked by the persistence context.
    /// </summary>
    bool AsNoTracking { get; }

    /// <summary>
    /// Gets a value indicating whether global query filters should be ignored.
    /// </summary>
    bool IgnoreQueryFilters { get; }

    /// <summary>
    /// Gets a value indicating whether soft-deleted entities should be included in the results.
    /// </summary>
    bool IncludeSoftDeleted { get; }
}
