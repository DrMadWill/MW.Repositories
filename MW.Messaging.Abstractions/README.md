# MW.BuildingBlocks

**MW.BuildingBlocks** — mikroservis ekosistemi üçün paylaşılan mesajlaşma kontraktları, korrelyasiya, müşahidəçilik (observability) və MassTransit-uyğun abstraksiyaları təmin edən .NET 8.0 kitabxanasıdır.

> **Vacib:** Bu layihə manual Outbox/Inbox implementasiyası ehtiva etmir. Etibarlılıq (reliability) üçün **MassTransit Transactional Outbox** istifadə olunur. BuildingBlocks yalnız event kontraktları, korrelyasiya, strukturlaşdırılmış loglama və müşahidəçilik üzərində fokuslanır.

## 📦 Quraşdırma

### NuGet Package Manager

```bash
Install-Package MW.BuildingBlocks
```

### .NET CLI

```bash
dotnet add package MW.BuildingBlocks
```

### PackageReference

```xml
<PackageReference Include="MW.BuildingBlocks" Version="1.0.0" />
```

## 🏗️ Struktur

```
MW.BuildingBlocks/
├── Audit/              # Opsional event audit jurnalı
├── Constants/          # Paylaşılan sabitlər (istiqamət, status)
├── Contracts/          # İnteqrasiya event kontraktları
├── Correlation/        # Distributed tracing korrelyasiya
├── Docs/               # Konvensiya və qaydalar sənədləri
├── Headers/            # Mesaj header sabitləri
├── MassTransit/        # MassTransit abstraksiyaları (observer, mapper)
├── Messaging/          # Mesajlaşma modelləri (metadata, kontekst)
└── Observability/      # Müşahidəçilik modelləri və sahə standartları
```

## 🚀 İstifadə

### İnteqrasiya Event Kontraktları (Contracts)

`IIntegrationEvent` interfeysi mikroservislər arasında dəyişdirilən bütün event-lər üçün standart kontraktdır. `IntegrationEvent` abstract sinfi isə əsas implementasiyanı təmin edir:

```csharp
using MW.BuildingBlocks.Contracts;

public class OrderCreatedEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    public decimal TotalAmount { get; init; }

    public override string EventName => "order.created.v1";
    public override string EventVersion => "v1";
    public override string SourceService => "OrderService";
}

// İstifadə
var @event = new OrderCreatedEvent
{
    OrderId = Guid.NewGuid(),
    TotalAmount = 150.00m,
    CorrelationId = "corr-123",
    CausationId = "cmd-456"
};
```

### Event Metadata (Messaging)

`EventMetadata` event payload-dan ayrı saxlanılan cross-cutting metadata modelidir. Loglama, tracing və diagnostika üçün istifadə olunur:

```csharp
using MW.BuildingBlocks.Messaging;

var metadata = new EventMetadata
{
    MessageId = Guid.NewGuid(),
    CorrelationId = "corr-123",
    CausationId = "cmd-456",
    TenantId = Guid.Parse("..."),
    UserId = Guid.Parse("..."),
    SourceService = "OrderService",
    TraceId = "trace-789"
};
```

### Servis Kimliyi (Messaging)

`ServiceIdentity` publish və ya consume edən servisin kimliyini təsvir edir:

```csharp
using MW.BuildingBlocks.Messaging;

var identity = new ServiceIdentity
{
    ServiceName = "OrderService",
    ServiceVersion = "1.2.0",
    Environment = "Production"
};
```

### Publish Kontekst Modeli (Messaging)

`PublishContextModel` event publish zamanı MassTransit header-lərinə map olunacaq metadata-nı standartlaşdırır:

```csharp
using MW.BuildingBlocks.Messaging;

var publishContext = new PublishContextModel
{
    CorrelationId = "corr-123",
    CausationId = "cmd-456",
    TenantId = Guid.Parse("..."),
    UserId = Guid.Parse("..."),
    SourceService = "OrderService",
    TraceId = "trace-789"
};
```

### Consumer Kontekst Modeli (Messaging)

`ConsumerContextModel` consume tərəfində mesaj metadata-sını biznes/application koduna ötürmək üçün istifadə olunur. Bu model MassTransit-in `ConsumeContext` tipindən asılılığı aradan qaldırır:

```csharp
using MW.BuildingBlocks.Messaging;

var consumerContext = new ConsumerContextModel
{
    MessageId = Guid.NewGuid(),
    CorrelationId = "corr-123",
    CausationId = "cmd-456",
    TenantId = Guid.Parse("..."),
    UserId = Guid.Parse("..."),
    SourceService = "OrderService",
    TraceId = "trace-789",
    EventName = "order.created.v1"
};
```

### Mesaj Header Sabitləri (Headers)

`MessageHeaders` MassTransit vasitəsilə mesaj publish/consume zamanı istifadə olunan standart header adlarını təmin edir:

