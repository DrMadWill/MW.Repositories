using MW.Saga.MassTransit.Options;

namespace MW.Saga.MassTransit.Scheduling;

/// <summary>
/// Helper for standardizing timeout-related naming and integration patterns
/// so timeout usage across sagas remains predictable.
/// </summary>
public static class SagaTimeoutConventions
{
    /// <summary>
    /// Gets the default timeout duration from the provided options.
    /// </summary>
    /// <param name="options">The saga options.</param>
    /// <returns>The default timeout as a <see cref="TimeSpan"/>.</returns>
    public static TimeSpan GetDefaultTimeout(SagaMassTransitOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return TimeSpan.FromSeconds(options.DefaultTimeoutInSeconds);
    }

    /// <summary>
    /// Generates a standardized timeout endpoint name for the given saga type.
    /// </summary>
    /// <param name="sagaName">The name of the saga.</param>
    /// <param name="prefix">Optional endpoint prefix.</param>
    /// <returns>A standardized timeout endpoint name.</returns>
    public static string GetTimeoutEndpointName(string sagaName, string? prefix = null)
    {
        ArgumentNullException.ThrowIfNull(sagaName);

        var sanitized = ToKebabCase(sagaName);
        return string.IsNullOrWhiteSpace(prefix)
            ? $"{sanitized}-timeout"
            : $"{prefix.Trim().ToLowerInvariant()}-{sanitized}-timeout";
    }

    private static string ToKebabCase(string name)
    {
        return string.Concat(name.Select((c, i) =>
            i > 0 && char.IsUpper(c) ? "-" + char.ToLowerInvariant(c) : char.ToLowerInvariant(c).ToString()));
    }
}
