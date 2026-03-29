using FluentAssertions;
using Moq;
using MW.Saga.MassTransit.Observers;
using MW.Saga.Models;
using MW.Saga.Observability;

namespace MW.Saga.MassTransit.Tests.Observers;

public class SagaObserverAdapterTests
{
    private readonly Guid _correlationId = Guid.NewGuid();
    private const string SagaName = "OrderSaga";
    private const string MessageName = "OrderSubmitted";
    private const string TraceId = "trace-123";

    [Fact]
    public void Constructor_Should_HandleNullObserver()
    {
        var act = () => new SagaObserverAdapter(observer: null, logger: null);

        act.Should().NotThrow();
    }

    #region NotifySagaStarted

    [Fact]
    public async Task NotifySagaStarted_Should_ForwardToObserver()
    {
        var mockObserver = new Mock<ISagaObserver>();
        SagaObservabilityContext? captured = null;
        mockObserver
            .Setup(o => o.OnSagaStartedAsync(It.IsAny<SagaObservabilityContext>(), It.IsAny<CancellationToken>()))
            .Callback<SagaObservabilityContext, CancellationToken>((ctx, _) => captured = ctx)
            .Returns(Task.CompletedTask);

        var adapter = new SagaObserverAdapter(mockObserver.Object);

        await adapter.NotifySagaStarted(_correlationId, SagaName, MessageName, TraceId);

        mockObserver.Verify(
            o => o.OnSagaStartedAsync(It.IsAny<SagaObservabilityContext>(), It.IsAny<CancellationToken>()),
            Times.Once);

        captured.Should().NotBeNull();
        captured!.CorrelationId.Should().Be(_correlationId);
        captured.SagaName.Should().Be(SagaName);
        captured.MessageName.Should().Be(MessageName);
        captured.Status.Should().Be(SagaStatus.Running);
        captured.TraceId.Should().Be(TraceId);
    }

    [Fact]
    public async Task NotifySagaStarted_Should_DoNothing_WhenObserverIsNull()
    {
        var adapter = new SagaObserverAdapter(observer: null);

        var act = () => adapter.NotifySagaStarted(_correlationId, SagaName, MessageName, TraceId);

        await act.Should().NotThrowAsync();
    }

    #endregion

    #region NotifyStateTransitioned

