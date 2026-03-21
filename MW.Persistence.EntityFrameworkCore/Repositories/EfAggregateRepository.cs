using Microsoft.EntityFrameworkCore;
using MW.Core.AggregateRoots;
using MW.Persistence.Abstractions.Repositories;

namespace MW.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
/// EF Core implementation of the aggregate root repository abstraction.
/// Aligns persistence operations with DDD aggregate boundaries.
/// </summary>
/// <typeparam name="TAggregate">The aggregate root type.</typeparam>
/// <typeparam name="TId">The type of the aggregate root identifier.</typeparam>
public class EfAggregateRepository<TAggregate, TId> : EfRepository<TAggregate, TId>, IAggregateRepository<TAggregate, TId>
    where TAggregate : class, IAggregateRoot<TId>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EfAggregateRepository{TAggregate, TId}"/> class.
    /// </summary>
    /// <param name="dbContext">The EF Core database context.</param>
    public EfAggregateRepository(DbContext dbContext) : base(dbContext)
    {
    }
}
