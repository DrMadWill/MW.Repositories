using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MW.Saga.Context;
using MW.Saga.MassTransit.Context;
using MW.Saga.MassTransit.Naming;
using MW.Saga.MassTransit.Observers;
using MW.Saga.MassTransit.Options;
using MW.Saga.MassTransit.Scheduling;
using MW.Saga.Observability;

namespace MW.Saga.MassTransit.Extensions;

/// <summary>
/// Main DI entry point for registering saga infrastructure in services using MassTransit.
/// Provides reusable, infrastructure-focused service registration without hardcoding
/// service-specific saga types or behaviors.
/// </summary>
public static class SagaMassTransitServiceCollectionExtensions
{
    /// <summary>
    /// Registers MassTransit saga infrastructure through a single entry point.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Action to configure saga MassTransit options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSagaMassTransitInfrastructure(
        this IServiceCollection services,
        Action<SagaMassTransitRegistrationOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        var registrationOptions = new SagaMassTransitRegistrationOptions();
        configure(registrationOptions);

        // Register scoped saga context accessor
        services.TryAddScoped<ScopedSagaContextAccessor>();
        services.TryAddScoped<ISagaContextAccessor>(sp => sp.GetRequiredService<ScopedSagaContextAccessor>());

        // Register saga execution context
        services.TryAddScoped<ISagaExecutionContext, MassTransitSagaExecutionContext>();

        // Register structured saga logger as the default observer if none provided
        services.TryAddSingleton<ISagaObserver, StructuredSagaLogger>();

        return services;
    }

    /// <summary>
    /// Binds <see cref="SagaMassTransitOptions"/> from a configuration section.
    /// </summary>
    /// <param name="registrationOptions">The registration options to bind into.</param>
    /// <param name="configuration">The configuration root.</param>
    /// <param name="sectionName">The configuration section name.</param>
    /// <returns>The registration options for chaining.</returns>
    public static SagaMassTransitRegistrationOptions BindOptions(
        this SagaMassTransitRegistrationOptions registrationOptions,
        IConfiguration configuration,
        string sectionName = SagaMassTransitOptions.SectionName)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(sectionName);
        section.Bind(registrationOptions.Options);
        return registrationOptions;
    }

    /// <summary>
    /// Registers a saga state machine with its state type in MassTransit.
    /// </summary>
    /// <typeparam name="TStateMachine">The saga state machine type.</typeparam>
    /// <typeparam name="TSaga">The saga state type.</typeparam>
    /// <param name="configurator">The MassTransit bus registration configurator.</param>
    /// <returns>The configurator for chaining.</returns>
    public static IBusRegistrationConfigurator RegisterSagaStateMachine<TStateMachine, TSaga>(
        this IBusRegistrationConfigurator configurator)
        where TStateMachine : MassTransitStateMachine<TSaga>
        where TSaga : class, SagaStateMachineInstance
    {
        ArgumentNullException.ThrowIfNull(configurator);

        configurator.AddSagaStateMachine<TStateMachine, TSaga>();
        return configurator;
    }
}

/// <summary>
/// Options for configuring MassTransit saga infrastructure registration.
/// </summary>
public class SagaMassTransitRegistrationOptions
{
    /// <summary>
    /// Gets or sets the saga infrastructure options.
    /// </summary>
    public SagaMassTransitOptions Options { get; set; } = new();
}
