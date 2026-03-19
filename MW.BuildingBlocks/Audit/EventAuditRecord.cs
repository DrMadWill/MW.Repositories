namespace MW.BuildingBlocks.Audit;

/// <summary>
/// Optional audit-journal record for teams that decide to persist event history
/// for business or support purposes.
/// <para>
/// <b>Important:</b> This is <b>not</b> a replacement for MassTransit Inbox/Outbox.
/// This is <b>not</b> duplicate detection storage.
/// Use this only if long-term event history is required for auditing or support.
/// </para>
/// </summary>
public class EventAuditRecord
{
    /// <summary>
    /// Gets or sets the unique identifier of the message.
    /// </summary>
    public Guid MessageId { get; set; }

    /// <summary>
    /// Gets or sets the explicit event name (e.g., <c>order.created.v1</c>).
    /// </summary>
    public string EventName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the direction of the event (e.g., <c>Published</c> or <c>Consumed</c>).
    /// </summary>
    public string Direction { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the service that published or consumed the event.
    /// </summary>
    public string Service { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the consumer that processed the event.
    /// Returns <c>null</c> if the record represents a publish operation.
    /// </summary>
    public string? Consumer { get; set; }

    /// <summary>
    /// Gets or sets the correlation identifier used for distributed tracing across services.
    /// Returns <c>null</c> if not available.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the date and time (UTC) when the event occurred.
    /// </summary>
    public DateTimeOffset OccurredAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the processing status (e.g., <c>Success</c>, <c>Failed</c>).
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error message if the event processing failed.
    /// Returns <c>null</c> if the processing was successful.
    /// </summary>
    public string? Error { get; set; }
}
