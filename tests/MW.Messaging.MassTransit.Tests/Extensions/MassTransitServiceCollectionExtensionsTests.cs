using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MW.Messaging.Context;
using MW.Messaging.Identity;
using MW.Messaging.MassTransit.Context;
using MW.Messaging.MassTransit.Extensions;
using MW.Messaging.Publishing;

namespace MW.Messaging.MassTransit.Tests.Extensions;

public class MassTransitServiceCollectionExtensionsTests
{
    [Fact]
    public void AddMassTransitMessaging_Should_RegisterIMessageContextAccessor()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddMassTransitMessaging(options =>
        {
            options.Options.RabbitMq.Host = "localhost";
            options.Options.EnableHealthChecks = false;
        });

        var descriptor = services.FirstOrDefault(
            d => d.ServiceType == typeof(IMessageContextAccessor));

        descriptor.Should().NotBeNull();
        descriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddMassTransitMessaging_Should_RegisterIPublishContextProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddMassTransitMessaging(options =>
        {
            options.Options.RabbitMq.Host = "localhost";
            options.Options.EnableHealthChecks = false;
        });

        var descriptor = services.FirstOrDefault(
            d => d.ServiceType == typeof(IPublishContextProvider));

        descriptor.Should().NotBeNull();
        descriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddMassTransitMessaging_Should_RegisterIIntegrationEventPublisher()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddMassTransitMessaging(options =>
        {
            options.Options.RabbitMq.Host = "localhost";
            options.Options.EnableHealthChecks = false;
        });

        var descriptor = services.FirstOrDefault(
            d => d.ServiceType == typeof(IIntegrationEventPublisher));

        descriptor.Should().NotBeNull();
        descriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddMassTransitMessaging_Should_RegisterIMessageHeaderMapper()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddMassTransitMessaging(options =>
        {
            options.Options.RabbitMq.Host = "localhost";
            options.Options.EnableHealthChecks = false;
        });

        var descriptor = services.FirstOrDefault(
            d => d.ServiceType == typeof(IMessageHeaderMapper));

        descriptor.Should().NotBeNull();
        descriptor!.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddMassTransitMessaging_Should_RegisterScopedMessageContextAccessor()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddMassTransitMessaging(options =>
        {
            options.Options.RabbitMq.Host = "localhost";
            options.Options.EnableHealthChecks = false;
        });

        var descriptor = services.FirstOrDefault(
            d => d.ServiceType == typeof(ScopedMessageContextAccessor));

        descriptor.Should().NotBeNull();
        descriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddMassTransitMessaging_Should_RegisterIMessageExecutionContext()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddMassTransitMessaging(options =>
        {
            options.Options.RabbitMq.Host = "localhost";
            options.Options.EnableHealthChecks = false;
        });

        var descriptor = services.FirstOrDefault(
            d => d.ServiceType == typeof(IMessageExecutionContext));

        descriptor.Should().NotBeNull();
        descriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddMassTransitMessaging_Should_RegisterServiceIdentityProvider_WhenServiceNameSet()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddMassTransitMessaging(options =>
        {
            options.Options.RabbitMq.Host = "localhost";
            options.Options.ServiceName = "test-service";
            options.Options.EnableHealthChecks = false;
        });

        var descriptor = services.FirstOrDefault(
            d => d.ServiceType == typeof(IServiceIdentityProvider));

        descriptor.Should().NotBeNull();
        descriptor!.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddMassTransitMessaging_Should_NotRegisterServiceIdentityProvider_WhenServiceNameEmpty()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddMassTransitMessaging(options =>
        {
            options.Options.RabbitMq.Host = "localhost";
            options.Options.EnableHealthChecks = false;
        });

        var descriptor = services.FirstOrDefault(
            d => d.ServiceType == typeof(IServiceIdentityProvider));

        descriptor.Should().BeNull();
    }

    [Fact]
    public void AddMassTransitMessaging_Should_ThrowOnNullServices()
    {
        IServiceCollection services = null!;

        var act = () => services.AddMassTransitMessaging(options => { });

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddMassTransitMessaging_Should_ThrowOnNullConfigure()
    {
        var services = new ServiceCollection();

        var act = () => services.AddMassTransitMessaging(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MassTransitMessagingOptions_Should_SupportFluentConfiguration()
    {
        var options = new MassTransitMessagingOptions();

        var result = options
            .AddConsumersFromAssembly(typeof(MassTransitServiceCollectionExtensionsTests).Assembly);

        result.Should().BeSameAs(options);
        options.ConsumerAssemblies.Should().HaveCount(1);
    }
}
