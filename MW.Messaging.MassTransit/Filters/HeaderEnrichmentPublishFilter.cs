using System.Diagnostics;
using MassTransit;
using MW.Messaging.Context;
using MessageHeaders = MW.Messaging.Headers.MessageHeaders;

namespace MW.Messaging.MassTransit.Filters;

public class HeaderEnrichmentPublishFilter<TMessage> : IFilter<PublishContext<TMessage>>
    where TMessage : class
{
    private readonly IPublishContextProvider _contextProvider;
    private readonly MW.Messaging.MassTransit.IMessageHeaderMapper _headerMapper;

    public HeaderEnrichmentPublishFilter(
        IPublishContextProvider contextProvider,
        MW.Messaging.MassTransit.IMessageHeaderMapper headerMapper)
    {
        _contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        _headerMapper = headerMapper ?? throw new ArgumentNullException(nameof(headerMapper));
    }

    public async Task Send(PublishContext<TMessage> context, IPipe<PublishContext<TMessage>> next)
    {
        var publishContext = _contextProvider.Create();
        var headers = _headerMapper.MapToHeaders(publishContext);

        foreach (var header in headers)
        {
            if (!context.Headers.TryGetHeader(header.Key, out _))
            {
                context.Headers.Set(header.Key, header.Value);
            }
        }

        if (context.Message is MW.Messaging.Contracts.IIntegrationEvent integrationEvent)
        {
            if (!context.Headers.TryGetHeader(MessageHeaders.EventName, out _))
                context.Headers.Set(MessageHeaders.EventName, integrationEvent.EventName);

            if (!context.Headers.TryGetHeader(MessageHeaders.EventVersion, out _))
                context.Headers.Set(MessageHeaders.EventVersion, integrationEvent.EventVersion);
        }

        // Enrich current Activity with publish metadata for distributed tracing
        var activity = Activity.Current;
        if (activity != null)
        {
            activity.SetTag("messaging.correlation_id", publishContext.CorrelationId);
            activity.SetTag("messaging.source_service", publishContext.SourceService);

            if (!string.IsNullOrWhiteSpace(publishContext.TraceId))
                activity.SetTag("messaging.trace_id", publishContext.TraceId);
        }

        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("headerEnrichment");
    }
}
