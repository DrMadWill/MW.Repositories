namespace MW.Saga.Constants;

/// <summary>
/// Shared constants for saga-related names and values.
/// Centralizes commonly repeated saga-related strings to avoid magic values
/// in saga infrastructure code.
/// </summary>
public static class SagaConstants
{
    /// <summary>
    /// Default state name representing a saga that has not yet started.
    /// </summary>
    public const string InitialState = "Initial";

    /// <summary>
    /// Default state name representing a saga that has completed.
    /// </summary>
    public const string FinalState = "Final";

    /// <summary>
    /// Default state name representing a saga that has faulted.
    /// </summary>
    public const string FaultedState = "Faulted";

    /// <summary>
    /// Default state name representing a saga that has timed out.
    /// </summary>
    public const string TimedOutState = "TimedOut";

    /// <summary>
    /// Default state name representing a saga that has been cancelled.
    /// </summary>
    public const string CancelledState = "Cancelled";
}
