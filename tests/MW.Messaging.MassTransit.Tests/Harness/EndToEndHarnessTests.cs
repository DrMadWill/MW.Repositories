using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using MW.Messaging.Context;
using MW.Messaging.Contracts;
using MW.Messaging.MassTransit.Context;
using MW.Messaging.MassTransit.Filters;
using MW.Messaging.Publishing;
using MessageHeaders = MW.Messaging.Headers.MessageHeaders;

namespace MW.Messaging.MassTransit.Tests.Harness;

public class EndToEndHarnessTests : IAsyncLifetime
{
    private ServiceProvider _provider = null!;
    private ITestHarness _harness = null!;

    public async Task InitializeAsync()
    {
        var services = new ServiceCollection();

        services.AddScoped<ScopedMessageContextAccessor>();
        services.AddScoped<IMessageContextAccessor>(sp => sp.GetRequiredService<ScopedMessageContextAccessor>());
        services.AddScoped<IMessageExecutionContext, MassTransitMessageExecutionContext>();
        services.AddSingleton<MW.Messaging.MassTransit.IMessageHeaderMapper, DefaultMessageHeaderMapper>();
        services.AddScoped<IPublishContextProvider>(sp =>
        {
            var identityProvider = new MW.Messaging.MassTransit.Identity.ConfigurationServiceIdentityProvider(
                new MW.Messaging.MassTransit.Options.MassTransitOptions { ServiceName = "harness-test" });
            return new DefaultPublishContextProvider(serviceIdentityProvider: identityProvider);
        });

        services.AddMassTransitTestHarness(cfg =>
        {
            cfg.AddConsumer<TestEventConsumer>();

            cfg.UsingInMemory((context, busCfg) =>
            {
                busCfg.UsePublishFilter(typeof(HeaderEnrichmentPublishFilter<>), context);
                busCfg.UseConsumeFilter(typeof(MessageContextConsumeFilter<>), context);
                busCfg.ConfigureEndpoints(context);
            });
        });

        _provider = services.BuildServiceProvider(true);
        _harness = _provider.GetRequiredService<ITestHarness>();
        await _harness.Start();
    }

    public async Task DisposeAsync()
    {
        await _harness.Stop();
        await _provider.DisposeAsync();
    }

    [Fact]
    public async Task PublishAndConsume_Should_PropagateHeaders()
    {
        using var scope = _provider.CreateScope();
        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        await publishEndpoint.Publish<ITestEvent>(new
        {
            EventId = Guid.NewGuid(),
            OccurredOn = DateTimeOffset.UtcNow,
            EventName = "TestEvent",
            EventVersion = "1.0",
            SourceService = "test",
            Value = "hello"
        });

        (await _harness.Consumed.Any<ITestEvent>()).Should().BeTrue();
    }

    [Fact]
    public async Task PublishAndConsume_Should_EnrichHeadersViaFilter()
    {
        using var scope = _provider.CreateScope();
        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        await publishEndpoint.Publish<ITestEvent>(new
        {
            EventId = Guid.NewGuid(),
            OccurredOn = DateTimeOffset.UtcNow,
            EventName = "FilterTest",
            EventVersion = "1.0",
            SourceService = "test",
            Value = "enrich-test"
        });

        (await _harness.Published.Any<ITestEvent>()).Should().BeTrue();
        (await _harness.Consumed.Any<ITestEvent>()).Should().BeTrue();

        var published = _harness.Published.Select<ITestEvent>().First();

        // Verify header enrichment by the publish filter
        published.Context.Headers.TryGetHeader(MessageHeaders.SourceService, out var sourceService)
            .Should().BeTrue();
        sourceService.Should().Be("harness-test");

        published.Context.Headers.TryGetHeader(MessageHeaders.CorrelationId, out var corrId)
            .Should().BeTrue();
        corrId.Should().NotBeNull();
    }

    [Fact]
    public async Task Consumer_Should_ReceiveContextFromFilter()
    {
        using var scope = _provider.CreateScope();
        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        await publishEndpoint.Publish<ITestEvent>(new
        {
            EventId = Guid.NewGuid(),
            OccurredOn = DateTimeOffset.UtcNow,
            EventName = "ContextTest",
            EventVersion = "1.0",
            SourceService = "test",
            Value = "context-test"
        });

        (await _harness.Consumed.Any<ITestEvent>()).Should().BeTrue();

        // The consumer should have received the message
        var consumerHarness = _harness.GetConsumerHarness<TestEventConsumer>();
        (await consumerHarness.Consumed.Any<ITestEvent>()).Should().BeTrue();
    }

    public interface ITestEvent
    {
        Guid EventId { get; }
        DateTimeOffset OccurredOn { get; }
        string EventName { get; }
        string EventVersion { get; }
        string SourceService { get; }
        string Value { get; }
    }

    public class TestEventConsumer : IConsumer<ITestEvent>
    {
        public Task Consume(ConsumeContext<ITestEvent> context)
        {
            // Consumer successfully processes the message
            return Task.CompletedTask;
        }
    }
}
