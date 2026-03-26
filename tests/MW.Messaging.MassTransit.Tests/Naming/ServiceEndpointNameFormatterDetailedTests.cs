using FluentAssertions;
using MW.Messaging.MassTransit.Naming;

namespace MW.Messaging.MassTransit.Tests.Naming;

public class ServiceEndpointNameFormatterDetailedTests
{
    [Fact]
    public void SanitizeName_Should_ProduceDeterministicOutput()
    {
        var formatter = new ServiceEndpointNameFormatter("order-service");

        var result1 = formatter.SanitizeName("OrderCreatedConsumer");
        var result2 = formatter.SanitizeName("OrderCreatedConsumer");

        result1.Should().Be(result2);
    }

    [Theory]
    [InlineData("order-service", "OrderCreatedConsumer")]
    [InlineData("payment-service", "PaymentProcessedConsumer")]
    [InlineData("notification-service", "EmailSentConsumer")]
    public void SanitizeName_Should_IncludeServicePrefixForTypicalConsumers(string prefix, string consumer)
    {
        var formatter = new ServiceEndpointNameFormatter(prefix);

        var result = formatter.SanitizeName(consumer);

        result.Should().StartWith(prefix + "-");
        result.Should().NotContainAny("A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z");
    }

    [Fact]
    public void SanitizeName_Should_BeKebabCase()
    {
        var formatter = new ServiceEndpointNameFormatter("svc");

        var result = formatter.SanitizeName("MyComplexConsumerName");

        result.Should().MatchRegex("^[a-z0-9-]+$");
    }

    [Fact]
    public void SanitizeName_Should_HandleSingleWordInput()
    {
        var formatter = new ServiceEndpointNameFormatter("svc");

        var result = formatter.SanitizeName("Consumer");

        result.Should().StartWith("svc-");
        result.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Consumer_Should_ProduceCorrectEndpointName()
    {
        var formatter = new ServiceEndpointNameFormatter("order-service");

        var result = formatter.Consumer<TestConsumer>();

        result.Should().StartWith("order-service-");
        result.Should().NotContainAny("A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z");
    }

    private class TestConsumer : global::MassTransit.IConsumer<TestMessage>
    {
        public Task Consume(global::MassTransit.ConsumeContext<TestMessage> context) => Task.CompletedTask;
    }

    public class TestMessage { }
}
