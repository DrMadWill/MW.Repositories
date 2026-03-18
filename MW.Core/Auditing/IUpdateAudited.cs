namespace MW.Core.Auditing;

/// <summary>
/// Represents an entity that tracks last update time.
/// </summary>
public interface IUpdateAudited
{
    /// <summary>
    /// Gets or sets the date and time (UTC) when the entity was last updated.
    /// </summary>
    DateTimeOffset? UpdatedAt { get; set; }
}
