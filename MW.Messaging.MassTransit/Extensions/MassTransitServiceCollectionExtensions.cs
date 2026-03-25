using System.Reflection;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MW.Messaging.Context;
using MW.Messaging.MassTransit.Context;
using MW.Messaging.MassTransit.Filters;
using MW.Messaging.MassTransit.Naming;
using MW.Messaging.MassTransit.Observers;
using MW.Messaging.MassTransit.Options;
using MW.Messaging.Publishing;

namespace MW.Messaging.MassTransit.Extensions;

public static class MassTransitServiceCollectionExtensions
{
    public static IServiceCollection AddMassTransitMessaging(
        this IServiceCollection services,
        Action<MassTransitMessagingOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        var options = new MassTransitMessagingOptions();
        configure(options);

        // Register scoped message context accessor
        services.TryAddScoped<ScopedMessageContextAccessor>();
        services.TryAddScoped<IMessageContextAccessor>(sp => sp.GetRequiredService<ScopedMessageContextAccessor>());

        // Register header mapper
        services.TryAddSingleton<MW.Messaging.MassTransit.IMessageHeaderMapper, DefaultMessageHeaderMapper>();

        // Register publish context provider
        services.TryAddScoped<IPublishContextProvider, DefaultPublishContextProvider>();

        // Register integration event publisher
        services.TryAddScoped<IIntegrationEventPublisher, Publishing.MassTransitIntegrationEventPublisher>();

        // Register MassTransit
        services.AddMassTransit(busConfig =>
        {
            // Consumer registration - assembly scanning
            if (options.ConsumerAssemblies.Count > 0)
            {
                foreach (var assembly in options.ConsumerAssemblies)
                {
                    busConfig.AddConsumers(assembly);
                }
            }

            // Custom consumer registration hook
            options.ConfigureConsumersAction?.Invoke(busConfig);

            // Endpoint naming
            var endpointFormatter = !string.IsNullOrWhiteSpace(options.Options.ServiceName)
                ? new ServiceEndpointNameFormatter(options.Options.ServiceName)
                : new ServiceEndpointNameFormatter(string.Empty);
            busConfig.SetEndpointNameFormatter(endpointFormatter);

            // Outbox configuration
            if (options.OutboxConfigurator != null)
            {
                options.OutboxConfigurator(busConfig);
            }

            // RabbitMQ transport
            busConfig.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqOptions = options.Options.RabbitMq;

                cfg.Host(rabbitMqOptions.Host, rabbitMqOptions.Port, rabbitMqOptions.VirtualHost, h =>
                {
                    h.Username(rabbitMqOptions.Username);
                    h.Password(rabbitMqOptions.Password);

                    if (rabbitMqOptions.UseSsl)
                    {
                        h.UseSsl(s =>
                        {
                            if (!string.IsNullOrWhiteSpace(rabbitMqOptions.SslServerName))
                                s.ServerName = rabbitMqOptions.SslServerName;

                            if (!string.IsNullOrWhiteSpace(rabbitMqOptions.SslCertificatePath))
                                s.CertificatePath = rabbitMqOptions.SslCertificatePath;
                        });
                    }
                });

                // Retry policy
                var retryOptions = options.Options.Retry;
                cfg.UseMessageRetry(retryConfig =>
                {
                    if (retryOptions.RetryIntervalsInSeconds.Length > 0)
                    {
                        retryConfig.Intervals(
                            retryOptions.RetryIntervalsInSeconds
                                .Select(s => TimeSpan.FromSeconds(s))
                                .ToArray());
                    }
                    else
                    {
                        retryConfig.Interval(retryOptions.RetryCount, TimeSpan.FromSeconds(2));
                    }
                });

                // Delayed redelivery policy
                var redeliveryOptions = options.Options.Redelivery;
                cfg.UseDelayedRedelivery(redeliveryConfig =>
                {
                    if (redeliveryOptions.RedeliveryIntervalsInSeconds.Length > 0)
                    {
                        redeliveryConfig.Intervals(
                            redeliveryOptions.RedeliveryIntervalsInSeconds
                                .Select(s => TimeSpan.FromSeconds(s))
                                .ToArray());
                    }
                    else
                    {
                        redeliveryConfig.Interval(redeliveryOptions.RedeliveryCount, TimeSpan.FromSeconds(15));
                    }
                });

                // Publish filter
                cfg.UsePublishFilter(typeof(HeaderEnrichmentPublishFilter<>), context);

                // Consume filter
                cfg.UseConsumeFilter(typeof(MessageContextConsumeFilter<>), context);

                // Observers
                cfg.ConnectPublishObserver(new MassTransitPublishObserverAdapter(
                    context.GetService<MW.Messaging.MassTransit.IPublishObserver>()));
                cfg.ConnectConsumeObserver(new MassTransitConsumeObserverAdapter(
                    context.GetService<MW.Messaging.MassTransit.IConsumeObserver>()));
                cfg.ConnectSendObserver(new MassTransitSendObserverAdapter(
                    context.GetService<MW.Messaging.MassTransit.ISendObserver>()));
                cfg.ConnectBusObserver(new BusLifecycleObserver(
                    context.GetRequiredService<Microsoft.Extensions.Logging.ILogger<BusLifecycleObserver>>()));

                // Custom RabbitMQ bus configuration hook
                options.ConfigureRabbitMqBusAction?.Invoke(context, cfg);

                cfg.ConfigureEndpoints(context);
            });
        });

        // Health checks
        if (options.Options.EnableHealthChecks)
        {
            var rabbitMqOptions = options.Options.RabbitMq;
            var protocol = rabbitMqOptions.UseSsl ? "amqps" : "amqp";
            var uriBuilder = new UriBuilder(protocol, rabbitMqOptions.Host, rabbitMqOptions.Port, rabbitMqOptions.VirtualHost)
            {
                UserName = Uri.EscapeDataString(rabbitMqOptions.Username),
                Password = Uri.EscapeDataString(rabbitMqOptions.Password)
            };
            services.AddHealthChecks()
                .AddRabbitMQ(uriBuilder.Uri, name: "rabbitmq");
        }

        return services;
    }

    /// <summary>
    /// Adds transactional outbox support using Entity Framework Core.
    /// </summary>
    public static IBusRegistrationConfigurator AddEntityFrameworkOutbox<TDbContext>(
        this IBusRegistrationConfigurator configurator,
        Action<IEntityFrameworkOutboxConfigurator>? configureOutbox = null)
        where TDbContext : DbContext
    {
        configurator.AddEntityFrameworkOutbox<TDbContext>(o =>
        {
            o.UseBusOutbox();
            configureOutbox?.Invoke(o);
        });

        return configurator;
    }
}

