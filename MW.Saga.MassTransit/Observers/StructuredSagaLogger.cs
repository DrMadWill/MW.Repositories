using Microsoft.Extensions.Logging;
using MW.Saga.Constants;
using MW.Saga.Models;
using MW.Saga.Observability;

namespace MW.Saga.MassTransit.Observers;

/// <summary>
/// Structured observability bridge for saga lifecycle and state transitions.
/// Implements <see cref="ISagaObserver"/> and produces structured log entries
/// with standardized metadata (saga name, correlation id, current state, status,
/// message name, source service, trace id).
/// </summary>
public class StructuredSagaLogger : ISagaObserver
{
    private readonly ILogger<StructuredSagaLogger> _logger;

    public StructuredSagaLogger(ILogger<StructuredSagaLogger> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public Task OnSagaStartedAsync(SagaObservabilityContext context, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Saga started: {SagaName} CorrelationId={CorrelationId} State={CurrentState} Status={Status} Message={MessageName} Source={SourceService} TraceId={TraceId}",
            context.SagaName,
            context.CorrelationId,
            context.CurrentState,
            context.Status,
            context.MessageName,
            context.SourceService,
            context.TraceId);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task OnStateTransitionedAsync(SagaObservabilityContext context, SagaTransitionInfo transition, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Saga state transitioned: {SagaName} CorrelationId={CorrelationId} From={FromState} To={ToState} TriggeredBy={TriggeredBy} Status={Status} TraceId={TraceId}",
            context.SagaName,
            context.CorrelationId,
            transition.FromState,
            transition.ToState,
            transition.TriggeredBy,
            context.Status,
            context.TraceId);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task OnSagaCompletedAsync(SagaObservabilityContext context, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Saga completed: {SagaName} CorrelationId={CorrelationId} State={CurrentState} Status={Status} TraceId={TraceId}",
            context.SagaName,
            context.CorrelationId,
            context.CurrentState,
            context.Status,
            context.TraceId);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task OnSagaFailedAsync(SagaObservabilityContext context, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning(
            "Saga failed: {SagaName} CorrelationId={CorrelationId} State={CurrentState} Status={Status} Message={MessageName} TraceId={TraceId}",
            context.SagaName,
            context.CorrelationId,
            context.CurrentState,
            context.Status,
            context.MessageName,
            context.TraceId);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task OnSagaTimedOutAsync(SagaObservabilityContext context, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning(
            "Saga timed out: {SagaName} CorrelationId={CorrelationId} State={CurrentState} Status={Status} TraceId={TraceId}",
            context.SagaName,
            context.CorrelationId,
            context.CurrentState,
            context.Status,
            context.TraceId);

        return Task.CompletedTask;
    }
}
