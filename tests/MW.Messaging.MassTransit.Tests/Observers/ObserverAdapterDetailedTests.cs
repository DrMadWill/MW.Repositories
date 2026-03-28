using FluentAssertions;
using Moq;
using MW.Messaging.MassTransit.Observers;
using MW.Messaging.Observability;
using MW.Messaging.Constants;
using MassTransit;
using MessageHeaders = MW.Messaging.Headers.MessageHeaders;

namespace MW.Messaging.MassTransit.Tests.Observers;

public class PublishObserverAdapterTests
{
    [Fact]
    public async Task PrePublish_Should_ForwardToObserver()
    {
        var observer = new Mock<MW.Messaging.MassTransit.IPublishObserver>();
        var adapter = new MassTransitPublishObserverAdapter(observer.Object);

        var publishContext = CreatePublishContext<TestMessage>();

        await adapter.PrePublish(publishContext);

        observer.Verify(o => o.OnPrePublish(It.Is<MessageLogContext>(
            ctx => ctx.Status == EventStatuses.Pending)), Times.Once);
    }

    [Fact]
    public async Task PostPublish_Should_ForwardToObserver()
    {
        var observer = new Mock<MW.Messaging.MassTransit.IPublishObserver>();
        var adapter = new MassTransitPublishObserverAdapter(observer.Object);

        var publishContext = CreatePublishContext<TestMessage>();

        await adapter.PostPublish(publishContext);

        observer.Verify(o => o.OnPostPublish(It.Is<MessageLogContext>(
            ctx => ctx.Status == EventStatuses.Success)), Times.Once);
    }

    [Fact]
    public async Task PublishFault_Should_ForwardToObserver()
    {
        var observer = new Mock<MW.Messaging.MassTransit.IPublishObserver>();
        var adapter = new MassTransitPublishObserverAdapter(observer.Object);
        var exception = new InvalidOperationException("test");

        var publishContext = CreatePublishContext<TestMessage>();

        await adapter.PublishFault(publishContext, exception);

        observer.Verify(o => o.OnPublishFault(
            It.Is<MessageLogContext>(ctx => ctx.Status == EventStatuses.Failed),
            exception), Times.Once);
    }

    [Fact]
    public async Task PrePublish_Should_SkipWhenNoObserver()
    {
        var adapter = new MassTransitPublishObserverAdapter(null);
        var publishContext = CreatePublishContext<TestMessage>();

        // Should not throw
        await adapter.PrePublish(publishContext);
    }

    [Fact]
    public async Task PrePublish_Should_PopulateMessageLogContext()
    {
        MessageLogContext? capturedCtx = null;
        var observer = new Mock<MW.Messaging.MassTransit.IPublishObserver>();
        observer.Setup(o => o.OnPrePublish(It.IsAny<MessageLogContext>()))
            .Callback<MessageLogContext>(ctx => capturedCtx = ctx);

        var adapter = new MassTransitPublishObserverAdapter(observer.Object);
        var messageId = Guid.NewGuid();
        var publishContext = CreatePublishContext<TestMessage>(messageId);

        await adapter.PrePublish(publishContext);

        capturedCtx.Should().NotBeNull();
        capturedCtx!.MessageId.Should().Be(messageId);
        capturedCtx.EventName.Should().Be(nameof(TestMessage));
    }

    [Fact]
    public async Task PrePublish_Should_ExtractHeaderValues()
    {
        MessageLogContext? capturedCtx = null;
        var observer = new Mock<MW.Messaging.MassTransit.IPublishObserver>();
        observer.Setup(o => o.OnPrePublish(It.IsAny<MessageLogContext>()))
            .Callback<MessageLogContext>(ctx => capturedCtx = ctx);

        var adapter = new MassTransitPublishObserverAdapter(observer.Object);
        var headers = new Dictionary<string, object>
        {
            [MessageHeaders.CorrelationId] = "pub-corr-123",
            [MessageHeaders.CausationId] = "pub-cause-456",
            [MessageHeaders.TraceId] = "pub-trace-789",
            [MessageHeaders.SourceService] = "pub-service",
            [MessageHeaders.EventName] = "OrderCreated",
            [MessageHeaders.EventVersion] = "1.0"
        };

        var publishContext = CreatePublishContext<TestMessage>(Guid.NewGuid(), headers);

        await adapter.PrePublish(publishContext);

        capturedCtx.Should().NotBeNull();
        capturedCtx!.CorrelationId.Should().Be("pub-corr-123");
        capturedCtx.CausationId.Should().Be("pub-cause-456");
        capturedCtx.TraceId.Should().Be("pub-trace-789");
        capturedCtx.SourceService.Should().Be("pub-service");
        capturedCtx.EventName.Should().Be("OrderCreated");
        capturedCtx.EventVersion.Should().Be("1.0");
    }

