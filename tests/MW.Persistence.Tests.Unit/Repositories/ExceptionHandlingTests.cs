using FluentAssertions;
using MW.Persistence.EntityFrameworkCore.Repositories;
using MW.Persistence.EntityFrameworkCore.Transactions;
using MW.Persistence.EntityFrameworkCore.UnitOfWork;
using MW.Persistence.Tests.Shared.Entities;
using MW.Persistence.Tests.Shared.Infrastructure;

namespace MW.Persistence.Tests.Unit.Repositories;

/// <summary>
/// PTST-023: Unit tests for exception handling strategy in persistence layer.
/// Validates that constructors and operations throw appropriate exceptions.
/// </summary>
public class ExceptionHandlingTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly TestDbContext _context;

    public ExceptionHandlingTests()
    {
        _factory = new TestDbContextFactory();
        _context = _factory.CreateContext();
    }

    [Fact]
    public void EfReadRepository_Should_ThrowArgumentNullException_OnNullDbContext()
    {
        Action act = () => new EfReadRepository<TestEntity, Guid>(null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("dbContext");
    }

    [Fact]
    public void EfWriteRepository_Should_ThrowArgumentNullException_OnNullDbContext()
    {
        Action act = () => new EfWriteRepository<TestEntity, Guid>(null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("dbContext");
    }

    [Fact]
    public void EfRepository_Should_ThrowArgumentNullException_OnNullDbContext()
    {
        Action act = () => new EfRepository<TestEntity, Guid>(null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("dbContext");
    }

    [Fact]
    public void EfAggregateRepository_Should_ThrowArgumentNullException_OnNullDbContext()
    {
        Action act = () => new EfAggregateRepository<TestAggregate, Guid>(null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("dbContext");
    }

    [Fact]
    public void EfProjectionReadRepository_Should_ThrowArgumentNullException_OnNullDbContext()
    {
        Action act = () => new EfProjectionReadRepository<TestEntity, Guid>(null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("dbContext");
    }

    [Fact]
    public void EfUnitOfWork_Should_ThrowArgumentNullException_OnNullDbContext()
    {
        Action act = () => new EfUnitOfWork(null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("dbContext");
    }

    [Fact]
    public void EfTransactionManager_Should_ThrowArgumentNullException_OnNullDbContext()
    {
        Action act = () => new EfTransactionManager(null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("dbContext");
    }

    [Fact]
    public void EfTransactionScope_Should_ThrowArgumentNullException_OnNullTransaction()
    {
        Action act = () => new EfTransactionScope(null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("transaction");
    }

    [Fact]
    public async Task Operations_OnDisposedContext_Should_Throw()
    {
        var repo = new EfReadRepository<TestEntity, Guid>(_context);
        _context.Dispose();

        Func<Task> act = () => repo.GetAllAsync();
        await act.Should().ThrowAsync<ObjectDisposedException>();
    }

    public void Dispose()
    {
        try { _factory.Dispose(); } catch { }
    }
}
