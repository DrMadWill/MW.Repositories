namespace MW.Messaging.Messaging;

/// <summary>
/// Shared model describing the publishing or consuming service identity.
/// Useful for logs, traces, dashboards, and support/debugging scenarios
/// to consistently identify the source service across the platform.
/// </summary>
public class ServiceIdentity
{
    /// <summary>
    /// Gets or sets the name of the service.
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the version of the service (e.g., <c>1.0.0</c>).
    /// </summary>
    public string ServiceVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the deployment environment (e.g., <c>Production</c>, <c>Staging</c>, <c>Development</c>).
    /// </summary>
    public string Environment { get; set; } = string.Empty;
}
