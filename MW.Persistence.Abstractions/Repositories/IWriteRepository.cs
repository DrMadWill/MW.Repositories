using MW.Core.Entities;

namespace MW.Persistence.Abstractions.Repositories;

/// <summary>
/// Represents a generic write repository abstraction for state-changing persistence operations.
/// This interface contains only mutation methods. Save/commit is the responsibility of <see cref="MW.Persistence.Abstractions.UnitOfWork.IUnitOfWork"/>.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public interface IWriteRepository<TEntity, TId> where TEntity : class, IEntity<TId>
{
    /// <summary>
    /// Adds a new entity.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a collection of entities.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(TEntity entity);

    /// <summary>
    /// Removes an entity.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    void Remove(TEntity entity);

    /// <summary>
    /// Removes a collection of entities.
    /// </summary>
    /// <param name="entities">The entities to remove.</param>
    void RemoveRange(IEnumerable<TEntity> entities);
}
