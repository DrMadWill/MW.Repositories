namespace MW.BuildingBlocks.Messaging;

/// <summary>
/// Reusable metadata model for event-related cross-cutting information.
/// Separated from the event payload itself and intended for logging,
/// tracing, and publish/consume diagnostics.
/// </summary>
public class EventMetadata
{
    /// <summary>
    /// Gets or sets the unique identifier of the message.
    /// </summary>
    public Guid MessageId { get; set; }

    /// <summary>
    /// Gets or sets the correlation identifier used for distributed tracing across services.
    /// Returns <c>null</c> if not available.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the causation identifier linking this message to the command or event that caused it.
    /// Returns <c>null</c> if not available.
    /// </summary>
    public string? CausationId { get; set; }

    /// <summary>
    /// Gets or sets the tenant identifier for multi-tenant support.
    /// Returns <c>null</c> if the system is not multi-tenant or tenant is not resolved.
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Gets or sets the user identifier associated with the message.
    /// Returns <c>null</c> if not available.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Gets or sets the name of the service that produced the message.
    /// </summary>
    public string SourceService { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the trace identifier for distributed tracing (e.g., OpenTelemetry trace id).
    /// Returns <c>null</c> if not available.
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    /// Gets or sets the date and time (UTC) when the metadata was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
