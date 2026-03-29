using MW.Saga.Models;

namespace MW.Saga.Observability;

/// <summary>
/// Standard observability model for saga logging, monitoring, and tracing-related metadata.
/// Provides a lightweight, structured context that can be used by saga observers,
/// logging middleware, and monitoring integrations.
/// <para>
/// This model is generic and not tied to MassTransit runtime objects.
/// </para>
/// </summary>
public class SagaObservabilityContext
{
    /// <summary>
    /// Gets or sets the name of the saga (e.g., <c>"OrderSaga"</c>).
    /// </summary>
    public string SagaName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique correlation identifier for the saga instance.
    /// </summary>
    public Guid CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the current fine-grained state name of the saga.
    /// </summary>
    public string CurrentState { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the high-level lifecycle status of the saga.
    /// </summary>
    public SagaStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the name of the message that triggered the current activity.
    /// </summary>
    public string? MessageName { get; set; }

    /// <summary>
    /// Gets or sets the name of the source service.
    /// </summary>
    public string? SourceService { get; set; }

    /// <summary>
    /// Gets or sets the distributed trace identifier.
    /// </summary>
    public string? TraceId { get; set; }
}
