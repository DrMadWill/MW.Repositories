namespace MW.Core.Auditing;

/// <summary>
/// Represents an entity that tracks who deleted it and when.
/// </summary>
public interface IDeletedAudit
{
    /// <summary>
    /// Gets or sets the identifier of the user who deleted the entity.
    /// </summary>
    Guid? DeletedBy { get; set; }

    /// <summary>
    /// Gets or sets the date and time (UTC) when the entity was deleted.
    /// </summary>
    DateTimeOffset? DeletedAt { get; set; }
}
