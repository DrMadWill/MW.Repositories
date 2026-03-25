using FluentAssertions;
using MW.Messaging.MassTransit.Observers;
using Microsoft.Extensions.Logging;
using Moq;

namespace MW.Messaging.MassTransit.Tests.Observers;

public class BusLifecycleObserverTests
{
    [Fact]
    public void Constructor_Should_ThrowOnNullLogger()
    {
        var act = () => new BusLifecycleObserver(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_Should_CreateWithLogger()
    {
        var logger = Mock.Of<ILogger<BusLifecycleObserver>>();

        var observer = new BusLifecycleObserver(logger);

        observer.Should().NotBeNull();
        observer.Should().BeAssignableTo<global::MassTransit.IBusObserver>();
    }
}
