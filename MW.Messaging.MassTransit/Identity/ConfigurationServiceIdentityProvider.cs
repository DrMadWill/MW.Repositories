using MW.Messaging.Identity;
using MW.Messaging.MassTransit.Options;
using MW.Messaging.Messaging;

namespace MW.Messaging.MassTransit.Identity;

public class ConfigurationServiceIdentityProvider : IServiceIdentityProvider
{
    private readonly ServiceIdentity _identity;

    public ConfigurationServiceIdentityProvider(MassTransitOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _identity = new ServiceIdentity
        {
            ServiceName = options.ServiceName
        };
    }

    public ServiceIdentity GetCurrent() => _identity;
}
