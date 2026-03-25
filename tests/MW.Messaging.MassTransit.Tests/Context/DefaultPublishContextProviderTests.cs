using FluentAssertions;
using Moq;
using MW.Messaging.Correlation;
using MW.Messaging.Identity;
using MW.Messaging.MassTransit.Context;
using MW.Messaging.Messaging;

namespace MW.Messaging.MassTransit.Tests.Context;

public class DefaultPublishContextProviderTests
{
    [Fact]
    public void Create_Should_UseCorrelationContext()
    {
        var correlationContext = new Mock<ICorrelationContext>();
        correlationContext.Setup(c => c.CorrelationId).Returns("corr-123");
        correlationContext.Setup(c => c.CausationId).Returns("cause-456");
        correlationContext.Setup(c => c.TraceId).Returns("trace-789");

        var provider = new DefaultPublishContextProvider(correlationContext.Object);

        var result = provider.Create();

        result.CorrelationId.Should().Be("corr-123");
        result.CausationId.Should().Be("cause-456");
        result.TraceId.Should().Be("trace-789");
    }

    [Fact]
    public void Create_Should_UseServiceIdentityProvider()
    {
        var identityProvider = new Mock<IServiceIdentityProvider>();
        identityProvider.Setup(p => p.GetCurrent()).Returns(new ServiceIdentity
        {
            ServiceName = "my-service",
            ServiceVersion = "1.0.0"
        });

        var provider = new DefaultPublishContextProvider(serviceIdentityProvider: identityProvider.Object);

        var result = provider.Create();

        result.SourceService.Should().Be("my-service");
    }

    [Fact]
    public void Create_Should_GenerateCorrelationIdWhenMissing()
    {
        var provider = new DefaultPublishContextProvider();

        var result = provider.Create();

        result.CorrelationId.Should().NotBeNullOrWhiteSpace();
        Guid.TryParse(result.CorrelationId, out _).Should().BeTrue();
    }

    [Fact]
    public void Create_Should_HandleNullDependenciesGracefully()
    {
        var provider = new DefaultPublishContextProvider();

        var result = provider.Create();

        result.Should().NotBeNull();
        result.CorrelationId.Should().NotBeNullOrWhiteSpace();
        result.SourceService.Should().BeEmpty();
    }
}
