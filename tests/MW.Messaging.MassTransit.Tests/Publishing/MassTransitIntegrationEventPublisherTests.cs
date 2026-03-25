using FluentAssertions;
using Moq;
using MassTransit;
using MW.Messaging.Contracts;
using MW.Messaging.Context;
using MW.Messaging.MassTransit.Publishing;
using MW.Messaging.Messaging;

namespace MW.Messaging.MassTransit.Tests.Publishing;

public class MassTransitIntegrationEventPublisherTests
{
    [Fact]
    public void Constructor_Should_ThrowOnNullPublishEndpoint()
    {
        var act = () => new MassTransitIntegrationEventPublisher(
            null!,
            Mock.Of<IPublishContextProvider>(),
            Mock.Of<IMessageHeaderMapper>());

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("publishEndpoint");
    }

    [Fact]
    public void Constructor_Should_ThrowOnNullContextProvider()
    {
        var act = () => new MassTransitIntegrationEventPublisher(
            Mock.Of<IPublishEndpoint>(),
            null!,
            Mock.Of<IMessageHeaderMapper>());

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("contextProvider");
    }

    [Fact]
    public void Constructor_Should_ThrowOnNullHeaderMapper()
    {
        var act = () => new MassTransitIntegrationEventPublisher(
            Mock.Of<IPublishEndpoint>(),
            Mock.Of<IPublishContextProvider>(),
            null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("headerMapper");
    }

    [Fact]
    public async Task PublishAsync_Should_ThrowOnNullEvent()
    {
        var publisher = new MassTransitIntegrationEventPublisher(
            Mock.Of<IPublishEndpoint>(),
            Mock.Of<IPublishContextProvider>(p => p.Create() == new PublishContextModel()),
            Mock.Of<IMessageHeaderMapper>(m => m.MapToHeaders(It.IsAny<PublishContextModel>()) == new Dictionary<string, object>()));

        var act = () => publisher.PublishAsync((IIntegrationEvent)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task PublishAsync_WithContext_Should_ThrowOnNullEvent()
    {
        var publisher = new MassTransitIntegrationEventPublisher(
            Mock.Of<IPublishEndpoint>(),
            Mock.Of<IPublishContextProvider>(),
            Mock.Of<IMessageHeaderMapper>(m => m.MapToHeaders(It.IsAny<PublishContextModel>()) == new Dictionary<string, object>()));

        var act = () => publisher.PublishAsync(null!, new PublishContextModel());

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task PublishAsync_WithContext_Should_ThrowOnNullContext()
    {
        var publisher = new MassTransitIntegrationEventPublisher(
            Mock.Of<IPublishEndpoint>(),
            Mock.Of<IPublishContextProvider>(),
            Mock.Of<IMessageHeaderMapper>());

        var mockEvent = Mock.Of<IIntegrationEvent>();

        var act = () => publisher.PublishAsync(mockEvent, (PublishContextModel)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
