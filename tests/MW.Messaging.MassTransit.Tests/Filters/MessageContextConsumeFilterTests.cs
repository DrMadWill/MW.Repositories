using FluentAssertions;
using Moq;
using MassTransit;
using MW.Messaging.MassTransit.Context;
using MW.Messaging.MassTransit.Filters;
using MW.Messaging.Messaging;
using MessageHeaders = MW.Messaging.Headers.MessageHeaders;

namespace MW.Messaging.MassTransit.Tests.Filters;

public class MessageContextConsumeFilterTests
{
    private readonly ScopedMessageContextAccessor _accessor = new();
    private readonly DefaultMessageHeaderMapper _headerMapper = new();

    private MessageContextConsumeFilter<TestMessage> CreateFilter()
    {
        return new MessageContextConsumeFilter<TestMessage>(_accessor, _headerMapper);
    }

    [Fact]
    public void Constructor_Should_ThrowOnNullAccessor()
    {
        var act = () => new MessageContextConsumeFilter<TestMessage>(null!, _headerMapper);
        act.Should().Throw<ArgumentNullException>().WithParameterName("accessor");
    }

    [Fact]
    public void Constructor_Should_ThrowOnNullHeaderMapper()
    {
        var act = () => new MessageContextConsumeFilter<TestMessage>(_accessor, null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("headerMapper");
    }

    [Fact]
    public async Task Send_Should_PopulateMessageContext()
    {
        var filter = CreateFilter();
        var headers = new Dictionary<string, object>
        {
            [MessageHeaders.CorrelationId] = "corr-123",
            [MessageHeaders.SourceService] = "origin-service",
            [MessageHeaders.EventName] = "TestEvent"
        };

        var consumeContext = CreateConsumeContext(headers, Guid.NewGuid());
        ConsumerContextModel? capturedContext = null;

        var nextPipe = new Mock<IPipe<ConsumeContext<TestMessage>>>();
        nextPipe.Setup(p => p.Send(It.IsAny<ConsumeContext<TestMessage>>()))
            .Callback<ConsumeContext<TestMessage>>(_ =>
            {
                capturedContext = _accessor.Current;
            });

        await filter.Send(consumeContext, nextPipe.Object);

        capturedContext.Should().NotBeNull();
        capturedContext!.CorrelationId.Should().Be("corr-123");
        capturedContext.SourceService.Should().Be("origin-service");
        capturedContext.EventName.Should().Be("TestEvent");
    }

    [Fact]
    public async Task Send_Should_ClearContextAfterExecution()
    {
        var filter = CreateFilter();
        var consumeContext = CreateConsumeContext(new Dictionary<string, object>
        {
            [MessageHeaders.CorrelationId] = "corr-456"
        }, Guid.NewGuid());

        var nextPipe = new Mock<IPipe<ConsumeContext<TestMessage>>>();

        await filter.Send(consumeContext, nextPipe.Object);

        _accessor.Current.Should().BeNull();
    }

    [Fact]
    public async Task Send_Should_ClearContextEvenOnException()
    {
        var filter = CreateFilter();
        var consumeContext = CreateConsumeContext(new Dictionary<string, object>(), Guid.NewGuid());

        var nextPipe = new Mock<IPipe<ConsumeContext<TestMessage>>>();
        nextPipe.Setup(p => p.Send(It.IsAny<ConsumeContext<TestMessage>>()))
            .ThrowsAsync(new InvalidOperationException("test error"));

        var act = () => filter.Send(consumeContext, nextPipe.Object);
        await act.Should().ThrowAsync<InvalidOperationException>();

        _accessor.Current.Should().BeNull();
    }

    [Fact]
    public async Task Send_Should_HandleMissingHeaders()
    {
        var filter = CreateFilter();
        var consumeContext = CreateConsumeContext(new Dictionary<string, object>(), Guid.NewGuid());

        ConsumerContextModel? capturedContext = null;
        var nextPipe = new Mock<IPipe<ConsumeContext<TestMessage>>>();
        nextPipe.Setup(p => p.Send(It.IsAny<ConsumeContext<TestMessage>>()))
            .Callback<ConsumeContext<TestMessage>>(_ =>
            {
                capturedContext = _accessor.Current;
            });

        await filter.Send(consumeContext, nextPipe.Object);

        capturedContext.Should().NotBeNull();
        capturedContext!.CorrelationId.Should().BeNull();
        capturedContext.SourceService.Should().BeNull();
        capturedContext.EventName.Should().BeNull();
    }

    [Fact]
    public async Task Send_Should_SetMessageIdFromContext()
    {
        var filter = CreateFilter();
        var messageId = Guid.NewGuid();
        var consumeContext = CreateConsumeContext(new Dictionary<string, object>(), messageId);

        ConsumerContextModel? capturedContext = null;
        var nextPipe = new Mock<IPipe<ConsumeContext<TestMessage>>>();
        nextPipe.Setup(p => p.Send(It.IsAny<ConsumeContext<TestMessage>>()))
            .Callback<ConsumeContext<TestMessage>>(_ =>
            {
                capturedContext = _accessor.Current;
            });

        await filter.Send(consumeContext, nextPipe.Object);

        capturedContext!.MessageId.Should().Be(messageId);
    }

    [Fact]
    public async Task Send_Should_GenerateMessageId_WhenNull()
    {
        var filter = CreateFilter();
        var consumeContext = CreateConsumeContext(new Dictionary<string, object>(), messageId: null);

        ConsumerContextModel? capturedContext = null;
        var nextPipe = new Mock<IPipe<ConsumeContext<TestMessage>>>();
        nextPipe.Setup(p => p.Send(It.IsAny<ConsumeContext<TestMessage>>()))
            .Callback<ConsumeContext<TestMessage>>(_ =>
            {
                capturedContext = _accessor.Current;
            });

        await filter.Send(consumeContext, nextPipe.Object);

        capturedContext.Should().NotBeNull();
        capturedContext!.MessageId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Send_Should_ClearContextAfterException_LeavingNoStaleState()
    {
        var filter = CreateFilter();
        var consumeContext = CreateConsumeContext(new Dictionary<string, object>
        {
            [MessageHeaders.CorrelationId] = "stale-check",
            [MessageHeaders.SourceService] = "stale-service"
        }, Guid.NewGuid());

        var nextPipe = new Mock<IPipe<ConsumeContext<TestMessage>>>();
        nextPipe.Setup(p => p.Send(It.IsAny<ConsumeContext<TestMessage>>()))
            .ThrowsAsync(new InvalidOperationException("pipeline failure"));

        var act = () => filter.Send(consumeContext, nextPipe.Object);
        await act.Should().ThrowAsync<InvalidOperationException>();

        // Verify no stale context remains
        _accessor.Current.Should().BeNull();

        // Verify a fresh send works correctly after failure
        var consumeContext2 = CreateConsumeContext(new Dictionary<string, object>
        {
            [MessageHeaders.CorrelationId] = "fresh"
        }, Guid.NewGuid());

        ConsumerContextModel? freshContext = null;
        var nextPipe2 = new Mock<IPipe<ConsumeContext<TestMessage>>>();
        nextPipe2.Setup(p => p.Send(It.IsAny<ConsumeContext<TestMessage>>()))
            .Callback<ConsumeContext<TestMessage>>(_ => { freshContext = _accessor.Current; });

        await filter.Send(consumeContext2, nextPipe2.Object);

        freshContext.Should().NotBeNull();
        freshContext!.CorrelationId.Should().Be("fresh");
    }

    private static ConsumeContext<TestMessage> CreateConsumeContext(
        IDictionary<string, object> headers, Guid? messageId)
    {
        var mock = new Mock<ConsumeContext<TestMessage>>();
        mock.Setup(c => c.MessageId).Returns(messageId);
        mock.Setup(c => c.Message).Returns(new TestMessage());

        var headerEntries = headers.Select(h => new KeyValuePair<string, object>(h.Key, h.Value)).ToArray();
        var headersMock = new Mock<global::MassTransit.Headers>();
        headersMock.Setup(h => h.GetAll()).Returns(headerEntries);
        mock.Setup(c => c.Headers).Returns(headersMock.Object);

        return mock.Object;
    }

    public class TestMessage { }
}