    [Fact]
    public async Task PrePublish_Should_PopulateTimestamp()
    {
        MessageLogContext? capturedCtx = null;
        var observer = new Mock<MW.Messaging.MassTransit.IPublishObserver>();
        observer.Setup(o => o.OnPrePublish(It.IsAny<MessageLogContext>()))
            .Callback<MessageLogContext>(ctx => capturedCtx = ctx);

        var adapter = new MassTransitPublishObserverAdapter(observer.Object);
        var before = DateTimeOffset.UtcNow;

        await adapter.PrePublish(CreatePublishContext<TestMessage>());

        var after = DateTimeOffset.UtcNow;
        capturedCtx!.Timestamp.Should().BeOnOrAfter(before);
        capturedCtx.Timestamp.Should().BeOnOrBefore(after);
    }

    [Fact]
    public async Task PrePublish_Should_PopulateEndpoint()
    {
        MessageLogContext? capturedCtx = null;
        var observer = new Mock<MW.Messaging.MassTransit.IPublishObserver>();
        observer.Setup(o => o.OnPrePublish(It.IsAny<MessageLogContext>()))
            .Callback<MessageLogContext>(ctx => capturedCtx = ctx);

        var adapter = new MassTransitPublishObserverAdapter(observer.Object);
        var publishContext = CreatePublishContext<TestMessage>(destinationAddress: new Uri("rabbitmq://localhost/test-queue"));

        await adapter.PrePublish(publishContext);

        capturedCtx!.Endpoint.Should().Be("rabbitmq://localhost/test-queue");
    }

    [Fact]
    public async Task PrePublish_Should_FallbackEventNameToTypeName_WhenNoHeader()
    {
        MessageLogContext? capturedCtx = null;
        var observer = new Mock<MW.Messaging.MassTransit.IPublishObserver>();
        observer.Setup(o => o.OnPrePublish(It.IsAny<MessageLogContext>()))
            .Callback<MessageLogContext>(ctx => capturedCtx = ctx);

        var adapter = new MassTransitPublishObserverAdapter(observer.Object);

        // No EventName header - should fallback to type name
        await adapter.PrePublish(CreatePublishContext<TestMessage>());

        capturedCtx!.EventName.Should().Be(nameof(TestMessage));
    }

    private static PublishContext<T> CreatePublishContext<T>(
        Guid? messageId = null,
        Dictionary<string, object>? headerValues = null,
        Uri? destinationAddress = null) where T : class, new()
    {
        var mock = new Mock<PublishContext<T>>();
        mock.Setup(c => c.MessageId).Returns(messageId ?? Guid.NewGuid());
        mock.Setup(c => c.Message).Returns(new T());
        mock.Setup(c => c.DestinationAddress).Returns(destinationAddress);

        var headersMock = new Mock<SendHeaders>();
        headersMock.Setup(h => h.TryGetHeader(It.IsAny<string>(), out It.Ref<object?>.IsAny))
            .Returns((string key, out object? value) =>
            {
                if (headerValues != null && headerValues.TryGetValue(key, out var val))
                {
                    value = val;
                    return true;
                }
                value = null;
                return false;
            });
        mock.Setup(c => c.Headers).Returns(headersMock.Object);

        return mock.Object;
    }

    public class TestMessage { }
}

