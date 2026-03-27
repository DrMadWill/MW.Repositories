using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MW.Messaging.Identity;
using MW.Messaging.MassTransit.Extensions;

namespace MW.Messaging.MassTransit.Tests.Identity;

public class MissingServiceIdentityTests
{
    [Fact]
    public void AddMassTransitMessaging_Should_RegisterServiceIdentityProvider_WhenServiceNameMissing()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddMassTransitMessaging(options =>
        {
            options.Options.RabbitMq.Host = "localhost";
            options.Options.EnableHealthChecks = false;
            // ServiceName not set — defaults to empty string
        });

        var descriptor = services.FirstOrDefault(
            d => d.ServiceType == typeof(IServiceIdentityProvider));

        descriptor.Should().NotBeNull("a safe default should always be registered");
        descriptor!.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }

    [Fact]
    public void SafeDefault_Should_ReturnEmptyServiceName_WhenNotConfigured()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddMassTransitMessaging(options =>
        {
            options.Options.RabbitMq.Host = "localhost";
            options.Options.EnableHealthChecks = false;
        });

        var sp = services.BuildServiceProvider();
        var provider = sp.GetRequiredService<IServiceIdentityProvider>();

        var identity = provider.GetCurrent();
        identity.Should().NotBeNull();
        identity.ServiceName.Should().BeEmpty();
    }

    [Fact]
    public void SafeDefault_Should_ReturnConfiguredServiceName_WhenSet()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddMassTransitMessaging(options =>
        {
            options.Options.RabbitMq.Host = "localhost";
            options.Options.ServiceName = "my-service";
            options.Options.EnableHealthChecks = false;
        });

        var sp = services.BuildServiceProvider();
        var provider = sp.GetRequiredService<IServiceIdentityProvider>();

        var identity = provider.GetCurrent();
        identity.ServiceName.Should().Be("my-service");
    }

    [Fact]
    public void SafeDefault_Should_NotThrow_WhenServiceNameIsNull()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddMassTransitMessaging(options =>
        {
            options.Options.RabbitMq.Host = "localhost";
            options.Options.ServiceName = null!;
            options.Options.EnableHealthChecks = false;
        });

        var sp = services.BuildServiceProvider();
        var provider = sp.GetRequiredService<IServiceIdentityProvider>();

        var act = () => provider.GetCurrent();
        act.Should().NotThrow();
    }

    [Fact]
    public void SafeDefault_Should_NotThrow_WhenServiceNameIsWhitespace()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddMassTransitMessaging(options =>
        {
            options.Options.RabbitMq.Host = "localhost";
            options.Options.ServiceName = "   ";
            options.Options.EnableHealthChecks = false;
        });

        var sp = services.BuildServiceProvider();
        var provider = sp.GetRequiredService<IServiceIdentityProvider>();

        var identity = provider.GetCurrent();
        identity.Should().NotBeNull();
    }
}
