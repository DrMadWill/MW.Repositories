using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MW.Messaging.MassTransit.Health;

public class MassTransitBusHealthCheck : IHealthCheck
{
    private readonly IBusControl _busControl;

    public MassTransitBusHealthCheck(IBusControl busControl)
    {
        _busControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var healthResult = _busControl.CheckHealth();

            return healthResult.Status switch
            {
                BusHealthStatus.Healthy => HealthCheckResult.Healthy("MassTransit bus is ready"),
                BusHealthStatus.Degraded => HealthCheckResult.Degraded("MassTransit bus is degraded",
                    data: BuildEndpointData(healthResult)),
                _ => HealthCheckResult.Unhealthy("MassTransit bus is not ready",
                    data: BuildEndpointData(healthResult))
            };
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("MassTransit bus health check failed", ex);
        }
    }

    private static IReadOnlyDictionary<string, object>? BuildEndpointData(BusHealthResult healthResult)
    {
        var data = new Dictionary<string, object>();

        foreach (var endpoint in healthResult.Endpoints)
        {
            data[endpoint.Key] = endpoint.Value.Status.ToString();
        }

        return data.Count > 0 ? data : null;
    }
}
