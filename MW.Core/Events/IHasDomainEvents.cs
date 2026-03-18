namespace MW.Core.Events;

/// <summary>
/// Represents an entity that can store and expose domain events.
/// Infrastructure can collect these events for publishing.
/// </summary>
public interface IHasDomainEvents
{
    /// <summary>
    /// Gets the domain events raised by this entity.
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Clears all domain events from this entity.
    /// </summary>
    void ClearDomainEvents();
}
