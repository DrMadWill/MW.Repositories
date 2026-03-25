using MassTransit;
using MW.Messaging.Contracts;
using MW.Messaging.Context;
using MessageHeaders = MW.Messaging.Headers.MessageHeaders;
using MW.Messaging.Messaging;
using MW.Messaging.Publishing;

namespace MW.Messaging.MassTransit.Publishing;

public class MassTransitIntegrationEventPublisher : IIntegrationEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IPublishContextProvider _contextProvider;
    private readonly MW.Messaging.MassTransit.IMessageHeaderMapper _headerMapper;

    public MassTransitIntegrationEventPublisher(
        IPublishEndpoint publishEndpoint,
        IPublishContextProvider contextProvider,
        MW.Messaging.MassTransit.IMessageHeaderMapper headerMapper)
    {
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        _contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        _headerMapper = headerMapper ?? throw new ArgumentNullException(nameof(headerMapper));
    }

    public Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        var context = _contextProvider.Create();
        return PublishAsync(integrationEvent, context, cancellationToken);
    }

    public async Task PublishAsync(IIntegrationEvent integrationEvent, PublishContextModel context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        ArgumentNullException.ThrowIfNull(context);

        var headers = _headerMapper.MapToHeaders(context);
        headers[MessageHeaders.EventName] = integrationEvent.EventName;
        headers[MessageHeaders.EventVersion] = integrationEvent.EventVersion;

        await _publishEndpoint.Publish(integrationEvent, integrationEvent.GetType(), ctx =>
        {
            foreach (var header in headers)
            {
                ctx.Headers.Set(header.Key, header.Value);
            }
        }, cancellationToken);
    }
}
