# MW.Messaging.MassTransit

**MW.Messaging.MassTransit** — `MW.Messaging.Abstractions`-da təyin edilmiş mesajlaşma abstraksiyalarının MassTransit və RabbitMQ transport layeri üzərində konkret infrastruktur implementasiyasıdır.

> **Vacib:** Bu layihə **MassTransit Transactional Outbox** vasitəsilə etibarlı mesaj çatdırılmasını təmin edir. Manual outbox/inbox implementasiyası tələb olunmur. Outbox/inbox EF Core inteqrasiyası opsional olaraq bir sətirdə aktivləşdirilir.

## 🎯 Məqsəd

Bu paket mikroservislər üçün MassTransit-əsaslı mesajlaşma infrastrukturunu bir `AddMassTransitMessaging()` çağırışı ilə qeydiyyatdan keçirir. RabbitMQ bağlantısı, consumer qeydiyyatı, retry/redelivery siyasətləri, header propagasiyası, distributed tracing, health check-lər və opsional transactional outbox dəstəyi təmin edir.

## 📦 Quraşdırma

### NuGet Package Manager

```bash
Install-Package MW.Messaging.MassTransit
```

### .NET CLI

```bash
dotnet add package MW.Messaging.MassTransit
```

### PackageReference

```xml
<PackageReference Include="MW.Messaging.MassTransit" Version="1.0.0" />
```

## ✅ Scope (Bu paketin məsuliyyəti)

- **MassTransit DI qeydiyyatı** — `AddMassTransitMessaging()` ilə bütün messaging infrastrukturunun bir nöqtədən konfiqurasiyası
- **RabbitMQ transport konfiqurasiyası** — Host, port, virtual host, username, password
- **Consumer qeydiyyatı** — Assembly-dən avtomatik consumer kəşfi
- **Retry / Redelivery siyasətləri** — Konfiqurasiya ilə idarə olunan retry və redelivery davranışı
- **Header enrichment (publish)** — `HeaderEnrichmentPublishFilter` ilə çıxış mesajlarına avtomatik header əlavə edilməsi
- **Header extraction (consume)** — `MessageContextConsumeFilter` ilə gələn mesajlardan kontekst çıxarılması
- **Publish/Consume/Send observer adapterləri** — Strukturlaşdırılmış loglama üçün MassTransit observer bridging
- **Bus lifecycle observer** — MassTransit bus yaradılma, start, stop hadisələrinin loglanması
- **Endpoint naming** — `ServiceEndpointNameFormatter` ilə kebab-case servis prefiksli queue adları
- **Health check-lər** — RabbitMQ və MassTransit bus sağlamlıq yoxlamaları
- **Distributed tracing** — `System.Diagnostics.Activity` əsaslı tracing (`MW.Messaging` ActivitySource)
- **Transactional outbox** — EF Core ilə opsional outbox/inbox dəstəyi
- **Servis kimliyi** — `ConfigurationServiceIdentityProvider` ilə konfiqurasiya-əsaslı servis identifikasiyası

## ❌ Non-Scope (Bu paketin məsuliyyətinə daxil deyil)

- **Event kontraktları** — `MW.Messaging.Abstractions`-da yerləşir
- **Mesaj kontekst abstraksiyaları** — `MW.Messaging.Abstractions`-da yerləşir
- **Repository abstraksiyaları** — `MW.Persistence.Abstractions`-da yerləşir
- **Identity implementasiyası** — HTTP/user token çıxarma `MW.Identity.Token`-da qalır
- **Biznes-spesifik consumer məntiqi** — Servis layihələrində olmalıdır
- **Domain event handler-ləri** — Application layerində qalmalıdır
- **Servis-spesifik sinxronizasiya/repair məntiqi** — Hər servisin öz daxili işidir

## 🏗️ Paket Strukturu

```
MW.Messaging.MassTransit/
├── Context/           # Mesaj kontekst implementasiyaları (accessor, execution, publish provider, header mapper)
├── Extensions/        # DI qeydiyyat extension metodları (AddMassTransitMessaging)
├── Filters/           # MassTransit publish/consume filter-ləri (header enrichment, context extraction)
├── Health/            # MassTransit bus health check implementasiyası
├── Identity/          # Konfiqurasiya-əsaslı servis kimliyi provider
├── Naming/            # Kebab-case endpoint naming formatter
├── Observers/         # MassTransit observer adapter-ləri (publish, consume, send, bus lifecycle)
├── Options/           # Konfiqurasiya modelləri (MassTransitOptions, RabbitMqOptions, RetryOptions, RedeliveryOptions)
├── Publishing/        # MassTransit-əsaslı inteqrasiya event publisher
├── Tracing/           # Distributed tracing ActivitySource
└── Docs/              # Arxitektura və inteqrasiya sənədləri
```

