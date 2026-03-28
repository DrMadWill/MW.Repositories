using MW.Saga.Models;

namespace MW.Saga.Context;

/// <summary>
/// Execution-context abstraction for saga-driven flows.
/// Exposes broader runtime metadata so that application code can consume
/// saga execution information safely without coupling to transport types.
/// <para>
/// This interface provides more detailed execution context compared to
/// <see cref="ISagaContext"/>, including causation and saga identity information.
/// </para>
/// </summary>
public interface ISagaExecutionContext
{
    /// <summary>
    /// Gets the unique correlation identifier for the saga instance.
    /// </summary>
    Guid CorrelationId { get; }

    /// <summary>
    /// Gets the causation identifier linking the current execution to its triggering event.
    /// Returns <c>null</c> if not available.
    /// </summary>
    string? CausationId { get; }

    /// <summary>
    /// Gets the distributed trace identifier for the current saga execution.
    /// Returns <c>null</c> if not available.
    /// </summary>
    string? TraceId { get; }

    /// <summary>
    /// Gets the name of the saga (e.g., <c>"OrderSaga"</c>, <c>"PaymentSaga"</c>).
    /// </summary>
    string SagaName { get; }

    /// <summary>
    /// Gets the current fine-grained state name of the saga.
    /// </summary>
    string CurrentState { get; }

    /// <summary>
    /// Gets the high-level lifecycle status of the saga.
    /// </summary>
    SagaStatus Status { get; }

    /// <summary>
    /// Gets the name of the service that initiated the saga.
    /// Returns <c>null</c> if not available.
    /// </summary>
    string? SourceService { get; }
}
