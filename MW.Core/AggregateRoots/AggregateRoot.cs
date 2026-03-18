using MW.Core.Entities;

namespace MW.Core.AggregateRoots;

/// <summary>
/// Base class for aggregate roots, extending <see cref="Entity{TId}"/>
/// with aggregate root semantics.
/// </summary>
/// <typeparam name="TId">The type of the aggregate root identifier.</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot<TId>
{
}