## 🚀 Quick Start

### 1. DI Qeydiyyatı

```csharp
using MW.Messaging.MassTransit.Extensions;

builder.Services.AddMassTransitMessaging(options =>
{
    // appsettings.json-dan konfiqurasiyanı bind et (tövsiyə olunan)
    options.BindOptions(builder.Configuration);

    // Consumer-ləri assembly-dən qeydiyyatdan keçir
    options.AddConsumersFromAssembly(typeof(Program).Assembly);

    // Opsional: Transactional outbox aktivləşdir
    options.UseEntityFrameworkOutbox<AppDbContext>();
});
```

### 2. appsettings.json Konfiqurasiyası

```json
{
  "Messaging": {
    "ServiceName": "order-service",
    "EnableHealthChecks": true,
    "RabbitMq": {
      "Host": "localhost",
      "Port": 5672,
      "VirtualHost": "/",
      "Username": "guest",
      "Password": "guest"
    },
    "Retry": {
      "RetryCount": 3,
      "RetryIntervalsInSeconds": [1, 2, 5],
      "ExceptionTypeFilters": ["System.TimeoutException"]
    },
    "Redelivery": {
      "RedeliveryCount": 3,
      "RedeliveryIntervalsInSeconds": [10, 30, 60]
    }
  }
}
```

### 3. Event Publish Etmə

```csharp
using MW.Messaging.Publishing;

public class OrderService
{
    private readonly IIntegrationEventPublisher _publisher;

    public OrderService(IIntegrationEventPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task PlaceOrderAsync(Order order, CancellationToken ct)
    {
        // ... biznes məntiqi ...

        await _publisher.PublishAsync(new OrderPlacedEvent
        {
            OrderId = order.Id,
            CustomerId = order.CustomerId
        }, ct);
    }
}
```

### 4. Consumer Yaratma

```csharp
using MassTransit;
using MW.Messaging.Context;

public class OrderPlacedConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly IMessageExecutionContext _executionContext;

    public OrderPlacedConsumer(IMessageExecutionContext executionContext)
    {
        _executionContext = executionContext;
    }

    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        // Propagasiya olunmuş kontekstə çıxış (correlation, tenant, user və s.)
        var correlationId = _executionContext.CorrelationId;
        var tenantId = _executionContext.TenantId;
        var sourceService = _executionContext.SourceService;

        // ... event-i emal et ...
    }
}
```

## 🔑 Əsas Konseptlər

| Konsept | Təsvir |
|---------|--------|
| **AddMassTransitMessaging** | Bütün messaging infrastrukturunu bir çağırışda qeydiyyat edir |
| **Header Enrichment** | `HeaderEnrichmentPublishFilter` çıxış mesajlarına avtomatik correlation, trace, tenant, user header-ləri əlavə edir |
| **Context Extraction** | `MessageContextConsumeFilter` gələn mesajlardan header-ləri çıxarıb `ScopedMessageContextAccessor`-a yazır |
| **Execution Context** | `MassTransitMessageExecutionContext` transport-agnostik metadata çıxışı təmin edir |
| **Publish Context** | `DefaultPublishContextProvider` cari execution flow-dan publish metadatası yaradır |
| **Header Mapping** | `DefaultMessageHeaderMapper` kontekst modelləri ilə transport header-ləri arasında iki istiqamətli çevirmə |
| **Service Identity** | `ConfigurationServiceIdentityProvider` konfiqurasiyadan servis adını oxuyur |
| **Endpoint Naming** | `ServiceEndpointNameFormatter` servis prefiksi ilə kebab-case queue adları yaradır |
| **Health Checks** | `MassTransitBusHealthCheck` bus vəziyyətini monitorinq edir |
| **Distributed Tracing** | `MessagingActivitySource` ("MW.Messaging") publish/consume əməliyyatları üçün Activity span-ları yaradır |
| **Observer Adapters** | MassTransit observer-lərini custom `IPublishObserver`/`IConsumeObserver`/`ISendObserver`-ə bridge edir |
| **Transactional Outbox** | EF Core ilə MassTransit transactional outbox/inbox inteqrasiyası |

