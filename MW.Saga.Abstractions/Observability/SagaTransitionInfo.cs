namespace MW.Saga.Observability;

/// <summary>
/// Structured model representing a saga state transition for logging,
/// diagnostics, and observability purposes.
/// <para>
/// Can be used in observer callbacks, audit logs, and monitoring dashboards
/// to track the progression of saga instances through their states.
/// </para>
/// </summary>
public class SagaTransitionInfo
{
    /// <summary>
    /// Gets or sets the state the saga transitioned from.
    /// </summary>
    public string FromState { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the state the saga transitioned to.
    /// </summary>
    public string ToState { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the event or message that triggered the transition.
    /// </summary>
    public string? TriggeredBy { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the transition occurred.
    /// </summary>
    public DateTime OccurredAt { get; set; }
}
