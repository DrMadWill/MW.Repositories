using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using MW.Messaging.MassTransit.Health;

namespace MW.Messaging.MassTransit.Tests.Health;

public class MassTransitBusHealthCheckTests
{
    [Fact]
    public void Constructor_Should_ThrowOnNullBusControl()
    {
        var act = () => new MassTransitBusHealthCheck(null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("busControl");
    }

    [Fact]
    public async Task CheckHealthAsync_Should_ReturnHealthy_WhenBusIsHealthy()
    {
        var busControl = new Mock<IBusControl>();
        busControl.Setup(b => b.CheckHealth())
            .Returns(BusHealthResult.Healthy("test",
                new Dictionary<string, EndpointHealthResult>()));

        var healthCheck = new MassTransitBusHealthCheck(busControl.Object);

        var result = await healthCheck.CheckHealthAsync(CreateContext());

        result.Status.Should().Be(HealthStatus.Healthy);
        result.Description.Should().Contain("ready");
    }

    [Fact]
    public async Task CheckHealthAsync_Should_ReturnDegraded_WhenBusIsDegraded()
    {
        var receiveEndpoint = Mock.Of<IReceiveEndpoint>();
        var endpoints = new Dictionary<string, EndpointHealthResult>
        {
            ["test-endpoint"] = EndpointHealthResult.Degraded(receiveEndpoint, "degraded")
        };
        var busControl = new Mock<IBusControl>();
        busControl.Setup(b => b.CheckHealth())
            .Returns(BusHealthResult.Degraded("degraded",
                new InvalidOperationException("test"), endpoints));

        var healthCheck = new MassTransitBusHealthCheck(busControl.Object);

        var result = await healthCheck.CheckHealthAsync(CreateContext());

        result.Status.Should().Be(HealthStatus.Degraded);
        result.Description.Should().Contain("degraded");
    }

    [Fact]
    public async Task CheckHealthAsync_Should_ReturnUnhealthy_WhenBusIsUnhealthy()
    {
        var receiveEndpoint = Mock.Of<IReceiveEndpoint>();
        var endpoints = new Dictionary<string, EndpointHealthResult>
        {
            ["test-endpoint"] = EndpointHealthResult.Unhealthy(
                receiveEndpoint, "unhealthy", new Exception("fail"))
        };
        var busControl = new Mock<IBusControl>();
        busControl.Setup(b => b.CheckHealth())
            .Returns(BusHealthResult.Unhealthy("unhealthy",
                new Exception("fail"), endpoints));

        var healthCheck = new MassTransitBusHealthCheck(busControl.Object);

        var result = await healthCheck.CheckHealthAsync(CreateContext());

        result.Status.Should().Be(HealthStatus.Unhealthy);
        result.Description.Should().Contain("not ready");
    }

    [Fact]
    public async Task CheckHealthAsync_Should_ReturnUnhealthy_WhenExceptionThrown()
    {
        var busControl = new Mock<IBusControl>();
        busControl.Setup(b => b.CheckHealth())
            .Throws(new InvalidOperationException("Bus not started"));

        var healthCheck = new MassTransitBusHealthCheck(busControl.Object);

        var result = await healthCheck.CheckHealthAsync(CreateContext());

        result.Status.Should().Be(HealthStatus.Unhealthy);
        result.Description.Should().Contain("failed");
        result.Exception.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public async Task CheckHealthAsync_Should_IncludeEndpointData_WhenDegraded()
    {
        var receiveEndpointA = Mock.Of<IReceiveEndpoint>();
        var receiveEndpointB = Mock.Of<IReceiveEndpoint>();
        var endpoints = new Dictionary<string, EndpointHealthResult>
        {
            ["queue-a"] = EndpointHealthResult.Degraded(receiveEndpointA, "degraded"),
            ["queue-b"] = EndpointHealthResult.Degraded(receiveEndpointB, "degraded")
        };
        var busControl = new Mock<IBusControl>();
        busControl.Setup(b => b.CheckHealth())
            .Returns(BusHealthResult.Degraded("degraded",
                new InvalidOperationException("test"), endpoints));

        var healthCheck = new MassTransitBusHealthCheck(busControl.Object);

        var result = await healthCheck.CheckHealthAsync(CreateContext());

        result.Data.Should().NotBeNull();
        result.Data.Should().ContainKey("queue-a");
        result.Data.Should().ContainKey("queue-b");
    }

    private static HealthCheckContext CreateContext()
    {
        return new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", Mock.Of<IHealthCheck>(), null, null)
        };
    }
}
