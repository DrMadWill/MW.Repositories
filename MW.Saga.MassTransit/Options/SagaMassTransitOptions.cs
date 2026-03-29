namespace MW.Saga.MassTransit.Options;

/// <summary>
/// Strongly typed options for saga MassTransit infrastructure registration.
/// Supports configuration binding so services do not need to hardcode infrastructure settings.
/// </summary>
public class SagaMassTransitOptions
{
    /// <summary>
    /// Default configuration section name.
    /// </summary>
    public const string SectionName = "SagaMassTransit";

    /// <summary>
    /// Gets or sets the service-level endpoint prefix for saga endpoints.
    /// Used for consistent kebab-case endpoint naming across saga state machines.
    /// </summary>
    public string EndpointPrefix { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the retry count for saga event handling.
    /// </summary>
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets the retry intervals in seconds for saga event handling.
    /// When specified, overrides <see cref="RetryCount"/> with explicit interval-based retry.
    /// </summary>
    public int[] RetryIntervalsInSeconds { get; set; } = [];

    /// <summary>
    /// Gets or sets the scheduler/timeout-related default timeout duration in seconds.
    /// Used for saga timeout conventions when a specific timeout is not provided.
    /// </summary>
    public int DefaultTimeoutInSeconds { get; set; } = 300;

    /// <summary>
    /// Gets or sets the concurrency limit for saga event handling.
    /// When set to a value greater than zero, limits the number of concurrent saga messages processed.
    /// </summary>
    public int ConcurrencyLimit { get; set; }

    /// <summary>
    /// Gets or sets whether to use the message scheduler for timeout support.
    /// </summary>
    public bool UseScheduler { get; set; }
}
