# MW.Saga.Abstractions — Arxitektura Qeydləri

## İlk Versiya Əhatə Dairəsi

`MW.Saga.Abstractions` paketi aşağıdakı sahələrə fokuslanır:

### 1. State Kontraktları
- `ISagaState` minimal kontraktı hər saga state modelinin implementasiya etməli olduğu əsas xüsusiyyətləri müəyyən edir.
- `SagaStateBase` abstract sinifi ümumi lifecycle xüsusiyyətlərini (CreatedAt, UpdatedAt, CompletedAt, FailedAt, Version) standartlaşdırır.
- `SagaStatus` enum-u yüksək səviyyəli saga lifecycle statusunu təmsil edir.

### 2. Context / Accessor Kontraktları
- `ISagaContext` cari saga execution metadata-sına read-only giriş təmin edir.
- `ISagaContextAccessor` scoped accessor vasitəsilə aktiv saga kontekstinə güvənli giriş verir.
- `ISagaExecutionContext` daha geniş icra kontekst məlumatları (CausationId, SagaName, TraceId) təqdim edir.

### 3. Korrelyasiya
- `ISagaCorrelationResolver<TMessage>` generic kontraktı gələn mesajlardan saga correlation ID-nin həllini standartlaşdırır.
- Transport və serialization formatından müstəqildir.

### 4. Observability
- `SagaObservabilityContext` logging və monitoring üçün standart kontekst modeli təmin edir.
- `ISagaObserver` saga lifecycle hadisələri üçün observer kontraktını müəyyən edir.
- `SagaTransitionInfo` state keçidlərini strukturlaşdırılmış formada təmsil edir.

### 5. Lifecycle Modelləri
- `SagaStarted`, `SagaStateChanged`, `SagaCompleted`, `SagaFailed`, `SagaTimedOut` modelləri saga lifecycle mərhələlərini təmsil edir.
- Logging, audit və monitoring inteqrasiyası üçün istifadəyə yararlıdır.

## Niyə Generic Workflow Engine Deyil?

Bu paket **saga-spesifik abstraction-lar** təqdim edir, amma ümumi workflow engine olmağa çalışmır. Səbəblər:

1. **Fokus:** Saga pattern-i spesifik bir distributed computing pattern-idir. Generic workflow engine fərqli abstractions tələb edər.
2. **Sadəlik:** Minimal, anlaşılan kontraktlar saxlamaq gələcək baxım və istifadə asanlığını artırır.
3. **Transport uyğunluğu:** MassTransit və digər saga framework-ləri ilə uyğun abstraction-lar saxlamaq inteqrasiya işini asanlaşdırır.
4. **Tədrici genişlənmə:** Ehtiyac olduqca yeni abstractions əlavə etmək mümkündür, amma əvvəlcədən həddən artıq abstractions yaratmaq qarşısını almaq lazımdır.

## Dizayn Prinsipləri

- **Transport-agnostik:** Heç bir MassTransit və ya digər transport tipinə birbaşa asılılıq yoxdur.
- **Minimal:** Yalnız ortaq platform səviyyəsindəki kontraktlar daxil edilir.
- **Genişlənə bilən:** Yeni kontraktlar əlavə oluna bilər, amma mövcud kontraktlar dəyişdirilməməlidir (Open-Closed).
- **Namespace uyğunluğu:** Namespace-lər qovluq strukturu ilə uyğundur (`MW.Saga.[Feature]`).

## MW.Messaging.Abstractions ilə Sərhəd

| Anlayış | Messaging | Saga |
|---|---|---|
| Korrelyasiya | Mesaj səviyyəsində (`ICorrelationContext`) | Saga instansı səviyyəsində (`ISagaCorrelationResolver<T>`) |
| Context | Mesaj icra konteksti (`IMessageExecutionContext`) | Saga icra konteksti (`ISagaExecutionContext`) |
| Observer | Mesaj publish/consume observer-ləri | Saga lifecycle observer-i (`ISagaObserver`) |
| Header-lər | Mesaj header-ləri (`MessageHeaders`) | Saga header-ləri (`SagaHeaders`) |

**Prinsip:** `MW.Messaging.Abstractions` mesaj ötürülməsi səviyyəsində işləyir, `MW.Saga.Abstractions` isə saga lifecycle səviyyəsində. İkisi arasında dublikat yoxdur — hər biri öz abstraction səviyyəsini təmsil edir.
