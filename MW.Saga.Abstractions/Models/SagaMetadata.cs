namespace MW.Saga.Models;

/// <summary>
/// Lightweight metadata model for saga monitoring, logging, and lifecycle visibility.
/// Provides essential saga information without requiring the full saga state object.
/// <para>
/// Suitable for structured logging, monitoring dashboards, and lifecycle event payloads
/// where a complete saga state snapshot is not necessary.
/// </para>
/// </summary>
public class SagaMetadata
{
    /// <summary>
    /// Gets or sets the unique correlation identifier for the saga instance.
    /// </summary>
    public Guid CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the causation identifier linking the saga to its triggering command or event.
    /// Returns <c>null</c> if not available.
    /// </summary>
    public string? CausationId { get; set; }

    /// <summary>
    /// Gets or sets the distributed trace identifier (e.g., OpenTelemetry trace id).
    /// Returns <c>null</c> if not available.
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    /// Gets or sets the name of the event that initiated the saga.
    /// </summary>
    public string? StartedByEvent { get; set; }

    /// <summary>
    /// Gets or sets the name of the service that initiated the saga.
    /// </summary>
    public string? SourceService { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the saga started.
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the saga was last updated.
    /// </summary>
    public DateTime? LastUpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the current fine-grained state name of the saga.
    /// </summary>
    public string CurrentState { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the high-level lifecycle status of the saga.
    /// </summary>
    public SagaStatus Status { get; set; }
}
