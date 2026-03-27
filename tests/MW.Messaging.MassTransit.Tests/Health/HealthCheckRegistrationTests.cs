using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MW.Messaging.MassTransit.Extensions;

namespace MW.Messaging.MassTransit.Tests.Health;

public class HealthCheckRegistrationTests
{
    [Fact]
    public void AddMassTransitMessaging_Should_RegisterHealthChecks_WhenEnabled()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddMassTransitMessaging(options =>
        {
            options.Options.RabbitMq.Host = "localhost";
            options.Options.EnableHealthChecks = true;
        });

        var descriptor = services.FirstOrDefault(
            d => d.ServiceType == typeof(HealthCheckService));

        descriptor.Should().NotBeNull("health check service should be registered");
    }

    [Fact]
    public void AddMassTransitMessaging_Should_NotRegisterOurHealthChecks_WhenDisabled()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddMassTransitMessaging(options =>
        {
            options.Options.RabbitMq.Host = "localhost";
            options.Options.EnableHealthChecks = false;
        });

        var sp = services.BuildServiceProvider();
        var healthOptions = sp.GetService<Microsoft.Extensions.Options.IOptions<HealthCheckServiceOptions>>();

        // When health checks are disabled, our custom checks should not be registered
        // (MassTransit may register its own, but "rabbitmq" is ours)
        if (healthOptions?.Value?.Registrations != null)
        {
            healthOptions.Value.Registrations
                .Should().NotContain(r => r.Name == "rabbitmq");
        }
    }

    [Fact]
    public void AddMassTransitMessaging_Should_RegisterMassTransitBusHealthCheck_WhenEnabled()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddMassTransitMessaging(options =>
        {
            options.Options.RabbitMq.Host = "localhost";
            options.Options.EnableHealthChecks = true;
        });

        var sp = services.BuildServiceProvider();
        var healthOptions = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<HealthCheckServiceOptions>>();

        healthOptions.Value.Registrations
            .Should().Contain(r => r.Name == "masstransit-bus");
    }

    [Fact]
    public void AddMassTransitMessaging_Should_RegisterRabbitMqHealthCheck_WhenEnabled()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddMassTransitMessaging(options =>
        {
            options.Options.RabbitMq.Host = "localhost";
            options.Options.EnableHealthChecks = true;
        });

        var sp = services.BuildServiceProvider();
        var healthOptions = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<HealthCheckServiceOptions>>();

        healthOptions.Value.Registrations
            .Should().Contain(r => r.Name == "rabbitmq");
    }
}
