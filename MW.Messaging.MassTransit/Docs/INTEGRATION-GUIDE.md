# MW.Messaging.MassTransit — Integration Guide

## Quick Start

### 1. Install the Package

Reference `MW.Messaging.MassTransit` in your service project.

### 2. Register Messaging in DI

```csharp
builder.Services.AddMassTransitMessaging(options =>
{
    // Configure RabbitMQ connection
    options.Options.RabbitMq.Host = "localhost";
    options.Options.RabbitMq.Port = 5672;
    options.Options.RabbitMq.Username = "guest";
    options.Options.RabbitMq.Password = "guest";

    // Service name for endpoint naming
    options.Options.ServiceName = "order-service";

    // Register consumers from assembly
    options.AddConsumersFromAssembly(typeof(Program).Assembly);

    // Optional: Configure retry
    options.Options.Retry.RetryCount = 3;
    options.Options.Retry.RetryIntervalsInSeconds = [1, 2, 5];

    // Optional: Configure redelivery
    options.Options.Redelivery.RedeliveryIntervalsInSeconds = [10, 30, 60];

    // Optional: Enable transactional outbox
    options.UseEntityFrameworkOutbox<AppDbContext>();

    // Optional: Health checks (enabled by default)
    options.Options.EnableHealthChecks = true;
});
```

### 3. Publish Integration Events

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

### 4. Create a Consumer

```csharp
public class OrderPlacedConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly IMessageContextAccessor _contextAccessor;

    public OrderPlacedConsumer(IMessageContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        // Access propagated context (correlation, tenant, user, etc.)
        var messageContext = _contextAccessor.Current;
        var correlationId = messageContext?.CorrelationId;

        // ... handle the event ...
    }
}
```

### 5. Header Propagation

Headers are automatically propagated through:
- **Publish filter**: Enriches outgoing messages with correlation, causation, trace, tenant, user, and service metadata
- **Consume filter**: Reads incoming headers and populates `IMessageContextAccessor`

Standard headers:
- `x-correlation-id`
- `x-causation-id`
- `x-trace-id`
- `x-tenant-id`
- `x-user-id`
- `x-source-service`
- `x-event-name`
- `x-event-version`

### 6. Outbox Configuration

For transactional outbox support with EF Core:

```csharp
options.UseEntityFrameworkOutbox<AppDbContext>(outbox =>
{
    // Customize outbox behavior if needed
});
```

### 7. Custom Observer Registration

Register custom observers for structured logging:

```csharp
services.AddSingleton<MW.Messaging.MassTransit.IPublishObserver, MyCustomPublishObserver>();
services.AddSingleton<MW.Messaging.MassTransit.IConsumeObserver, MyCustomConsumeObserver>();
services.AddSingleton<MW.Messaging.MassTransit.ISendObserver, MyCustomSendObserver>();
```

### 8. Endpoint Naming

Endpoints follow kebab-case with optional service prefix:
- Service: `order-service`
- Consumer: `OrderPlacedConsumer`
- Queue: `order-service-order-placed`
