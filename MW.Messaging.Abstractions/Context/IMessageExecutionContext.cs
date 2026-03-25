namespace MW.Messaging.Context;

/// <summary>
/// Represents transport-agnostic execution metadata for message-driven flows.
/// <para>
/// This is an application-facing abstraction that exposes the most important message context
/// values needed by business logic. It aligns with <see cref="Messaging.ConsumerContextModel"/>
/// and existing correlation/message metadata concepts.
/// </para>
/// <para>
/// <b>IMessageExecutionContext vs ConsumerContextModel:</b>
/// <see cref="Messaging.ConsumerContextModel"/> is a plain data-transfer model.
/// <c>IMessageExecutionContext</c> is the runtime abstraction that application code should depend on.
/// Mapping from the data model to this abstraction should happen in the implementation layer.
/// Business logic should prefer this abstraction over transport-specific runtime objects.
/// </para>
/// <para>
/// This interface must not depend on HTTP types or MassTransit types directly.
/// </para>
/// </summary>
public interface IMessageExecutionContext
{
    /// <summary>Gets the correlation identifier for distributed tracing.</summary>
    string? CorrelationId { get; }

    /// <summary>Gets the causation identifier linking this message to its cause.</summary>
    string? CausationId { get; }

    /// <summary>Gets the distributed trace identifier.</summary>
    string? TraceId { get; }

    /// <summary>Gets the identifier of the user associated with this message flow.</summary>
    Guid? UserId { get; }

    /// <summary>Gets the tenant identifier for multi-tenant scenarios.</summary>
    Guid? TenantId { get; }

    /// <summary>Gets the name of the service that originated this message.</summary>
    string? SourceService { get; }

    /// <summary>Gets the unique message identifier.</summary>
    Guid MessageId { get; }
}
