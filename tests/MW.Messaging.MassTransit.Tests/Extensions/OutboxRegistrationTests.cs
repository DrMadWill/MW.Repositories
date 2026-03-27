using FluentAssertions;
using MW.Messaging.MassTransit.Extensions;

namespace MW.Messaging.MassTransit.Tests.Extensions;

public class OutboxRegistrationTests
{
    [Fact]
    public void UseEntityFrameworkOutbox_Should_SetOutboxConfigurator()
    {
        var options = new MassTransitMessagingOptions();

        options.UseEntityFrameworkOutbox<TestDbContext>();

        options.OutboxConfigurator.Should().NotBeNull();
    }

    [Fact]
    public void UseEntityFrameworkOutbox_Should_ReturnSameOptionsForFluency()
    {
        var options = new MassTransitMessagingOptions();

        var result = options.UseEntityFrameworkOutbox<TestDbContext>();

        result.Should().BeSameAs(options);
    }

    [Fact]
    public void UseEntityFrameworkInboxOutbox_Should_SetOutboxConfigurator()
    {
        var options = new MassTransitMessagingOptions();

        options.UseEntityFrameworkInboxOutbox<TestDbContext>();

        options.OutboxConfigurator.Should().NotBeNull();
    }

    [Fact]
    public void UseEntityFrameworkInboxOutbox_Should_ReturnSameOptionsForFluency()
    {
        var options = new MassTransitMessagingOptions();

        var result = options.UseEntityFrameworkInboxOutbox<TestDbContext>();

        result.Should().BeSameAs(options);
    }

    [Fact]
    public void UseEntityFrameworkOutbox_Should_OverrideExistingConfigurator()
    {
        var options = new MassTransitMessagingOptions();

        options.UseEntityFrameworkOutbox<TestDbContext>();
        var first = options.OutboxConfigurator;

        options.UseEntityFrameworkInboxOutbox<TestDbContext>();
        var second = options.OutboxConfigurator;

        first.Should().NotBeSameAs(second);
    }

    [Fact]
    public void UseEntityFrameworkOutbox_WithCustomConfig_Should_SetOutboxConfigurator()
    {
        var options = new MassTransitMessagingOptions();

        options.UseEntityFrameworkOutbox<TestDbContext>(_ => { });

        options.OutboxConfigurator.Should().NotBeNull();
        // The configurator is a delegate that will be called at MassTransit registration time.
        // We verify it was captured, not that it ran (it runs during bus registration).
    }
}

/// <summary>
/// Minimal DbContext stub for outbox registration tests.
/// </summary>
public class TestDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public TestDbContext(Microsoft.EntityFrameworkCore.DbContextOptions<TestDbContext> options)
        : base(options)
    {
    }
}
