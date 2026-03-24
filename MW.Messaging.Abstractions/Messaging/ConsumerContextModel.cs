namespace MW.Messaging.Messaging;

/// <summary>
/// Shared consumer-side metadata model for exposing message-related context to business/application code.
/// Application code can use this model instead of depending directly on MassTransit's
/// <c>ConsumeContext</c> types, keeping business logic transport-agnostic.
/// </summary>
public class ConsumerContextModel
{
    /// <summary>
    /// Gets or sets the unique identifier of the consumed message.
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
    /// Gets or sets the name of the service that published the message.
    /// Returns <c>null</c> if not available.
    /// </summary>
    public string? SourceService { get; set; }

    /// <summary>
    /// Gets or sets the trace identifier for distributed tracing (e.g., OpenTelemetry trace id).
    /// Returns <c>null</c> if not available.
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    /// Gets or sets the explicit event name extracted from the message headers.
    /// Returns <c>null</c> if not available.
    /// </summary>
    public string? EventName { get; set; }
}
