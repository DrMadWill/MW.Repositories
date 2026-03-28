# MW.Saga.Abstractions

**MW.Saga.Abstractions** — uzunmüddətli paylanmış proseslər (saga) üçün transport-agnostik abstraction-lar və kontrakt-lar təqdim edən paylaşılan kitabxanadır.

## 🎯 Məqsəd

Bu paket saga ilə bağlı kontraktları, state modelləri, context abstraction-ları, korrelyasiya abstraction-larını və müşahidə (observability) primitivlərini təmin edir. Beləliklə, bütün saga implementasiyaları eyni platformaya uyğun formada qurula bilər.

## 📦 Quraşdırma

```bash
dotnet add package MW.Saga.Abstractions
```

## ✅ Paketin Əhatə Dairəsi (Scope)

Bu paketə **daxil** olan anlayışlar:

| Kateqoriya | Təsvir |
|---|---|
| **State Kontraktları** | `ISagaState`, `SagaStateBase`, `SagaStatus` |
| **Context Abstraction-ları** | `ISagaContext`, `ISagaContextAccessor`, `ISagaExecutionContext` |
| **Korrelyasiya** | `ISagaCorrelationResolver<TMessage>` |
| **Müşahidə (Observability)** | `SagaObservabilityContext`, `ISagaObserver`, `SagaTransitionInfo` |
| **Lifecycle Modelləri** | `SagaStarted`, `SagaStateChanged`, `SagaCompleted`, `SagaFailed`, `SagaTimedOut` |
| **Sabitlər** | `SagaConstants`, `SagaHeaders` |
| **Metadata** | `SagaMetadata` |

## ❌ Paketin Əhatə Dairəsinə Daxil Olmayanlar

Bu paketə **daxil olmayan** anlayışlar:

- ❌ MassTransit qeydiyyatı və konfiqurasiyası
- ❌ Persistence / verilənlər bazası setup-u
- ❌ Scheduler konfiqurasiyası
- ❌ Biznes-spesifik saga workflow-ları
- ❌ Transport-spesifik implementasiya detalları

## 🏗️ Paket Strukturu

```
MW.Saga.Abstractions/
├── Contracts/           → Saga state kontraktları (ISagaState)
├── Models/              → State base class, status enum, metadata
├── Context/             → Saga context abstraction-ları
├── Correlation/         → Saga korrelyasiya resolver-i
├── Lifecycle/           → Saga lifecycle event modelləri
├── Observability/       → Observer kontraktı və observability modelləri
├── Constants/           → Sabit dəyərlər (state adları, header-lər)
└── Docs/                → Arxitektura qeydləri
```

## 🔑 Əsas Anlayışlar

| Tip | Namespace | Təsvir |
|---|---|---|
| `ISagaState` | `MW.Saga.Contracts` | Hər saga state modelinin implementasiya etməli olduğu minimal kontrakt |
| `SagaStateBase` | `MW.Saga.Models` | Ümumi saga state xüsusiyyətləri ilə abstract base class |
| `SagaStatus` | `MW.Saga.Models` | Yüksək səviyyəli saga lifecycle status enum-u |
| `SagaMetadata` | `MW.Saga.Models` | Logging və monitoring üçün yüngül metadata modeli |
| `ISagaContext` | `MW.Saga.Context` | Cari saga kontekstinə read-only giriş |
| `ISagaContextAccessor` | `MW.Saga.Context` | Scoped saga context accessor-u |
| `ISagaExecutionContext` | `MW.Saga.Context` | Geniş icra kontekst metadata-sı |
| `ISagaCorrelationResolver<T>` | `MW.Saga.Correlation` | Mesajlardan korrelyasiya həll edən kontrakt |
| `SagaObservabilityContext` | `MW.Saga.Observability` | Müşahidə üçün standart kontekst modeli |
| `ISagaObserver` | `MW.Saga.Observability` | Saga lifecycle müşahidə kontraktı |
| `SagaTransitionInfo` | `MW.Saga.Observability` | State keçidi modeli |
| `SagaConstants` | `MW.Saga.Constants` | Saga ilə əlaqəli sabit dəyərlər |
| `SagaHeaders` | `MW.Saga.Constants` | Saga-aware mesaj header adları |

## 🏛️ Asılılıq İstiqaməti

```
MW.Saga.Abstractions (bu paket — heç bir xarici asılılıq yoxdur)
        ▲
        │
MW.Saga.MassTransit (gələcək implementasiya paketi)
```

## 🔒 Abstraction vs İmplementasiya Sərhədi

| Bu Paketdə ✅ | İmplementasiya Paketlərində (gələcək) 🔧 |
|---|---|
| `ISagaState` kontraktı | MassTransit `SagaStateMachineInstance` mapping |
| `ISagaContext` | Runtime `ConsumeContext`-dən populate etmə |
| `ISagaContextAccessor` | Scoped DI qeydiyyatı |
| `ISagaObserver` | MassTransit observer binding |
| `ISagaCorrelationResolver<T>` | Spesifik message tipləri üçün korrelyasiya |

## 🔗 MW.Messaging.Abstractions ilə Sərhəd

Bəzi anlayışlar (korrelyasiya, observability, header-lər) messaging abstraction paketi ilə üst-üstə düşə bilər:

| Anlayış | MW.Messaging.Abstractions | MW.Saga.Abstractions |
|---|---|---|
| Korrelyasiya | `ICorrelationContext` (mesaj səviyyəsində) | `ISagaCorrelationResolver<T>` (saga-spesifik) |
| Observability | Mesaj publish/consume müşahidəsi | Saga lifecycle müşahidəsi |
| Header-lər | `MessageHeaders` (mesaj metadata) | `SagaHeaders` (saga metadata) |
| Context | `IMessageExecutionContext` | `ISagaExecutionContext` |

**Prinsip:** Messaging abstraction-ları mesaj səviyyəsində işləyir, saga abstraction-ları isə saga lifecycle səviyyəsində işləyir. Hər iki paket bir-birindən müstəqildir.

## 🔧 Tələblər

- .NET 8.0+
- C# 12+