```csharp
using MW.BuildingBlocks.Headers;

// Header-lər
string correlationHeader = MessageHeaders.CorrelationId;   // "x-correlation-id"
string causationHeader = MessageHeaders.CausationId;       // "x-causation-id"
string tenantHeader = MessageHeaders.TenantId;             // "x-tenant-id"
string userHeader = MessageHeaders.UserId;                 // "x-user-id"
string sourceHeader = MessageHeaders.SourceService;        // "x-source-service"
string eventNameHeader = MessageHeaders.EventName;         // "x-event-name"
string versionHeader = MessageHeaders.EventVersion;        // "x-event-version"
string traceHeader = MessageHeaders.TraceId;               // "x-trace-id"
```

### Korrelyasiya Konteksti (Correlation)

`ICorrelationContext` distributed tracing və request korrelyasiyası üçün paylaşılan abstraksiyasıdır. Biznes məntiqi birbaşa MassTransit kontekst tiplərinə bağlı olmur:

```csharp
using MW.BuildingBlocks.Correlation;

public class MyService
{
    private readonly ICorrelationContext _correlation;

    public MyService(ICorrelationContext correlation)
    {
        _correlation = correlation;
    }

    public void Process()
    {
        var correlationId = _correlation.CorrelationId;
        var causationId = _correlation.CausationId;
        var traceId = _correlation.TraceId;
    }
}
```

### Müşahidəçilik — Mesaj Log Konteksti (Observability)

`MessageLogContext` event publish/consume loglama üçün paylaşılan modeldir. Bütün servislər eyni strukturda log yazır:

```csharp
using MW.BuildingBlocks.Observability;

var logContext = new MessageLogContext
{
    MessageId = Guid.NewGuid(),
    EventName = "order.created.v1",
    EventVersion = "v1",
    CorrelationId = "corr-123",
    CausationId = "cmd-456",
    TraceId = "trace-789",
    SourceService = "OrderService",
    Consumer = "PaymentConsumer",
    Endpoint = "order-created-queue",
    Status = "Consumed"
};
```

### Müşahidəçilik Sahə Standartları (Observability)

`ObservabilityFields` Grafana/Graylog/Loki/Tempo axtarışları üçün standart sahə adlarını təmin edir:

```csharp
using MW.BuildingBlocks.Observability;

// Sahə adları
string traceField = ObservabilityFields.TraceId;           // "trace_id"
string correlationField = ObservabilityFields.CorrelationId; // "correlation_id"
string messageField = ObservabilityFields.MessageId;       // "message_id"
string eventField = ObservabilityFields.EventName;         // "event_name"
string serviceField = ObservabilityFields.ServiceName;     // "service_name"
string consumerField = ObservabilityFields.ConsumerName;   // "consumer_name"
string endpointField = ObservabilityFields.EndpointName;   // "endpoint_name"
string versionField = ObservabilityFields.EventVersion;    // "event_version"
string statusField = ObservabilityFields.Status;           // "status"
string durationField = ObservabilityFields.DurationMs;     // "duration_ms"
```

### MassTransit Header Mapping (MassTransit)

`IMessageHeaderMapper` paylaşılan metadata modelləri ilə MassTransit header-ləri arasında mapping-i mərkəzləşdirir:

```csharp
using MW.BuildingBlocks.MassTransit;
using MW.BuildingBlocks.Messaging;

public class MyHeaderMapper : IMessageHeaderMapper
{
    public IDictionary<string, object> MapToHeaders(PublishContextModel context)
    {
        var headers = new Dictionary<string, object>();
        if (context.CorrelationId != null)
            headers["x-correlation-id"] = context.CorrelationId;
        if (context.SourceService != null)
            headers["x-source-service"] = context.SourceService;
        // ... digər header-lər
        return headers;
    }

    public ConsumerContextModel MapFromHeaders(IDictionary<string, object> headers)
    {
        return new ConsumerContextModel
        {
            CorrelationId = headers.TryGetValue("x-correlation-id", out var c) ? c?.ToString() : null,
            SourceService = headers.TryGetValue("x-source-service", out var s) ? s?.ToString() : null,
            // ... digər sahələr
        };
    }
}
```

### MassTransit Observer-lər (MassTransit)

Publish, consume və send əməliyyatları üçün strukturlaşdırılmış loglama observer-ləri:

