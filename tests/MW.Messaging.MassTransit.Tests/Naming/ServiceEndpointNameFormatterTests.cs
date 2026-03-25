using FluentAssertions;
using MW.Messaging.MassTransit.Naming;

namespace MW.Messaging.MassTransit.Tests.Naming;

public class ServiceEndpointNameFormatterTests
{
    [Fact]
    public void SanitizeName_Should_ApplyServicePrefix()
    {
        var formatter = new ServiceEndpointNameFormatter("order-service");

        var result = formatter.SanitizeName("OrderCreatedConsumer");

        result.Should().StartWith("order-service-");
    }

    [Fact]
    public void SanitizeName_Should_UseKebabCase()
    {
        var formatter = new ServiceEndpointNameFormatter("order-service");

        var result = formatter.SanitizeName("OrderCreatedConsumer");

        result.Should().NotContain("OrderCreated");
        result.Should().Contain("order-service-");
    }

    [Fact]
    public void SanitizeName_Should_HandleEmptyPrefix()
    {
        var formatter = new ServiceEndpointNameFormatter(string.Empty);

        var result = formatter.SanitizeName("OrderCreatedConsumer");

        result.Should().NotBeNullOrWhiteSpace();
        result.Should().NotStartWith("-");
    }

    [Fact]
    public void SanitizeName_Should_HandleWhitespacePrefix()
    {
        var formatter = new ServiceEndpointNameFormatter("  ");

        var result = formatter.SanitizeName("OrderCreatedConsumer");

        result.Should().NotStartWith("-");
        result.Should().NotStartWith(" ");
    }

    [Fact]
    public void SanitizeName_Should_TrimAndLowercasePrefix()
    {
        var formatter = new ServiceEndpointNameFormatter("  ORDER-SERVICE  ");

        var result = formatter.SanitizeName("Test");

        result.Should().StartWith("order-service-");
    }
}
