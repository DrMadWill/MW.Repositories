using MW.Core.Entities;

namespace MW.Core.AggregateRoots;

/// <summary>
/// Marker interface for aggregate roots.
/// Repository patterns should target aggregate roots.
/// </summary>
public interface IAggregateRoot<TId> : IEntity<TId>
{
}
