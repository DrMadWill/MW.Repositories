# MW.Saga.MassTransit

`MW.Saga.MassTransit`, `MW.Saga.Abstractions` üzərində qurulmuş MassTransit əsaslı saga infrastruktur inteqrasiya paketidir.

## Nə ehtiva edir

- **State-machine qeydiyyatı** — Saga state machine və state tipləri üçün standartlaşdırılmış DI qeydiyyat helperləri
- **Persistence quraşdırması** — EF Core əsaslı saga state saxlama üçün təkrar istifadə edilə bilən persistensiya helperləri
- **Kontekst yayılması** — Saga icra kontekstinin runtime-da doldurulması və scope əsaslı accessor
- **Müşahidə inteqrasiyası** — `ISagaObserver` üzərindən strukturlaşdırılmış logging və lifecycle observability
- **Timeout/scheduler dəstəyi** — MassTransit message scheduler inteqrasiyası və timeout konvensiyaları
- **Retry/fault hook-ları** — Saga event handling üçün infrastruktur səviyyəsində retry və fault idarəetmə
- **Endpoint adlandırma** — Kebab-case formatında servise-spesifik prefikslərlə saga endpoint adlandırma konvensiyası
- **Korrelyasiya adapter** — `ISagaCorrelationResolver<TMessage>` abstraksiyasından MassTransit korrelyasiya konfiqurasiyasına köprü
- **Saga definition/konvensiya** — Endpoint, retry və concurrency parametrlərini standartlaşdıran `StandardSagaDefinition<TSaga>`
- **Test harness helper** — Saga state-machine test ssenariləri üçün təkrar istifadə edilə bilən test infrastrukturu
- **Konfiqurasiya binding** — `SagaMassTransitOptions` modelinin `IConfiguration`-dan binding dəstəyi

## Nə ehtiva etmir

- Business-spesifik saga workflow-ları (`OrderSaga`, `AuctionSaga`, `PaymentSaga` və s.)
- Servis-spesifik kompensasiya məntiqi
- Domain-spesifik proses orkestrrasiya qaydaları

## Quraşdırma

```csharp
services.AddSagaMassTransitInfrastructure(options =>
{
    options.BindOptions(configuration);
});
```

## Konfiqurasiya

```json
{
  "SagaMassTransit": {
    "EndpointPrefix": "my-service",
    "RetryCount": 3,
    "RetryIntervalsInSeconds": [2, 5, 10],
    "DefaultTimeoutInSeconds": 300,
    "ConcurrencyLimit": 0,
    "UseScheduler": false
  }
}
```

## Asılılıqlar

- `MW.Saga.Abstractions` — Saga kontraktları, kontekst abstraksiyaları, lifecycle modelləri
- `MW.Messaging.Abstractions` — Paylaşılan mesajlaşma metadata/korrelyasiya konseptləri
- `MassTransit` — State machine dəstəyi
- `MassTransit.EntityFrameworkCore` — EF Core saga persistensiyası
- `Microsoft.EntityFrameworkCore` — ORM framework

## Arxitektura Sərhədi

Bu paket **infrastruktur inteqrasiya təbəqəsidir**. Yalnız bunları ehtiva etməlidir:
- MassTransit inteqrasiyası
- Persistensiya
- Kontekst yayılması
- Müşahidə
- Retry/fault/scheduler hook-ları

Bu paketə **əlavə edilməməlidir**:
- `OrderSaga`, `AuctionSaga`, `PaymentSaga` və s.
- Servis-spesifik kompensasiya qaydaları
- Business workflow qərarları
