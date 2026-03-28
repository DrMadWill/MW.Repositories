namespace MW.Saga.Contracts;

/// <summary>
/// Minimal contract that every saga state model must implement.
/// Provides the essential correlation and current state information
/// required to identify and track a saga instance.
/// <para>
/// This contract is transport-agnostic and does not depend on
/// MassTransit or any other messaging framework types.
/// </para>
/// </summary>
public interface ISagaState
{
    /// <summary>
    /// Gets or sets the unique correlation identifier for the saga instance.
    /// Used to correlate messages and events to the correct saga.
    /// </summary>
    Guid CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the current state name of the saga.
    /// Represents the fine-grained state-machine state (e.g., <c>"OrderPlaced"</c>, <c>"PaymentReceived"</c>).
    /// </summary>
    string CurrentState { get; set; }
}
