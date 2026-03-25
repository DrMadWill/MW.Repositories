# Migration Guide: MW.BuildingBlocks → MW.Messaging.Abstractions

## Xülasə

`MW.BuildingBlocks` layihəsi `MW.Messaging.Abstractions` olaraq yenidən adlandırılıb. Bu sənəd keçid prosesini izah edir.

## Dəyişikliklər

| Köhnə | Yeni |
|-------|------|
| Layihə adı: `MW.BuildingBlocks` | Layihə adı: `MW.Messaging.Abstractions` |
| Assembly: `MW.BuildingBlocks` | Assembly: `MW.Messaging.Abstractions` |
| Root Namespace: `MW.BuildingBlocks` | Root Namespace: `MW.Messaging` |
| NuGet: `MW.BuildingBlocks` | NuGet: `MW.Messaging.Abstractions` |

## Namespace Dəyişiklikləri

| Köhnə Namespace | Yeni Namespace |
|-----------------|----------------|
| `MW.BuildingBlocks.Audit` | `MW.Messaging.Audit` |
| `MW.BuildingBlocks.Constants` | `MW.Messaging.Constants` |
| `MW.BuildingBlocks.Contracts` | `MW.Messaging.Contracts` |
| `MW.BuildingBlocks.Correlation` | `MW.Messaging.Correlation` |
| `MW.BuildingBlocks.Headers` | `MW.Messaging.Headers` |
| `MW.BuildingBlocks.MassTransit` | `MW.Messaging.MassTransit` |
| `MW.BuildingBlocks.Messaging` | `MW.Messaging.Messaging` |
| `MW.BuildingBlocks.Observability` | `MW.Messaging.Observability` |

## Tələb olunan Kod Dəyişiklikləri

### 1. PackageReference yeniləyin

```xml
<!-- Köhnə -->
<PackageReference Include="MW.BuildingBlocks" Version="1.0.0" />

<!-- Yeni -->
<PackageReference Include="MW.Messaging.Abstractions" Version="1.0.0" />
```

### 2. Using direktivlərini yeniləyin

Bütün `using MW.BuildingBlocks.*` ifadələrini `using MW.Messaging.*` ilə əvəz edin:

```csharp
// Köhnə
using MW.BuildingBlocks.Contracts;
using MW.BuildingBlocks.Messaging;
using MW.BuildingBlocks.Correlation;
using MW.BuildingBlocks.Observability;
using MW.BuildingBlocks.MassTransit;
using MW.BuildingBlocks.Headers;
using MW.BuildingBlocks.Audit;
using MW.BuildingBlocks.Constants;

// Yeni
using MW.Messaging.Contracts;
using MW.Messaging.Messaging;
using MW.Messaging.Correlation;
using MW.Messaging.Observability;
using MW.Messaging.MassTransit;
using MW.Messaging.Headers;
using MW.Messaging.Audit;
using MW.Messaging.Constants;
```

### 3. Fully Qualified Namespace referanslarını yeniləyin

Kodda `MW.BuildingBlocks.` prefiksi ilə istifadə olunan bütün fully qualified referansları `MW.Messaging.` ilə əvəz edin.

### 4. ProjectReference yeniləyin (əgər varsa)

```xml
<!-- Köhnə -->
<ProjectReference Include="..\MW.BuildingBlocks\MW.BuildingBlocks.csproj" />

<!-- Yeni -->
<ProjectReference Include="..\MW.Messaging.Abstractions\MW.Messaging.Abstractions.csproj" />
```

## Breaking Changes

- **Paket adı dəyişib:** `MW.BuildingBlocks` → `MW.Messaging.Abstractions`
- **Bütün namespace-lər dəyişib:** `MW.BuildingBlocks.*` → `MW.Messaging.*`
- **Assembly adı dəyişib:** `MW.BuildingBlocks.dll` → `MW.Messaging.Abstractions.dll`

## Avtomatik Keçid (Find & Replace)

Aşağıdakı find-replace əməliyyatlarını tətbiq edin:

1. `MW.BuildingBlocks` → `MW.Messaging` (namespace və using-lər üçün)
2. `MW.BuildingBlocks.csproj` → `MW.Messaging.Abstractions.csproj` (ProjectReference üçün)
3. `PackageReference Include="MW.BuildingBlocks"` → `PackageReference Include="MW.Messaging.Abstractions"` (NuGet üçün)
