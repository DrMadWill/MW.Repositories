namespace MW.Core.Models.Abstractions;

/// <summary>
/// Represents a soft-deletable entity.
/// For richer soft delete support with metadata, use <see cref="MW.Core.Entities.ISoftDelete"/>.
/// </summary>
public interface IHasDelete
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is marked as deleted.
    /// </summary>
    bool IsDeleted { get; set; }
}