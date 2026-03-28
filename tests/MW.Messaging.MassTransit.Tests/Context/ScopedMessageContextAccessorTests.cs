using FluentAssertions;
using MW.Messaging.Context;
using MW.Messaging.MassTransit.Context;
using MW.Messaging.Messaging;

namespace MW.Messaging.MassTransit.Tests.Context;

public class ScopedMessageContextAccessorTests
{
    [Fact]
    public void Current_Should_BeNullByDefault()
    {
        var accessor = new ScopedMessageContextAccessor();

        accessor.Current.Should().BeNull();
    }

    [Fact]
    public void Should_ImplementIMessageContextAccessor()
    {
        var accessor = new ScopedMessageContextAccessor();

        accessor.Should().BeAssignableTo<IMessageContextAccessor>();
    }

    [Fact]
    public void SetContext_Should_StoreContext()
    {
        var accessor = new ScopedMessageContextAccessor();
        var context = new ConsumerContextModel
        {
            CorrelationId = "test-corr",
            SourceService = "test-service"
        };

        accessor.SetContext(context);

        accessor.Current.Should().BeSameAs(context);
        accessor.Current!.CorrelationId.Should().Be("test-corr");
    }

    [Fact]
    public void SetContext_Should_ThrowOnNull()
    {
        var accessor = new ScopedMessageContextAccessor();

        var act = () => accessor.SetContext(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ClearContext_Should_SetCurrentToNull()
    {
        var accessor = new ScopedMessageContextAccessor();
        accessor.SetContext(new ConsumerContextModel { CorrelationId = "test" });

        accessor.ClearContext();

        accessor.Current.Should().BeNull();
    }

    [Fact]
    public void SetContext_Should_OverwritePreviousContext()
    {
        var accessor = new ScopedMessageContextAccessor();
        var first = new ConsumerContextModel { CorrelationId = "first" };
        var second = new ConsumerContextModel { CorrelationId = "second" };

        accessor.SetContext(first);
        accessor.SetContext(second);

        accessor.Current.Should().BeSameAs(second);
        accessor.Current!.CorrelationId.Should().Be("second");
    }

    [Fact]
    public void ClearContext_Should_BeIdempotent()
    {
        var accessor = new ScopedMessageContextAccessor();

        // Clear without setting should not throw
        accessor.ClearContext();
        accessor.ClearContext();

        accessor.Current.Should().BeNull();
    }
}
