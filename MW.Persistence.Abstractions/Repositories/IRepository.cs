using MW.Core.Entities;

namespace MW.Persistence.Abstractions.Repositories;

/// <summary>
/// Represents a combined repository abstraction that provides both read and write operations.
/// Inherits from <see cref="IReadRepository{TEntity, TId}"/> and <see cref="IWriteRepository{TEntity, TId}"/>.
/// Use this when both behaviors are needed through a single dependency.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public interface IRepository<TEntity, TId> : IReadRepository<TEntity, TId>, IWriteRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>
{
}