public class ConsumeObserverAdapterTests
{
    [Fact]
    public async Task PreConsume_Should_ForwardToObserver()
    {
        var observer = new Mock<MW.Messaging.MassTransit.IConsumeObserver>();
        var adapter = new MassTransitConsumeObserverAdapter(observer.Object);

        var consumeContext = CreateConsumeContext<TestMessage>();

        await adapter.PreConsume(consumeContext);

        observer.Verify(o => o.OnPreConsume(It.Is<MessageLogContext>(
            ctx => ctx.Status == EventStatuses.Pending)), Times.Once);
    }

    [Fact]
    public async Task PostConsume_Should_ForwardToObserver()
    {
        var observer = new Mock<MW.Messaging.MassTransit.IConsumeObserver>();
        var adapter = new MassTransitConsumeObserverAdapter(observer.Object);

        var consumeContext = CreateConsumeContext<TestMessage>();

        await adapter.PostConsume(consumeContext);

        observer.Verify(o => o.OnPostConsume(It.Is<MessageLogContext>(
            ctx => ctx.Status == EventStatuses.Success)), Times.Once);
    }

    [Fact]
    public async Task ConsumeFault_Should_ForwardToObserver()
    {
        var observer = new Mock<MW.Messaging.MassTransit.IConsumeObserver>();
        var adapter = new MassTransitConsumeObserverAdapter(observer.Object);
        var exception = new InvalidOperationException("test");

        var consumeContext = CreateConsumeContext<TestMessage>();

        await adapter.ConsumeFault(consumeContext, exception);

        observer.Verify(o => o.OnConsumeFault(
            It.Is<MessageLogContext>(ctx => ctx.Status == EventStatuses.Failed),
            exception), Times.Once);
    }

    [Fact]
    public async Task PreConsume_Should_SkipWhenNoObserver()
    {
        var adapter = new MassTransitConsumeObserverAdapter(null);
        var consumeContext = CreateConsumeContext<TestMessage>();

        // Should not throw
        await adapter.PreConsume(consumeContext);
    }

    [Fact]
    public async Task PreConsume_Should_PopulateEndpointMetadata()
    {
        MessageLogContext? capturedCtx = null;
        var observer = new Mock<MW.Messaging.MassTransit.IConsumeObserver>();
        observer.Setup(o => o.OnPreConsume(It.IsAny<MessageLogContext>()))
            .Callback<MessageLogContext>(ctx => capturedCtx = ctx);

        var adapter = new MassTransitConsumeObserverAdapter(observer.Object);
        var consumeContext = CreateConsumeContext<TestMessage>();

        await adapter.PreConsume(consumeContext);

        capturedCtx.Should().NotBeNull();
        capturedCtx!.Endpoint.Should().NotBeNull();
    }

    [Fact]
    public async Task PreConsume_Should_ExtractConsumerName()
    {
        MessageLogContext? capturedCtx = null;
        var observer = new Mock<MW.Messaging.MassTransit.IConsumeObserver>();
        observer.Setup(o => o.OnPreConsume(It.IsAny<MessageLogContext>()))
            .Callback<MessageLogContext>(ctx => capturedCtx = ctx);

        var adapter = new MassTransitConsumeObserverAdapter(observer.Object);
        var consumeContext = CreateConsumeContext<TestMessage>(
            inputAddress: new Uri("rabbitmq://localhost/order-events"));

        await adapter.PreConsume(consumeContext);

        capturedCtx.Should().NotBeNull();
        capturedCtx!.Consumer.Should().Be("order-events");
    }

