using FluentAssertions;
using Moq;
using MassTransit;
using MW.Saga.Correlation;
using MW.Saga.MassTransit.Correlation;
using MW.Saga.MassTransit.Tests.Filters;

namespace MW.Saga.MassTransit.Tests.Correlation;

public class SagaCorrelationBridgeTests
{
    [Fact]
    public void CorrelateUsing_Should_ThrowOnNullConfigurator()
    {
        var resolverMock = new Mock<ISagaCorrelationResolver<TestSagaMessage>>();

        var act = () => SagaCorrelationBridge.CorrelateUsing<TestSagaState, TestSagaMessage>(null!, resolverMock.Object);

        act.Should().Throw<ArgumentNullException>().WithParameterName("configurator");
    }

    [Fact]
    public void CorrelateUsing_Should_ThrowOnNullResolver()
    {
        var configuratorMock = new Mock<IEventCorrelationConfigurator<TestSagaState, TestSagaMessage>>();

        var act = () => SagaCorrelationBridge.CorrelateUsing(configuratorMock.Object, (ISagaCorrelationResolver<TestSagaMessage>)null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("resolver");
    }

    [Fact]
    public void CorrelateUsing_Should_InvokeCorrelateById()
    {
        var configuratorMock = new Mock<IEventCorrelationConfigurator<TestSagaState, TestSagaMessage>>();
        var resolverMock = new Mock<ISagaCorrelationResolver<TestSagaMessage>>();

        SagaCorrelationBridge.CorrelateUsing(configuratorMock.Object, resolverMock.Object);

        configuratorMock.Verify(
            c => c.CorrelateById(It.IsAny<Func<ConsumeContext<TestSagaMessage>, Guid>>()),
            Times.Once);
    }
}
