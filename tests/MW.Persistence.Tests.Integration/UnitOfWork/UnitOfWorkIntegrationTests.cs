using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MW.Persistence.EntityFrameworkCore.Repositories;
using MW.Persistence.EntityFrameworkCore.UnitOfWork;
using MW.Persistence.Tests.Shared.Builders;
using MW.Persistence.Tests.Shared.Entities;
using MW.Persistence.Tests.Shared.Infrastructure;

namespace MW.Persistence.Tests.Integration.UnitOfWork;

/// <summary>
/// PTST-026: Integration tests for UnitOfWork commit behavior.
/// </summary>
public class UnitOfWorkIntegrationTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly TestDbContext _context;
    private readonly EfUnitOfWork _unitOfWork;
    private readonly EfRepository<TestEntity, Guid> _repository;

    public UnitOfWorkIntegrationTests()
    {
        _factory = new TestDbContextFactory();
        _context = _factory.CreateContext();
        _unitOfWork = new EfUnitOfWork(_context);
        _repository = new EfRepository<TestEntity, Guid>(_context);
    }

    [Fact]
    public async Task SaveChangesAsync_Should_CommitRepositoryChanges()
    {
        var entity = TestEntityBuilder.Default().WithName("UoW-Test").Build();
        await _repository.AddAsync(entity);

        var affected = await _unitOfWork.SaveChangesAsync();

        affected.Should().Be(1);

        using var verifyContext = _factory.CreateContext();
        var found = await verifyContext.TestEntities.FindAsync(entity.Id);
        found.Should().NotBeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_Should_CommitMultipleOperations()
    {
        await _repository.AddAsync(TestEntityBuilder.Default().Build());
        await _repository.AddAsync(TestEntityBuilder.Default().Build());

        var affected = await _unitOfWork.SaveChangesAsync();

        affected.Should().Be(2);
    }

    [Fact]
    public async Task MultipleRepositories_Should_ShareSameUnitOfWork()
    {
        var entityRepo = new EfRepository<TestEntity, Guid>(_context);
        var aggregateRepo = new EfAggregateRepository<TestAggregate, Guid>(_context);

        await entityRepo.AddAsync(TestEntityBuilder.Default().Build());
        await aggregateRepo.AddAsync(TestAggregateBuilder.Default().Build());

        // Single SaveChanges should commit both
        var affected = await _unitOfWork.SaveChangesAsync();

        affected.Should().Be(2);
    }

    [Fact]
    public async Task Changes_Should_NotPersist_WithoutSaveChanges()
    {
        await _repository.AddAsync(TestEntityBuilder.Default().Build());

        // NOT calling SaveChangesAsync

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
