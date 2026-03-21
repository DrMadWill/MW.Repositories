using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MW.Core.Entities;
using MW.Persistence.Abstractions.Repositories;
using MW.Persistence.Abstractions.Specifications;

namespace MW.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
/// EF Core implementation of the read-only repository abstraction.
/// Provides entity read operations using <see cref="DbContext"/> with no-tracking queries by default.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public class EfReadRepository<TEntity, TId> : IReadRepository<TEntity, TId>
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
    /// Initializes a new instance of the <see cref="EfReadRepository{TEntity, TId}"/> class.
    /// </summary>
    /// <param name="dbContext">The EF Core database context.</param>
    public EfReadRepository(DbContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        DbSet = dbContext.Set<TEntity>();
    }

    /// <inheritdoc />
    public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(new object?[] { id }, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TEntity>> FindAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        return await query.ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().AnyAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<int> CountAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().CountAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Applies the specification to the queryable, including criteria, ordering, and paging.
    /// </summary>
    /// <param name="specification">The specification to apply.</param>
    /// <returns>The filtered and configured queryable.</returns>
    protected virtual IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> specification)
    {
        IQueryable<TEntity> query = DbSet.AsNoTracking();

        if (specification.Criteria is not null)
            query = query.Where(specification.Criteria);

        if (specification.OrderBy is not null)
            query = query.OrderBy(specification.OrderBy);
        else if (specification.OrderByDescending is not null)
            query = query.OrderByDescending(specification.OrderByDescending);

        if (specification.Skip.HasValue)
            query = query.Skip(specification.Skip.Value);

        if (specification.Take.HasValue)
            query = query.Take(specification.Take.Value);

        return query;
    }
}
