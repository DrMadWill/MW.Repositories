using MW.Saga.Contracts;

namespace MW.Saga.Models;

/// <summary>
/// Reusable abstract base class for saga state models.
/// Provides common lifecycle and state-tracking properties so all saga
/// implementations follow a consistent shape.
/// <para>
/// Concrete saga state classes should inherit from this base class
/// and add workflow-specific properties as needed.
/// </para>
/// </summary>
public abstract class SagaStateBase : ISagaState
{
    /// <summary>
    /// Gets or sets the unique correlation identifier for the saga instance.
    /// </summary>
    public Guid CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the current state name of the saga (e.g., <c>"OrderPlaced"</c>, <c>"PaymentReceived"</c>).
    /// </summary>
    public string CurrentState { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the UTC timestamp when the saga was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the saga was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the saga completed, or <c>null</c> if not yet completed.
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the saga failed, or <c>null</c> if no failure occurred.
    /// </summary>
    public DateTime? FailedAt { get; set; }

    /// <summary>
    /// Gets or sets the concurrency/version token for optimistic concurrency control.
    /// This field should be incremented on each state update and used for optimistic locking
    /// during persistence operations to detect conflicting updates.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Gets a value indicating whether the saga has completed successfully.
    /// </summary>
    public bool IsCompleted => CompletedAt.HasValue;

    /// <summary>
    /// Gets a value indicating whether the saga has failed.
    /// </summary>
    public bool IsFailed => FailedAt.HasValue;
}
