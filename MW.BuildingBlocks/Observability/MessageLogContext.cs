namespace MW.BuildingBlocks.Observability;

/// <summary>
/// Shared observability model for structured event publish/consume logging.
/// Since the platform relies on MassTransit transactional outbox for reliability,
/// observability comes from structured logging and tracing instead of custom
/// Inbox/Outbox tables. This model ensures publish and consume logs follow a
/// consistent, queryable structure across services.
/// </summary>
public class MessageLogContext
{
    /// <summary>
    /// Gets or sets the unique identifier of the message.
    /// Returns <c>null</c> if not available.
    /// </summary>
    public Guid? MessageId { get; set; }

    /// <summary>
    /// Gets or sets the explicit event name (e.g., <c>order.created.v1</c>).
    /// Returns <c>null</c> if not available.
    /// </summary>
    public string? EventName { get; set; }

    /// <summary>
    /// Gets or sets the version of the event contract.
    /// Returns <c>null</c> if not available.
    /// </summary>
    public string? EventVersion { get; set; }

    /// <summary>
    /// Gets or sets the correlation identifier used for distributed tracing across services.
    /// Returns <c>null</c> if not available.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the causation identifier linking this log entry to the originating command or event.
    /// Returns <c>null</c> if not available.
    /// </summary>
    public string? CausationId { get; set; }

    /// <summary>
    /// Gets or sets the trace identifier for distributed tracing (e.g., OpenTelemetry trace id).
    /// Returns <c>null</c> if not available.
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    /// Gets or sets the name of the service that published the message.
    /// Returns <c>null</c> if not available.
    /// </summary>
    public string? SourceService { get; set; }

    /// <summary>
    /// Gets or sets the name of the consumer processing the message.
    /// Returns <c>null</c> if not available.
    /// </summary>
    public string? Consumer { get; set; }

    /// <summary>
    /// Gets or sets the endpoint where the message was received.
    /// Returns <c>null</c> if not available.
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// Gets or sets the status of the message processing (e.g., <c>Published</c>, <c>Consumed</c>, <c>Failed</c>).
    /// Returns <c>null</c> if not available.
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the log entry was created.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
}
