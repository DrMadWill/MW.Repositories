namespace MW.Core.Auditing;

/// <summary>
/// Represents an entity that tracks who last updated it.
/// </summary>
public interface IUpdatedAudit
{
    /// <summary>
    /// Gets or sets the identifier of the user who last updated the entity.
    /// </summary>
    Guid? UpdatedBy { get; set; }
}
