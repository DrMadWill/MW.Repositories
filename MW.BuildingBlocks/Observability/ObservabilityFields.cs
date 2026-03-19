namespace MW.BuildingBlocks.Observability;

/// <summary>
/// Standard field names for Grafana/Graylog/Loki/Tempo log searches and correlations.
/// Using stable, consistent field names ensures that event logs can be correlated
/// with traces across observability tooling.
/// </summary>
public static class ObservabilityFields
{
    /// <summary>
    /// Field name for the distributed trace identifier.
    /// </summary>
    public const string TraceId = "trace_id";

    /// <summary>
    /// Field name for the correlation identifier used in distributed tracing.
    /// </summary>
    public const string CorrelationId = "correlation_id";

    /// <summary>
    /// Field name for the unique message identifier.
    /// </summary>
    public const string MessageId = "message_id";

    /// <summary>
    /// Field name for the explicit event name.
    /// </summary>
    public const string EventName = "event_name";

    /// <summary>
    /// Field name for the source service name.
    /// </summary>
    public const string ServiceName = "service_name";

    /// <summary>
    /// Field name for the consumer processing the message.
    /// </summary>
    public const string ConsumerName = "consumer_name";

    /// <summary>
    /// Field name for the endpoint where the message was received.
    /// </summary>
    public const string EndpointName = "endpoint_name";

    /// <summary>
    /// Field name for the event contract version.
    /// </summary>
    public const string EventVersion = "event_version";

    /// <summary>
    /// Field name for the processing status.
    /// </summary>
    public const string Status = "status";

    /// <summary>
    /// Field name for the processing duration in milliseconds.
    /// </summary>
    public const string DurationMs = "duration_ms";

    /// <summary>
    /// Field name for the causation identifier.
    /// </summary>
    public const string CausationId = "causation_id";
}
