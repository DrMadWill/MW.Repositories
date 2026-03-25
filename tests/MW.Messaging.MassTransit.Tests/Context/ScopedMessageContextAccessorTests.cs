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
}
