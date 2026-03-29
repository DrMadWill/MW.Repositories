using FluentAssertions;
using Moq;
using MassTransit;
using MW.Saga.Context;
using MW.Saga.Contracts;
using MW.Saga.MassTransit.Context;
using MW.Saga.MassTransit.Filters;
using MW.Saga.Models;
using MtHeaders = global::MassTransit.Headers;
using MessageHeaders = MW.Messaging.Headers.MessageHeaders;

namespace MW.Saga.MassTransit.Tests.Filters;

public class TestSagaState : SagaStateMachineInstance, ISagaState
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = "Processing";
}

public class TestSagaMessage { }

public class SimpleSaga : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
}

public class SagaContextPopulationFilterTests
{
    private readonly ScopedSagaContextAccessor _accessor = new();
    private readonly Guid _correlationId = Guid.NewGuid();

    private Mock<SagaConsumeContext<TestSagaState, TestSagaMessage>> CreateContextMock(
        TestSagaState? saga = null,
        string? sourceService = null)
    {
        saga ??= new TestSagaState { CorrelationId = _correlationId, CurrentState = "Processing" };

        var headersMock = new Mock<MtHeaders>();

        if (sourceService is not null)
        {
            object? headerVal = sourceService;
            headersMock
                .Setup(h => h.TryGetHeader(MessageHeaders.SourceService, out headerVal))
                .Returns(true);
        }

        var contextMock = new Mock<SagaConsumeContext<TestSagaState, TestSagaMessage>>();
        contextMock.Setup(c => c.CorrelationId).Returns(_correlationId);
        contextMock.Setup(c => c.Saga).Returns(saga);
        contextMock.Setup(c => c.Headers).Returns(headersMock.Object);

        return contextMock;
    }

    private static Mock<IPipe<SagaConsumeContext<TestSagaState, TestSagaMessage>>> CreateNextPipeMock(
        Exception? throwOnSend = null)
    {
        var pipeMock = new Mock<IPipe<SagaConsumeContext<TestSagaState, TestSagaMessage>>>();

        if (throwOnSend is not null)
        {
            pipeMock
                .Setup(p => p.Send(It.IsAny<SagaConsumeContext<TestSagaState, TestSagaMessage>>()))
                .ThrowsAsync(throwOnSend);
        }
        else
        {
            pipeMock
                .Setup(p => p.Send(It.IsAny<SagaConsumeContext<TestSagaState, TestSagaMessage>>()))
                .Returns(Task.CompletedTask);
        }

        return pipeMock;
    }

