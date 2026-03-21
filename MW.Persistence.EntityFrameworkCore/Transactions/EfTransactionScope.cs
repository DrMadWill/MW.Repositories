using Microsoft.EntityFrameworkCore.Storage;
using MW.Persistence.Abstractions.Transactions;

namespace MW.Persistence.EntityFrameworkCore.Transactions;

/// <summary>
/// EF Core implementation of a transaction scope.
/// Wraps an <see cref="IDbContextTransaction"/> to provide commit and rollback operations.
/// </summary>
public class EfTransactionScope : ITransactionScope
{
    private readonly IDbContextTransaction _transaction;

    /// <summary>
    /// Initializes a new instance of the <see cref="EfTransactionScope"/> class.
    /// </summary>
    /// <param name="transaction">The underlying EF Core database transaction.</param>
    public EfTransactionScope(IDbContextTransaction transaction)
    {
        _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
    }

    /// <inheritdoc />
    public virtual async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.CommitAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.RollbackAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await _transaction.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
