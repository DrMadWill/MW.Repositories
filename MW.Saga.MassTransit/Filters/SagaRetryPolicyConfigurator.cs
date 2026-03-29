using MassTransit;
using MW.Saga.MassTransit.Options;

namespace MW.Saga.MassTransit.Filters;

/// <summary>
/// Provides reusable retry policy integration for saga event handling.
/// Standardizes retry behavior so transient failures are handled consistently.
/// </summary>
public static class SagaRetryPolicyConfigurator
{
    /// <summary>
    /// Applies retry policy to a receive endpoint configurator based on the provided options.
    /// </summary>
    /// <param name="configurator">The receive endpoint configurator to add retry to.</param>
    /// <param name="options">The saga options containing retry settings.</param>
    public static void ApplyRetryPolicy(
        IReceiveEndpointConfigurator configurator,
        SagaMassTransitOptions options)
    {
        ArgumentNullException.ThrowIfNull(configurator);
        ArgumentNullException.ThrowIfNull(options);

        configurator.UseMessageRetry(retryConfig =>
        {
            if (options.RetryIntervalsInSeconds.Length > 0)
            {
                retryConfig.Intervals(
                    options.RetryIntervalsInSeconds
                        .Select(s => TimeSpan.FromSeconds(s))
                        .ToArray());
            }
            else
            {
                retryConfig.Interval(options.RetryCount, TimeSpan.FromSeconds(2));
            }
        });
    }
}
