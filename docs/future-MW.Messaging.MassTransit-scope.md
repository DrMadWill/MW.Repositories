# MW.Messaging.MassTransit — Future Project Scope

## Xülasə

Bu sənəd gələcəkdə yaradılacaq `MW.Messaging.MassTransit` layihəsinin scope-unu və məsuliyyətlərini müəyyən edir.

## Abstraction vs Implementation Analizi

### MW.Messaging.Abstractions-da qalacaqlar (kontraktlar)

| Element | Tip | İzah |
|---------|-----|------|
| `IIntegrationEvent` | Interface | Event kontraktı — transport-agnostik |
| `IntegrationEvent` | Abstract class | Base event — transport-agnostik |
| `ICorrelationContext` | Interface | Korrelyasiya abstraksiyası |
| `IMessageHeaderMapper` | Interface | Header mapping kontraktı |
| `IPublishObserver` | Interface | Publish observer kontraktı |
| `IConsumeObserver` | Interface | Consume observer kontraktı |
| `ISendObserver` | Interface | Send observer kontraktı |
| `PublishContextModel` | Model | Publish metadata — transport-agnostik |
| `ConsumerContextModel` | Model | Consume metadata — transport-agnostik |
| `EventMetadata` | Model | Cross-cutting metadata |
| `MessageLogContext` | Model | Loglama modeli |
| `MessageHeaders` | Constants | Header adları |
| `ObservabilityFields` | Constants | Observability sahə adları |

### MW.Messaging.MassTransit-da olacaqlar (implementasiyalar)

| Məsuliyyət | İzah |
|------------|------|
| **MassTransit registration extensions** | `IServiceCollection.AddMassTransitMessaging()` kimi DI extension-lar |
| **RabbitMQ configuration** | RabbitMQ host, virtual host, credential konfigurasiyası |
| **Default header mapper implementation** | `IMessageHeaderMapper`-in konkret MassTransit implementasiyası |
| **Publish observer implementation** | `IPublishObserver`-in MassTransit `IPublishObserver`-ə bağlanması |
| **Consume observer implementation** | `IConsumeObserver`-in MassTransit `IConsumeObserver`-ə bağlanması |
| **Send observer implementation** | `ISendObserver`-in MassTransit `ISendObserver`-ə bağlanması |
| **Retry/redelivery configuration** | Retry policy, redelivery interval konfiqurasiyası |
| **Outbox registration helpers** | MassTransit Transactional Outbox DI registration |
| **Endpoint naming conventions** | Queue/exchange adlandırma konvensiyaları |
| **Correlation context implementation** | `ICorrelationContext`-in MassTransit middleware implementasiyası |

## Dependency Diagramı

```
Application/Services
    │
    ├──→ MW.Messaging.Abstractions  (kontraktlar, interfeyslər)
    │
    └──→ MW.Messaging.MassTransit   (konkret implementasiya)
             │
             ├──→ MW.Messaging.Abstractions
             ├──→ MassTransit (NuGet)
             ├──→ MassTransit.RabbitMQ (NuGet)
             └──→ MassTransit.EntityFrameworkCore (NuGet, outbox üçün)
```

## Gözlənilən Paket Strukturu

```
MW.Messaging.MassTransit/
├── Configuration/
│   ├── RabbitMqOptions.cs
│   ├── RetryOptions.cs
│   └── OutboxOptions.cs
├── Extensions/
│   ├── MassTransitServiceCollectionExtensions.cs
│   └── EndpointConventionExtensions.cs
├── Observers/
│   ├── DefaultPublishObserver.cs
│   ├── DefaultConsumeObserver.cs
│   └── DefaultSendObserver.cs
├── Headers/
│   └── DefaultMessageHeaderMapper.cs
├── Correlation/
│   └── MassTransitCorrelationContext.cs
└── MW.Messaging.MassTransit.csproj
```

## Backlog Qeydi

Bu layihə messaging refactor-un **növbəti mərhələsidir**. Mövcud `MW.Messaging.Abstractions` sabitləşdikdən sonra yaradılmalıdır.

### Prioritet sırası:
1. `MW.Messaging.MassTransit` layihəsini yarat
2. Registration extensions implementasiya et
3. Default observer-ləri implementasiya et
4. Header mapper implementasiya et
5. Correlation context middleware yaz
6. Retry/redelivery configuration əlavə et
7. Outbox registration helpers əlavə et
8. Endpoint naming conventions implementasiya et
