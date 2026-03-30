namespace MW.OrderRegistration.ApiDemo.Contracts;

/// <summary>
/// HTTP response contract for saga/process-level status.
/// Exposes workflow orchestration state without leaking raw MassTransit internals.
/// </summary>
public class ProcessStatusResponse
{
    /// <summary>Correlation identifier for the saga instance.</summary>
    public Guid CorrelationId { get; init; }

    /// <summary>Order identifier linked to this process.</summary>
    public Guid OrderId { get; init; }

    /// <summary>Current saga/process state (e.g., AwaitingInventory, OrderCompleted).</summary>
    public string CurrentState { get; init; } = string.Empty;

    /// <summary>High-level process status (e.g., Running, Completed, Failed, TimedOut).</summary>
    public string ProcessStatus { get; init; } = string.Empty;

    /// <summary>When the process was started.</summary>
    public DateTime? StartedAt { get; init; }

    /// <summary>When the process completed successfully.</summary>
    public DateTime? CompletedAt { get; init; }

    /// <summary>When the process failed or timed out.</summary>
    public DateTime? FailedAt { get; init; }

    /// <summary>Failure reason (if applicable).</summary>
    public string? FailureReason { get; init; }

    /// <summary>Inventory reservation tracking ID (if available).</summary>
    public Guid? InventoryReservationId { get; init; }

    /// <summary>Payment attempt tracking ID (if available).</summary>
    public Guid? PaymentAttemptId { get; init; }
}
