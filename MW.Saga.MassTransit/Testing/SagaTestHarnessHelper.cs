using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using MW.Saga.MassTransit.Options;

namespace MW.Saga.MassTransit.Testing;

/// <summary>
/// Reusable helper for saga-oriented MassTransit test harness setup.
/// Simplifies test infrastructure for state-machine testing scenarios.
/// </summary>
public static class SagaTestHarnessHelper
{
    /// <summary>
    /// Configures a service collection with MassTransit test harness for saga testing.
    /// </summary>
    /// <typeparam name="TSaga">The saga state type.</typeparam>
    /// <typeparam name="TStateMachine">The state machine type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional configuration action for the bus.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSagaTestHarness<TSaga, TStateMachine>(
        this IServiceCollection services,
        Action<IBusRegistrationConfigurator>? configure = null)
        where TSaga : class, SagaStateMachineInstance
        where TStateMachine : MassTransitStateMachine<TSaga>
    {
        services.AddMassTransitTestHarness(cfg =>
        {
            cfg.AddSagaStateMachine<TStateMachine, TSaga>();
            configure?.Invoke(cfg);
        });

        return services;
    }
}