    [Fact]
    public async Task NotifyStateTransitioned_Should_ForwardToObserver()
    {
        var mockObserver = new Mock<ISagaObserver>();
        SagaObservabilityContext? capturedContext = null;
        SagaTransitionInfo? capturedTransition = null;
        mockObserver
            .Setup(o => o.OnStateTransitionedAsync(
                It.IsAny<SagaObservabilityContext>(),
                It.IsAny<SagaTransitionInfo>(),
                It.IsAny<CancellationToken>()))
            .Callback<SagaObservabilityContext, SagaTransitionInfo, CancellationToken>((ctx, t, _) =>
            {
                capturedContext = ctx;
                capturedTransition = t;
            })
            .Returns(Task.CompletedTask);

        var adapter = new SagaObserverAdapter(mockObserver.Object);

        await adapter.NotifyStateTransitioned(
            _correlationId, SagaName, "Initial", "Processing", "OrderSubmitted", TraceId);

        mockObserver.Verify(
            o => o.OnStateTransitionedAsync(
                It.IsAny<SagaObservabilityContext>(),
                It.IsAny<SagaTransitionInfo>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        capturedContext.Should().NotBeNull();
        capturedContext!.CorrelationId.Should().Be(_correlationId);
        capturedContext.SagaName.Should().Be(SagaName);
        capturedContext.CurrentState.Should().Be("Processing");
        capturedContext.Status.Should().Be(SagaStatus.Running);
        capturedContext.TraceId.Should().Be(TraceId);

        capturedTransition.Should().NotBeNull();
        capturedTransition!.FromState.Should().Be("Initial");
        capturedTransition.ToState.Should().Be("Processing");
        capturedTransition.TriggeredBy.Should().Be("OrderSubmitted");
    }

    [Fact]
    public async Task NotifyStateTransitioned_Should_DoNothing_WhenObserverIsNull()
    {
        var adapter = new SagaObserverAdapter(observer: null);

        var act = () => adapter.NotifyStateTransitioned(
            _correlationId, SagaName, "Initial", "Processing", "OrderSubmitted", TraceId);

        await act.Should().NotThrowAsync();
    }

    #endregion

    #region NotifySagaCompleted

    [Fact]
    public async Task NotifySagaCompleted_Should_ForwardToObserver()
    {
        var mockObserver = new Mock<ISagaObserver>();
        SagaObservabilityContext? captured = null;
        mockObserver
            .Setup(o => o.OnSagaCompletedAsync(It.IsAny<SagaObservabilityContext>(), It.IsAny<CancellationToken>()))
            .Callback<SagaObservabilityContext, CancellationToken>((ctx, _) => captured = ctx)
            .Returns(Task.CompletedTask);

        var adapter = new SagaObserverAdapter(mockObserver.Object);

        await adapter.NotifySagaCompleted(_correlationId, SagaName, "Final", TraceId);

        mockObserver.Verify(
            o => o.OnSagaCompletedAsync(It.IsAny<SagaObservabilityContext>(), It.IsAny<CancellationToken>()),
            Times.Once);

        captured.Should().NotBeNull();
        captured!.CorrelationId.Should().Be(_correlationId);
        captured.SagaName.Should().Be(SagaName);
        captured.CurrentState.Should().Be("Final");
        captured.Status.Should().Be(SagaStatus.Completed);
        captured.TraceId.Should().Be(TraceId);
    }

    [Fact]
    public async Task NotifySagaCompleted_Should_DoNothing_WhenObserverIsNull()
    {
        var adapter = new SagaObserverAdapter(observer: null);

        var act = () => adapter.NotifySagaCompleted(_correlationId, SagaName, "Final", TraceId);

        await act.Should().NotThrowAsync();
    }

    #endregion

    #region NotifySagaFailed

    [Fact]
    public async Task NotifySagaFailed_Should_ForwardToObserver()
    {
        var mockObserver = new Mock<ISagaObserver>();
        SagaObservabilityContext? captured = null;
        mockObserver
            .Setup(o => o.OnSagaFailedAsync(It.IsAny<SagaObservabilityContext>(), It.IsAny<CancellationToken>()))
            .Callback<SagaObservabilityContext, CancellationToken>((ctx, _) => captured = ctx)
            .Returns(Task.CompletedTask);

        var adapter = new SagaObserverAdapter(mockObserver.Object);

        await adapter.NotifySagaFailed(_correlationId, SagaName, "Processing", MessageName, TraceId);

        mockObserver.Verify(
            o => o.OnSagaFailedAsync(It.IsAny<SagaObservabilityContext>(), It.IsAny<CancellationToken>()),
            Times.Once);

        captured.Should().NotBeNull();
        captured!.CorrelationId.Should().Be(_correlationId);
        captured.SagaName.Should().Be(SagaName);
        captured.CurrentState.Should().Be("Processing");
        captured.MessageName.Should().Be(MessageName);
        captured.Status.Should().Be(SagaStatus.Failed);
        captured.TraceId.Should().Be(TraceId);
    }

    [Fact]
    public async Task NotifySagaFailed_Should_DoNothing_WhenObserverIsNull()
    {
        var adapter = new SagaObserverAdapter(observer: null);

        var act = () => adapter.NotifySagaFailed(_correlationId, SagaName, "Processing", MessageName, TraceId);

        await act.Should().NotThrowAsync();
    }

    #endregion

    #region NotifySagaTimedOut

    [Fact]
    public async Task NotifySagaTimedOut_Should_ForwardToObserver()
    {
        var mockObserver = new Mock<ISagaObserver>();
        SagaObservabilityContext? captured = null;
        mockObserver
            .Setup(o => o.OnSagaTimedOutAsync(It.IsAny<SagaObservabilityContext>(), It.IsAny<CancellationToken>()))
            .Callback<SagaObservabilityContext, CancellationToken>((ctx, _) => captured = ctx)
            .Returns(Task.CompletedTask);

        var adapter = new SagaObserverAdapter(mockObserver.Object);

        await adapter.NotifySagaTimedOut(_correlationId, SagaName, "WaitingForPayment", TraceId);

        mockObserver.Verify(
            o => o.OnSagaTimedOutAsync(It.IsAny<SagaObservabilityContext>(), It.IsAny<CancellationToken>()),
            Times.Once);

        captured.Should().NotBeNull();
        captured!.CorrelationId.Should().Be(_correlationId);
        captured.SagaName.Should().Be(SagaName);
        captured.CurrentState.Should().Be("WaitingForPayment");
        captured.Status.Should().Be(SagaStatus.TimedOut);
        captured.TraceId.Should().Be(TraceId);
    }

    [Fact]
    public async Task NotifySagaTimedOut_Should_DoNothing_WhenObserverIsNull()
    {
        var adapter = new SagaObserverAdapter(observer: null);

        var act = () => adapter.NotifySagaTimedOut(_correlationId, SagaName, "WaitingForPayment", TraceId);

        await act.Should().NotThrowAsync();
    }

    #endregion
}
