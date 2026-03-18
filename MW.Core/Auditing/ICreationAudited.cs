namespace MW.Core.Auditing;

/// <summary>
/// Represents an entity that tracks creation time.
/// </summary>
public interface ICreationAudited
{
    /// <summary>
    /// Gets or sets the date and time (UTC) when the entity was created.
    /// </summary>
    DateTimeOffset CreatedAt { get; set; }
}
