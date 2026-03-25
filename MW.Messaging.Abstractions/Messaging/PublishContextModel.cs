namespace MW.Messaging.Messaging;

/// <summary>
/// Shared publish metadata model used before mapping data into MassTransit headers/context.
/// This model does not replace MassTransit publish APIs — it standardizes the metadata
/// your application wants to attach whenever an event is published.
/// </summary>
public class PublishContextModel
{
    /// <summary>
    /// Gets or sets the correlation identifier used for distributed tracing across services.
    /// Returns <c>null</c> if not available.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the causation identifier linking this publish to the command or event that caused it.
    /// Returns <c>null</c> if not available.
    /// </summary>
    public string? CausationId { get; set; }

    /// <summary>
    /// Gets or sets the tenant identifier for multi-tenant support.
    /// Returns <c>null</c> if the system is not multi-tenant or tenant is not resolved.
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Gets or sets the user identifier associated with the publish action.
    /// Returns <c>null</c> if not available.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Gets or sets the name of the service publishing the event.
    /// </summary>
    public string SourceService { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the trace identifier for distributed tracing (e.g., OpenTelemetry trace id).
    /// Returns <c>null</c> if not available.
    /// </summary>
    public string? TraceId { get; set; }
}
