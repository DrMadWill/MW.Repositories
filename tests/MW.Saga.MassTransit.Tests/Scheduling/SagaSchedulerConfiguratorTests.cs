using FluentAssertions;
using Moq;
using MassTransit;
using MW.Saga.MassTransit.Options;
using MW.Saga.MassTransit.Scheduling;

namespace MW.Saga.MassTransit.Tests.Scheduling;

public class SagaSchedulerConfiguratorTests
{
    [Fact]
    public void ConfigureScheduler_Should_ThrowOnNullConfigurator()
    {
        var options = new SagaMassTransitOptions();

        var act = () => SagaSchedulerConfigurator.ConfigureScheduler(null!, options);

        act.Should().Throw<ArgumentNullException>().WithParameterName("configurator");
    }

    [Fact]
    public void ConfigureScheduler_Should_ThrowOnNullOptions()
    {
        var configurator = new Mock<IBusFactoryConfigurator>().Object;

        var act = () => SagaSchedulerConfigurator.ConfigureScheduler(configurator, null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("options");
    }

    [Fact]
    public void ConfigureScheduler_Should_NotThrow_WhenSchedulerEnabled()
    {
        var configurator = new Mock<IBusFactoryConfigurator>().Object;
        var options = new SagaMassTransitOptions { UseScheduler = true };

        var act = () => SagaSchedulerConfigurator.ConfigureScheduler(configurator, options);

        act.Should().NotThrow();
    }

    [Fact]
    public void ConfigureScheduler_Should_NotThrow_WhenSchedulerDisabled()
    {
        var configurator = new Mock<IBusFactoryConfigurator>().Object;
        var options = new SagaMassTransitOptions { UseScheduler = false };

        var act = () => SagaSchedulerConfigurator.ConfigureScheduler(configurator, options);

        act.Should().NotThrow();
    }
}
