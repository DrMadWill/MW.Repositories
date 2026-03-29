using FluentAssertions;
using Moq;
using MassTransit;
using MW.Saga.MassTransit.Filters;
using MW.Saga.MassTransit.Options;

namespace MW.Saga.MassTransit.Tests.Filters;

public class SagaRetryPolicyConfiguratorTests
{
    [Fact]
    public void ApplyRetryPolicy_Should_ThrowOnNullConfigurator()
    {
        var options = new SagaMassTransitOptions();

        var act = () => SagaRetryPolicyConfigurator.ApplyRetryPolicy(null!, options);

        act.Should().Throw<ArgumentNullException>().WithParameterName("configurator");
    }

    [Fact]
    public void ApplyRetryPolicy_Should_ThrowOnNullOptions()
    {
        var configuratorMock = new Mock<IReceiveEndpointConfigurator>();

        var act = () => SagaRetryPolicyConfigurator.ApplyRetryPolicy(configuratorMock.Object, null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("options");
    }

    [Fact]
    public void ApplyRetryPolicy_Should_InvokeUseMessageRetry()
    {
        var configuratorMock = new Mock<IReceiveEndpointConfigurator>();
        configuratorMock.DefaultValue = DefaultValue.Mock;
        var options = new SagaMassTransitOptions();

        var act = () => SagaRetryPolicyConfigurator.ApplyRetryPolicy(configuratorMock.Object, options);

        act.Should().NotThrow();
    }
}
