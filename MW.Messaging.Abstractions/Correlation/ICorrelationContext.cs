namespace MW.Messaging.Correlation;

/// <summary>
/// Shared abstraction for distributed tracing and request correlation across services.
/// Provides a unified way to expose correlation data independently from broker-specific
/// or transport-specific types, so that business logic does not depend directly
/// on MassTransit context types.
/// </summary>
public interface ICorrelationContext
{
    /// <summary>
    /// Gets the correlation identifier used for distributed tracing across services.
    /// Returns <c>null</c> if not available.
    /// </summary>
    string? CorrelationId { get; }

    /// <summary>
    /// Gets the causation identifier linking this context to the command or event that caused it.
    /// Returns <c>null</c> if not available.
    /// </summary>
    string? CausationId { get; }

    /// <summary>
    /// Gets the trace identifier for distributed tracing (e.g., OpenTelemetry trace id).
    /// Returns <c>null</c> if not available.
    /// </summary>
    string? TraceId { get; }
}
