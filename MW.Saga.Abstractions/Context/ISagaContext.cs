using MW.Saga.Models;

namespace MW.Saga.Context;

/// <summary>
/// Application-facing abstraction that exposes the currently executing saga context
/// without exposing MassTransit or other transport-specific runtime primitives.
/// <para>
/// Designed to be read-only from the application perspective so that business logic
/// can safely access saga metadata without coupling to infrastructure types.
/// </para>
/// </summary>
public interface ISagaContext
{
    /// <summary>
    /// Gets the unique correlation identifier for the currently executing saga instance.
    /// </summary>
    Guid CorrelationId { get; }

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

    /// <summary>
    /// Gets the name of the event that started the saga.
    /// Returns <c>null</c> if not available.
    /// </summary>
    string? StartedByEvent { get; }

    /// <summary>
    /// Gets the distributed trace identifier for the current saga execution.
    /// Returns <c>null</c> if not available.
    /// </summary>
    string? TraceId { get; }
}
