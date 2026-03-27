using MassTransit;
using MW.Messaging.Contracts;
using MW.Messaging.Context;
using MW.Messaging.Messaging;
using MW.Messaging.Publishing;
using MW.Messaging.Validation;

namespace MW.Messaging.MassTransit.Publishing;

public class MassTransitIntegrationEventPublisher : IIntegrationEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IPublishContextProvider _contextProvider;
    private readonly IIntegrationEventValidator? _validator;

    public MassTransitIntegrationEventPublisher(
        IPublishEndpoint publishEndpoint,
        IPublishContextProvider contextProvider,
        IIntegrationEventValidator? validator = null)
    {
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        _contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        _validator = validator;
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

        _validator?.Validate(integrationEvent);

        await _publishEndpoint.Publish(integrationEvent, integrationEvent.GetType(), cancellationToken);
    }
}
