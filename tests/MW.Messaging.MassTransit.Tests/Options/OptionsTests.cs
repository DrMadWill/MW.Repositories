using FluentAssertions;
using MW.Messaging.MassTransit.Options;

namespace MW.Messaging.MassTransit.Tests.Options;

public class OptionsTests
{
    [Fact]
    public void RabbitMqOptions_Should_HaveDefaultValues()
    {
        var options = new RabbitMqOptions();

        options.Host.Should().Be("localhost");
        options.Port.Should().Be(5672);
        options.VirtualHost.Should().Be("/");
        options.Username.Should().Be("guest");
        options.Password.Should().Be("guest");
    }

    [Fact]
    public void RabbitMqOptions_Should_HaveSectionName()
    {
        RabbitMqOptions.SectionName.Should().Be("RabbitMq");
    }

    [Fact]
    public void RetryOptions_Should_HaveDefaultValues()
    {
        var options = new RetryOptions();

        options.RetryCount.Should().Be(3);
        options.RetryIntervalsInSeconds.Should().NotBeEmpty();
        options.ExceptionTypeFilters.Should().BeNull();
    }

    [Fact]
    public void RetryOptions_Should_HaveSectionName()
    {
        RetryOptions.SectionName.Should().Be("Messaging:Retry");
    }

    [Fact]
    public void RedeliveryOptions_Should_HaveDefaultValues()
    {
        var options = new RedeliveryOptions();

        options.RedeliveryCount.Should().Be(3);
        options.RedeliveryIntervalsInSeconds.Should().NotBeEmpty();
    }

    [Fact]
    public void RedeliveryOptions_Should_HaveSectionName()
    {
        RedeliveryOptions.SectionName.Should().Be("Messaging:Redelivery");
    }

    [Fact]
    public void MassTransitOptions_Should_HaveDefaultValues()
    {
        var options = new MassTransitOptions();

        options.RabbitMq.Should().NotBeNull();
        options.Retry.Should().NotBeNull();
        options.Redelivery.Should().NotBeNull();
        options.ServiceName.Should().BeEmpty();
        options.EnableHealthChecks.Should().BeTrue();
    }

    [Fact]
    public void MassTransitOptions_Should_HaveSectionName()
    {
        MassTransitOptions.SectionName.Should().Be("Messaging");
    }

    [Fact]
    public void RabbitMqOptions_Should_AllowCustomConfiguration()
    {
        var options = new RabbitMqOptions
        {
            Host = "rabbitmq.prod.internal",
            Port = 5673,
            VirtualHost = "/production",
            Username = "app_user",
            Password = "secure_pass"
        };

        options.Host.Should().Be("rabbitmq.prod.internal");
        options.Port.Should().Be(5673);
        options.VirtualHost.Should().Be("/production");
        options.Username.Should().Be("app_user");
        options.Password.Should().Be("secure_pass");
    }
}
