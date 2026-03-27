using System.Diagnostics;
using MassTransit;
using MW.Messaging.Headers;
using MW.Messaging.MassTransit.Tracing;

namespace MW.Messaging.MassTransit.Filters;

public class MessageContextConsumeFilter<TMessage> : IFilter<ConsumeContext<TMessage>>
    where TMessage : class
{
    private readonly Context.ScopedMessageContextAccessor _accessor;
    private readonly MW.Messaging.MassTransit.IMessageHeaderMapper _headerMapper;

    public MessageContextConsumeFilter(
        Context.ScopedMessageContextAccessor accessor,
        MW.Messaging.MassTransit.IMessageHeaderMapper headerMapper)
    {
        _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        _headerMapper = headerMapper ?? throw new ArgumentNullException(nameof(headerMapper));
    }

    public async Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
    {
        var headers = new Dictionary<string, object>();

        foreach (var header in context.Headers.GetAll())
        {
            if (header.Value != null)
                headers[header.Key] = header.Value;
        }

        var consumerContext = _headerMapper.MapFromHeaders(headers);
        consumerContext.MessageId = context.MessageId ?? Guid.NewGuid();

        _accessor.SetContext(consumerContext);

        using var activity = MessagingActivitySource.Source.StartActivity(
            "messaging.consume",
            ActivityKind.Consumer);

        // Enrich activity with consume metadata for distributed tracing
        if (activity != null)
        {
            activity.SetTag("messaging.correlation_id", consumerContext.CorrelationId);
            activity.SetTag("messaging.source_service", consumerContext.SourceService);
            activity.SetTag("messaging.event_name", consumerContext.EventName);

            if (!string.IsNullOrWhiteSpace(consumerContext.TraceId))
                activity.SetTag("messaging.trace_id", consumerContext.TraceId);
        }

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
        context.CreateFilterScope("messageContextPropagation");
    }
}
