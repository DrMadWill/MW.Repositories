using MW.Core.Events;

namespace MW.Core.Entities;

/// <summary>
/// Base class for entities providing identity, equality, transient check, and domain event support.
/// </summary>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public abstract class Entity<TId> : IEntity<TId>, IHasDomainEvents
{
    /// <inheritdoc />
    public TId Id { get; set; } = default!;

    private readonly List<IDomainEvent> _domainEvents = new();

    /// <inheritdoc />
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Adds a domain event to this entity.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Removes a domain event from this entity.
    /// </summary>
    /// <param name="domainEvent">The domain event to remove.</param>
    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    /// <inheritdoc />
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Indicates whether the entity has not been persisted yet (Id is default).
    /// </summary>
    public bool IsTransient()
    {
        return EqualityComparer<TId>.Default.Equals(Id, default!);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        if (IsTransient() || other.IsTransient())
            return false;

        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        if (IsTransient())
            return base.GetHashCode();

        return Id!.GetHashCode();
    }

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
    {
        return !(left == right);
    }
}
