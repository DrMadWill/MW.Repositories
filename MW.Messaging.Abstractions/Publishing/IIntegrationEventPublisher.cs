namespace MW.Messaging.Publishing;

/// <summary>
/// Application-facing contract for publishing integration events to the message bus.
/// <para>
/// This abstraction hides transport-specific implementation details (e.g., MassTransit, RabbitMQ)
/// so that business code never depends on a concrete messaging library.
/// </para>
/// <para>
/// Only cross-service integration events should be published through this abstraction.
/// Domain events that stay within a single service boundary should use in-process dispatching instead.
/// </para>
/// <para>
/// A concrete implementation will be provided by <c>MW.Messaging.MassTransit</c>.
/// </para>
/// </summary>
public interface IIntegrationEventPublisher
{
    /// <summary>
    /// Publishes an integration event to the message bus.
    /// </summary>
    /// <param name="integrationEvent">The integration event to publish.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous publish operation.</returns>
    Task PublishAsync(Contracts.IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes an integration event to the message bus with additional publish context metadata.
    /// </summary>
    /// <param name="integrationEvent">The integration event to publish.</param>
    /// <param name="context">Contextual metadata (correlation, tenant, user, etc.) to attach to the published message.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous publish operation.</returns>
    Task PublishAsync(Contracts.IIntegrationEvent integrationEvent, Messaging.PublishContextModel context, CancellationToken cancellationToken = default);
}
