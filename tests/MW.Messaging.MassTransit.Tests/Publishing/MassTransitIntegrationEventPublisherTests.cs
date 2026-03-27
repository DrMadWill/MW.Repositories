using FluentAssertions;
using Moq;
using MassTransit;
using MW.Messaging.Contracts;
using MW.Messaging.Context;
using MW.Messaging.MassTransit.Publishing;
using MW.Messaging.Messaging;
using MW.Messaging.Validation;

namespace MW.Messaging.MassTransit.Tests.Publishing;

public class MassTransitIntegrationEventPublisherTests
{
    [Fact]
    public void Constructor_Should_ThrowOnNullPublishEndpoint()
    {
        var act = () => new MassTransitIntegrationEventPublisher(
            null!,
            Mock.Of<IPublishContextProvider>());

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("publishEndpoint");
    }

    [Fact]
    public void Constructor_Should_ThrowOnNullContextProvider()
    {
        var act = () => new MassTransitIntegrationEventPublisher(
            Mock.Of<IPublishEndpoint>(),
            null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("contextProvider");
    }

    [Fact]
    public void Constructor_Should_AcceptNullValidator()
    {
        var publisher = new MassTransitIntegrationEventPublisher(
            Mock.Of<IPublishEndpoint>(),
            Mock.Of<IPublishContextProvider>());

        publisher.Should().NotBeNull();
    }

    [Fact]
    public async Task PublishAsync_Should_ThrowOnNullEvent()
    {
        var publisher = new MassTransitIntegrationEventPublisher(
            Mock.Of<IPublishEndpoint>(),
            Mock.Of<IPublishContextProvider>(p => p.Create() == new PublishContextModel()));

        var act = () => publisher.PublishAsync((IIntegrationEvent)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task PublishAsync_WithContext_Should_ThrowOnNullEvent()
    {
        var publisher = new MassTransitIntegrationEventPublisher(
            Mock.Of<IPublishEndpoint>(),
            Mock.Of<IPublishContextProvider>());

        var act = () => publisher.PublishAsync(null!, new PublishContextModel());

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task PublishAsync_WithContext_Should_ThrowOnNullContext()
    {
        var publisher = new MassTransitIntegrationEventPublisher(
            Mock.Of<IPublishEndpoint>(),
            Mock.Of<IPublishContextProvider>());

        var mockEvent = Mock.Of<IIntegrationEvent>();

        var act = () => publisher.PublishAsync(mockEvent, (PublishContextModel)null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task PublishAsync_Should_InvokeValidatorBeforePublish()
    {
        var validator = new Mock<IIntegrationEventValidator>();
        var publishEndpoint = new Mock<IPublishEndpoint>();
        var mockEvent = Mock.Of<IIntegrationEvent>(e => e.EventName == "TestEvent" && e.EventVersion == "1.0");

        var publisher = new MassTransitIntegrationEventPublisher(
            publishEndpoint.Object,
            Mock.Of<IPublishContextProvider>(p => p.Create() == new PublishContextModel()),
            validator.Object);

        await publisher.PublishAsync(mockEvent);

        validator.Verify(v => v.Validate(mockEvent), Times.Once);
    }

    [Fact]
    public async Task PublishAsync_Should_FailFastWhenValidationFails()
    {
        var validator = new Mock<IIntegrationEventValidator>();
        validator.Setup(v => v.Validate(It.IsAny<IIntegrationEvent>()))
            .Throws(new InvalidOperationException("Invalid event"));

        var publishEndpoint = new Mock<IPublishEndpoint>();
        var mockEvent = Mock.Of<IIntegrationEvent>(e => e.EventName == "TestEvent" && e.EventVersion == "1.0");

        var publisher = new MassTransitIntegrationEventPublisher(
            publishEndpoint.Object,
            Mock.Of<IPublishContextProvider>(p => p.Create() == new PublishContextModel()),
            validator.Object);

        var act = () => publisher.PublishAsync(mockEvent);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Invalid event");

        publishEndpoint.Verify(
            p => p.Publish(It.IsAny<object>(), It.IsAny<Type>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task PublishAsync_Should_PublishWhenNoValidatorRegistered()
    {
        var publishEndpoint = new Mock<IPublishEndpoint>();
        var mockEvent = Mock.Of<IIntegrationEvent>(e => e.EventName == "TestEvent" && e.EventVersion == "1.0");

        var publisher = new MassTransitIntegrationEventPublisher(
            publishEndpoint.Object,
            Mock.Of<IPublishContextProvider>(p => p.Create() == new PublishContextModel()));

        await publisher.PublishAsync(mockEvent);

        publishEndpoint.Verify(
            p => p.Publish(mockEvent, mockEvent.GetType(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
