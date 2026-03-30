using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using MW.Messaging.MassTransit.Extensions;
using MW.OrderRegistration.ApiDemo.TestInfrastructure;
using MW.OrderRegistration.ConsoleDemo.Configuration;
using MW.OrderRegistration.ConsoleDemo.Consumers;
using MW.OrderRegistration.ConsoleDemo.Infrastructure.Persistence;
using MW.OrderRegistration.ConsoleDemo.Saga;
using MW.OrderRegistration.ConsoleDemo.Services;
using MW.Persistence.DependencyInjection.Extensions;
using MW.Persistence.DependencyInjection.Options;
using MW.Saga.MassTransit.Extensions;
using Serilog;

// ── Bootstrap Serilog ───────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // ── Resolve scenario ────────────────────────────────────────────────────
    var demoSettings = new DemoSettings();
    builder.Configuration.GetSection(DemoSettings.SectionName).Bind(demoSettings);
    // For API host, scenario defaults from config; per-request override is done in the controller.
    // Note: Scenario override mutates singleton state — safe for sequential demo usage only.
    // For concurrent requests with different scenarios, scenario context should be propagated
    // through message headers instead of shared state.
    demoSettings.ResolvedScenario = ScenarioResolver.Resolve(Array.Empty<string>(), demoSettings);
    builder.Services.AddSingleton(demoSettings);

    Log.Information("══════════════════════════════════════════════════════════");
    Log.Information("  MW.OrderRegistration.ApiDemo");
    Log.Information("  Default scenario: {Scenario}", demoSettings.ResolvedScenario);
    Log.Information("══════════════════════════════════════════════════════════");

    // ── ASP.NET Core services ───────────────────────────────────────────────
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "MW.OrderRegistration.ApiDemo",
            Version = "v1",
            Description = "API demo host for testing order registration flow through persistence, messaging, and saga infrastructure."
        });

        // Group debug/test endpoints separately from business APIs in Swagger
        options.TagActionsBy(api =>
        {
            if (api.GroupName != null)
                return new[] { api.GroupName };

            var controllerName = api.ActionDescriptor.RouteValues["controller"] ?? "Default";

            // Map test controllers to debug-prefixed tags for clear separation
            return controllerName switch
            {
                "TestRepository" => new[] { "Debug: Repository" },
                "TestMessaging" => new[] { "Debug: Messaging" },
                "TestSaga" => new[] { "Debug: Saga" },
                "TestPersistence" => new[] { "Debug: Persistence" },
                "TestIntegration" => new[] { "Debug: Integration" },
                "TestSummary" => new[] { "Debug: Summary" },
                "TestHealth" => new[] { "Debug: Health" },
                _ => new[] { controllerName }
            };
        });

        options.DocInclusionPredicate((_, _) => true);
    });

    // ── Debug/test infrastructure (development-only) ────────────────────────
    // Environment guard: test services are registered only in Development
    // or when explicitly enabled via configuration.
    var enableTestEndpoints = builder.Environment.IsDevelopment()
        || builder.Configuration.GetValue<bool>("TestEndpoints:Enabled");

    if (enableTestEndpoints)
    {
        builder.Services.AddSingleton<TestConsumedEventStore>();
        Log.Information("Debug/test endpoints enabled (environment: {Environment})", builder.Environment.EnvironmentName);
    }

    // ── Persistence (shared infrastructure) ─────────────────────────────────
    var connectionString = builder.Configuration.GetConnectionString("DemoDb")!;
    builder.Services.AddEfCorePersistence<DemoDbContext>(options =>
    {
        options.ConnectionString = connectionString;
        options.Provider = DatabaseProvider.PostgreSql;
        options.MigrationAssembly = typeof(DemoDbContext).Assembly.FullName;
        options.EnableSensitiveDataLogging = true;
        options.EnableDetailedErrors = true;
        options.EnableHealthCheck = false;
    });

    // ── Application services (reused from ConsoleDemo) ──────────────────────
    builder.Services.AddScoped<OrderCreationService>();
    builder.Services.AddScoped<InventoryService>();
    builder.Services.AddScoped<PaymentService>();
    builder.Services.AddScoped<OrderFinalizationService>();
    builder.Services.AddScoped<OrderSummaryService>();

    // ── Saga infrastructure (shared) ────────────────────────────────────────
    builder.Services.AddSagaMassTransitInfrastructure(options =>
    {
        options.BindOptions(builder.Configuration);
    });

    // ── Messaging infrastructure (shared) ───────────────────────────────────
    builder.Services.AddMassTransitMessaging(options =>
    {
        options.BindOptions(builder.Configuration);

        // Register consumers (reused from ConsoleDemo)
        options.ConfigureConsumers(cfg =>
        {
            cfg.AddConsumer<InventoryReservationRequestedConsumer>();
            cfg.AddConsumer<PaymentRequestedConsumer>();
            cfg.AddConsumer<OrderRegistrationCompletedConsumer>();
            cfg.AddConsumer<OrderRegistrationFailedConsumer>();
            cfg.AddConsumer<OrderRegistrationTimedOutConsumer>();

            // Register test event consumer for debug/test messaging endpoints
            if (enableTestEndpoints)
            {
                cfg.AddConsumer<TestEventConsumer>();
            }

            // Register saga state machine with EF Core persistence
            cfg.AddSagaStateMachine<OrderRegistrationStateMachine, OrderRegistrationSagaState>()
                .EntityFrameworkRepository(r =>
                {
                    r.ExistingDbContext<DemoDbContext>();
                });

            // Register in-memory scheduler for saga timeout support
            cfg.AddDelayedMessageScheduler();
        });

        // Outbox for transactional messaging
        options.UseEntityFrameworkOutbox<DemoDbContext>();

        // Configure scheduler for timeout support on the bus
        options.ConfigureRabbitMqBus((context, cfg) =>
        {
            cfg.UseDelayedMessageScheduler();
        });
    });

    var app = builder.Build();

    // ── Database initialization ─────────────────────────────────────────────
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
        Log.Information("Ensuring database is created...");
        await db.Database.EnsureCreatedAsync();
        Log.Information("Database ready.");
    }

    // ── Middleware pipeline ──────────────────────────────────────────────────
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MW.OrderRegistration.ApiDemo v1");
        options.RoutePrefix = string.Empty; // Swagger UI at root
    });

    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        };
    });

    // ── Environment guard for debug/test endpoints ─────────────────────────
    if (!enableTestEndpoints)
    {
        app.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/api/test"))
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new
                {
                    Message = "Debug/test endpoints are disabled in this environment. " +
                              "Set ASPNETCORE_ENVIRONMENT=Development or TestEndpoints:Enabled=true to enable."
                });
                return;
            }

            await next();
        });
    }

    app.MapControllers();

    if (!enableTestEndpoints)
    {
        Log.Information("Debug/test endpoints are DISABLED (environment: {Environment}). " +
                        "Set ASPNETCORE_ENVIRONMENT=Development or TestEndpoints:Enabled=true to enable.",
            app.Environment.EnvironmentName);
    }

    Log.Information("API demo host started. Swagger UI available at the root URL.");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "API demo host terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
