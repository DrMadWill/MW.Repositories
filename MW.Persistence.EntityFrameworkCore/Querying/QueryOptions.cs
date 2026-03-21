using MW.Persistence.Abstractions.Queries;

namespace MW.Persistence.EntityFrameworkCore.Querying;

/// <summary>
/// Default implementation of <see cref="IQueryOptions"/>.
/// Provides intention-based query options that control non-business query behaviors
/// such as change tracking, global query filters, and soft-delete inclusion.
/// </summary>
public class QueryOptions : IQueryOptions
{
    /// <inheritdoc />
    public bool AsNoTracking { get; init; } = true;

    /// <inheritdoc />
    public bool IgnoreQueryFilters { get; init; }

    /// <inheritdoc />
    public bool IncludeSoftDeleted { get; init; }

    /// <summary>
    /// Gets the default query options (no tracking, respect query filters, exclude soft-deleted).
    /// </summary>
    public static QueryOptions Default => new();

    /// <summary>
    /// Gets query options that enable change tracking (for update scenarios).
    /// </summary>
    public static QueryOptions Tracked => new() { AsNoTracking = false };
}
