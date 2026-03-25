using System.Diagnostics;
using MW.Messaging.Context;
using MW.Messaging.Correlation;
using MW.Messaging.Identity;
using MW.Messaging.Messaging;

namespace MW.Messaging.MassTransit.Context;

public class DefaultPublishContextProvider : IPublishContextProvider
{
    private readonly ICorrelationContext? _correlationContext;
    private readonly IServiceIdentityProvider? _serviceIdentityProvider;

    public DefaultPublishContextProvider(
        ICorrelationContext? correlationContext = null,
        IServiceIdentityProvider? serviceIdentityProvider = null)
    {
        _correlationContext = correlationContext;
        _serviceIdentityProvider = serviceIdentityProvider;
    }

    public PublishContextModel Create()
    {
        var model = new PublishContextModel
        {
            CorrelationId = _correlationContext?.CorrelationId ?? Guid.NewGuid().ToString(),
            CausationId = _correlationContext?.CausationId,
            TraceId = _correlationContext?.TraceId ?? Activity.Current?.TraceId.ToString()
        };

        if (_serviceIdentityProvider != null)
        {
            var identity = _serviceIdentityProvider.GetCurrent();
            model.SourceService = identity.ServiceName;
        }

        return model;
    }
}
