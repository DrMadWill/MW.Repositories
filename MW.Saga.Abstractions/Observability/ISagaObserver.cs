namespace MW.Saga.Observability;

/// <summary>
/// Observer contract for saga lifecycle visibility and observability integration.
/// <para>
/// Implementations can be used to hook into saga lifecycle events for logging,
/// monitoring, metrics collection, distributed tracing, and audit trail generation.
/// </para>
/// <para>
/// This interface is generic and reusable. Concrete implementations belong in
/// infrastructure packages such as <c>MW.Saga.MassTransit</c>.
/// </para>
/// </summary>
public interface ISagaObserver
{
    /// <summary>
    /// Called when a saga instance has been started.
    /// </summary>
    /// <param name="context">The observability context containing saga metadata.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task OnSagaStartedAsync(SagaObservabilityContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Called when a saga instance transitions from one state to another.
    /// </summary>
    /// <param name="context">The observability context containing saga metadata.</param>
    /// <param name="transition">Details about the state transition.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task OnStateTransitionedAsync(SagaObservabilityContext context, SagaTransitionInfo transition, CancellationToken cancellationToken = default);

    /// <summary>
    /// Called when a saga instance has completed successfully.
    /// </summary>
    /// <param name="context">The observability context containing saga metadata.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task OnSagaCompletedAsync(SagaObservabilityContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Called when a saga instance has failed.
    /// </summary>
    /// <param name="context">The observability context containing saga metadata.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task OnSagaFailedAsync(SagaObservabilityContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Called when a saga instance has timed out.
    /// </summary>
    /// <param name="context">The observability context containing saga metadata.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task OnSagaTimedOutAsync(SagaObservabilityContext context, CancellationToken cancellationToken = default);
}
