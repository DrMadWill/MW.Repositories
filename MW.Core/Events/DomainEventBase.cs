namespace MW.Core.Events;

/// <summary>
/// Base class for domain events providing a default timestamp.
/// </summary>
public abstract class DomainEventBase : IDomainEvent
{
    /// <inheritdoc />
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
