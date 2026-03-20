namespace MW.Persistence.Abstractions.UnitOfWork;

/// <summary>
/// Represents the Unit of Work pattern abstraction.
/// Responsible for committing all pending persistence changes as a single atomic operation.
/// Repository abstractions should not save changes automatically — commit logic is centralized here.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Saves all pending changes to the underlying data store.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of state entries written to the data store.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
