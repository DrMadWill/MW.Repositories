using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MW.Persistence.EntityFrameworkCore.Querying;
using MW.Persistence.EntityFrameworkCore.Repositories;
using MW.Persistence.Tests.Shared.Builders;
using MW.Persistence.Tests.Shared.Entities;
using MW.Persistence.Tests.Shared.Infrastructure;

namespace MW.Persistence.Tests.Integration.Querying;

/// <summary>
/// PTST-028: Integration tests for query options behavior.
/// PTST-017: Validates NoTracking default policy.
/// </summary>
public class QueryOptionsIntegrationTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly TestDbContext _context;

    public QueryOptionsIntegrationTests()
    {
        _factory = new TestDbContextFactory();
        _context = _factory.CreateContext();
    }

    [Fact]
    public async Task GetAllAsync_Should_UseNoTrackingByDefault()
    {
        _context.TestEntities.Add(TestEntityBuilder.Default().Build());
        await _context.SaveChangesAsync();

        var readRepo = new EfReadRepository<TestEntity, Guid>(_context);
        var results = await readRepo.GetAllAsync();

        foreach (var entity in results)
        {
            _context.Entry(entity).State.Should().Be(EntityState.Detached,
                "Read repository should return detached (no-tracking) entities by default");
        }
    }

    [Fact]
    public async Task FindAsync_Should_UseNoTrackingByDefault()
    {
        _context.TestEntities.Add(TestEntityBuilder.Default().WithName("Test").Build());
        await _context.SaveChangesAsync();

        var readRepo = new EfReadRepository<TestEntity, Guid>(_context);
        var results = await readRepo.FindAsync(e => e.Name == "Test");

        foreach (var entity in results)
        {
            _context.Entry(entity).State.Should().Be(EntityState.Detached);
        }
    }

    [Fact]
    public void QueryOptions_Default_Should_HaveCorrectValues()
    {
        var defaults = QueryOptions.Default;

        defaults.AsNoTracking.Should().BeTrue("Default should use no-tracking");
        defaults.IgnoreQueryFilters.Should().BeFalse("Default should respect query filters");
        defaults.IncludeSoftDeleted.Should().BeFalse("Default should exclude soft-deleted");
    }

    [Fact]
    public void QueryOptions_Tracked_Should_DisableNoTracking()
    {
        var tracked = QueryOptions.Tracked;

        tracked.AsNoTracking.Should().BeFalse("Tracked option should enable tracking");
    }

    public void Dispose()
    {
        _context.Dispose();
        _factory.Dispose();
    }
}
