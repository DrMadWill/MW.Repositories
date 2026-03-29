using FluentAssertions;
using MW.Saga.MassTransit.Options;
using MW.Saga.MassTransit.Scheduling;

namespace MW.Saga.MassTransit.Tests.Scheduling;

public class SagaTimeoutConventionsTests
{
    [Fact]
    public void GetDefaultTimeout_Should_ThrowOnNullOptions()
    {
        var act = () => SagaTimeoutConventions.GetDefaultTimeout(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("options");
    }

    [Fact]
    public void GetDefaultTimeout_Should_ReturnCorrectTimeSpan()
    {
        var options = new SagaMassTransitOptions { DefaultTimeoutInSeconds = 300 };

        var result = SagaTimeoutConventions.GetDefaultTimeout(options);

        result.Should().Be(TimeSpan.FromMinutes(5));
    }

    [Fact]
    public void GetDefaultTimeout_Should_ReturnCustomTimeSpan()
    {
        var options = new SagaMassTransitOptions { DefaultTimeoutInSeconds = 600 };

        var result = SagaTimeoutConventions.GetDefaultTimeout(options);

        result.Should().Be(TimeSpan.FromMinutes(10));
    }

    [Fact]
    public void GetTimeoutEndpointName_Should_ThrowOnNullSagaName()
    {
        var act = () => SagaTimeoutConventions.GetTimeoutEndpointName(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("sagaName");
    }

    [Fact]
    public void GetTimeoutEndpointName_Should_ReturnKebabCaseWithTimeoutSuffix()
    {
        var result = SagaTimeoutConventions.GetTimeoutEndpointName("OrderSaga");

        result.Should().Be("order-saga-timeout");
    }

    [Fact]
    public void GetTimeoutEndpointName_Should_IncludePrefix_WhenProvided()
    {
        var result = SagaTimeoutConventions.GetTimeoutEndpointName("OrderSaga", prefix: "my-svc");

        result.Should().Be("my-svc-order-saga-timeout");
    }

    [Fact]
    public void GetTimeoutEndpointName_Should_TrimAndLowercasePrefix()
    {
        var result = SagaTimeoutConventions.GetTimeoutEndpointName("OrderSaga", prefix: " MyPrefix ");

        result.Should().Be("myprefix-order-saga-timeout");
    }

    [Fact]
    public void GetTimeoutEndpointName_Should_OmitPrefix_WhenNull()
    {
        var result = SagaTimeoutConventions.GetTimeoutEndpointName("OrderSaga", prefix: null);

        result.Should().Be("order-saga-timeout");
    }

    [Fact]
    public void GetTimeoutEndpointName_Should_OmitPrefix_WhenWhitespace()
    {
        var result = SagaTimeoutConventions.GetTimeoutEndpointName("OrderSaga", prefix: "   ");

        result.Should().Be("order-saga-timeout");
    }

    [Fact]
    public void GetTimeoutEndpointName_Should_BeDeterministic()
    {
        var first = SagaTimeoutConventions.GetTimeoutEndpointName("OrderSaga", prefix: "svc");
        var second = SagaTimeoutConventions.GetTimeoutEndpointName("OrderSaga", prefix: "svc");

        first.Should().Be(second);
    }
}