## 📊 Header Propagasiya Axını

### Publish Axını

```
IIntegrationEventPublisher.PublishAsync(event)
    │
    ▼
Opsional validasiya (IIntegrationEventValidator)
    │
    ▼
HeaderEnrichmentPublishFilter
    ├── IPublishContextProvider-dən kontekst alır
    ├── DefaultMessageHeaderMapper ilə header-lərə çevirir
    ├── Activity span yaradır (messaging.publish, ActivityKind.Producer)
    └── Header-ləri MassTransit mesajına əlavə edir
    │
    ▼
MassTransit → RabbitMQ
    │
    ▼
MassTransitPublishObserverAdapter (opsional loglama)
```

### Consume Axını

```
RabbitMQ → MassTransit
    │
    ▼
MessageContextConsumeFilter
    ├── Mesaj header-lərini toplayır
    ├── DefaultMessageHeaderMapper ilə ConsumerContextModel-ə çevirir
    ├── ScopedMessageContextAccessor-a yazır
    ├── Activity span yaradır (messaging.consume, ActivityKind.Consumer)
    └── Consumer-i icra edir
    │
    ▼
Consumer (IMessageExecutionContext vasitəsilə kontekstə çıxış)
    │
    ▼
Filter finally blokunda konteksti təmizləyir
    │
    ▼
MassTransitConsumeObserverAdapter (opsional loglama)
```

### Standart Header-lər

| Header | Sabit | Təsvir |
|--------|-------|--------|
| `x-correlation-id` | `MessageHeaders.CorrelationId` | Distributed tracing korrelyasiya ID-si |
| `x-causation-id` | `MessageHeaders.CausationId` | Səbəb mesajının ID-si |
| `x-trace-id` | `MessageHeaders.TraceId` | Trace ID-si |
| `x-tenant-id` | `MessageHeaders.TenantId` | Multi-tenant identifikatoru |
| `x-user-id` | `MessageHeaders.UserId` | İstifadəçi identifikatoru |
| `x-source-service` | `MessageHeaders.SourceService` | Mənbə servisin adı |
| `x-event-name` | `MessageHeaders.EventName` | Event adı |
| `x-event-version` | `MessageHeaders.EventVersion` | Event versiyası |

## ⚙️ Konfiqurasiya Modelləri

### MassTransitOptions

```csharp
public class MassTransitOptions
{
    public RabbitMqOptions RabbitMq { get; set; }
    public RetryOptions Retry { get; set; }
    public RedeliveryOptions Redelivery { get; set; }
    public string ServiceName { get; set; }
    public bool EnableHealthChecks { get; set; } = true;
}
```

### RabbitMqOptions

```csharp
public class RabbitMqOptions
{
    public string Host { get; set; } = "localhost";
    public ushort Port { get; set; } = 5672;
    public string VirtualHost { get; set; } = "/";
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
}
```

### RetryOptions

```csharp
public class RetryOptions
{
    public int RetryCount { get; set; } = 3;
    public int[] RetryIntervalsInSeconds { get; set; } = [1, 2, 4];
    public string[]? ExceptionTypeFilters { get; set; }
}
```

### RedeliveryOptions

```csharp
public class RedeliveryOptions
{
    public int RedeliveryCount { get; set; } = 3;
    public int[] RedeliveryIntervalsInSeconds { get; set; } = [5, 15, 30];
}
```

## 📋 Qeydiyyat Olunan Servislər

### Avtomatik Qeydiyyat (AddMassTransitMessaging ilə)

| Abstraksiya | İmplementasiya | Lifetime | Təsvir |
|-------------|----------------|----------|--------|
| `IMessageContextAccessor` | `ScopedMessageContextAccessor` | Scoped | Consumer daxilində mesaj kontekstinə read-only çıxış |
| `IMessageExecutionContext` | `MassTransitMessageExecutionContext` | Scoped | Transport-agnostik execution metadata |
| `IMessageHeaderMapper` | `DefaultMessageHeaderMapper` | Singleton | Header ↔ model mapping |
| `IPublishContextProvider` | `DefaultPublishContextProvider` | Scoped | Execution flow-dan publish kontekst yaratma |
| `IIntegrationEventPublisher` | `MassTransitIntegrationEventPublisher` | Scoped | MassTransit vasitəsilə event publish etmə |
| `IServiceIdentityProvider` | `ConfigurationServiceIdentityProvider` | Singleton | Konfiqurasiya-əsaslı servis kimliyi |
| `IBusControl` | (MassTransit) | Singleton | MassTransit bus |
| Health Checks | `MassTransitBusHealthCheck` | — | RabbitMQ + bus sağlamlıq yoxlaması |

