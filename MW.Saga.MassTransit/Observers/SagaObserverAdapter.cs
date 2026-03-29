using Microsoft.Extensions.Logging;
using MW.Saga.Models;
using MW.Saga.Observability;

namespace MW.Saga.MassTransit.Observers;

/// <summary>
/// MassTransit saga observer adapter that bridges saga lifecycle events
/// into the abstraction-layer <see cref="ISagaObserver"/>.
/// Forwards events such as started, state transitioned, completed, failed, and timed out
/// using structured <see cref="SagaObservabilityContext"/> and <see cref="SagaTransitionInfo"/> models.
/// <para>
/// This adapter is designed to be called from saga filters or state machine activities.
/// It does not implement MassTransit-specific observer interfaces to remain decoupled.
/// </para>
/// </summary>
public class SagaObserverAdapter
{
    private readonly MW.Saga.Observability.ISagaObserver? _observer;
    private readonly ILogger<SagaObserverAdapter>? _logger;

    public SagaObserverAdapter(
        MW.Saga.Observability.ISagaObserver? observer = null,
        ILogger<SagaObserverAdapter>? logger = null)
    {
        _observer = observer;
        _logger = logger;
    }

    /// <summary>
    /// Notifies the observer that a saga has started.
    /// </summary>
    public async Task NotifySagaStarted(Guid correlationId, string sagaName, string? messageName = null, string? traceId = null, CancellationToken cancellationToken = default)
    {
        if (_observer == null) return;

        var context = new SagaObservabilityContext
        {
            CorrelationId = correlationId,
            SagaName = sagaName,
            MessageName = messageName,
            Status = SagaStatus.Running,
            TraceId = traceId ?? System.Diagnostics.Activity.Current?.TraceId.ToString()
        };

        await _observer.OnSagaStartedAsync(context, cancellationToken);
    }

    /// <summary>
    /// Notifies the observer of a state transition.
    /// </summary>
    public async Task NotifyStateTransitioned(Guid correlationId, string sagaName, string fromState, string toState, string? triggeredBy = null, string? traceId = null, CancellationToken cancellationToken = default)
    {
        if (_observer == null) return;

        var context = new SagaObservabilityContext
        {
            CorrelationId = correlationId,
            SagaName = sagaName,
            CurrentState = toState,
            Status = SagaStatus.Running,
            TraceId = traceId ?? System.Diagnostics.Activity.Current?.TraceId.ToString()
        };

        var transition = new SagaTransitionInfo
        {
            FromState = fromState,
            ToState = toState,
            TriggeredBy = triggeredBy,
            OccurredAt = DateTime.UtcNow
        };

        await _observer.OnStateTransitionedAsync(context, transition, cancellationToken);
    }

    /// <summary>
    /// Notifies the observer that a saga has completed.
    /// </summary>
    public async Task NotifySagaCompleted(Guid correlationId, string sagaName, string finalState, string? traceId = null, CancellationToken cancellationToken = default)
    {
        if (_observer == null) return;

        var context = new SagaObservabilityContext
        {
            CorrelationId = correlationId,
            SagaName = sagaName,
            CurrentState = finalState,
            Status = SagaStatus.Completed,
            TraceId = traceId ?? System.Diagnostics.Activity.Current?.TraceId.ToString()
        };

        await _observer.OnSagaCompletedAsync(context, cancellationToken);
    }

    /// <summary>
    /// Notifies the observer that a saga has failed.
    /// </summary>
    public async Task NotifySagaFailed(Guid correlationId, string sagaName, string currentState, string? messageName = null, string? traceId = null, CancellationToken cancellationToken = default)
    {
        if (_observer == null) return;

        var context = new SagaObservabilityContext
        {
            CorrelationId = correlationId,
            SagaName = sagaName,
            CurrentState = currentState,
            MessageName = messageName,
            Status = SagaStatus.Failed,
            TraceId = traceId ?? System.Diagnostics.Activity.Current?.TraceId.ToString()
        };

        await _observer.OnSagaFailedAsync(context, cancellationToken);
    }

    /// <summary>
    /// Notifies the observer that a saga has timed out.
    /// </summary>
    public async Task NotifySagaTimedOut(Guid correlationId, string sagaName, string currentState, string? traceId = null, CancellationToken cancellationToken = default)
    {
        if (_observer == null) return;

        var context = new SagaObservabilityContext
        {
            CorrelationId = correlationId,
            SagaName = sagaName,
            CurrentState = currentState,
            Status = SagaStatus.TimedOut,
            TraceId = traceId ?? System.Diagnostics.Activity.Current?.TraceId.ToString()
        };

        await _observer.OnSagaTimedOutAsync(context, cancellationToken);
    }
}
