using MassTransit;

namespace MW.Messaging.MassTransit.Naming;

public class ServiceEndpointNameFormatter : KebabCaseEndpointNameFormatter
{
    private readonly string _servicePrefix;

    public ServiceEndpointNameFormatter(string servicePrefix, bool includeNamespace = false)
        : base(includeNamespace)
    {
        _servicePrefix = string.IsNullOrWhiteSpace(servicePrefix) 
            ? string.Empty 
            : servicePrefix.Trim().ToLowerInvariant() + "-";
    }

    public override string SanitizeName(string name)
    {
        return _servicePrefix + base.SanitizeName(name);
    }
}
