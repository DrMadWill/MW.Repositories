using FluentAssertions;
using Moq;
using MW.Messaging.MassTransit.Observers;
using MW.Messaging.Observability;

namespace MW.Messaging.MassTransit.Tests.Observers;

public class ObserverAdapterTests
{
    [Fact]
    public void PublishObserverAdapter_Should_HandleNullObserver()
    {
        var adapter = new MassTransitPublishObserverAdapter(null);

        // Should not throw
        adapter.Should().NotBeNull();
    }

    [Fact]
    public void ConsumeObserverAdapter_Should_HandleNullObserver()
    {
        var adapter = new MassTransitConsumeObserverAdapter(null);

        adapter.Should().NotBeNull();
    }

    [Fact]
    public void SendObserverAdapter_Should_HandleNullObserver()
    {
        var adapter = new MassTransitSendObserverAdapter(null);

        adapter.Should().NotBeNull();
    }

    [Fact]
    public void PublishObserverAdapter_Should_ImplementMassTransitInterface()
    {
        var adapter = new MassTransitPublishObserverAdapter(null);

        adapter.Should().BeAssignableTo<global::MassTransit.IPublishObserver>();
    }

    [Fact]
    public void ConsumeObserverAdapter_Should_ImplementMassTransitInterface()
    {
        var adapter = new MassTransitConsumeObserverAdapter(null);

        adapter.Should().BeAssignableTo<global::MassTransit.IConsumeObserver>();
    }

    [Fact]
    public void SendObserverAdapter_Should_ImplementMassTransitInterface()
    {
        var adapter = new MassTransitSendObserverAdapter(null);

        adapter.Should().BeAssignableTo<global::MassTransit.ISendObserver>();
    }
}
