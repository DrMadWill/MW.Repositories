using MW.Saga.Models;

namespace MW.Saga.Lifecycle;

/// <summary>
/// Lifecycle event model representing the start of a saga instance.
/// Suitable for logging, audit trails, and monitoring integration.
/// </summary>
public class SagaStarted
{
    /// <summary>
    /// Gets or sets the unique correlation identifier for the saga instance.
    /// </summary>
    public Guid CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the name of the saga.
    /// </summary>
    public string SagaName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the event that initiated the saga.
    /// </summary>
    public string? StartedByEvent { get; set; }

    /// <summary>
    /// Gets or sets the name of the source service that triggered the saga.
    /// </summary>
    public string? SourceService { get; set; }

    /// <summary>
    /// Gets or sets the distributed trace identifier.
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the saga started.
    /// </summary>
    public DateTime OccurredAt { get; set; }
}

/// <summary>
/// Lifecycle event model representing a state change within a saga instance.
/// Suitable for logging, audit trails, and monitoring integration.
/// </summary>
public class SagaStateChanged
{
    /// <summary>
    /// Gets or sets the unique correlation identifier for the saga instance.
    /// </summary>
    public Guid CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the name of the saga.
    /// </summary>
    public string SagaName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the state the saga transitioned from.
    /// </summary>
    public string FromState { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the state the saga transitioned to.
    /// </summary>
    public string ToState { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the event or message that triggered the state change.
    /// </summary>
    public string? TriggeredBy { get; set; }

    /// <summary>
    /// Gets or sets the distributed trace identifier.
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the state change occurred.
    /// </summary>
    public DateTime OccurredAt { get; set; }
}

/// <summary>
/// Lifecycle event model representing the successful completion of a saga instance.
/// Suitable for logging, audit trails, and monitoring integration.
/// </summary>
public class SagaCompleted
{
    /// <summary>
    /// Gets or sets the unique correlation identifier for the saga instance.
    /// </summary>
    public Guid CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the name of the saga.
    /// </summary>
    public string SagaName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the final state of the saga.
    /// </summary>
    public string FinalState { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the distributed trace identifier.
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the saga completed.
    /// </summary>
    public DateTime OccurredAt { get; set; }
}

/// <summary>
/// Lifecycle event model representing a saga instance failure.
/// Suitable for logging, audit trails, and monitoring integration.
/// </summary>
public class SagaFailed
{
    /// <summary>
    /// Gets or sets the unique correlation identifier for the saga instance.
    /// </summary>
    public Guid CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the name of the saga.
    /// </summary>
    public string SagaName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the state the saga was in when the failure occurred.
    /// </summary>
    public string CurrentState { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the high-level lifecycle status at failure time.
    /// </summary>
    public SagaStatus Status { get; set; } = SagaStatus.Failed;

    /// <summary>
    /// Gets or sets the reason or description of the failure.
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Gets or sets the distributed trace identifier.
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the failure occurred.
    /// </summary>
    public DateTime OccurredAt { get; set; }
}

/// <summary>
/// Lifecycle event model representing a saga instance timeout.
/// Suitable for logging, audit trails, and monitoring integration.
/// </summary>
public class SagaTimedOut
{
    /// <summary>
    /// Gets or sets the unique correlation identifier for the saga instance.
    /// </summary>
    public Guid CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the name of the saga.
    /// </summary>
    public string SagaName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the state the saga was in when the timeout occurred.
    /// </summary>
    public string CurrentState { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the high-level lifecycle status at timeout.
    /// </summary>
    public SagaStatus Status { get; set; } = SagaStatus.TimedOut;

    /// <summary>
    /// Gets or sets the distributed trace identifier.
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the timeout occurred.
    /// </summary>
    public DateTime OccurredAt { get; set; }
}
