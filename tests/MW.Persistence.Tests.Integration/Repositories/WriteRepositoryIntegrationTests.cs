using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MW.Persistence.EntityFrameworkCore.Repositories;
using MW.Persistence.Tests.Shared.Builders;
using MW.Persistence.Tests.Shared.Entities;
using MW.Persistence.Tests.Shared.Infrastructure;

namespace MW.Persistence.Tests.Integration.Repositories;

/// <summary>
/// PTST-025: Integration tests for write repository behavior using SQLite.
/// </summary>
public class WriteRepositoryIntegrationTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly TestDbContext _context;
    private readonly EfWriteRepository<TestEntity, Guid> _repository;

    public WriteRepositoryIntegrationTests()
    {
        _factory = new TestDbContextFactory();
        _context = _factory.CreateContext();
        _repository = new EfWriteRepository<TestEntity, Guid>(_context);
    }

    [Fact]
    public async Task AddAsync_Should_PersistToDatabase()
    {
        var entity = TestEntityBuilder.Default().WithName("Persisted").Build();

        await _repository.AddAsync(entity);
        await _context.SaveChangesAsync();

        // Verify using a separate context to ensure persistence
        using var verifyContext = _factory.CreateContext();
        var found = await verifyContext.TestEntities.FindAsync(entity.Id);
        found.Should().NotBeNull();
        found!.Name.Should().Be("Persisted");
    }

    [Fact]
    public async Task AddRangeAsync_Should_PersistAllEntities()
    {
        var entities = Enumerable.Range(1, 5)
            .Select(i => TestEntityBuilder.Default().WithName($"Batch-{i}").Build())
            .ToList();

        await _repository.AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        using var verifyContext = _factory.CreateContext();
        var count = await verifyContext.TestEntities.CountAsync();
        count.Should().Be(5);
    }

    [Fact]
    public async Task Update_Should_PersistModification()
    {
        var entity = TestEntityBuilder.Default().WithName("Original").WithValue(1).Build();
        _context.TestEntities.Add(entity);
        await _context.SaveChangesAsync();

        entity.Name = "Modified";
        entity.Value = 999;
        _repository.Update(entity);
        await _context.SaveChangesAsync();

        using var verifyContext = _factory.CreateContext();
        var found = await verifyContext.TestEntities.FindAsync(entity.Id);
        found!.Name.Should().Be("Modified");
        found.Value.Should().Be(999);
    }

    [Fact]
    public async Task Remove_Should_DeleteFromDatabase()
    {
        var entity = TestEntityBuilder.Default().Build();
        _context.TestEntities.Add(entity);
        await _context.SaveChangesAsync();

        _repository.Remove(entity);
        await _context.SaveChangesAsync();

        using var verifyContext = _factory.CreateContext();
        var found = await verifyContext.TestEntities.FindAsync(entity.Id);
        found.Should().BeNull();
    }

    [Fact]
    public async Task RemoveRange_Should_DeleteAllFromDatabase()
    {
        var entities = new[]
        {
            TestEntityBuilder.Default().Build(),
            TestEntityBuilder.Default().Build(),
            TestEntityBuilder.Default().Build()
        };
        _context.TestEntities.AddRange(entities);
        await _context.SaveChangesAsync();

        _repository.RemoveRange(entities);
        await _context.SaveChangesAsync();

        using var verifyContext = _factory.CreateContext();
        var count = await verifyContext.TestEntities.CountAsync();
        count.Should().Be(0);
    }

    public void Dispose()
    {
        _context.Dispose();
        _factory.Dispose();
    }
}
