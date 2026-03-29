using FluentAssertions;
using Moq;
using MassTransit;
using MW.Saga.Contracts;
using MW.Saga.MassTransit.Context;
using MW.Saga.MassTransit.Filters;
using MtHeaders = global::MassTransit.Headers;
using MessageHeaders = MW.Messaging.Headers.MessageHeaders;

namespace MW.Saga.MassTransit.Tests.Integration;

public class RegressionSagaState : SagaStateMachineInstance, ISagaState
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = "TestState";
}

public class RegressionMessage { }

public class SagaContextCleanupRegressionTests
{
    private readonly ScopedSagaContextAccessor _accessor = new();
    private readonly Guid _correlationId = Guid.NewGuid();

    private Mock<SagaConsumeContext<RegressionSagaState, RegressionMessage>> CreateContextMock()
    {
        var saga = new RegressionSagaState
        {
            CorrelationId = _correlationId,
            CurrentState = "TestState"
        };

        var headersMock = new Mock<MtHeaders>();
        var contextMock = new Mock<SagaConsumeContext<RegressionSagaState, RegressionMessage>>();
        contextMock.Setup(c => c.CorrelationId).Returns(_correlationId);
        contextMock.Setup(c => c.Saga).Returns(saga);
        contextMock.Setup(c => c.Headers).Returns(headersMock.Object);

        return contextMock;
    }

    [Fact]
    public async Task ContextCleanup_Should_ClearAfterSuccess()
    {
        var filter = new SagaContextPopulationFilter<RegressionSagaState, RegressionMessage>(_accessor);
        var contextMock = CreateContextMock();

        var pipeMock = new Mock<IPipe<SagaConsumeContext<RegressionSagaState, RegressionMessage>>>();
        pipeMock
            .Setup(p => p.Send(It.IsAny<SagaConsumeContext<RegressionSagaState, RegressionMessage>>()))
            .Returns(Task.CompletedTask);

        await filter.Send(contextMock.Object, pipeMock.Object);

        _accessor.Current.Should().BeNull("context must be cleared after successful execution");
    }

    [Fact]
    public async Task ContextCleanup_Should_ClearAfterException()
    {
        var filter = new SagaContextPopulationFilter<RegressionSagaState, RegressionMessage>(_accessor);
        var contextMock = CreateContextMock();

        var pipeMock = new Mock<IPipe<SagaConsumeContext<RegressionSagaState, RegressionMessage>>>();
        pipeMock
            .Setup(p => p.Send(It.IsAny<SagaConsumeContext<RegressionSagaState, RegressionMessage>>()))
            .ThrowsAsync(new InvalidOperationException("simulated failure"));

        var act = () => filter.Send(contextMock.Object, pipeMock.Object);

        await act.Should().ThrowAsync<InvalidOperationException>();
        _accessor.Current.Should().BeNull("context must be cleared even when an exception occurs");
    }

    [Fact]
    public async Task ContextCleanup_Should_NotLeaveStaleContext_AfterMultipleFailures()
    {
        var filter = new SagaContextPopulationFilter<RegressionSagaState, RegressionMessage>(_accessor);

        var pipeMock = new Mock<IPipe<SagaConsumeContext<RegressionSagaState, RegressionMessage>>>();
        pipeMock
            .Setup(p => p.Send(It.IsAny<SagaConsumeContext<RegressionSagaState, RegressionMessage>>()))
            .ThrowsAsync(new InvalidOperationException("repeated failure"));

        for (var i = 0; i < 3; i++)
        {
            var contextMock = CreateContextMock();

            var act = () => filter.Send(contextMock.Object, pipeMock.Object);

            await act.Should().ThrowAsync<InvalidOperationException>();
            _accessor.Current.Should().BeNull(
                $"context must be cleared after failure iteration {i + 1}");
        }
    }
}