    [Fact]
    public async Task PreConsume_Should_ExtractHeaderValues()
    {
        MessageLogContext? capturedCtx = null;
        var observer = new Mock<MW.Messaging.MassTransit.IConsumeObserver>();
        observer.Setup(o => o.OnPreConsume(It.IsAny<MessageLogContext>()))
            .Callback<MessageLogContext>(ctx => capturedCtx = ctx);

        var adapter = new MassTransitConsumeObserverAdapter(observer.Object);
        var headers = new Dictionary<string, object>
        {
            [MessageHeaders.CorrelationId] = "consume-corr",
            [MessageHeaders.CausationId] = "consume-cause",
            [MessageHeaders.TraceId] = "consume-trace",
            [MessageHeaders.SourceService] = "consume-service",
            [MessageHeaders.EventName] = "OrderPlaced",
            [MessageHeaders.EventVersion] = "2.0"
        };

        var consumeContext = CreateConsumeContext<TestMessage>(headerValues: headers);

        await adapter.PreConsume(consumeContext);

        capturedCtx.Should().NotBeNull();
        capturedCtx!.CorrelationId.Should().Be("consume-corr");
        capturedCtx.CausationId.Should().Be("consume-cause");
        capturedCtx.TraceId.Should().Be("consume-trace");
        capturedCtx.SourceService.Should().Be("consume-service");
        capturedCtx.EventName.Should().Be("OrderPlaced");
        capturedCtx.EventVersion.Should().Be("2.0");
    }

    [Fact]
    public async Task PreConsume_Should_PopulateTimestamp()
    {
        MessageLogContext? capturedCtx = null;
        var observer = new Mock<MW.Messaging.MassTransit.IConsumeObserver>();
        observer.Setup(o => o.OnPreConsume(It.IsAny<MessageLogContext>()))
            .Callback<MessageLogContext>(ctx => capturedCtx = ctx);

        var adapter = new MassTransitConsumeObserverAdapter(observer.Object);
        var before = DateTimeOffset.UtcNow;

        await adapter.PreConsume(CreateConsumeContext<TestMessage>());

        var after = DateTimeOffset.UtcNow;
        capturedCtx!.Timestamp.Should().BeOnOrAfter(before);
        capturedCtx.Timestamp.Should().BeOnOrBefore(after);
    }

    private static ConsumeContext<T> CreateConsumeContext<T>(
        Uri? inputAddress = null,
        Dictionary<string, object>? headerValues = null) where T : class, new()
    {
        var mock = new Mock<ConsumeContext<T>>();
        mock.Setup(c => c.MessageId).Returns(Guid.NewGuid());
        mock.Setup(c => c.Message).Returns(new T());

        var headersMock = new Mock<global::MassTransit.Headers>();
        headersMock.Setup(h => h.TryGetHeader(It.IsAny<string>(), out It.Ref<object?>.IsAny))
            .Returns((string key, out object? value) =>
            {
                if (headerValues != null && headerValues.TryGetValue(key, out var val))
                {
                    value = val;
                    return true;
                }
                value = null;
                return false;
            });
        mock.Setup(c => c.Headers).Returns(headersMock.Object);

        var receiveContextMock = new Mock<ReceiveContext>();
        receiveContextMock.Setup(r => r.InputAddress).Returns(inputAddress ?? new Uri("rabbitmq://localhost/test-endpoint"));
        mock.Setup(c => c.ReceiveContext).Returns(receiveContextMock.Object);

        return mock.Object;
    }

    public class TestMessage { }
}

public class SendObserverAdapterTests
{
    [Fact]
    public async Task PreSend_Should_ForwardToObserver()
    {
        var observer = new Mock<MW.Messaging.MassTransit.ISendObserver>();
        var adapter = new MassTransitSendObserverAdapter(observer.Object);

        var sendContext = CreateSendContext<TestMessage>();

        await adapter.PreSend(sendContext);

        observer.Verify(o => o.OnPreSend(It.Is<MessageLogContext>(
            ctx => ctx.Status == EventStatuses.Pending)), Times.Once);
    }

    [Fact]
    public async Task PostSend_Should_ForwardToObserver()
    {
        var observer = new Mock<MW.Messaging.MassTransit.ISendObserver>();
        var adapter = new MassTransitSendObserverAdapter(observer.Object);

        var sendContext = CreateSendContext<TestMessage>();

        await adapter.PostSend(sendContext);

        observer.Verify(o => o.OnPostSend(It.Is<MessageLogContext>(
            ctx => ctx.Status == EventStatuses.Success)), Times.Once);
    }

    [Fact]
    public async Task SendFault_Should_ForwardToObserver()
    {
        var observer = new Mock<MW.Messaging.MassTransit.ISendObserver>();
        var adapter = new MassTransitSendObserverAdapter(observer.Object);
        var exception = new InvalidOperationException("test");

        var sendContext = CreateSendContext<TestMessage>();

        await adapter.SendFault(sendContext, exception);

        observer.Verify(o => o.OnSendFault(
            It.Is<MessageLogContext>(ctx => ctx.Status == EventStatuses.Failed),
            exception), Times.Once);
    }

