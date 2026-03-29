using MassTransit;
using MW.Saga.MassTransit.Options;

namespace MW.Saga.MassTransit.Scheduling;

/// <summary>
/// Reusable scheduler/timeout infrastructure support for saga workflows.
/// Provides helper methods to configure MassTransit message scheduling
/// for timeout-based saga transitions.
/// </summary>
public static class SagaSchedulerConfigurator
{
    /// <summary>
    /// Configures the message scheduler on the bus factory configurator
    /// for saga timeout support.
    /// </summary>
    /// <param name="configurator">The bus factory configurator.</param>
    /// <param name="options">The saga options containing scheduler settings.</param>
    public static void ConfigureScheduler(
        IBusFactoryConfigurator configurator,
        SagaMassTransitOptions options)
    {
        ArgumentNullException.ThrowIfNull(configurator);
        ArgumentNullException.ThrowIfNull(options);

        if (options.UseScheduler)
        {
            configurator.UseDelayedMessageScheduler();
        }
    }
}
