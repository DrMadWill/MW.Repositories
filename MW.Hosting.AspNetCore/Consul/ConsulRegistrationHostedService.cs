using Consul;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MW.Hosting.AspNetCore.Options;

namespace MW.Hosting.AspNetCore.Consul;

public class ConsulRegistrationHostedService : IHostedService
{
    private readonly IConsulClient _consulClient;
    private readonly ConsulOptions _options;
    private readonly ILogger<ConsulRegistrationHostedService> _logger;

    public ConsulRegistrationHostedService(
        IConsulClient consulClient,
        IOptions<ConsulOptions> options,
        ILogger<ConsulRegistrationHostedService> logger)
    {
        _consulClient = consulClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Registering service {ServiceName} ({ServiceId}) with Consul at {ConsulAddress}",
            _options.ServiceName, _options.ServiceId, _options.ConsulAddress);

        var serviceUri = new Uri(_options.ServiceAddress);
        var healthCheckUrl = _options.ServiceAddress.TrimEnd('/') + _options.HealthCheckPath;

        var registration = new AgentServiceRegistration
        {
            ID = _options.ServiceId,
            Name = _options.ServiceName,
            Address = serviceUri.Host,
            Port = serviceUri.Port,
            Tags = _options.Tags,
            Meta = _options.Meta,
            Check = new AgentServiceCheck
            {
                HTTP = healthCheckUrl,
                Interval = ParseTimeSpan(_options.HealthCheckInterval),
                Timeout = ParseTimeSpan(_options.HealthCheckTimeout),
                DeregisterCriticalServiceAfter = ParseTimeSpan(_options.DeregisterCriticalServiceAfter)
            }
        };

        await _consulClient.Agent.ServiceRegister(registration, cancellationToken);

        _logger.LogInformation(
            "Service {ServiceName} ({ServiceId}) registered with Consul successfully",
            _options.ServiceName, _options.ServiceId);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Deregistering service {ServiceId} from Consul",
            _options.ServiceId);

        try
        {
            await _consulClient.Agent.ServiceDeregister(_options.ServiceId, cancellationToken);
            _logger.LogInformation("Service {ServiceId} deregistered from Consul", _options.ServiceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deregistering service {ServiceId} from Consul", _options.ServiceId);
        }
    }

    private static TimeSpan ParseTimeSpan(string value)
    {
        if (value.EndsWith("s", StringComparison.OrdinalIgnoreCase))
        {
            if (int.TryParse(value[..^1], out var seconds))
                return TimeSpan.FromSeconds(seconds);
        }
        else if (value.EndsWith("m", StringComparison.OrdinalIgnoreCase))
        {
            if (int.TryParse(value[..^1], out var minutes))
                return TimeSpan.FromMinutes(minutes);
        }
        else if (value.EndsWith("ms", StringComparison.OrdinalIgnoreCase))
        {
            if (int.TryParse(value[..^2], out var milliseconds))
                return TimeSpan.FromMilliseconds(milliseconds);
        }

        return TimeSpan.FromSeconds(10);
    }
}
