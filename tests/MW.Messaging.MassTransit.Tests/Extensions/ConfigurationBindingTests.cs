using FluentAssertions;
using Microsoft.Extensions.Configuration;
using MW.Messaging.MassTransit.Extensions;
using MW.Messaging.MassTransit.Options;

namespace MW.Messaging.MassTransit.Tests.Extensions;

public class ConfigurationBindingTests
{
    [Fact]
    public void BindOptions_Should_BindFromConfiguration()
    {
        var configData = new Dictionary<string, string?>
        {
            ["Messaging:ServiceName"] = "test-service",
            ["Messaging:EnableHealthChecks"] = "false",
            ["Messaging:RabbitMq:Host"] = "rabbitmq.prod",
            ["Messaging:RabbitMq:Port"] = "5673",
            ["Messaging:RabbitMq:Username"] = "admin",
            ["Messaging:RabbitMq:Password"] = "secret",
            ["Messaging:Retry:RetryCount"] = "5"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        var options = new MassTransitMessagingOptions();
        options.BindOptions(configuration);

        options.Options.ServiceName.Should().Be("test-service");
        options.Options.EnableHealthChecks.Should().BeFalse();
        options.Options.RabbitMq.Host.Should().Be("rabbitmq.prod");
        options.Options.RabbitMq.Port.Should().Be(5673);
        options.Options.RabbitMq.Username.Should().Be("admin");
        options.Options.RabbitMq.Password.Should().Be("secret");
        options.Options.Retry.RetryCount.Should().Be(5);
    }

    [Fact]
    public void BindOptions_Should_UseCustomSectionName()
    {
        var configData = new Dictionary<string, string?>
        {
            ["CustomSection:ServiceName"] = "custom-service"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        var options = new MassTransitMessagingOptions();
        options.BindOptions(configuration, "CustomSection");

        options.Options.ServiceName.Should().Be("custom-service");
    }

    [Fact]
    public void BindOptions_Should_KeepDefaultsWhenSectionMissing()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        var options = new MassTransitMessagingOptions();
        options.BindOptions(configuration);

        options.Options.ServiceName.Should().BeEmpty();
        options.Options.RabbitMq.Host.Should().Be("localhost");
        options.Options.EnableHealthChecks.Should().BeTrue();
    }

    [Fact]
    public void BindOptions_Should_ReturnSameOptionsForFluency()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        var options = new MassTransitMessagingOptions();
        var result = options.BindOptions(configuration);

        result.Should().BeSameAs(options);
    }

    [Fact]
    public void BindOptions_Should_ThrowOnNullConfiguration()
    {
        var options = new MassTransitMessagingOptions();
        var act = () => options.BindOptions(null!);
        act.Should().Throw<ArgumentNullException>();
    }
}
