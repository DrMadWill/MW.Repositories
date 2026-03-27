# MW.Messaging.MassTransit — Integration Guide

## Quick Start

### 1. Install the Package

Reference `MW.Messaging.MassTransit` in your service project.

### 2. Register Messaging in DI

```csharp
builder.Services.AddMassTransitMessaging(options =>
{
    // Bind options from configuration (recommended)
    options.BindOptions(builder.Configuration);

    // Or configure manually:
    // options.Options.RabbitMq.Host = "localhost";
    // options.Options.RabbitMq.Port = 5672;
    // options.Options.RabbitMq.Username = "guest";
    // options.Options.RabbitMq.Password = "guest";

    // Service name for endpoint naming and identity
    options.Options.ServiceName = "order-service";

    // Register consumers from assembly
    options.AddConsumersFromAssembly(typeof(Program).Assembly);

    // Optional: Configure retry with exception filtering
    options.Options.Retry.RetryCount = 3;
    options.Options.Retry.RetryIntervalsInSeconds = [1, 2, 5];
    options.Options.Retry.ExceptionTypeFilters = ["System.TimeoutException"];

    // Optional: Configure redelivery
    options.Options.Redelivery.RedeliveryIntervalsInSeconds = [10, 30, 60];

    // Optional: Enable transactional outbox (includes inbox-state)
    options.UseEntityFrameworkOutbox<AppDbContext>();

    // Optional: Health checks (enabled by default)
    options.Options.EnableHealthChecks = true;
});
```

### 3. Configuration Binding (appsettings.json)

```json
{
  "Messaging": {
    "ServiceName": "order-service",
    "EnableHealthChecks": true,
    "RabbitMq": {
      "Host": "localhost",
      "Port": 5672,
      "Username": "guest",
      "Password": "guest"
    },
    "Retry": {
      "RetryCount": 3,
      "RetryIntervalsInSeconds": [1, 2, 5],
      "ExceptionTypeFilters": ["System.TimeoutException"]
    },
    "Redelivery": {
      "RedeliveryIntervalsInSeconds": [10, 30, 60]
    }
  }
}
```

### 4. Publish Integration Events

```csharp
public class OrderService
{
    private readonly IIntegrationEventPublisher _publisher;

    public OrderService(IIntegrationEventPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task PlaceOrderAsync(Order order, CancellationToken ct)
    {
        // ... business logic ...

        await _publisher.PublishAsync(new OrderPlacedEvent
        {
            OrderId = order.Id,
            CustomerId = order.CustomerId
        }, ct);
    }
}
```

### 5. Create a Consumer

```csharp
public class OrderPlacedConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly IMessageExecutionContext _executionContext;

    public OrderPlacedConsumer(IMessageExecutionContext executionContext)
    {
        _executionContext = executionContext;
    }

    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        // Access propagated context (correlation, tenant, user, etc.)
        var correlationId = _executionContext.CorrelationId;
        var tenantId = _executionContext.TenantId;

        // ... handle the event ...
    }
}
```

### 6. Header Propagation

Headers are automatically propagated through:
- **Publish filter**: Enriches outgoing messages with correlation, causation, trace, tenant, user, and service metadata
- **Consume filter**: Reads incoming headers, populates `IMessageContextAccessor`, and clears after execution

Standard headers:
- `x-correlation-id`
- `x-causation-id`
- `x-trace-id`
- `x-tenant-id`
- `x-user-id`
- `x-source-service`
- `x-event-name`
- `x-event-version`

### 7. Outbox Configuration

For transactional outbox support with EF Core:

```csharp
options.UseEntityFrameworkOutbox<AppDbContext>(outbox =>
{
    // Customize outbox behavior if needed
});
```

This is the single recommended entry point for outbox configuration.
It enables both publisher-side outbox (UseBusOutbox) by default.

### 8. Custom Observer Registration

Register custom observers for structured logging:

```csharp
services.AddSingleton<MW.Messaging.MassTransit.IPublishObserver, MyCustomPublishObserver>();
services.AddSingleton<MW.Messaging.MassTransit.IConsumeObserver, MyCustomConsumeObserver>();
services.AddSingleton<MW.Messaging.MassTransit.ISendObserver, MyCustomSendObserver>();
```

### 9. Endpoint Naming

Endpoints follow kebab-case with optional service prefix:
- Service: `order-service`
- Consumer: `OrderPlacedConsumer`
- Queue: `order-service-order-placed`

---

## Service Registration Reference

### Required Registrations

| Registration | Purpose | Default |
|---|---|---|
| `AddMassTransitMessaging()` | Core messaging setup | **Must call** |
| `options.Options.ServiceName` | Service identity & endpoint naming | Empty string |
| `options.Options.RabbitMq.*` | RabbitMQ connection settings | localhost/guest |

### Automatically Registered Services

| Service | Lifetime | Description |
|---|---|---|
| `IMessageContextAccessor` | Scoped | Read-only access to current consumer context |
| `IMessageExecutionContext` | Scoped | Transport-agnostic execution metadata |
| `IPublishContextProvider` | Scoped | Creates publish context from execution flow |
| `IIntegrationEventPublisher` | Scoped | Publishes integration events via MassTransit |
| `IMessageHeaderMapper` | Singleton | Maps context models to/from message headers |
| `IServiceIdentityProvider` | Singleton | Provides service identity (when `ServiceName` is set) |
| `ScopedMessageContextAccessor` | Scoped | Internal accessor for consume pipeline |

### Optional Registrations (User-Provided)

| Service | Purpose | Notes |
|---|---|---|
| `IIntegrationEventValidator` | Validates events before publish | Optional; publisher skips validation if not registered |
| `ICorrelationContext` | Provides ambient correlation/trace IDs | Optional; auto-generates correlation ID if missing |
| `IPublishObserver` | Custom publish observability | Optional; no-op if not registered |
| `IConsumeObserver` | Custom consume observability | Optional; no-op if not registered |
| `ISendObserver` | Custom send observability | Optional; kept for completeness, may be deferred |

### Optional Configuration

| Feature | How to Enable | Default |
|---|---|---|
| Health checks | `options.Options.EnableHealthChecks = true` | Enabled |
| Retry policy | `options.Options.Retry.*` | 3 retries at [1, 2, 4]s |
| Exception filtering | `options.Options.Retry.ExceptionTypeFilters` | Not set (all exceptions retried) |
| Redelivery | `options.Options.Redelivery.*` | 3 redeliveries at [5, 15, 30]s |
| Transactional outbox | `options.UseEntityFrameworkOutbox<TDbContext>()` | Not enabled |
| Configuration binding | `options.BindOptions(configuration)` | Manual setup |

### ISendObserver Scope Decision

`ISendObserver` is kept in the current implementation for completeness. It follows the same adapter pattern as publish and consume observers. Service teams may register it if send-flow observability is needed. It can be deferred to a later phase if not required for current use cases.
