namespace MW.Persistence.Abstractions.Transactions;

/// <summary>
/// Represents a handle to a persistence transaction scope.
/// Consumers use this to commit or rollback an active transaction.
/// </summary>
public interface ITransactionScope : IAsyncDisposable
{
    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
