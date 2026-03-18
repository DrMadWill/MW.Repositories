namespace MW.Core.Auditing;

/// <summary>
/// Represents an entity that tracks who deleted it.
/// </summary>
public interface IDeletedAudit
{
    /// <summary>
    /// Gets or sets the identifier of the user who deleted the entity.
    /// </summary>
    Guid? DeletedBy { get; set; }
}
