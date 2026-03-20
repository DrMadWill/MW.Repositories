using System.Linq.Expressions;
using MW.Core.Entities;

namespace MW.Persistence.Abstractions.Repositories;

/// <summary>
/// Represents a read-only repository abstraction that supports projection-based queries.
/// Use this for query-side optimized reads where full entity materialization is not required.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public interface IProjectionReadRepository<TEntity, in TId> where TEntity : class, IEntity<TId>
{
    /// <summary>
    /// Projects entities matching the specified predicate into a target type.
    /// </summary>
    /// <typeparam name="TResult">The type of the projected result.</typeparam>
    /// <param name="predicate">The condition to filter entities.</param>
    /// <param name="selector">The projection expression.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of projected results.</returns>
    Task<IReadOnlyList<TResult>> ProjectAsync<TResult>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Projects a single entity by its identifier into a target type.
    /// </summary>
    /// <typeparam name="TResult">The type of the projected result.</typeparam>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="selector">The projection expression.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The projected result if found; otherwise, <c>null</c>.</returns>
    Task<TResult?> ProjectByIdAsync<TResult>(
        TId id,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken cancellationToken = default);
}
