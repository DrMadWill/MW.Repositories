# MW.Messaging.Abstractions

**MW.Messaging.Abstractions** — mikroservis ekosistemi üçün transport-agnostik mesajlaşma/eventing abstraksiyalarını, kontraktları, korrelyasiya, müşahidəçilik (observability) və MassTransit-uyğun interfeyslər təmin edən .NET 8.0 kitabxanasıdır.

> **Vacib:** Bu layihə manual Outbox/Inbox implementasiyası ehtiva etmir. Etibarlılıq (reliability) üçün **MassTransit Transactional Outbox** istifadə olunur. MW.Messaging.Abstractions yalnız event kontraktları, korrelyasiya, strukturlaşdırılmış loglama və müşahidəçilik üzərində fokuslanır.

## 🎯 Məqsəd

Bu paket mikroservislər arasında mesajlaşma/eventing üçün paylaşılan abstraksiyalar təmin edir. Konkret transport implementasiyasından (RabbitMQ, Kafka, və s.) asılı olmayan kontraktlar, modellər və interfeyslər bu paketdə yerləşir.

## 📦 Quraşdırma

### NuGet Package Manager

```bash
Install-Package MW.Messaging.Abstractions
```

### .NET CLI

```bash
dotnet add package MW.Messaging.Abstractions
```

### PackageReference

```xml
<PackageReference Include="MW.Messaging.Abstractions" Version="1.0.0" />
```

## ✅ Scope (Bu paketin məsuliyyəti)

- **İnteqrasiya event kontraktları** — `IIntegrationEvent`, `IntegrationEvent`
- **Mesaj header-ləri** — `MessageHeaders` standart header sabitləri
- **Korrelyasiya** — `ICorrelationContext` distributed tracing abstraksiyası
- **Publish/consume kontekst modelləri** — `PublishContextModel`, `ConsumerContextModel`
- **Müşahidəçilik abstraksiyaları** — `MessageLogContext`, `ObservabilityFields`
- **MassTransit abstraksiyaları** — Observer interfeyslər (`IPublishObserver`, `IConsumeObserver`, `ISendObserver`), `IMessageHeaderMapper`
- **Event metadata** — `EventMetadata`, `ServiceIdentity`
- **Audit** — `EventAuditRecord` (opsional event tarixçəsi)
- **Sabitlər** — `EventDirections`, `EventStatuses`
- **Sənədlər** — Event adlandırma, loglama qaydaları, fault idarəetmə

## ❌ Non-Scope (Bu paketin məsuliyyətinə daxil deyil)

- **Repository abstraksiyaları** — `MW.Persistence.Abstractions`-da yerləşir
- **Identity implementasiyası** — HTTP/user token çıxarma `MW.Identity.Token`-da qalır
- **Manual outbox/inbox implementasiyası** — MassTransit Transactional Outbox istifadə olunur
- **Biznes-spesifik repair məntiqi** — Servis səviyyəsində qalmalıdır
- **Servis-spesifik sinxronizasiya məntiqi** — Hər servisin öz daxili işidir
- **Konkret MassTransit implementasiyası** — Gələcəkdə `MW.Messaging.MassTransit`-da olacaq

## 🏗️ Paket Strukturu

```
MW.Messaging.Abstractions/
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

## 🔑 Əsas Konseptlər

| Konsept | Təsvir |
|---------|--------|
| **İnteqrasiya Event** | Mikroservislər arası asınxron kommunikasiya kontraktı |
| **Korrelyasiya** | Distributed tracing üçün request/event əlaqələndirmə |
| **Header Mapping** | Mesaj metadata-sının transport header-lərinə çevrilməsi |
| **Observability** | Strukturlaşdırılmış loglama və monitorinq standartları |
| **Audit Trail** | Opsional event tarixçəsi jurnalı |

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
                │ implemented by (gələcək)
                ▼
┌──────────────────────────────────────────┐
│      MW.Messaging.MassTransit            │
│  (konkret MassTransit implementasiyası)  │
└──────────────────────────────────────────┘
```

**Asılılıq qaydaları:**
- Application/servis layihələri `MW.Messaging.Abstractions`-dan asılıdır
- Messaging infrastruktur implementasiyası gələcəkdə `MW.Messaging.MassTransit`-da olacaq
- HTTP user identity `MW.Identity.Token`-da qalır — mesaj konteksti ilə qarışmamalıdır
- Persistence concern-ləri `MW.Persistence.*`-da qalır — messaging-dən asılı olmamalıdır

## 🚀 İstifadə

### İnteqrasiya Event Kontraktları (Contracts)

```csharp
using MW.Messaging.Contracts;

public class OrderCreatedEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    public decimal TotalAmount { get; init; }

    public override string EventName => "order.created.v1";
    public override string EventVersion => "v1";
    public override string SourceService => "OrderService";
}
```

### Event Metadata (Messaging)

```csharp
using MW.Messaging.Messaging;

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

### Publish / Consumer Kontekst Modelləri

```csharp
using MW.Messaging.Messaging;

var publishContext = new PublishContextModel
{
    CorrelationId = "corr-123",
    CausationId = "cmd-456",
    TenantId = Guid.Parse("..."),
    UserId = Guid.Parse("..."),
    SourceService = "OrderService",
    TraceId = "trace-789"
};

