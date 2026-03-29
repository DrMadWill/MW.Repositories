using MassTransit;
using MW.Saga.MassTransit.Options;

namespace MW.Saga.MassTransit.Naming;

/// <summary>
/// Saga-specific endpoint name formatter that applies a service prefix
/// and kebab-case formatting to saga state machine endpoints.
/// Ensures saga endpoints follow a predictable naming convention across services.
/// </summary>
public class SagaEndpointNameFormatter : KebabCaseEndpointNameFormatter
{
    private readonly string _prefix;

    /// <summary>
    /// Initializes a new instance with the specified endpoint prefix.
    /// </summary>
    /// <param name="prefix">The service-level endpoint prefix. Can be empty.</param>
    /// <param name="includeNamespace">Whether to include the namespace in the endpoint name.</param>
    public SagaEndpointNameFormatter(string prefix, bool includeNamespace = false)
        : base(includeNamespace)
    {
        _prefix = string.IsNullOrWhiteSpace(prefix)
            ? string.Empty
            : prefix.Trim().ToLowerInvariant() + "-";
    }

    /// <inheritdoc />
    public override string SanitizeName(string name)
    {
        return _prefix + base.SanitizeName(name);
    }

    /// <summary>
    /// Creates a <see cref="SagaEndpointNameFormatter"/> from the given options.
    /// </summary>
    public static SagaEndpointNameFormatter FromOptions(SagaMassTransitOptions options)
    {
        return new SagaEndpointNameFormatter(options.EndpointPrefix);
    }
}