    [Fact]
    public async Task PreSend_Should_SkipWhenNoObserver()
    {
        var adapter = new MassTransitSendObserverAdapter(null);
        var sendContext = CreateSendContext<TestMessage>();

        // Should not throw
        await adapter.PreSend(sendContext);
    }

    [Fact]
    public async Task PreSend_Should_ExtractHeaderValues()
    {
        MessageLogContext? capturedCtx = null;
        var observer = new Mock<MW.Messaging.MassTransit.ISendObserver>();
        observer.Setup(o => o.OnPreSend(It.IsAny<MessageLogContext>()))
            .Callback<MessageLogContext>(ctx => capturedCtx = ctx);

        var adapter = new MassTransitSendObserverAdapter(observer.Object);
        var headers = new Dictionary<string, object>
        {
            [MessageHeaders.CorrelationId] = "send-corr",
            [MessageHeaders.SourceService] = "send-service",
            [MessageHeaders.EventName] = "SendEvent"
        };

        var sendContext = CreateSendContext<TestMessage>(headerValues: headers);

        await adapter.PreSend(sendContext);

        capturedCtx.Should().NotBeNull();
        capturedCtx!.CorrelationId.Should().Be("send-corr");
        capturedCtx.SourceService.Should().Be("send-service");
        capturedCtx.EventName.Should().Be("SendEvent");
    }

    [Fact]
    public async Task PreSend_Should_PopulateEndpoint()
    {
        MessageLogContext? capturedCtx = null;
        var observer = new Mock<MW.Messaging.MassTransit.ISendObserver>();
        observer.Setup(o => o.OnPreSend(It.IsAny<MessageLogContext>()))
            .Callback<MessageLogContext>(ctx => capturedCtx = ctx);

        var adapter = new MassTransitSendObserverAdapter(observer.Object);
        var sendContext = CreateSendContext<TestMessage>(
            destinationAddress: new Uri("rabbitmq://localhost/send-queue"));

        await adapter.PreSend(sendContext);

        capturedCtx!.Endpoint.Should().Be("rabbitmq://localhost/send-queue");
    }

    [Fact]
    public async Task PreSend_Should_PopulateTimestamp()
    {
        MessageLogContext? capturedCtx = null;
        var observer = new Mock<MW.Messaging.MassTransit.ISendObserver>();
        observer.Setup(o => o.OnPreSend(It.IsAny<MessageLogContext>()))
            .Callback<MessageLogContext>(ctx => capturedCtx = ctx);

        var adapter = new MassTransitSendObserverAdapter(observer.Object);
        var before = DateTimeOffset.UtcNow;

        await adapter.PreSend(CreateSendContext<TestMessage>());

        var after = DateTimeOffset.UtcNow;
        capturedCtx!.Timestamp.Should().BeOnOrAfter(before);
        capturedCtx.Timestamp.Should().BeOnOrBefore(after);
    }

    private static SendContext<T> CreateSendContext<T>(
        Dictionary<string, object>? headerValues = null,
        Uri? destinationAddress = null) where T : class, new()
    {
        var mock = new Mock<SendContext<T>>();
        mock.Setup(c => c.MessageId).Returns(Guid.NewGuid());
        mock.Setup(c => c.Message).Returns(new T());
        mock.Setup(c => c.DestinationAddress).Returns(destinationAddress);

        var headersMock = new Mock<SendHeaders>();
        headersMock.Setup(h => h.TryGetHeader(It.IsAny<string>(), out It.Ref<object?>.IsAny))
            .Returns((string key, out object? value) =>
            {
                if (headerValues != null && headerValues.TryGetValue(key, out var val))
                {
                    value = val;
                    return true;
                }
                value = null;
                return false;
            });
        mock.Setup(c => c.Headers).Returns(headersMock.Object);

        return mock.Object;
    }

    public class TestMessage { }
}
