namespace MW.Messaging.Contracts;

/// <summary>
/// Base implementation of <see cref="IIntegrationEvent"/> providing sensible defaults
/// for integration events exchanged between microservices.
/// </summary>
public abstract class IntegrationEvent : IIntegrationEvent
{
    /// <inheritdoc />
    public Guid EventId { get; init; } = Guid.NewGuid();

    /// <inheritdoc />
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;

    /// <inheritdoc />
    public abstract string EventName { get; }

    /// <inheritdoc />
    public abstract string EventVersion { get; }

    /// <inheritdoc />
    public abstract string SourceService { get; }

    /// <inheritdoc />
    public string? CorrelationId { get; init; }

    /// <inheritdoc />
    public string? CausationId { get; init; }
}