    [Fact]
    public void Constructor_Should_ThrowOnNullAccessor()
    {
        var act = () => new SagaContextPopulationFilter<TestSagaState, TestSagaMessage>(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("accessor");
    }

    [Fact]
    public async Task Send_Should_PopulateSagaContext()
    {
        var filter = new SagaContextPopulationFilter<TestSagaState, TestSagaMessage>(_accessor);
        var contextMock = CreateContextMock();
        ISagaContext? captured = null;

        var pipeMock = new Mock<IPipe<SagaConsumeContext<TestSagaState, TestSagaMessage>>>();
        pipeMock
            .Setup(p => p.Send(It.IsAny<SagaConsumeContext<TestSagaState, TestSagaMessage>>()))
            .Callback(() => captured = _accessor.Current)
            .Returns(Task.CompletedTask);

        await filter.Send(contextMock.Object, pipeMock.Object);

        captured.Should().NotBeNull();
        captured!.CorrelationId.Should().Be(_correlationId);
        captured.CurrentState.Should().Be("Processing");
        captured.Status.Should().Be(SagaStatus.Running);
        ((MutableSagaContext)captured).SagaName.Should().Be(nameof(TestSagaState));
    }

    [Fact]
    public async Task Send_Should_ClearContextAfterExecution()
    {
        var filter = new SagaContextPopulationFilter<TestSagaState, TestSagaMessage>(_accessor);
        var contextMock = CreateContextMock();
        var pipeMock = CreateNextPipeMock();

        await filter.Send(contextMock.Object, pipeMock.Object);

        _accessor.Current.Should().BeNull();
    }

    [Fact]
    public async Task Send_Should_ClearContextEvenOnException()
    {
        var filter = new SagaContextPopulationFilter<TestSagaState, TestSagaMessage>(_accessor);
        var contextMock = CreateContextMock();
        var pipeMock = CreateNextPipeMock(throwOnSend: new InvalidOperationException("boom"));

        var act = () => filter.Send(contextMock.Object, pipeMock.Object);

        await act.Should().ThrowAsync<InvalidOperationException>();
        _accessor.Current.Should().BeNull();
    }

    [Fact]
    public async Task Send_Should_SetSourceService_FromHeaders()
    {
        var filter = new SagaContextPopulationFilter<TestSagaState, TestSagaMessage>(_accessor);
        var contextMock = CreateContextMock(sourceService: "test-service");
        ISagaContext? captured = null;

        var pipeMock = new Mock<IPipe<SagaConsumeContext<TestSagaState, TestSagaMessage>>>();
        pipeMock
            .Setup(p => p.Send(It.IsAny<SagaConsumeContext<TestSagaState, TestSagaMessage>>()))
            .Callback(() => captured = _accessor.Current)
            .Returns(Task.CompletedTask);

        await filter.Send(contextMock.Object, pipeMock.Object);

        captured.Should().NotBeNull();
        captured!.SourceService.Should().Be("test-service");
    }

    [Fact]
    public async Task Send_Should_PopulateSagaName_FromType()
    {
        var filter = new SagaContextPopulationFilter<TestSagaState, TestSagaMessage>(_accessor);
        var contextMock = CreateContextMock();
        ISagaContext? captured = null;

        var pipeMock = new Mock<IPipe<SagaConsumeContext<TestSagaState, TestSagaMessage>>>();
        pipeMock
            .Setup(p => p.Send(It.IsAny<SagaConsumeContext<TestSagaState, TestSagaMessage>>()))
            .Callback(() => captured = _accessor.Current)
            .Returns(Task.CompletedTask);

        await filter.Send(contextMock.Object, pipeMock.Object);

        captured.Should().NotBeNull();
        ((MutableSagaContext)captured!).SagaName.Should().Be(nameof(TestSagaState));
    }

    [Fact]
    public async Task Send_Should_UseEmptyState_WhenSagaDoesNotImplementISagaState()
    {
        var accessor = new ScopedSagaContextAccessor();
        var filter = new SagaContextPopulationFilter<SimpleSaga, TestSagaMessage>(accessor);

        var saga = new SimpleSaga { CorrelationId = _correlationId };

        var headersMock = new Mock<MtHeaders>();
        var contextMock = new Mock<SagaConsumeContext<SimpleSaga, TestSagaMessage>>();
        contextMock.Setup(c => c.CorrelationId).Returns(_correlationId);
        contextMock.Setup(c => c.Saga).Returns(saga);
        contextMock.Setup(c => c.Headers).Returns(headersMock.Object);

        ISagaContext? captured = null;
        var pipeMock = new Mock<IPipe<SagaConsumeContext<SimpleSaga, TestSagaMessage>>>();
        pipeMock
            .Setup(p => p.Send(It.IsAny<SagaConsumeContext<SimpleSaga, TestSagaMessage>>()))
            .Callback(() => captured = accessor.Current)
            .Returns(Task.CompletedTask);

        await filter.Send(contextMock.Object, pipeMock.Object);

        captured.Should().NotBeNull();
        captured!.CurrentState.Should().BeEmpty();
    }

    [Fact]
    public void Probe_Should_CreateFilterScope()
    {
        var filter = new SagaContextPopulationFilter<TestSagaState, TestSagaMessage>(_accessor);
        var scopeMock = new Mock<ProbeContext>();
        var probeMock = new Mock<ProbeContext>();
        probeMock.Setup(p => p.CreateScope(It.IsAny<string>())).Returns(scopeMock.Object);

        filter.Probe(probeMock.Object);

        probeMock.Verify(p => p.CreateScope("filters"), Times.Once);
        scopeMock.Verify(s => s.Add("filterType", "sagaContextPopulation"), Times.Once);
    }
}
