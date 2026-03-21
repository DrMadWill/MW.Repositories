using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MW.Persistence.EntityFrameworkCore.Repositories;
using MW.Persistence.Tests.Shared.Builders;
using MW.Persistence.Tests.Shared.Entities;
using MW.Persistence.Tests.Shared.Infrastructure;

namespace MW.Persistence.Tests.Integration.Repositories;

/// <summary>
/// PTST-024: Integration tests for read repository behavior using SQLite.
/// </summary>
public class ReadRepositoryIntegrationTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly TestDbContext _context;
    private readonly EfReadRepository<TestEntity, Guid> _repository;

    public ReadRepositoryIntegrationTests()
    {
        _factory = new TestDbContextFactory();
        _context = _factory.CreateContext();
        _repository = new EfReadRepository<TestEntity, Guid>(_context);
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnTrackedEntity()
    {
        var entity = TestEntityBuilder.Default().WithName("Tracked").Build();
        _context.TestEntities.Add(entity);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(entity.Id);

        result.Should().NotBeNull();
        // FindAsync returns tracked entity
        _context.Entry(result!).State.Should().NotBe(EntityState.Detached);
    }

    [Fact]
    public async Task GetAllAsync_Should_ReturnEntitiesInNoTrackingMode()
    {
        _context.TestEntities.Add(TestEntityBuilder.Default().Build());
        await _context.SaveChangesAsync();

        var results = await _repository.GetAllAsync();

        results.Should().HaveCount(1);
        // AsNoTracking means entities are detached
        foreach (var entity in results)
        {
            _context.Entry(entity).State.Should().Be(EntityState.Detached);
        }
    }

    [Fact]
    public async Task FindAsync_WithSpecification_Should_ApplySpecCriteria()
    {
        _context.TestEntities.Add(TestEntityBuilder.Default().WithName("Alpha").WithValue(10).Build());
        _context.TestEntities.Add(TestEntityBuilder.Default().WithName("Beta").WithValue(20).Build());
        _context.TestEntities.Add(TestEntityBuilder.Default().WithName("Gamma").WithValue(30).Build());
        await _context.SaveChangesAsync();

        var spec = new MW.Persistence.Tests.Shared.Specifications.TestEntityByNameSpecification("Beta");
        var results = await _repository.FindAsync(spec);

        results.Should().HaveCount(1);
        results[0].Name.Should().Be("Beta");
    }

    [Fact]
    public async Task FindAsync_WithPagedSpecification_Should_ApplyPaging()
    {
        for (int i = 0; i < 10; i++)
        {
            _context.TestEntities.Add(TestEntityBuilder.Default().WithName($"Entity-{i:D2}").WithValue(i).Build());
        }
        await _context.SaveChangesAsync();

        var spec = new MW.Persistence.Tests.Shared.Specifications.TestEntityPagedSpecification(skip: 2, take: 3);
        var results = await _repository.FindAsync(spec);

        results.Should().HaveCount(3);
    }

    [Fact]
    public async Task ExistsAsync_And_CountAsync_Should_WorkConsistently()
    {
        _context.TestEntities.Add(TestEntityBuilder.Default().WithName("Unique").Build());
        await _context.SaveChangesAsync();

        var exists = await _repository.ExistsAsync(e => e.Name == "Unique");
        var count = await _repository.CountAsync(e => e.Name == "Unique");

        exists.Should().BeTrue();
        count.Should().Be(1);
    }

    public void Dispose()
    {
        _context.Dispose();
        _factory.Dispose();
    }
}
