using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MW.Messaging.MassTransit.Extensions;
using MW.OrderRegistration.ConsoleDemo.Configuration;
using MW.OrderRegistration.ConsoleDemo.Consumers;
using MW.OrderRegistration.ConsoleDemo.Domain.Enums;
using MW.OrderRegistration.ConsoleDemo.Events;
using MW.OrderRegistration.ConsoleDemo.Infrastructure.Persistence;
using MW.OrderRegistration.ConsoleDemo.Saga;
using MW.OrderRegistration.ConsoleDemo.Services;
using MW.Persistence.DependencyInjection.Extensions;
using MW.Persistence.DependencyInjection.Options;
using MW.Saga.MassTransit.Extensions;
using Serilog;

// ── Bootstrap ───────────────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.Configuration
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false)
        .AddEnvironmentVariables();

    builder.Services.AddSerilog();

    // ── Resolve scenario ────────────────────────────────────────────────────
    var demoSettings = new DemoSettings();
    builder.Configuration.GetSection(DemoSettings.SectionName).Bind(demoSettings);
    var scenario = ScenarioResolver.Resolve(args, demoSettings);
    builder.Services.AddSingleton(demoSettings);

    Log.Information("══════════════════════════════════════════════════════════");
    Log.Information("  MW.OrderRegistration.ConsoleDemo");
    Log.Information("  Active scenario: {Scenario}", scenario);
    Log.Information("══════════════════════════════════════════════════════════");

    // ── Persistence (Issue 5-6) ─────────────────────────────────────────────
    var connectionString = builder.Configuration.GetConnectionString("DemoDb")!;
    builder.Services.AddEfCorePersistence<DemoDbContext>(options =>
    {
        options.ConnectionString = connectionString;
        options.Provider = DatabaseProvider.SqlServer;
        options.MigrationAssembly = typeof(DemoDbContext).Assembly.FullName;
        options.EnableSensitiveDataLogging = true;
        options.EnableDetailedErrors = true;
        options.EnableHealthCheck = false;
    });

    // ── Application services (Issue 18) ─────────────────────────────────────
    builder.Services.AddScoped<OrderCreationService>();
    builder.Services.AddScoped<InventoryService>();
    builder.Services.AddScoped<PaymentService>();
    builder.Services.AddScoped<OrderFinalizationService>();
    builder.Services.AddScoped<OrderSummaryService>();

    // ── Saga infrastructure (Issue 11) ──────────────────────────────────────
    builder.Services.AddSagaMassTransitInfrastructure(options =>
    {
        options.BindOptions(builder.Configuration);
    });

    // ── Messaging infrastructure (Issue 12) ─────────────────────────────────
    builder.Services.AddMassTransitMessaging(options =>
    {
        options.BindOptions(builder.Configuration);

        // Register consumers
        options.ConfigureConsumers(cfg =>
        {
            cfg.AddConsumer<InventoryReservationRequestedConsumer>();
            cfg.AddConsumer<PaymentRequestedConsumer>();
            cfg.AddConsumer<OrderRegistrationCompletedConsumer>();
            cfg.AddConsumer<OrderRegistrationFailedConsumer>();
            cfg.AddConsumer<OrderRegistrationTimedOutConsumer>();

            // Register saga state machine with EF Core persistence
            cfg.AddSagaStateMachine<OrderRegistrationStateMachine, OrderRegistrationSagaState>()
                .EntityFrameworkRepository(r =>
                {
                    r.ExistingDbContext<DemoDbContext>();
                });

            // Register in-memory scheduler for saga timeout support
            cfg.AddDelayedMessageScheduler();
        });

        // Outbox for transactional messaging (Issue 19)
        options.UseEntityFrameworkOutbox<DemoDbContext>();

        // Configure scheduler for timeout support on the bus
        options.ConfigureRabbitMqBus((context, cfg) =>
        {
            cfg.UseDelayedMessageScheduler();
        });
    });

    var host = builder.Build();

    // ── Database initialization (Issue 25) ──────────────────────────────────
    using (var scope = host.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
        Log.Information("Ensuring database is created...");
        await db.Database.EnsureCreatedAsync();
        Log.Information("Database ready.");
    }

    // ── Start host ──────────────────────────────────────────────────────────
    await host.StartAsync();

    Log.Information("Host started. Running demo scenario: {Scenario}", scenario);

    // ── Execute demo flow ───────────────────────────────────────────────────
    using (var scope = host.Services.CreateScope())
    {
        var orderService = scope.ServiceProvider.GetRequiredService<OrderCreationService>();
        var publisher = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        // Create order through repository abstraction
        var order = await orderService.CreateOrderAsync(
            buyerId: "demo-buyer-001",
            items: new List<(string, int, decimal)>
            {
                ("Widget A", 2, 29.99m),
                ("Widget B", 1, 49.99m)
            });

        logger.LogInformation(
            "[Demo] Order created — OrderId={OrderId}. Publishing OrderRegistrationStarted...",
            order.Id);

        // Publish saga-starting event
        await publisher.Publish(new OrderRegistrationStarted
        {
            CorrelationId = order.Id.ToString(),
            OrderId = order.Id,
            BuyerId = order.BuyerId,
            TotalAmount = order.TotalAmount
        });

        logger.LogInformation("[Demo] OrderRegistrationStarted published. Waiting for saga to complete...");

        // Wait for the saga to process
        var delaySeconds = scenario == DemoScenario.Timeout
            ? demoSettings.PaymentTimeoutSeconds + 15
            : demoSettings.ResultQueryDelaySeconds;
        await Task.Delay(TimeSpan.FromSeconds(delaySeconds));

        // Query and display final summary
        var summaryService = scope.ServiceProvider.GetRequiredService<OrderSummaryService>();
        var db = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
        var sagaState = await db.OrderRegistrationSagaStates
            .FirstOrDefaultAsync(s => s.OrderId == order.Id);

        await summaryService.PrintSummaryAsync(order.Id, sagaState);
    }

    Log.Information("Demo complete. Shutting down...");
    await host.StopAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Demo host terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
