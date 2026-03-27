using FluentAssertions;
using MW.Messaging.MassTransit.Options;

namespace MW.Messaging.MassTransit.Tests.Options;

public class RetryExceptionFilteringTests
{
    [Fact]
    public void RetryOptions_ExceptionTypeFilters_ShouldBeNullByDefault()
    {
        var options = new RetryOptions();
        options.ExceptionTypeFilters.Should().BeNull();
    }

    [Fact]
    public void RetryOptions_ExceptionTypeFilters_ShouldAcceptTypeNames()
    {
        var options = new RetryOptions
        {
            ExceptionTypeFilters = new[]
            {
                "System.InvalidOperationException",
                "System.TimeoutException"
            }
        };

        options.ExceptionTypeFilters.Should().HaveCount(2);
    }

    [Fact]
    public void RetryOptions_ShouldHaveDefaultRetryIntervals()
    {
        var options = new RetryOptions();
        options.RetryIntervalsInSeconds.Should().BeEquivalentTo(new[] { 1, 2, 4 });
    }

    [Fact]
    public void RetryOptions_ShouldAllowCustomIntervals()
    {
        var options = new RetryOptions
        {
            RetryIntervalsInSeconds = new[] { 5, 10, 30, 60 }
        };

        options.RetryIntervalsInSeconds.Should().HaveCount(4);
    }

    [Fact]
    public void RedeliveryOptions_ShouldHaveDefaultIntervals()
    {
        var options = new RedeliveryOptions();
        options.RedeliveryIntervalsInSeconds.Should().BeEquivalentTo(new[] { 5, 15, 30 });
    }

    [Fact]
    public void RedeliveryOptions_ShouldAllowCustomIntervals()
    {
        var options = new RedeliveryOptions
        {
            RedeliveryIntervalsInSeconds = new[] { 10, 30, 60 }
        };

        options.RedeliveryIntervalsInSeconds.Should().HaveCount(3);
    }
}
