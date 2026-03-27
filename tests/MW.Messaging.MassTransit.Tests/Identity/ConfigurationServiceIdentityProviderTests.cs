using FluentAssertions;
using MW.Messaging.MassTransit.Identity;
using MW.Messaging.MassTransit.Options;
using MW.Messaging.Identity;

namespace MW.Messaging.MassTransit.Tests.Identity;

public class ConfigurationServiceIdentityProviderTests
{
    [Fact]
    public void Constructor_Should_ThrowOnNullOptions()
    {
        var act = () => new ConfigurationServiceIdentityProvider(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetCurrent_Should_ReturnServiceName()
    {
        var options = new MassTransitOptions { ServiceName = "order-service" };
        var provider = new ConfigurationServiceIdentityProvider(options);

        var identity = provider.GetCurrent();

        identity.ServiceName.Should().Be("order-service");
    }

    [Fact]
    public void GetCurrent_Should_ReturnEmptyWhenServiceNameIsEmpty()
    {
        var options = new MassTransitOptions { ServiceName = string.Empty };
        var provider = new ConfigurationServiceIdentityProvider(options);

        var identity = provider.GetCurrent();

        identity.ServiceName.Should().BeEmpty();
    }

    [Fact]
    public void GetCurrent_Should_ReturnStableIdentity()
    {
        var options = new MassTransitOptions { ServiceName = "stable-service" };
        var provider = new ConfigurationServiceIdentityProvider(options);

        var identity1 = provider.GetCurrent();
        var identity2 = provider.GetCurrent();

        identity1.ServiceName.Should().Be(identity2.ServiceName);
    }

    [Fact]
    public void Should_ImplementIServiceIdentityProvider()
    {
        var provider = new ConfigurationServiceIdentityProvider(new MassTransitOptions());
        provider.Should().BeAssignableTo<IServiceIdentityProvider>();
    }
}
