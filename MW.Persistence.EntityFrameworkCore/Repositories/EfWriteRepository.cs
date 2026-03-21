using Microsoft.EntityFrameworkCore;
using MW.Core.Entities;
using MW.Persistence.Abstractions.Repositories;

namespace MW.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
/// EF Core implementation of the write repository abstraction.
/// Provides entity mutation operations using <see cref="DbContext"/>.
/// Save/commit is not handled here — it is the responsibility of <see cref="MW.Persistence.Abstractions.UnitOfWork.IUnitOfWork"/>.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public class EfWriteRepository<TEntity, TId> : IWriteRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>
{
    /// <summary>
    /// The underlying EF Core database context.
    /// </summary>
    protected readonly DbContext DbContext;

    /// <summary>
    /// The <see cref="DbSet{TEntity}"/> for persisting entities.
    /// </summary>
    protected readonly DbSet<TEntity> DbSet;

    /// <summary>
    /// Initializes a new instance of the <see cref="EfWriteRepository{TEntity, TId}"/> class.
    /// </summary>
    /// <param name="dbContext">The EF Core database context.</param>
    public EfWriteRepository(DbContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        DbSet = dbContext.Set<TEntity>();
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
