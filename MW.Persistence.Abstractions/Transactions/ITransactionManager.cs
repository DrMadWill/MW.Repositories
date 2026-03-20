namespace MW.Persistence.Abstractions.Transactions;

/// <summary>
/// Represents an abstraction for persistence-level transaction management.
/// Provides explicit transaction control without exposing provider-specific transaction types.
/// </summary>
public interface ITransactionManager
{
    /// <summary>
    /// Begins a new transaction.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A handle to the started transaction.</returns>
    Task<ITransactionScope> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
