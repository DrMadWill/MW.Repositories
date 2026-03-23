using MediatR;

namespace MW.Core.Events;

/// <summary>
/// Marker interface for domain events.
/// Inherits from <see cref="INotification"/> so domain events can be published
/// directly through MediatR without a wrapper notification type.
/// </summary>
public interface IDomainEvent : INotification
{
    /// <summary>
    /// Gets the date and time (UTC) when the event occurred.
    /// </summary>
    DateTimeOffset OccurredOn { get; }
}
