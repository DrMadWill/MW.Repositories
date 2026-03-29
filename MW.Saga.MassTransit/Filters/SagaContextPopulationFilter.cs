using System.Diagnostics;
using MassTransit;
using MW.Messaging.Headers;
using MessageHeaders = MW.Messaging.Headers.MessageHeaders;
using MW.Saga.Contracts;
using MW.Saga.Models;

namespace MW.Saga.MassTransit.Filters;

/// <summary>
/// Pipeline filter that populates the saga runtime context during state-machine event handling
/// and clears it safely after execution completes (including failure paths).
/// </summary>
/// <typeparam name="TSaga">The saga state type.</typeparam>
/// <typeparam name="TMessage">The message type being handled.</typeparam>
internal class SagaContextPopulationFilter<TSaga, TMessage> :
    IFilter<SagaConsumeContext<TSaga, TMessage>>
    where TSaga : class, ISaga
    where TMessage : class
{
    private readonly Context.ScopedSagaContextAccessor _accessor;

    public SagaContextPopulationFilter(Context.ScopedSagaContextAccessor accessor)
    {
        _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
    }

    public async Task Send(SagaConsumeContext<TSaga, TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
    {
        var sagaContext = new Context.MutableSagaContext
        {
            CorrelationId = context.CorrelationId ?? Guid.Empty,
            CurrentState = TryGetCurrentState(context.Saga),
            Status = SagaStatus.Running,
            SagaName = typeof(TSaga).Name,
            TraceId = Activity.Current?.TraceId.ToString(),
            SourceService = context.Headers.TryGetHeader(MessageHeaders.SourceService, out var source) ? source?.ToString() : null
        };

        _accessor.SetContext(sagaContext);

        try
        {
            await next.Send(context);
        }
        finally
        {
            _accessor.ClearContext();
        }
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("sagaContextPopulation");
    }

    private static string TryGetCurrentState(TSaga saga)
    {
        if (saga is ISagaState sagaState)
            return sagaState.CurrentState;
        return string.Empty;
    }
}
