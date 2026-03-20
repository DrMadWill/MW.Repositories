using System.Linq.Expressions;
using MW.Core.Entities;
using MW.Persistence.Abstractions.Specifications;

namespace MW.Persistence.Abstractions.Repositories;

/// <summary>
/// Represents a generic read-only repository abstraction for querying entities.
/// This interface exposes only read operations and must not include any persistence-changing methods.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public interface IReadRepository<TEntity, in TId> where TEntity : class, IEntity<TId>
{
    /// <summary>
    /// Gets an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The entity if found; otherwise, <c>null</c>.</returns>
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of all entities.</returns>
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds entities matching the specified predicate.
    /// </summary>
    /// <param name="predicate">The condition to filter entities.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of entities matching the predicate.</returns>
    Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds entities matching the given specification.
    /// </summary>
    /// <param name="specification">The specification describing the query criteria.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of entities matching the specification.</returns>
    Task<IReadOnlyList<TEntity>> FindAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether any entity matches the specified predicate.
    /// </summary>
    /// <param name="predicate">The condition to check.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns><c>true</c> if any entity matches the predicate; otherwise, <c>false</c>.</returns>
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of all entities.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The total number of entities.</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of entities matching the specified predicate.
    /// </summary>
    /// <param name="predicate">The condition to count entities.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of entities matching the predicate.</returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
}
