using FluentAssertions;
using Moq;
using MassTransit;
using MW.Messaging.Contracts;
using MW.Messaging.Context;
using MW.Messaging.MassTransit.Publishing;
using MW.Messaging.Messaging;
using MW.Messaging.Validation;

namespace MW.Messaging.MassTransit.Tests.Publishing;

public class PublishContextOverloadTests
{
    [Fact]
    public async Task PublishAsync_WithExplicitContext_Should_PublishEventSuccessfully()
    {
        var publishEndpoint = new Mock<IPublishEndpoint>();
        var contextProvider = new Mock<IPublishContextProvider>();
        var mockEvent = Mock.Of<IIntegrationEvent>(
            e => e.EventName == "TestEvent" && e.EventVersion == "1.0");

        var publisher = new MassTransitIntegrationEventPublisher(
            publishEndpoint.Object,
            contextProvider.Object);

        var explicitContext = new PublishContextModel
        {
            CorrelationId = "explicit-corr",
            SourceService = "explicit-service"
        };

        await publisher.PublishAsync(mockEvent, explicitContext);

        // Publisher should publish the event
        publishEndpoint.Verify(
            p => p.Publish(mockEvent, mockEvent.GetType(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task PublishAsync_WithExplicitContext_Should_NotCallContextProvider()
    {
        var publishEndpoint = new Mock<IPublishEndpoint>();
        var contextProvider = new Mock<IPublishContextProvider>();
        var mockEvent = Mock.Of<IIntegrationEvent>(
            e => e.EventName == "TestEvent" && e.EventVersion == "1.0");

        var publisher = new MassTransitIntegrationEventPublisher(
            publishEndpoint.Object,
            contextProvider.Object);

        var explicitContext = new PublishContextModel
        {
            CorrelationId = "explicit-corr"
        };

        await publisher.PublishAsync(mockEvent, explicitContext);

        // When explicit context is provided, context provider should NOT be called
        contextProvider.Verify(p => p.Create(), Times.Never);
    }

    [Fact]
    public async Task PublishAsync_WithoutContext_Should_CallContextProvider()
    {
        var publishEndpoint = new Mock<IPublishEndpoint>();
        var contextProvider = new Mock<IPublishContextProvider>();
        contextProvider.Setup(p => p.Create()).Returns(new PublishContextModel());
        var mockEvent = Mock.Of<IIntegrationEvent>(
            e => e.EventName == "TestEvent" && e.EventVersion == "1.0");

        var publisher = new MassTransitIntegrationEventPublisher(
            publishEndpoint.Object,
            contextProvider.Object);

        await publisher.PublishAsync(mockEvent);

        // Without explicit context, context provider should be called
        contextProvider.Verify(p => p.Create(), Times.Once);
    }

    [Fact]
    public async Task PublishAsync_WithExplicitContext_Should_StillValidate()
    {
        var publishEndpoint = new Mock<IPublishEndpoint>();
        var validator = new Mock<IIntegrationEventValidator>();
        var mockEvent = Mock.Of<IIntegrationEvent>(
            e => e.EventName == "TestEvent" && e.EventVersion == "1.0");

        var publisher = new MassTransitIntegrationEventPublisher(
            publishEndpoint.Object,
            Mock.Of<IPublishContextProvider>(),
            validator.Object);

        var explicitContext = new PublishContextModel { CorrelationId = "test" };

        await publisher.PublishAsync(mockEvent, explicitContext);

        validator.Verify(v => v.Validate(mockEvent), Times.Once);
    }

    [Fact]
    public async Task PublishAsync_WithExplicitContext_Should_FailFast_WhenValidationFails()
    {
        var publishEndpoint = new Mock<IPublishEndpoint>();
        var validator = new Mock<IIntegrationEventValidator>();
        validator.Setup(v => v.Validate(It.IsAny<IIntegrationEvent>()))
            .Throws(new InvalidOperationException("Validation failed"));

        var mockEvent = Mock.Of<IIntegrationEvent>(
            e => e.EventName == "TestEvent" && e.EventVersion == "1.0");

        var publisher = new MassTransitIntegrationEventPublisher(
            publishEndpoint.Object,
            Mock.Of<IPublishContextProvider>(),
            validator.Object);

        var explicitContext = new PublishContextModel { CorrelationId = "test" };

        var act = () => publisher.PublishAsync(mockEvent, explicitContext);

        await act.Should().ThrowAsync<InvalidOperationException>();
        publishEndpoint.Verify(
            p => p.Publish(It.IsAny<object>(), It.IsAny<Type>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
