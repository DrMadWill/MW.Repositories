namespace MW.Messaging.Headers;

/// <summary>
/// Shared constants for message header names used when publishing and consuming
/// messages through MassTransit. All publish/consume pipelines must use
/// these constants to ensure consistent header naming across services.
/// </summary>
public static class MessageHeaders
{
    /// <summary>
    /// Header name for the correlation identifier used in distributed tracing.
    /// </summary>
    public const string CorrelationId = "x-correlation-id";

    /// <summary>
    /// Header name for the causation identifier linking a message to its cause.
    /// </summary>
    public const string CausationId = "x-causation-id";

    /// <summary>
    /// Header name for the tenant identifier in multi-tenant systems.
    /// </summary>
    public const string TenantId = "x-tenant-id";

    /// <summary>
    /// Header name for the user identifier associated with the message.
    /// </summary>
    public const string UserId = "x-user-id";

    /// <summary>
    /// Header name for the source service that published the message.
    /// </summary>
    public const string SourceService = "x-source-service";

    /// <summary>
    /// Header name for the explicit event name.
    /// </summary>
    public const string EventName = "x-event-name";

    /// <summary>
    /// Header name for the event contract version.
    /// </summary>
    public const string EventVersion = "x-event-version";

    /// <summary>
    /// Header name for the distributed trace identifier (e.g., OpenTelemetry trace id).
    /// </summary>
    public const string TraceId = "x-trace-id";
}
