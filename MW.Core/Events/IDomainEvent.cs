namespace MW.Core.Events;

/// <summary>
/// Marker interface for domain events.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the date and time (UTC) when the event occurred.
    /// </summary>
    DateTimeOffset OccurredOn { get; }
}
