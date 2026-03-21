using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MW.Persistence.EntityFrameworkCore.Repositories;
using MW.Persistence.Tests.Shared.Builders;
using MW.Persistence.Tests.Shared.Entities;
using MW.Persistence.Tests.Shared.Infrastructure;

namespace MW.Persistence.Tests.Integration.Repositories;

/// <summary>
/// PTST-029: Integration tests for soft delete behavior with query filters.
/// </summary>
public class SoftDeleteIntegrationTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly TestDbContext _context;
    private readonly EfReadRepository<SoftDeletableEntity, Guid> _readRepository;
    private readonly EfWriteRepository<SoftDeletableEntity, Guid> _writeRepository;

    public SoftDeleteIntegrationTests()
    {
        _factory = new TestDbContextFactory();
        _context = _factory.CreateContext();
        _readRepository = new EfReadRepository<SoftDeletableEntity, Guid>(_context);
        _writeRepository = new EfWriteRepository<SoftDeletableEntity, Guid>(_context);
    }

    [Fact]
    public async Task GetAllAsync_Should_ExcludeSoftDeletedByDefault()
    {
        _context.SoftDeletableEntities.Add(SoftDeletableEntityBuilder.Default().WithName("Active").Build());
        _context.SoftDeletableEntities.Add(SoftDeletableEntityBuilder.Default().WithName("Deleted").AsDeleted().Build());
        await _context.SaveChangesAsync();

        var results = await _readRepository.GetAllAsync();

        results.Should().HaveCount(1);
        results[0].Name.Should().Be("Active");
    }

    [Fact]
    public async Task CountAsync_Should_ExcludeSoftDeleted()
    {
        _context.SoftDeletableEntities.Add(SoftDeletableEntityBuilder.Default().Build());
        _context.SoftDeletableEntities.Add(SoftDeletableEntityBuilder.Default().AsDeleted().Build());
        _context.SoftDeletableEntities.Add(SoftDeletableEntityBuilder.Default().AsDeleted().Build());
        await _context.SaveChangesAsync();

        var count = await _readRepository.CountAsync();

        count.Should().Be(1);
    }

    [Fact]
    public async Task FindAsync_Should_ExcludeSoftDeleted()
    {
        _context.SoftDeletableEntities.Add(
            SoftDeletableEntityBuilder.Default().WithName("Active-Find").Build());
        _context.SoftDeletableEntities.Add(
            SoftDeletableEntityBuilder.Default().WithName("Deleted-Find").AsDeleted().Build());
        await _context.SaveChangesAsync();

        var results = await _readRepository.FindAsync(e => e.Name.Contains("Find"));

        results.Should().HaveCount(1);
        results[0].Name.Should().Be("Active-Find");
    }

    [Fact]
    public async Task ExistsAsync_Should_ExcludeSoftDeleted()
    {
        _context.SoftDeletableEntities.Add(
            SoftDeletableEntityBuilder.Default().WithName("GhostEntity").AsDeleted().Build());
        await _context.SaveChangesAsync();

        var exists = await _readRepository.ExistsAsync(e => e.Name == "GhostEntity");

        exists.Should().BeFalse();
    }

    [Fact]
    public async Task IgnoreQueryFilters_Should_IncludeSoftDeleted()
    {
        _context.SoftDeletableEntities.Add(SoftDeletableEntityBuilder.Default().WithName("Active").Build());
        _context.SoftDeletableEntities.Add(SoftDeletableEntityBuilder.Default().WithName("Deleted").AsDeleted().Build());
        await _context.SaveChangesAsync();

        // Directly use DbSet with IgnoreQueryFilters to verify soft delete exists
        var allIncludingDeleted = await _context.SoftDeletableEntities
            .IgnoreQueryFilters()
            .ToListAsync();

        allIncludingDeleted.Should().HaveCount(2);
    }

    [Fact]
    public async Task SoftDeletedEntity_Should_SetIsDeletedAndDeletedAt()
    {
        var entity = SoftDeletableEntityBuilder.Default().WithName("ToDelete").Build();
        _context.SoftDeletableEntities.Add(entity);
        await _context.SaveChangesAsync();

        entity.IsDeleted = true;
        entity.DeletedAt = DateTimeOffset.UtcNow;
        entity.DeletedBy = Guid.NewGuid();
        _context.SoftDeletableEntities.Update(entity);
        await _context.SaveChangesAsync();

        // Should be excluded from normal queries
        var results = await _readRepository.GetAllAsync();
        results.Should().BeEmpty();

        // But should exist in DB
        var inDb = await _context.SoftDeletableEntities
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(e => e.Id == entity.Id);
        inDb.Should().NotBeNull();
        inDb!.IsDeleted.Should().BeTrue();
        inDb.DeletedAt.Should().NotBeNull();
        inDb.DeletedBy.Should().NotBeNull();
    }

    public void Dispose()
    {
        _context.Dispose();
        _factory.Dispose();
    }
}
