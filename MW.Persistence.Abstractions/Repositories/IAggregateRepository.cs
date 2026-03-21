using MW.Core.AggregateRoots;

namespace MW.Persistence.Abstractions.Repositories;

/// <summary>
/// Represents a repository abstraction for aggregate roots.
/// This contract aligns persistence operations with DDD aggregate boundaries.
/// </summary>
/// <typeparam name="TAggregate">The type of the aggregate root. Must implement <see cref="IAggregateRoot{TId}"/>.</typeparam>
/// <typeparam name="TId">The type of the aggregate root identifier.</typeparam>
public interface IAggregateRepository<TAggregate, TId> : IRepository<TAggregate, TId>
    where TAggregate : class, IAggregateRoot<TId>
{
}
