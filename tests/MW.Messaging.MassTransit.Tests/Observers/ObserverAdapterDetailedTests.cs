using FluentAssertions;
using Moq;
using MW.Messaging.MassTransit.Observers;
using MW.Messaging.Observability;
using MW.Messaging.Constants;
using MassTransit;

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

    private static PublishContext<T> CreatePublishContext<T>(Guid? messageId = null) where T : class, new()
    {
        var mock = new Mock<PublishContext<T>>();
        mock.Setup(c => c.MessageId).Returns(messageId ?? Guid.NewGuid());
        mock.Setup(c => c.Message).Returns(new T());

        var headersMock = new Mock<SendHeaders>();
        headersMock.Setup(h => h.TryGetHeader(It.IsAny<string>(), out It.Ref<object?>.IsAny)).Returns(false);
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

    private static ConsumeContext<T> CreateConsumeContext<T>() where T : class, new()
    {
        var mock = new Mock<ConsumeContext<T>>();
        mock.Setup(c => c.MessageId).Returns(Guid.NewGuid());
        mock.Setup(c => c.Message).Returns(new T());

        var headersMock = new Mock<global::MassTransit.Headers>();
        headersMock.Setup(h => h.TryGetHeader(It.IsAny<string>(), out It.Ref<object?>.IsAny)).Returns(false);
        mock.Setup(c => c.Headers).Returns(headersMock.Object);

        var receiveContextMock = new Mock<ReceiveContext>();
        receiveContextMock.Setup(r => r.InputAddress).Returns(new Uri("rabbitmq://localhost/test-endpoint"));
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

    private static SendContext<T> CreateSendContext<T>() where T : class, new()
    {
        var mock = new Mock<SendContext<T>>();
        mock.Setup(c => c.MessageId).Returns(Guid.NewGuid());
        mock.Setup(c => c.Message).Returns(new T());

        var headersMock = new Mock<SendHeaders>();
        headersMock.Setup(h => h.TryGetHeader(It.IsAny<string>(), out It.Ref<object?>.IsAny)).Returns(false);
        mock.Setup(c => c.Headers).Returns(headersMock.Object);

        return mock.Object;
    }

    public class TestMessage { }
}
