using FluentAssertions;
using Microsoft.Extensions.Configuration;
using MW.Saga.MassTransit.Options;

namespace MW.Saga.MassTransit.Tests.Options;

public class SagaMassTransitOptionsTests
{
    [Fact]
    public void SectionName_ShouldBe_SagaMassTransit()
    {
        SagaMassTransitOptions.SectionName.Should().Be("SagaMassTransit");
    }

    [Fact]
    public void DefaultValues_ShouldBeCorrect()
    {
        var options = new SagaMassTransitOptions();

        options.EndpointPrefix.Should().BeEmpty();
        options.RetryCount.Should().Be(3);
        options.RetryIntervalsInSeconds.Should().BeEmpty();
        options.DefaultTimeoutInSeconds.Should().Be(300);
        options.ConcurrencyLimit.Should().Be(0);
        options.UseScheduler.Should().BeFalse();
    }

    [Fact]
    public void Properties_CanBeSetAndRead()
    {
        var options = new SagaMassTransitOptions
        {
            EndpointPrefix = "my-prefix",
            RetryCount = 10,
            RetryIntervalsInSeconds = [1, 2, 3],
            DefaultTimeoutInSeconds = 600,
            ConcurrencyLimit = 5,
            UseScheduler = true
        };

        options.EndpointPrefix.Should().Be("my-prefix");
        options.RetryCount.Should().Be(10);
        options.RetryIntervalsInSeconds.Should().BeEquivalentTo(new[] { 1, 2, 3 });
        options.DefaultTimeoutInSeconds.Should().Be(600);
        options.ConcurrencyLimit.Should().Be(5);
        options.UseScheduler.Should().BeTrue();
    }

    [Fact]
    public void Configuration_ShouldBindAllValues()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SagaMassTransit:EndpointPrefix"] = "my-service",
                ["SagaMassTransit:RetryCount"] = "5",
                ["SagaMassTransit:DefaultTimeoutInSeconds"] = "600",
                ["SagaMassTransit:UseScheduler"] = "true",
                ["SagaMassTransit:ConcurrencyLimit"] = "10"
            }).Build();

        var options = new SagaMassTransitOptions();
        config.GetSection(SagaMassTransitOptions.SectionName).Bind(options);

        options.EndpointPrefix.Should().Be("my-service");
        options.RetryCount.Should().Be(5);
        options.DefaultTimeoutInSeconds.Should().Be(600);
        options.UseScheduler.Should().BeTrue();
        options.ConcurrencyLimit.Should().Be(10);
    }

    [Fact]
    public void Configuration_ShouldBindRetryIntervalsInSeconds()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SagaMassTransit:RetryIntervalsInSeconds:0"] = "5",
                ["SagaMassTransit:RetryIntervalsInSeconds:1"] = "10",
                ["SagaMassTransit:RetryIntervalsInSeconds:2"] = "30"
            }).Build();

        var options = new SagaMassTransitOptions();
        config.GetSection(SagaMassTransitOptions.SectionName).Bind(options);

        options.RetryIntervalsInSeconds.Should().BeEquivalentTo(new[] { 5, 10, 30 });
    }
}