### Opsional Qeydiyyat (İstifadəçi tərəfindən)

| Abstraksiya | Məqsəd | Qeyd |
|-------------|--------|------|
| `IIntegrationEventValidator` | Event-ləri publish etmədən əvvəl validasiya | Opsional; qeydiyyat yoxdursa publisher validasiyanı atlayır |
| `ICorrelationContext` | Ambient korrelyasiya/trace ID-ləri | Opsional; yoxdursa CorrelationId avtomatik yaradılır |
| `IPublishObserver` | Custom publish müşahidəçiliyi | Opsional; qeydiyyat yoxdursa no-op |
| `IConsumeObserver` | Custom consume müşahidəçiliyi | Opsional; qeydiyyat yoxdursa no-op |
| `ISendObserver` | Custom send müşahidəçiliyi | Opsional; qeydiyyat yoxdursa no-op |

## 🔧 Əlavə Konfiqurasiya

### Transactional Outbox (Yalnız Publisher)

```csharp
options.UseEntityFrameworkOutbox<AppDbContext>();
```

### Transactional Outbox + Inbox (Publisher + Consumer)

```csharp
options.UseEntityFrameworkInboxOutbox<AppDbContext>();
```

### Custom Observer Qeydiyyatı

```csharp
services.AddSingleton<MW.Messaging.MassTransit.IPublishObserver, MyPublishObserver>();
services.AddSingleton<MW.Messaging.MassTransit.IConsumeObserver, MyConsumeObserver>();
services.AddSingleton<MW.Messaging.MassTransit.ISendObserver, MySendObserver>();
```

### Endpoint Naming Nümunəsi

| ServiceName | Consumer | Queue Adı |
|-------------|----------|-----------|
| `order-service` | `OrderPlacedConsumer` | `order-service-order-placed` |
| `payment-service` | `PaymentCompletedConsumer` | `payment-service-payment-completed` |

## 🏛️ Dependency Direction (Asılılıq İstiqaməti)

```
┌──────────────────────────────────────────┐
│         Application / Services           │
│    (servisləriniz bu paketdən asılıdır)  │
└───────────────┬──────────────────────────┘
                │ depends on
                ▼
┌──────────────────────────────────────────┐
│      MW.Messaging.Abstractions           │
│  (kontraktlar, interfeyslər, modellər)   │
└───────────────┬──────────────────────────┘
                │ implemented by
                ▼
┌──────────────────────────────────────────┐
│      MW.Messaging.MassTransit            │  ◄── Bu paket
│  (konkret MassTransit implementasiyası)  │
└──────────────────────────────────────────┘
```

## 📚 Sənədlər

| Sənəd | Təsvir |
|-------|--------|
| [İnteqrasiya Təlimatı](Docs/INTEGRATION-GUIDE.md) | Addım-addım quraşdırma və istifadə təlimatları |
| [Arxitektura Sərhədi](Docs/ARCHITECTURE-BOUNDARY.md) | Layihənin scope və arxitektura qaydaları |

## 📋 Asılılıqlar

| Paket | Versiya | Təsvir |
|-------|---------|--------|
| MassTransit.RabbitMQ | 8.2.5 | MassTransit RabbitMQ transport |
| MassTransit.EntityFrameworkCore | 8.2.5 | Transactional outbox dəstəyi |
| AspNetCore.HealthChecks.Rabbitmq | 8.0.0 | RabbitMQ health check |
| Microsoft.Extensions.Configuration.Binder | 8.0.1 | Konfiqurasiya binding |
| Microsoft.Extensions.DependencyInjection.Abstractions | 8.0.1 | DI abstraksiyaları |
| Microsoft.Extensions.Diagnostics.HealthChecks | 8.0.4 | Health check infrastrukturu |
| Microsoft.Extensions.Logging.Abstractions | 8.0.1 | Loglama abstraksiyaları |
| **MW.Messaging.Abstractions** | (project ref) | Mesajlaşma kontraktları və abstraksiyaları |

## 🔧 Tələblər

- .NET 8.0+
- C# 12+
- RabbitMQ (default: localhost:5672)

## 📄 Lisenziya

Bu layihə MIT lisenziyası altında paylanır.
