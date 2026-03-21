using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MW.Core.Entities;
using MW.Persistence.Abstractions.Repositories;
using MW.Persistence.Abstractions.Specifications;

namespace MW.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
/// EF Core implementation of the combined read and write repository abstraction.
/// Provides both query and mutation operations through a single dependency.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public class EfRepository<TEntity, TId> : EfReadRepository<TEntity, TId>, IRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EfRepository{TEntity, TId}"/> class.
    /// </summary>
    /// <param name="dbContext">The EF Core database context.</param>
    public EfRepository(DbContext dbContext) : base(dbContext)
    {
    }

    /// <inheritdoc />
    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);
    }

    /// <inheritdoc />
    public virtual void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    /// <inheritdoc />
    public virtual void Remove(TEntity entity)
    {
        DbSet.Remove(entity);
    }

    /// <inheritdoc />
    public virtual void RemoveRange(IEnumerable<TEntity> entities)
    {
        DbSet.RemoveRange(entities);
    }
}
