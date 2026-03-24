# Capability Boundary Documentation

Bu sənəd solution-dakı hər capability qrupunun məsuliyyət sərhədlərini izah edir.

## Solution Strukturu

```
Domain/
├── MW.Core                        — DDD əsas kitabxanası
└── MW.Application.Abstractions    — Application-layer kontraktları

Repository/
├── MW.Persistence.Abstractions           — Persistence kontraktları
├── MW.Persistence.DependencyInjection    — DI registration
└── MW.Persistence.EntityFrameworkCore    — EF Core implementasiyaları

Identity/
└── MW.Identity.Token              — HTTP/user token idarəetmə

Message/
└── MW.Messaging.Abstractions      — Mesajlaşma/eventing abstraksiyaları
```

---

## Domain Capability

### MW.Core

- **Məsuliyyət:** DDD əsas primitvləri — Entity, AggregateRoot, ValueObject, DomainEvent, BusinessRule, Auditing, MultiTenancy
- **Asılılıq:** MediatR.Contracts (yalnız `INotification` üçün)
- **Qeyd:** Domain event-lər `IDomainEvent : INotification` interfeysi ilə ifadə olunur. Bunlar **in-process** domain event-lərdir, integration event-lər deyil.

### MW.Application.Abstractions

- **Məsuliyyət:** CQRS kontraktları (ICommand, IQuery), Error modellər, Authorization markerləri, Caching, Validation, Pagination, Context, Time
- **Asılılıq:** MediatR, CSharpFunctionalExtensions, Dr.Pagination, EF Core
- **Qeyd:** Messaging kontraktları bu capability-yə **aid deyil**. İnteqrasiya event kontraktları `Message` capability-sində yerləşir.

### Sərhəd Qaydası

- Domain event-lər (`IDomainEvent`) — MW.Core-da yerləşir, in-process notification-dır
- İnteqrasiya event-ləri (`IIntegrationEvent`) — MW.Messaging.Abstractions-da yerləşir, cross-service communication üçündür
- Bu iki konsept qarışdırılmamalıdır

---

## Repository Capability

### MW.Persistence.Abstractions

- **Məsuliyyət:** Repository, UnitOfWork, Specification, Transaction, Query interfeyslər
- **Asılılıq:** MW.Core (NuGet 1.0.1)

### MW.Persistence.EntityFrameworkCore

- **Məsuliyyət:** Bütün persistence abstraksiyalarının EF Core implementasiyası
- **Asılılıq:** MW.Core, MW.Persistence.Abstractions, EF Core

### MW.Persistence.DependencyInjection

- **Məsuliyyət:** Persistence servislərinin DI registration-ı
- **Asılılıq:** MW.Persistence.EntityFrameworkCore, EF Core providers

### Messaging-dən Asılılıq Yoxlaması

Repository capability-si **messaging-dən asılı olmamalıdır**:
- ✅ MW.Persistence.Abstractions — MW.Messaging.Abstractions-a referansı **yoxdur**
- ✅ MW.Persistence.DependencyInjection — MW.Messaging.Abstractions-a referansı **yoxdur**
- ✅ MW.Persistence.EntityFrameworkCore — MW.Messaging.Abstractions-a referansı **yoxdur**

Bu doğru arxitektura yanaşmasıdır. Persistence concern-ləri messaging-dən müstəqil olmalıdır.

---

## Identity Capability

### MW.Identity.Token

- **Məsuliyyət:** HTTP/user token çıxarma, claim extraction, JWT idarəetmə
- **Asılılıq:** Microsoft.AspNetCore.App, Newtonsoft.Json
- **Qeyd:** Bu paket **HTTP kontekstinə** bağlıdır — request-dən user identity çıxarır.

### Identity vs Message Sərhədi

| Concern | Layihə | İzah |
|---------|--------|------|
| HTTP user token/claim | MW.Identity.Token | Request pipeline-da user identity |
| Message correlation/tracing | MW.Messaging.Abstractions | Transport-agnostik mesaj metadata |
| User ID mesaj header-də | MW.Messaging.Abstractions | `MessageHeaders.UserId` sabiti |

**Qaydalar:**
- Message kontraktları HTTP-spesifik identity məntiqinə birbaşa asılı **olmamalıdır**
- User identity mesaja `PublishContextModel.UserId` vasitəsilə header kimi ötürülür
- Consumer tərəfində identity `ConsumerContextModel.UserId`-dən oxunur
- HTTP context ilə message context **ayrı saxlanılmalıdır**

---

## Message Capability

### MW.Messaging.Abstractions

- **Məsuliyyət:** Transport-agnostik mesajlaşma/eventing abstraksiyaları
- **Asılılıq:** Heç bir external asılılıq yoxdur (standalone paket)
- **Scope:** İnteqrasiya event kontraktları, header-lər, korrelyasiya, metadata modellər, observability, MassTransit abstraksiyaları

### MassTransit Abstraksiya Sərhədi

`MW.Messaging.Abstractions/MassTransit/` qovluğu **yalnız abstraksiyalar/interfeyslər** ehtiva edir:
- `IPublishObserver` — publish observer interfeysi
- `IConsumeObserver` — consume observer interfeysi
- `ISendObserver` — send observer interfeysi
- `IMessageHeaderMapper` — header mapping kontraktı

Bu interfeyslerin **konkret implementasiyaları** gələcəkdə `MW.Messaging.MassTransit` layihəsində olacaq.
