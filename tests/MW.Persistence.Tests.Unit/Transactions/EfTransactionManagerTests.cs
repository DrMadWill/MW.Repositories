using FluentAssertions;
using MW.Persistence.EntityFrameworkCore.Transactions;
using MW.Persistence.Tests.Shared.Infrastructure;

namespace MW.Persistence.Tests.Unit.Transactions;

/// <summary>
/// PTST-020: Unit tests for EfTransactionManager behavior.
/// Uses SQLite in-memory for transaction testing.
/// </summary>
public class EfTransactionManagerTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly TestDbContext _context;
    private readonly EfTransactionManager _transactionManager;

    public EfTransactionManagerTests()
    {
        _factory = new TestDbContextFactory();
        _context = _factory.CreateContext();
        _transactionManager = new EfTransactionManager(_context);
    }

    [Fact]
    public async Task BeginTransactionAsync_Should_ReturnTransactionScope()
    {
        var scope = await _transactionManager.BeginTransactionAsync();

        scope.Should().NotBeNull();

        await scope.DisposeAsync();
    }

    [Fact]
    public async Task TransactionScope_Should_SupportCommit()
    {
        await using var scope = await _transactionManager.BeginTransactionAsync();

        Func<Task> act = () => scope.CommitAsync();

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task TransactionScope_Should_SupportRollback()
    {
        await using var scope = await _transactionManager.BeginTransactionAsync();

        Func<Task> act = () => scope.RollbackAsync();

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_WhenDbContextIsNull()
    {
        Action act = () => new EfTransactionManager(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("dbContext");
    }

    public void Dispose()
    {
        _context.Dispose();
        _factory.Dispose();
    }
}
