using FluentAssertions;
using Moq;
using MassTransit;
using MW.Messaging.MassTransit.Context;
using MW.Messaging.MassTransit.Filters;
using MW.Messaging.Messaging;
using MessageHeaders = MW.Messaging.Headers.MessageHeaders;

namespace MW.Messaging.MassTransit.Tests.Filters;

public class HeaderEnrichmentPublishFilterTests
{
    private readonly Mock<MW.Messaging.Context.IPublishContextProvider> _contextProvider = new();
    private readonly DefaultMessageHeaderMapper _headerMapper = new();

    [Fact]
    public void Constructor_Should_ThrowOnNullContextProvider()
    {
        var act = () => new HeaderEnrichmentPublishFilter<TestMessage>(null!, _headerMapper);
        act.Should().Throw<ArgumentNullException>().WithParameterName("contextProvider");
    }

    [Fact]
    public void Constructor_Should_ThrowOnNullHeaderMapper()
    {
        var act = () => new HeaderEnrichmentPublishFilter<TestMessage>(_contextProvider.Object, null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("headerMapper");
    }

    [Fact]
    public async Task Send_Should_EnrichHeadersFromPublishContext()
    {
        _contextProvider.Setup(p => p.Create()).Returns(new PublishContextModel
        {
            CorrelationId = "test-corr",
            CausationId = "test-cause",
            SourceService = "test-service",
            TraceId = "test-trace"
        });

        var setHeaders = new Dictionary<string, object>();
        var publishContext = CreatePublishContext<TestMessage>(
            new TestMessage(), new Dictionary<string, object>(), setHeaders);

        var nextPipe = new Mock<IPipe<PublishContext<TestMessage>>>();
        var filter = new HeaderEnrichmentPublishFilter<TestMessage>(_contextProvider.Object, _headerMapper);

        await filter.Send(publishContext, nextPipe.Object);

        setHeaders.Should().ContainKey(MessageHeaders.CorrelationId);
        setHeaders[MessageHeaders.CorrelationId].Should().Be("test-corr");
        setHeaders.Should().ContainKey(MessageHeaders.SourceService);
        setHeaders[MessageHeaders.SourceService].Should().Be("test-service");
    }

    [Fact]
    public async Task Send_Should_EnrichEventNameAndVersion_ForIntegrationEvent()
    {
        _contextProvider.Setup(p => p.Create()).Returns(new PublishContextModel { SourceService = "svc" });

        var setHeaders = new Dictionary<string, object>();
        var evt = new TestIntegrationEvent("OrderCreated", "1.0");
        var publishContext = CreatePublishContext<TestIntegrationEvent>(
            evt, new Dictionary<string, object>(), setHeaders);

        var nextPipe = new Mock<IPipe<PublishContext<TestIntegrationEvent>>>();
        var filter = new HeaderEnrichmentPublishFilter<TestIntegrationEvent>(_contextProvider.Object, _headerMapper);

        await filter.Send(publishContext, nextPipe.Object);

        setHeaders.Should().ContainKey(MessageHeaders.EventName);
        setHeaders[MessageHeaders.EventName].Should().Be("OrderCreated");
        setHeaders.Should().ContainKey(MessageHeaders.EventVersion);
        setHeaders[MessageHeaders.EventVersion].Should().Be("1.0");
    }

    [Fact]
    public async Task Send_Should_NotOverwriteExistingHeaders()
    {
        _contextProvider.Setup(p => p.Create()).Returns(new PublishContextModel
        {
            CorrelationId = "new-corr",
            SourceService = "new-service"
        });

        var existingHeaders = new Dictionary<string, object>
        {
            [MessageHeaders.CorrelationId] = "existing-corr"
        };
        var setHeaders = new Dictionary<string, object>();
        var publishContext = CreatePublishContext<TestMessage>(
            new TestMessage(), existingHeaders, setHeaders);

        var nextPipe = new Mock<IPipe<PublishContext<TestMessage>>>();
        var filter = new HeaderEnrichmentPublishFilter<TestMessage>(_contextProvider.Object, _headerMapper);

        await filter.Send(publishContext, nextPipe.Object);

        // Should not overwrite existing header
        setHeaders.Should().NotContainKey(MessageHeaders.CorrelationId);
        // Should add new header
        setHeaders.Should().ContainKey(MessageHeaders.SourceService);
    }

    [Fact]
    public async Task Send_Should_CallNextPipe()
    {
        _contextProvider.Setup(p => p.Create()).Returns(new PublishContextModel());

        var publishContext = CreatePublishContext<TestMessage>(
            new TestMessage(), new Dictionary<string, object>(), new Dictionary<string, object>());

        var nextPipe = new Mock<IPipe<PublishContext<TestMessage>>>();
        var filter = new HeaderEnrichmentPublishFilter<TestMessage>(_contextProvider.Object, _headerMapper);

        await filter.Send(publishContext, nextPipe.Object);

        nextPipe.Verify(p => p.Send(publishContext), Times.Once);
    }

    [Fact]
    public async Task Send_Should_HandlePartiallyMissingContext()
    {
        _contextProvider.Setup(p => p.Create()).Returns(new PublishContextModel
        {
            CorrelationId = "only-corr"
        });

        var setHeaders = new Dictionary<string, object>();
        var publishContext = CreatePublishContext<TestMessage>(
            new TestMessage(), new Dictionary<string, object>(), setHeaders);

        var nextPipe = new Mock<IPipe<PublishContext<TestMessage>>>();
        var filter = new HeaderEnrichmentPublishFilter<TestMessage>(_contextProvider.Object, _headerMapper);

        await filter.Send(publishContext, nextPipe.Object);

        setHeaders.Should().ContainKey(MessageHeaders.CorrelationId);
        setHeaders.Should().NotContainKey(MessageHeaders.SourceService);
        setHeaders.Should().NotContainKey(MessageHeaders.TraceId);
    }

    private static PublishContext<T> CreatePublishContext<T>(
        T message,
        Dictionary<string, object> existingHeaders,
        Dictionary<string, object> setHeaders) where T : class
    {
        var headersMock = new Mock<SendHeaders>();
        headersMock.Setup(h => h.TryGetHeader(It.IsAny<string>(), out It.Ref<object?>.IsAny))
            .Returns((string key, out object? value) =>
            {
                if (existingHeaders.TryGetValue(key, out var existing))
                {
                    value = existing;
                    return true;
                }
                if (setHeaders.TryGetValue(key, out var setVal))
                {
                    value = setVal;
                    return true;
                }
                value = null;
                return false;
            });
        headersMock.Setup(h => h.Set(It.IsAny<string>(), It.IsAny<object?>(), It.IsAny<bool>()))
            .Callback<string, object?, bool>((key, value, _) => setHeaders[key] = value!);
        headersMock.Setup(h => h.Set(It.IsAny<string>(), It.IsAny<string?>()))
            .Callback<string, string?>((key, value) => setHeaders[key] = value!);

        var mock = new Mock<PublishContext<T>>();
        mock.Setup(c => c.Message).Returns(message);
        mock.Setup(c => c.Headers).Returns(headersMock.Object);
        mock.As<SendContext>().Setup(c => c.Headers).Returns(headersMock.Object);

        return mock.Object;
    }

    public class TestMessage { }

    public class TestIntegrationEvent : MW.Messaging.Contracts.IIntegrationEvent
    {
        public TestIntegrationEvent(string eventName, string eventVersion)
        {
            EventName = eventName;
            EventVersion = eventVersion;
        }

        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
        public string EventName { get; }
        public string EventVersion { get; }
        public string SourceService => "test";
        public string? CorrelationId { get; init; }
        public string? CausationId { get; init; }
    }
}