```csharp
using MW.BuildingBlocks.MassTransit;
using MW.BuildingBlocks.Observability;

// Publish observer
public class MyPublishObserver : IPublishObserver
{
    public Task OnPrePublish(MessageLogContext logContext)
    {
        // Publish öncəsi log
        return Task.CompletedTask;
    }

    public Task OnPostPublish(MessageLogContext logContext)
    {
        // Uğurlu publish sonrası log
        return Task.CompletedTask;
    }

    public Task OnPublishFault(MessageLogContext logContext, Exception exception)
    {
        // Publish xətası logu
        return Task.CompletedTask;
    }
}

// Consume observer
public class MyConsumeObserver : IConsumeObserver
{
    public Task OnPreConsume(MessageLogContext logContext) => Task.CompletedTask;
    public Task OnPostConsume(MessageLogContext logContext) => Task.CompletedTask;
    public Task OnConsumeFault(MessageLogContext logContext, Exception exception) => Task.CompletedTask;
}
```

### Event Audit Jurnalı (Audit)

`EventAuditRecord` uzunmüddətli event tarixçəsi lazım olan komandalar üçün opsional audit modelidir:

> **Diqqət:** Bu model MassTransit Inbox/Outbox əvəzi **deyil**. Dublikat aşkarlama üçün istifadə olunmamalıdır. Yalnız biznes və ya dəstək məqsədləri üçün event tarixçəsi lazım olduqda istifadə edin.

```csharp
using MW.BuildingBlocks.Audit;

var record = new EventAuditRecord
{
    MessageId = Guid.NewGuid(),
    EventName = "order.created.v1",
    Direction = "Published",
    Service = "OrderService",
    Consumer = null,
    CorrelationId = "corr-123",
    Status = "Success",
    Error = null
};
```

### Sabitlər (Constants)

Event istiqamət və status sabitləri:

```csharp
using MW.BuildingBlocks.Constants;

// İstiqamətlər
string published = EventDirections.Published;   // "Published"
string consumed = EventDirections.Consumed;     // "Consumed"

// Statuslar
string success = EventStatuses.Success;         // "Success"
string failed = EventStatuses.Failed;           // "Failed"
string pending = EventStatuses.Pending;         // "Pending"
```

## 📋 Interface-lər

| Interface | Təsvir |
|-----------|--------|
| `IIntegrationEvent` | Mikroservislər arası event kontraktı |
| `ICorrelationContext` | Distributed tracing korrelyasiya abstraksiyası |
| `IMessageHeaderMapper` | MassTransit header mapping kontraktı |
| `IPublishObserver` | Publish loglama observer interfeysi |
| `IConsumeObserver` | Consume loglama observer interfeysi |
| `ISendObserver` | Send loglama observer interfeysi |

## 📋 Modellər

| Model | Təsvir |
|-------|--------|
| `IntegrationEvent` | İnteqrasiya event base sinfi |
| `EventMetadata` | Event cross-cutting metadata |
| `ServiceIdentity` | Servis kimliyi modeli |
| `PublishContextModel` | Publish-vaxtı metadata modeli |
| `ConsumerContextModel` | Consumer-tərəfi metadata modeli |
| `MessageLogContext` | Event loglama müşahidəçilik modeli |
| `EventAuditRecord` | Opsional event audit jurnalı |

## 📋 Sabit Sinifləri

| Sinif | Təsvir |
|-------|--------|
| `MessageHeaders` | Mesaj header adı sabitləri (`x-correlation-id`, `x-trace-id`, ...) |
| `ObservabilityFields` | Grafana/Graylog sahə adı standartları (`trace_id`, `correlation_id`, ...) |
| `EventDirections` | Event istiqamət sabitləri (`Published`, `Consumed`) |
| `EventStatuses` | Event status sabitləri (`Success`, `Failed`, `Pending`) |

## 📚 Sənədlər

| Sənəd | Təsvir |
|-------|--------|
| [Event Adlandırma Konvensiyası](Docs/EventNamingConvention.md) | Event adlandırma və versiyalama qaydaları |
| [Event Loglama Qaydaları](Docs/EventLoggingGuidelines.md) | Strukturlaşdırılmış event loglama təlimatları |
| [Fault Event İdarəetmə](Docs/FaultEventHandlingGuideline.md) | MassTransit `Fault<T>` idarəetmə qaydaları |

## 🏛️ Arxitektura Qərarı

Bu BuildingBlocks layihəsi aşağıdakı prinsiplərə əsaslanır:

- **MassTransit Transactional Outbox** etibarlılıq həllidir
- **MassTransit Consumer Outbox / InboxState** dublikat aşkarlama həllidir
- **BuildingBlocks** bu mexanizmləri manual olaraq təkrarlamamalıdır
- **Event görünürlüyü** strukturlaşdırılmış loglama, korrelyasiya, fault idarəetmə və müşahidəçilik inteqrasiyaları vasitəsilə təmin olunmalıdır

## ⚠️ Bu Layihə Ehtiva Etmir

- Manual Outbox implementasiyası
- Manual Inbox implementasiyası
- Broker-spesifik biznes məntiqi
- Servis-spesifik kod

## 🔧 Tələblər

- .NET 8.0+
- C# 12+

## 📄 Lisenziya

Bu layihə MIT lisenziyası altında paylanır.