var consumerContext = new ConsumerContextModel
{
    MessageId = Guid.NewGuid(),
    CorrelationId = "corr-123",
    SourceService = "OrderService",
    EventName = "order.created.v1"
};
```

### Mesaj Header Sabitləri (Headers)

```csharp
using MW.Messaging.Headers;

string correlationHeader = MessageHeaders.CorrelationId;   // "x-correlation-id"
string causationHeader = MessageHeaders.CausationId;       // "x-causation-id"
string tenantHeader = MessageHeaders.TenantId;             // "x-tenant-id"
string sourceHeader = MessageHeaders.SourceService;        // "x-source-service"
string traceHeader = MessageHeaders.TraceId;               // "x-trace-id"
```

### Korrelyasiya Konteksti (Correlation)

```csharp
using MW.Messaging.Correlation;

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

### Müşahidəçilik (Observability)

```csharp
using MW.Messaging.Observability;

var logContext = new MessageLogContext
{
    MessageId = Guid.NewGuid(),
    EventName = "order.created.v1",
    CorrelationId = "corr-123",
    SourceService = "OrderService",
    Consumer = "PaymentConsumer",
    Status = "Consumed"
};
```

### MassTransit Abstraksiyaları

```csharp
using MW.Messaging.MassTransit;
using MW.Messaging.Messaging;

public class MyHeaderMapper : IMessageHeaderMapper
{
    public IDictionary<string, object> MapToHeaders(PublishContextModel context)
    {
        var headers = new Dictionary<string, object>();
        if (context.CorrelationId != null)
            headers["x-correlation-id"] = context.CorrelationId;
        return headers;
    }

    public ConsumerContextModel MapFromHeaders(IDictionary<string, object> headers)
    {
        return new ConsumerContextModel
        {
            CorrelationId = headers.TryGetValue("x-correlation-id", out var c) ? c?.ToString() : null,
        };
    }
}
```

## 📋 Interface-lər

| Interface | Namespace | Təsvir |
|-----------|-----------|--------|
| `IIntegrationEvent` | `MW.Messaging.Contracts` | Mikroservislər arası event kontraktı |
| `ICorrelationContext` | `MW.Messaging.Correlation` | Distributed tracing korrelyasiya abstraksiyası |
| `IMessageHeaderMapper` | `MW.Messaging.MassTransit` | MassTransit header mapping kontraktı |
| `IPublishObserver` | `MW.Messaging.MassTransit` | Publish loglama observer interfeysi |
| `IConsumeObserver` | `MW.Messaging.MassTransit` | Consume loglama observer interfeysi |
| `ISendObserver` | `MW.Messaging.MassTransit` | Send loglama observer interfeysi |

## 📋 Modellər

| Model | Namespace | Təsvir |
|-------|-----------|--------|
| `IntegrationEvent` | `MW.Messaging.Contracts` | İnteqrasiya event base sinfi |
| `EventMetadata` | `MW.Messaging.Messaging` | Event cross-cutting metadata |
| `ServiceIdentity` | `MW.Messaging.Messaging` | Servis kimliyi modeli |
| `PublishContextModel` | `MW.Messaging.Messaging` | Publish-vaxtı metadata modeli |
| `ConsumerContextModel` | `MW.Messaging.Messaging` | Consumer-tərəfi metadata modeli |
| `MessageLogContext` | `MW.Messaging.Observability` | Event loglama müşahidəçilik modeli |
| `EventAuditRecord` | `MW.Messaging.Audit` | Opsional event audit jurnalı |

## 📋 Sabit Sinifləri

| Sinif | Namespace | Təsvir |
|-------|-----------|--------|
| `MessageHeaders` | `MW.Messaging.Headers` | Mesaj header adı sabitləri |
| `ObservabilityFields` | `MW.Messaging.Observability` | Grafana/Graylog sahə adı standartları |
| `EventDirections` | `MW.Messaging.Constants` | Event istiqamət sabitləri |
| `EventStatuses` | `MW.Messaging.Constants` | Event status sabitləri |

## 📚 Sənədlər

| Sənəd | Təsvir |
|-------|--------|
| [Event Adlandırma Konvensiyası](Docs/EventNamingConvention.md) | Event adlandırma və versiyalama qaydaları |
| [Event Loglama Qaydaları](Docs/EventLoggingGuidelines.md) | Strukturlaşdırılmış event loglama təlimatları |
| [Fault Event İdarəetmə](Docs/FaultEventHandlingGuideline.md) | MassTransit `Fault<T>` idarəetmə qaydaları |

## 🏛️ Arxitektura Qərarı

Bu layihə aşağıdakı prinsiplərə əsaslanır:

- **MassTransit Transactional Outbox** etibarlılıq həllidir
- **MassTransit Consumer Outbox / InboxState** dublikat aşkarlama həllidir
- **MW.Messaging.Abstractions** bu mexanizmləri manual olaraq təkrarlamamalıdır
- **Event görünürlüyü** strukturlaşdırılmış loglama, korrelyasiya, fault idarəetmə və müşahidəçilik inteqrasiyaları vasitəsilə təmin olunmalıdır

## ⚠️ Bu Layihə Ehtiva Etmir

- Manual Outbox implementasiyası
- Manual Inbox implementasiyası
- Broker-spesifik biznes məntiqi
- Servis-spesifik kod
- Konkret MassTransit registration/configuration

## 🔧 Tələblər

- .NET 8.0+
- C# 12+

## 📄 Lisenziya

Bu layihə MIT lisenziyası altında paylanır.
