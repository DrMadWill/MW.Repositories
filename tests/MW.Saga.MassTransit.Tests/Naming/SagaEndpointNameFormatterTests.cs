using FluentAssertions;
using MW.Saga.MassTransit.Naming;
using MW.Saga.MassTransit.Options;

namespace MW.Saga.MassTransit.Tests.Naming;

public class SagaEndpointNameFormatterTests
{
    [Fact]
    public void SanitizeName_WithPrefix_ShouldApplyLowercasePrefixFollowedByDash()
    {
        var formatter = new SagaEndpointNameFormatter("myservice");

        var result = formatter.SanitizeName("OrderSagaState");

        result.Should().StartWith("myservice-");
    }

    [Fact]
    public void SanitizeName_WithEmptyPrefix_ShouldNotAddPrefix()
    {
        var formatter = new SagaEndpointNameFormatter(string.Empty);

        var result = formatter.SanitizeName("OrderSagaState");

        result.Should().NotStartWith("-");
    }

    [Fact]
    public void SanitizeName_WithWhitespaceOnlyPrefix_ShouldNotAddPrefix()
    {
        var formatter = new SagaEndpointNameFormatter("   ");

        var result = formatter.SanitizeName("OrderSagaState");

        result.Should().NotStartWith("-");
        result.Should().NotStartWith(" ");
    }

    [Fact]
    public void SanitizeName_ShouldApplyKebabCase()
    {
        var formatter = new SagaEndpointNameFormatter(string.Empty);

        var result = formatter.SanitizeName("OrderSagaState");

        result.Should().Be("order-saga-state");
    }

    [Fact]
    public void SanitizeName_WithPrefix_ShouldTrimLeadingAndTrailingSpaces()
    {
        var formatter = new SagaEndpointNameFormatter("  myservice  ");

        var result = formatter.SanitizeName("OrderSagaState");

        result.Should().StartWith("myservice-");
        result.Should().NotContain("  ");
    }

    [Fact]
    public void SanitizeName_WithPrefix_ShouldLowercasePrefix()
    {
        var formatter = new SagaEndpointNameFormatter("MyService");

        var result = formatter.SanitizeName("OrderSagaState");

        result.Should().StartWith("myservice-");
    }

    [Fact]
    public void SanitizeName_ShouldReturnDeterministicOutput()
    {
        var formatter = new SagaEndpointNameFormatter("prefix");

        var result1 = formatter.SanitizeName("OrderSagaState");
        var result2 = formatter.SanitizeName("OrderSagaState");

        result1.Should().Be(result2);
    }

    [Fact]
    public void FromOptions_ShouldCreateFormatterFromOptions()
    {
        var options = new SagaMassTransitOptions { EndpointPrefix = "my-service" };

        var formatter = SagaEndpointNameFormatter.FromOptions(options);

        var result = formatter.SanitizeName("OrderSagaState");
        result.Should().StartWith("my-service-");
    }
}
