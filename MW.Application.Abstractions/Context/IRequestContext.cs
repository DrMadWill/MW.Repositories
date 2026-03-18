namespace MW.Application.Abstractions.Context;

/// <summary>
/// Provides request-level metadata that flows across application services.
/// Used for logging, distributed tracing, tenant resolution, and diagnostics.
/// Infrastructure layer provides the implementation.
/// </summary>
public interface IRequestContext
{
    /// <summary>
    /// Gets the correlation identifier used for distributed tracing across services.
    /// </summary>
    string CorrelationId { get; }

    /// <summary>
    /// Gets the unique identifier for the current request.
    /// </summary>
    string RequestId { get; }

    /// <summary>
    /// Gets the tenant identifier for multi-tenant support.
    /// Returns <c>null</c> if the system is not multi-tenant or tenant is not resolved.
    /// </summary>
    Guid? TenantId { get; }

    /// <summary>
    /// Gets the culture/language code for the current request.
    /// </summary>
    string Culture { get; }

    /// <summary>
    /// Gets the IP address of the client making the request.
    /// Returns <c>null</c> if not available.
    /// </summary>
    string? ClientIp { get; }
}
