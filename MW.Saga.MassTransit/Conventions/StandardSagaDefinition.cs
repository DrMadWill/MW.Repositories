using MassTransit;
using MW.Saga.MassTransit.Options;

namespace MW.Saga.MassTransit.Conventions;

/// <summary>
/// Reusable saga definition/convention patterns so endpoint, retry,
/// concurrency, and partitioning-related setup can be standardized.
/// Provides a base saga definition that applies common infrastructure settings.
/// </summary>
/// <typeparam name="TSaga">The saga state type.</typeparam>
public class StandardSagaDefinition<TSaga> : SagaDefinition<TSaga>
    where TSaga : class, ISaga
{
    private readonly SagaMassTransitOptions _options;

    /// <summary>
    /// Initializes a new instance with default options.
    /// </summary>
    public StandardSagaDefinition()
        : this(new SagaMassTransitOptions())
    {
    }

    /// <summary>
    /// Initializes a new instance with the specified options.
    /// </summary>
    /// <param name="options">The saga options for configuring conventions.</param>
    public StandardSagaDefinition(SagaMassTransitOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    protected override void ConfigureSaga(
        IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<TSaga> sagaConfigurator,
        IRegistrationContext context)
    {
        // Concurrency limit
        if (_options.ConcurrencyLimit > 0)
        {
            endpointConfigurator.PrefetchCount = _options.ConcurrencyLimit;
            endpointConfigurator.ConcurrentMessageLimit = _options.ConcurrencyLimit;
        }

        // Retry policy
        endpointConfigurator.UseMessageRetry(retryConfig =>
        {
            if (_options.RetryIntervalsInSeconds.Length > 0)
            {
                retryConfig.Intervals(
                    _options.RetryIntervalsInSeconds
                        .Select(s => TimeSpan.FromSeconds(s))
                        .ToArray());
            }
            else
            {
                retryConfig.Interval(_options.RetryCount, TimeSpan.FromSeconds(2));
            }
        });
    }
}
