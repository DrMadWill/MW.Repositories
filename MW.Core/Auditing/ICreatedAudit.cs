namespace MW.Core.Auditing;

/// <summary>
/// Represents an entity that tracks who created it.
/// </summary>
public interface ICreatedAudit
{
    /// <summary>
    /// Gets or sets the identifier of the user who created the entity.
    /// </summary>
    Guid? CreatedBy { get; set; }
}
