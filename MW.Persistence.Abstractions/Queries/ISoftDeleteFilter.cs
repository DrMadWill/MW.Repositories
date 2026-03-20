namespace MW.Persistence.Abstractions.Queries;

/// <summary>
/// Represents a filter contract for soft-delete-aware querying.
/// This is an optional contract — not all repositories are required to support soft delete.
/// </summary>
public interface ISoftDeleteFilter
{
    /// <summary>
    /// Gets a value indicating whether soft-deleted entities should be included in query results.
    /// </summary>
    bool IncludeDeleted { get; }

    /// <summary>
    /// Gets a value indicating whether only soft-deleted entities should be returned.
    /// </summary>
    bool OnlyDeleted { get; }
}
