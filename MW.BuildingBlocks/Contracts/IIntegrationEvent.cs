namespace MW.BuildingBlocks.Contracts;

/// <summary>
/// Defines the standard contract for integration events exchanged between microservices.
/// All cross-service events must implement this interface to ensure a consistent
/// and predictable event structure across the platform.
/// </summary>
public interface IIntegrationEvent
{
    /// <summary>
    /// Gets the unique identifier of the event instance.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Gets the date and time (UTC) when the event occurred.
    /// </summary>
    DateTimeOffset OccurredOn { get; }

    /// <summary>
    /// Gets the explicit name of the event (e.g., <c>order.created.v1</c>).
    /// </summary>
    string EventName { get; }

    /// <summary>
    /// Gets the version of the event contract (e.g., <c>v1</c>).
    /// </summary>
    string EventVersion { get; }

    /// <summary>
    /// Gets the name of the service that published the event.
    /// </summary>
    string SourceService { get; }

    /// <summary>
    /// Gets the correlation identifier used for distributed tracing across services.
    /// Returns <c>null</c> if not available.
    /// </summary>
    string? CorrelationId { get; }

    /// <summary>
    /// Gets the causation identifier linking this event to the command or event that caused it.
    /// Returns <c>null</c> if not available.
    /// </summary>
    string? CausationId { get; }
}
