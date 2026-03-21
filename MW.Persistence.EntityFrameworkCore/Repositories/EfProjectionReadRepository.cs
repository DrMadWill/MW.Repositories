using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MW.Core.Entities;
using MW.Persistence.Abstractions.Repositories;

namespace MW.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
/// EF Core implementation of the projection read repository abstraction.
/// Provides query-side optimized reads where full entity materialization is not required.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public class EfProjectionReadRepository<TEntity, TId> : IProjectionReadRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>
{
    /// <summary>
    /// The underlying EF Core database context.
    /// </summary>
    protected readonly DbContext DbContext;

    /// <summary>
    /// The <see cref="DbSet{TEntity}"/> for querying entities.
    /// </summary>
    protected readonly DbSet<TEntity> DbSet;

    /// <summary>
    /// Initializes a new instance of the <see cref="EfProjectionReadRepository{TEntity, TId}"/> class.
    /// </summary>
    /// <param name="dbContext">The EF Core database context.</param>
    public EfProjectionReadRepository(DbContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        DbSet = dbContext.Set<TEntity>();
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TResult>> ProjectAsync<TResult>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(predicate)
            .Select(selector)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<TResult?> ProjectByIdAsync<TResult>(
        TId id,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(e => e.Id!.Equals(id))
            .Select(selector)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
