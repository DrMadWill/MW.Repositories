namespace MW.Saga.Models;

/// <summary>
/// High-level lifecycle status enum for saga instances.
/// Represents the generic process status independent from fine-grained
/// state-machine state names.
/// <para>
/// Suitable for dashboards, monitoring, logging, and generic saga lifecycle tracking
/// across different business workflows.
/// </para>
/// </summary>
public enum SagaStatus
{
    /// <summary>
    /// The saga has not yet started processing.
    /// </summary>
    NotStarted = 0,

    /// <summary>
    /// The saga is currently running and processing events.
    /// </summary>
    Running = 1,

    /// <summary>
    /// The saga has completed successfully.
    /// </summary>
    Completed = 2,

    /// <summary>
    /// The saga has failed due to an error.
    /// </summary>
    Failed = 3,

    /// <summary>
    /// The saga was explicitly cancelled.
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// The saga has timed out without completing.
    /// </summary>
    TimedOut = 5
}
