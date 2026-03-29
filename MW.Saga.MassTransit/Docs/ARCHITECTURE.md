# MW.Saga.MassTransit — Arxitektura Qeydi

## Məqsəd

`MW.Saga.MassTransit` paketi **infrastruktur inteqrasiya təbəqəsi** kimi xidmət edir.
`MW.Saga.Abstractions`-da təyin edilmiş abstraksiyaların MassTransit əsaslı konkret implementasiyasını təmin edir.

## Paket Tərkibi

| Kateqoriya | Təsvir |
|---|---|
| MassTransit inteqrasiyası | State machine qeydiyyatı, bus konfiqurasiyası |
| Persistensiya | EF Core saga state saxlama |
| Kontekst yayılması | Scoped saga kontekst accessor və execution context |
| Müşahidə | Strukturlaşdırılmış logging, ISagaObserver adapter |
| Retry/fault/scheduler | İnfrastruktur səviyyəsində hook-lar |

## Paketə Daxil Olmamalıdır

- `OrderSaga`, `AuctionSaga`, `PaymentSaga` kimi business saga workflow-ları
- Servis-spesifik kompensasiya qaydaları
- Business workflow qərarları
- Domain-spesifik proses orkestrrasiya məntiqi

## MW.Messaging.MassTransit ilə Sərhəd

Bəzi elementlər (retry, observability, endpoint naming, headers, correlation) `MW.Messaging.MassTransit` ilə örtüşə bilər.

### Yenidən istifadə edilə bilən (Messaging-dən)
- `ServiceEndpointNameFormatter` naming konvensiyası — saga endpoint formatter eyni pattern-i izləyir
- Header konstantları (`X-Source-Service` və s.)
- `IServiceIdentityProvider` — servis identifikasiyası
- Distributed tracing pattern-i (ActivitySource)

### Saga-ya Xas Olanlar (Bu paketdə qalmalıdır)
- `ISagaContextAccessor` / `ISagaExecutionContext` implementasiyaları
- `SagaContextPopulationFilter` — saga-spesifik kontekst doldurma
- `SagaObserverAdapter` / `StructuredSagaLogger` — saga lifecycle observability
- `SagaCorrelationBridge` — saga korrelyasiya adapter
- `StandardSagaDefinition` — saga endpoint/retry/concurrency konvensiyaları
- `SagaStateBaseMap` — saga state EF Core mapping
- `SagaSchedulerConfigurator` / `SagaTimeoutConventions` — saga timeout infrastrukturu
- `SagaFaultFilter` — saga fault handling hook
- `SagaTestHarnessHelper` — saga test infrastrukturu

## Duplikasiya Riskinin Azaldılması

- Naming konvensiyaları üçün `MW.Messaging.MassTransit.Naming.ServiceEndpointNameFormatter` pattern-indən yararlanılıb
- Saga-spesifik observability `MW.Saga.Observability` abstraksiyalarına əsaslanır (messaging observability-dən fərqlidir)
- Retry pattern eynidir, amma saga-spesifik konfiqurasiya ilə ayrılıb
