using MassTransit;

namespace MW.Saga.MassTransit.Filters;

/// <summary>
/// Reusable infrastructure hook for saga fault behavior.
/// Provides a generic fault handling pipeline component that can be extended
/// for failed-state handling and observability without embedding business-specific
/// compensation logic.
/// </summary>
/// <typeparam name="TSaga">The saga state type.</typeparam>
/// <typeparam name="TMessage">The message type being handled.</typeparam>
public class SagaFaultFilter<TSaga, TMessage> :
    IFilter<SagaConsumeContext<TSaga, TMessage>>
    where TSaga : class, ISaga
    where TMessage : class
{
    private readonly Action<SagaConsumeContext<TSaga, TMessage>, Exception>? _onFault;

    /// <summary>
    /// Initializes a new instance of the <see cref="SagaFaultFilter{TSaga, TMessage}"/> class.
    /// </summary>
    /// <param name="onFault">Optional fault callback for infrastructure-level handling.</param>
    public SagaFaultFilter(Action<SagaConsumeContext<TSaga, TMessage>, Exception>? onFault = null)
    {
        _onFault = onFault;
    }

    public async Task Send(SagaConsumeContext<TSaga, TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
    {
        try
        {
            await next.Send(context);
        }
        catch (Exception ex)
        {
            _onFault?.Invoke(context, ex);
            throw;
        }
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("sagaFaultHandling");
    }
}
