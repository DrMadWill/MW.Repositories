using FluentAssertions;
using MW.Persistence.EntityFrameworkCore.Repositories;
using MW.Persistence.EntityFrameworkCore.Transactions;
using MW.Persistence.Tests.Shared.Builders;
using MW.Persistence.Tests.Shared.Entities;
using MW.Persistence.Tests.Shared.Infrastructure;

namespace MW.Persistence.Tests.Integration.Transactions;

/// <summary>
/// PTST-030: Integration tests for transaction behavior.
/// </summary>
public class TransactionIntegrationTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly TestDbContext _context;
    private readonly EfTransactionManager _transactionManager;
    private readonly EfRepository<TestEntity, Guid> _repository;

    public TransactionIntegrationTests()
    {
        _factory = new TestDbContextFactory();
        _context = _factory.CreateContext();
        _transactionManager = new EfTransactionManager(_context);
        _repository = new EfRepository<TestEntity, Guid>(_context);
    }

    [Fact]
    public async Task CommitTransaction_Should_PersistChanges()
    {
        await using var scope = await _transactionManager.BeginTransactionAsync();

        var entity = TestEntityBuilder.Default().WithName("Committed").Build();
        await _repository.AddAsync(entity);
        await _context.SaveChangesAsync();
        await scope.CommitAsync();

        using var verifyContext = _factory.CreateContext();
        var found = await verifyContext.TestEntities.FindAsync(entity.Id);
        found.Should().NotBeNull();
        found!.Name.Should().Be("Committed");
    }

    [Fact]
    public async Task RollbackTransaction_Should_DiscardChanges()
    {
        await using var scope = await _transactionManager.BeginTransactionAsync();

        var entity = TestEntityBuilder.Default().WithName("Rolled Back").Build();
        await _repository.AddAsync(entity);
        await _context.SaveChangesAsync();
        await scope.RollbackAsync();

        using var verifyContext = _factory.CreateContext();
        var found = await verifyContext.TestEntities.FindAsync(entity.Id);
        found.Should().BeNull();
    }

    [Fact]
    public async Task DisposeWithoutCommit_Should_DiscardChanges()
    {
        var entityId = Guid.NewGuid();

        {
            await using var scope = await _transactionManager.BeginTransactionAsync();
            var entity = TestEntityBuilder.Default().WithId(entityId).WithName("Auto Rollback").Build();
            await _repository.AddAsync(entity);
            await _context.SaveChangesAsync();
            // Dispose without commit (implicit rollback)
        }

        using var verifyContext = _factory.CreateContext();
        var found = await verifyContext.TestEntities.FindAsync(entityId);
        found.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Dispose();
        _factory.Dispose();
    }
}
