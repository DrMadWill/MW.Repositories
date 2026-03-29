using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MW.Saga.MassTransit.Persistence;
using MW.Saga.Models;

namespace MW.Saga.MassTransit.Tests.Persistence;

public class TestSagaState : SagaStateBase { }

public class TestSagaStateMap : SagaStateBaseMap<TestSagaState>
{
    public bool ConfigureAdditionalCalled { get; private set; }

    protected override void ConfigureAdditional(EntityTypeBuilder<TestSagaState> builder)
    {
        ConfigureAdditionalCalled = true;
    }
}

public class TestSagaDbContext : DbContext
{
    public TestSagaDbContext(DbContextOptions<TestSagaDbContext> options) : base(options) { }

    public DbSet<TestSagaState> SagaStates { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TestSagaStateMap());
    }
}

public class SagaStateBaseMapTests
{
    private static TestSagaDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestSagaDbContext>()
            .UseInMemoryDatabase($"test-saga-db-{Guid.NewGuid()}")
            .Options;

        return new TestSagaDbContext(options);
    }

    [Fact]
    public void Configure_Should_SetCorrelationIdAsKey()
    {
        using var dbContext = CreateContext();
        var entityType = dbContext.Model.FindEntityType(typeof(TestSagaState))!;

        var primaryKey = entityType.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
        primaryKey!.Properties.Should().ContainSingle(p => p.Name == "CorrelationId");
    }

    [Fact]
    public void Configure_Should_SetCurrentStateAsRequired()
    {
        using var dbContext = CreateContext();
        var entityType = dbContext.Model.FindEntityType(typeof(TestSagaState))!;

        var property = entityType.FindProperty("CurrentState");
        property.Should().NotBeNull();
        property!.IsNullable.Should().BeFalse();
        property.GetMaxLength().Should().Be(128);
    }

    [Fact]
    public void Configure_Should_SetVersionAsConcurrencyToken()
    {
        using var dbContext = CreateContext();
        var entityType = dbContext.Model.FindEntityType(typeof(TestSagaState))!;

        var property = entityType.FindProperty("Version");
        property.Should().NotBeNull();
        property!.IsConcurrencyToken.Should().BeTrue();
    }

    [Fact]
    public void Configure_Should_CallConfigureAdditional()
    {
        var map = new TestSagaStateMap();
        var options = new DbContextOptionsBuilder<TestSagaDbContext>()
            .UseInMemoryDatabase($"test-saga-db-{Guid.NewGuid()}")
            .Options;

        using var dbContext = new TestSagaDbContext(options);

        // Invoke Configure directly to verify ConfigureAdditional is called on this instance.
        var modelBuilder = new ModelBuilder();
        var entityBuilder = modelBuilder.Entity<TestSagaState>();
        map.Configure(entityBuilder);

        map.ConfigureAdditionalCalled.Should().BeTrue();
    }

    [Fact]
    public void Configure_Should_MapLifecycleTimestamps()
    {
        using var dbContext = CreateContext();
        var entityType = dbContext.Model.FindEntityType(typeof(TestSagaState))!;

        var createdAt = entityType.FindProperty("CreatedAt");
        createdAt.Should().NotBeNull();
        createdAt!.IsNullable.Should().BeFalse();

        var updatedAt = entityType.FindProperty("UpdatedAt");
        updatedAt.Should().NotBeNull();
        updatedAt!.IsNullable.Should().BeFalse();
    }
}
