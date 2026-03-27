using FluentAssertions;
using MW.Messaging.MassTransit.Context;
using MW.Messaging.Context;
using MW.Messaging.Messaging;

namespace MW.Messaging.MassTransit.Tests.Context;

public class MassTransitMessageExecutionContextTests
{
    [Fact]
    public void Constructor_Should_ThrowOnNullAccessor()
    {
        var act = () => new MassTransitMessageExecutionContext(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_ImplementIMessageExecutionContext()
    {
        var accessor = new ScopedMessageContextAccessor();
        var context = new MassTransitMessageExecutionContext(accessor);
        context.Should().BeAssignableTo<IMessageExecutionContext>();
    }

    [Fact]
    public void Should_MapAllFieldsFromConsumerContext()
    {
        var accessor = new ScopedMessageContextAccessor();
        var messageId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        accessor.SetContext(new ConsumerContextModel
        {
            MessageId = messageId,
            CorrelationId = "corr-123",
            CausationId = "cause-456",
            TraceId = "trace-789",
            UserId = userId,
            TenantId = tenantId,
            SourceService = "origin-service"
        });

        var context = new MassTransitMessageExecutionContext(accessor);

        context.MessageId.Should().Be(messageId);
        context.CorrelationId.Should().Be("corr-123");
        context.CausationId.Should().Be("cause-456");
        context.TraceId.Should().Be("trace-789");
        context.UserId.Should().Be(userId);
        context.TenantId.Should().Be(tenantId);
        context.SourceService.Should().Be("origin-service");
    }

    [Fact]
    public void Should_ReturnDefaultsWhenNoContextSet()
    {
        var accessor = new ScopedMessageContextAccessor();
        var context = new MassTransitMessageExecutionContext(accessor);

        context.CorrelationId.Should().BeNull();
        context.CausationId.Should().BeNull();
        context.TraceId.Should().BeNull();
        context.UserId.Should().BeNull();
        context.TenantId.Should().BeNull();
        context.SourceService.Should().BeNull();
        context.MessageId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void Should_ReflectContextChanges()
    {
        var accessor = new ScopedMessageContextAccessor();
        var context = new MassTransitMessageExecutionContext(accessor);

        context.CorrelationId.Should().BeNull();

        accessor.SetContext(new ConsumerContextModel
        {
            CorrelationId = "updated-corr"
        });

        context.CorrelationId.Should().Be("updated-corr");
    }
}
