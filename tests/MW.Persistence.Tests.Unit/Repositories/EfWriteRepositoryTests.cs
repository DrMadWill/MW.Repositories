using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MW.Persistence.EntityFrameworkCore.Repositories;
using MW.Persistence.Tests.Shared.Builders;
using MW.Persistence.Tests.Shared.Entities;
using MW.Persistence.Tests.Shared.Infrastructure;

namespace MW.Persistence.Tests.Unit.Repositories;

/// <summary>
/// PTST-011: Unit tests for EfWriteRepository behavior.
/// </summary>
public class EfWriteRepositoryTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly TestDbContext _context;
    private readonly EfWriteRepository<TestEntity, Guid> _repository;

    public EfWriteRepositoryTests()
    {
        _factory = new TestDbContextFactory();
        _context = _factory.CreateContext();
        _repository = new EfWriteRepository<TestEntity, Guid>(_context);
    }

    [Fact]
    public async Task AddAsync_Should_AddEntityToContext()
    {
        var entity = TestEntityBuilder.Default().Build();

        await _repository.AddAsync(entity);
        await _context.SaveChangesAsync();

        var found = await _context.TestEntities.FindAsync(entity.Id);
        found.Should().NotBeNull();
        found!.Name.Should().Be(entity.Name);
    }

    [Fact]
    public async Task AddRangeAsync_Should_AddMultipleEntities()
    {
        var entities = new[]
        {
            TestEntityBuilder.Default().WithName("First").Build(),
            TestEntityBuilder.Default().WithName("Second").Build()
        };

        await _repository.AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        var count = await _context.TestEntities.CountAsync();
        count.Should().Be(2);
    }

    [Fact]
    public async Task Update_Should_ModifyExistingEntity()
    {
        var entity = TestEntityBuilder.Default().WithName("Original").Build();
        _context.TestEntities.Add(entity);
        await _context.SaveChangesAsync();

        entity.Name = "Updated";
        _repository.Update(entity);
        await _context.SaveChangesAsync();

        var updated = await _context.TestEntities.FindAsync(entity.Id);
        updated!.Name.Should().Be("Updated");
    }

    [Fact]
    public async Task Remove_Should_DeleteEntity()
    {
        var entity = TestEntityBuilder.Default().Build();
        _context.TestEntities.Add(entity);
        await _context.SaveChangesAsync();

        _repository.Remove(entity);
        await _context.SaveChangesAsync();

        var found = await _context.TestEntities.FindAsync(entity.Id);
        found.Should().BeNull();
    }

    [Fact]
    public async Task RemoveRange_Should_DeleteMultipleEntities()
    {
        var entities = new[]
        {
            TestEntityBuilder.Default().Build(),
            TestEntityBuilder.Default().Build()
        };
        _context.TestEntities.AddRange(entities);
        await _context.SaveChangesAsync();

        _repository.RemoveRange(entities);
        await _context.SaveChangesAsync();

        var count = await _context.TestEntities.CountAsync();
        count.Should().Be(0);
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_WhenDbContextIsNull()
    {
        Action act = () => new EfWriteRepository<TestEntity, Guid>(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("dbContext");
    }

    [Fact]
    public async Task AddAsync_Should_NotPersist_WithoutSaveChanges()
    {
        var entity = TestEntityBuilder.Default().Build();

        await _repository.AddAsync(entity);
        // Not calling SaveChanges

        using var verifyContext = _factory.CreateContext();
        var found = await verifyContext.TestEntities.FindAsync(entity.Id);
        found.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Dispose();
        _factory.Dispose();
    }
}
