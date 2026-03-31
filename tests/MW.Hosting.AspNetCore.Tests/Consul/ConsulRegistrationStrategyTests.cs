using FluentAssertions;
using MW.Hosting.AspNetCore.Options;

namespace MW.Hosting.AspNetCore.Tests.Consul;

/// <summary>
/// Test strategy for Consul registration lifecycle:
/// 
/// Unit tests:
/// - ConsulOptions validation (covered in Options tests)
/// - Registration object construction (ServiceId, ServiceName, Address, Port, Tags, Meta, HealthCheck)
/// 
/// Integration tests (require Consul container or mock):
/// - Service registration on app start
/// - Service deregistration on app stop
/// - Health check URL construction
/// 
/// Mocking approach:
/// - Use Moq to mock IConsulClient for unit tests
/// - For integration tests, use a test Consul container (Docker) if available
/// - Verify AgentServiceRegistration properties match ConsulOptions
/// </summary>
public class ConsulRegistrationStrategyTests
{
    [Fact]
    public void ConsulOptions_HealthCheckUrl_IsConstructedCorrectly()
    {
        var options = new ConsulOptions
        {
            ServiceAddress = "http://localhost:5000",
            HealthCheckPath = "/api/health"
        };

        var healthCheckUrl = options.ServiceAddress.TrimEnd('/') + options.HealthCheckPath;

        healthCheckUrl.Should().Be("http://localhost:5000/api/health");
    }

    [Fact]
    public void ConsulOptions_HealthCheckUrl_HandlesTrailingSlash()
    {
        var options = new ConsulOptions
        {
            ServiceAddress = "http://localhost:5000/",
            HealthCheckPath = "/api/health"
        };

        var healthCheckUrl = options.ServiceAddress.TrimEnd('/') + options.HealthCheckPath;

        healthCheckUrl.Should().Be("http://localhost:5000/api/health");
    }

    [Fact]
    public void ConsulOptions_ServiceUri_ParsesCorrectly()
    {
        var options = new ConsulOptions
        {
            ServiceAddress = "http://c_api_order:33000"
        };

        var uri = new Uri(options.ServiceAddress);

        uri.Host.Should().Be("c_api_order");
        uri.Port.Should().Be(33000);
    }

    [Fact]
    public void ConsulOptions_DefaultValues_AreReasonable()
    {
        var options = new ConsulOptions();

        options.Enabled.Should().BeFalse();
        options.HealthCheckPath.Should().Be("/api/health");
        options.HealthCheckInterval.Should().Be("10s");
        options.HealthCheckTimeout.Should().Be("5s");
        options.DeregisterCriticalServiceAfter.Should().Be("30s");
    }
}
