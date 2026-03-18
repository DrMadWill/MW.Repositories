namespace MW.Core.Entities;

/// <summary>
/// Represents a soft-deletable entity with deletion metadata.
/// </summary>
public interface ISoftDelete
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is marked as deleted.
    /// </summary>
    bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was deleted.
    /// </summary>
    DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who deleted the entity.
    /// </summary>
    Guid? DeletedBy { get; set; }
}
