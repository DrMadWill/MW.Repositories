using MW.Persistence.Abstractions.Queries;

namespace MW.Persistence.EntityFrameworkCore.Querying;

/// <summary>
/// Default implementation of <see cref="ISoftDeleteFilter"/>.
/// Controls soft-delete-aware querying behavior for repositories that support it.
/// </summary>
public class SoftDeleteFilter : ISoftDeleteFilter
{
    /// <inheritdoc />
    public bool IncludeDeleted { get; init; }

    /// <inheritdoc />
    public bool OnlyDeleted { get; init; }

    /// <summary>
    /// Gets a filter that excludes soft-deleted entities (default behavior).
    /// </summary>
    public static SoftDeleteFilter Default => new();

    /// <summary>
    /// Gets a filter that includes both active and soft-deleted entities.
    /// </summary>
    public static SoftDeleteFilter WithDeleted => new() { IncludeDeleted = true };

    /// <summary>
    /// Gets a filter that returns only soft-deleted entities.
    /// </summary>
    public static SoftDeleteFilter DeletedOnly => new() { OnlyDeleted = true };
}
