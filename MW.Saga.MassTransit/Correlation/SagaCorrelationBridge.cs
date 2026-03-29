using MassTransit;
using MW.Saga.Correlation;

namespace MW.Saga.MassTransit.Correlation;

/// <summary>
/// Bridges <see cref="ISagaCorrelationResolver{TMessage}"/> from the abstraction layer
/// into MassTransit saga correlation configuration.
/// Allows correlation to be configured using abstraction-layer contracts
/// without ad-hoc correlation wiring in each saga.
/// </summary>
public static class SagaCorrelationBridge
{
    /// <summary>
    /// Configures MassTransit saga event correlation using an <see cref="ISagaCorrelationResolver{TMessage}"/>.
    /// </summary>
    /// <typeparam name="TSaga">The saga state type.</typeparam>
    /// <typeparam name="TMessage">The message type to correlate.</typeparam>
    /// <param name="configurator">The MassTransit event correlator.</param>
    /// <param name="resolver">The abstraction-layer correlation resolver.</param>
    public static void CorrelateUsing<TSaga, TMessage>(
        IEventCorrelationConfigurator<TSaga, TMessage> configurator,
        ISagaCorrelationResolver<TMessage> resolver)
        where TSaga : class, SagaStateMachineInstance
        where TMessage : class
    {
        ArgumentNullException.ThrowIfNull(configurator);
        ArgumentNullException.ThrowIfNull(resolver);

        configurator.CorrelateById(ctx => resolver.Resolve(ctx.Message));
    }
}
