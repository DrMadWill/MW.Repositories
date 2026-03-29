using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MW.Saga.MassTransit.Observers;
using MW.Saga.Models;
using MW.Saga.Observability;

namespace MW.Saga.MassTransit.Tests.Observers;

public class StructuredSagaLoggerTests
{
    private readonly Mock<ILogger<StructuredSagaLogger>> _mockLogger = new();

    private static SagaObservabilityContext CreateContext(SagaStatus status = SagaStatus.Running) => new()
    {
        CorrelationId = Guid.NewGuid(),
        SagaName = "OrderSaga",
        CurrentState = "Processing",
        Status = status,
        MessageName = "OrderSubmitted",
        TraceId = "trace-456"
    };

    private static SagaTransitionInfo CreateTransition() => new()
    {
        FromState = "Initial",
        ToState = "Processing",
        TriggeredBy = "OrderSubmitted",
        OccurredAt = DateTime.UtcNow
    };

    [Fact]
    public void Constructor_Should_ThrowOnNullLogger()
    {
        var act = () => new StructuredSagaLogger(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("logger");
    }

    [Fact]
    public void Should_ImplementISagaObserver()
    {
        var logger = new StructuredSagaLogger(_mockLogger.Object);

        logger.Should().BeAssignableTo<ISagaObserver>();
    }

    [Fact]
    public async Task OnSagaStartedAsync_Should_NotThrow()
    {
        var logger = new StructuredSagaLogger(_mockLogger.Object);

        var act = () => logger.OnSagaStartedAsync(CreateContext(), CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task OnStateTransitionedAsync_Should_NotThrow()
    {
        var logger = new StructuredSagaLogger(_mockLogger.Object);

        var act = () => logger.OnStateTransitionedAsync(CreateContext(), CreateTransition(), CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task OnSagaCompletedAsync_Should_NotThrow()
    {
        var logger = new StructuredSagaLogger(_mockLogger.Object);

        var act = () => logger.OnSagaCompletedAsync(CreateContext(SagaStatus.Completed), CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task OnSagaFailedAsync_Should_NotThrow()
    {
        var logger = new StructuredSagaLogger(_mockLogger.Object);

        var act = () => logger.OnSagaFailedAsync(CreateContext(SagaStatus.Failed), CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task OnSagaTimedOutAsync_Should_NotThrow()
    {
        var logger = new StructuredSagaLogger(_mockLogger.Object);

        var act = () => logger.OnSagaTimedOutAsync(CreateContext(SagaStatus.TimedOut), CancellationToken.None);

        await act.Should().NotThrowAsync();
    }
}