/// <summary>
/// Options for configuring MassTransit messaging infrastructure.
/// </summary>
public class MassTransitMessagingOptions
{
    public MassTransitOptions Options { get; set; } = new();
    public List<Assembly> ConsumerAssemblies { get; } = new();
    public Action<IBusRegistrationConfigurator>? ConfigureConsumersAction { get; set; }
    public Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>? ConfigureRabbitMqBusAction { get; set; }
    public Action<IBusRegistrationConfigurator>? OutboxConfigurator { get; set; }

    public MassTransitMessagingOptions AddConsumersFromAssembly(Assembly assembly)
    {
        ConsumerAssemblies.Add(assembly);
        return this;
    }

    public MassTransitMessagingOptions ConfigureConsumers(Action<IBusRegistrationConfigurator> configure)
    {
        ConfigureConsumersAction = configure;
        return this;
    }

    public MassTransitMessagingOptions ConfigureRabbitMqBus(
        Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator> configure)
    {
        ConfigureRabbitMqBusAction = configure;
        return this;
    }

    public MassTransitMessagingOptions UseEntityFrameworkOutbox<TDbContext>(
        Action<IEntityFrameworkOutboxConfigurator>? configureOutbox = null)
        where TDbContext : DbContext
    {
        OutboxConfigurator = cfg => cfg.AddEntityFrameworkOutbox<TDbContext>(o =>
        {
            o.UseBusOutbox();
            configureOutbox?.Invoke(o);
        });
        return this;
    }
}
