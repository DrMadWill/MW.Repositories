# MW.Extensions

**MW.Extensions** — Clean Architecture və Domain-Driven Design (DDD) prinsiplərinə əsaslanan .NET 8.0 mono-repo kitabxana toplusudur. Bu repo domain modellər, application-layer abstraksiyalar, persistence kontraktları, EF Core implementasiyaları, DI composition, mikroservis mesajlaşma infrastrukturu və JWT identity idarəetməsini əhatə edir.

## 📦 Layihələr

| Layihə | Təsvir | Asılılıqlar |
|--------|--------|-------------|
| [MW.Core](MW.Core/) | DDD əsas kitabxanası — Entity, AggregateRoot, ValueObject, DomainEvent, BusinessRule, Auditing, MultiTenancy | — |
| [MW.Application.Abstractions](MW.Application.Abstractions/) | Application-layer kontraktları — CQRS, Errors, Authorization, Caching, Validation, Pagination, Context, Time | MediatR, CSharpFunctionalExtensions, Dr.Pagination |
| [MW.Persistence.Abstractions](MW.Persistence.Abstractions/) | Persistence kontraktları — Repository, UnitOfWork, Specification, Transaction, Query interfeyslər | MW.Core |
| [MW.Persistence.EntityFrameworkCore](MW.Persistence.EntityFrameworkCore/) | EF Core implementasiyaları — bütün persistence abstraksiyalarının konkret reallaşdırılması | MW.Core, MW.Persistence.Abstractions, EF Core 8.0.4 |
| [MW.Persistence.DependencyInjection](MW.Persistence.DependencyInjection/) | Persistence DI composition — DbContext, repository, UnitOfWork, transaction qeydiyyatı | MW.Persistence.Abstractions, MW.Persistence.EntityFrameworkCore |
| [MW.Messaging.Abstractions](MW.Messaging.Abstractions/) | Mesajlaşma abstraksiyaları — Contracts, Correlation, Observability, MassTransit interfeyslər, Audit | — |
| [MW.Messaging.MassTransit](MW.Messaging.MassTransit/) | MassTransit implementasiyası — RabbitMQ transport, filters, observers, outbox, health checks | MW.Messaging.Abstractions, MassTransit 8.2.5 |
| [MW.Identity.Token](MW.Identity.Token/) | JWT identity idarəetməsi — ICurrentUser, ClaimsPrincipal extensions, ClaimConstants, SystemRole | Microsoft.AspNetCore.App, Newtonsoft.Json |

## 🏛️ Arxitektura

```
┌─────────────────────────────────────────────────────────────┐
│                   MW.Application.Abstractions                │
│         CQRS · Errors · Authorization · Caching              │
│         Validation · Pagination · Context · Time             │
├─────────────────────────────────────────────────────────────┤
│                          MW.Core                             │
│    Entities · AggregateRoots · ValueObjects · Events         │
│    Rules · Auditing · MultiTenancy · Exceptions              │
├──────────────────────────┬──────────────────────────────────┤
│  MW.Persistence.          │  MW.Persistence.                 │
│  Abstractions             │  EntityFrameworkCore             │
│  (kontraktlar)            │  (EF Core implementasiyalar)     │
├──────────────────────────┴──────────────────────────────────┤
│               MW.Persistence.DependencyInjection             │
│          (DI composition — DbContext, repos, UoW)            │
├──────────────────────────┬──────────────────────────────────┤
│  MW.Messaging.            │  MW.Messaging.                   │
│  Abstractions             │  MassTransit                     │
│  (kontraktlar)            │  (MassTransit implementasiya)    │
├──────────────────────────┴──────────────────────────────────┤
│                      MW.Identity.Token                        │
│   ICurrentUser · ClaimsPrincipal Extensions · ClaimConstants  │
│   SystemRole · DependencyInjection · JWT Claims               │
└─────────────────────────────────────────────────────────────┘
```

## 📋 Persistence Abstraksiya ↔ İmplementasiya Əhatəsi

Bütün 12 persistence abstraksiya tam implementasiya olunub:

| Abstraksiya | İmplementasiya | Status |
|-------------|----------------|--------|
| `IReadRepository<TEntity, TId>` | `EfReadRepository<TEntity, TId>` | ✅ |
| `IWriteRepository<TEntity, TId>` | `EfWriteRepository<TEntity, TId>` | ✅ |
| `IRepository<TEntity, TId>` | `EfRepository<TEntity, TId>` | ✅ |
| `IAggregateRepository<TAggregate, TId>` | `EfAggregateRepository<TAggregate, TId>` | ✅ |
| `IProjectionReadRepository<TEntity, TId>` | `EfProjectionReadRepository<TEntity, TId>` | ✅ |
| `IUnitOfWork` | `EfUnitOfWork` | ✅ |
| `ISpecification<TEntity>` | `BaseSpecification<TEntity>` | ✅ |
| `IQuerySpecification<TEntity, TResult>` | `QuerySpecification<TEntity, TResult>` | ✅ |
| `ITransactionManager` | `EfTransactionManager` | ✅ |
| `ITransactionScope` | `EfTransactionScope` | ✅ |
| `IQueryOptions` | `QueryOptions` | ✅ |
| `ISoftDeleteFilter` | `SoftDeleteFilter` | ✅ |

## 🚀 Quraşdırma

### NuGet Package Manager

```bash
Install-Package MW.Core
Install-Package MW.Application.Abstractions
Install-Package MW.Persistence.Abstractions
Install-Package MW.Persistence.EntityFrameworkCore
Install-Package MW.Persistence.DependencyInjection
Install-Package MW.Messaging.Abstractions
Install-Package MW.Messaging.MassTransit
Install-Package MW.Identity.Token
```

### .NET CLI

```bash
dotnet add package MW.Core
dotnet add package MW.Application.Abstractions
dotnet add package MW.Persistence.Abstractions
dotnet add package MW.Persistence.EntityFrameworkCore
dotnet add package MW.Persistence.DependencyInjection
dotnet add package MW.Messaging.Abstractions
dotnet add package MW.Messaging.MassTransit
dotnet add package MW.Identity.Token
```

## 🚀 İstifadə Nümunələri

### Entity və Aggregate Root

```csharp
using MW.Core.Entities;
using MW.Core.AggregateRoots;

public class Product : Entity<Guid>
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }

    public Product(string name, decimal price)
    {
        Id = Guid.NewGuid();
        Name = name;
        Price = price;
    }
}

public class Order : AggregateRoot<Guid>
{
    public DateTime OrderDate { get; private set; }
    public List<OrderItem> Items { get; private set; } = new();

    public void AddItem(Product product, int quantity)
    {
        Items.Add(new OrderItem(product, quantity));
        AddDomainEvent(new OrderItemAddedEvent(Id, product.Id));
    }
}
```

### CQRS — Command və Query

```csharp
using MW.Application.Abstractions.CQRS;

public record CreateProductCommand(string Name, decimal Price) : ICommand<Guid>;
public record GetProductByIdQuery(Guid Id) : IQuery<ProductDto>;
```

### Repository Pattern

```csharp
using MW.Persistence.Abstractions.Repositories;
using MW.Persistence.Abstractions.UnitOfWork;

public class ProductService
{
    private readonly IRepository<Product, Guid> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IRepository<Product, Guid> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> CreateAsync(string name, decimal price, CancellationToken ct)
    {
        var product = new Product(name, price);
        await _repository.AddAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return product.Id;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _repository.GetByIdAsync(id, ct);
    }
}
```

### Specification Pattern

```csharp
using MW.Persistence.EntityFrameworkCore.Specifications;

public class ExpensiveProductsSpec : BaseSpecification<Product>
{
    public ExpensiveProductsSpec(decimal minPrice)
        : base(p => p.Price >= minPrice)
    {
        ApplyOrderBy(p => p.Name);
        ApplyPaging(0, 50);
    }
}

// İstifadə
var spec = new ExpensiveProductsSpec(minPrice: 10);
var products = await _readRepository.FindAsync(spec, ct);
```

### Transaction Management

```csharp
using MW.Persistence.Abstractions.Transactions;

public class TransferService
{
    private readonly ITransactionManager _transactionManager;
    private readonly IUnitOfWork _unitOfWork;

    public async Task TransferAsync(Guid fromId, Guid toId, decimal amount, CancellationToken ct)
    {
        await using var scope = await _transactionManager.BeginTransactionAsync(ct);
        try
        {
            // ... əməliyyatlar ...
            await _unitOfWork.SaveChangesAsync(ct);
            await scope.CommitAsync(ct);
        }
        catch
        {
            await scope.RollbackAsync(ct);
            throw;
        }
    }
}
```

### Integration Events (Messaging)

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

### MassTransit Mesajlaşma Qeydiyyatı

```csharp
using MW.Messaging.MassTransit.Extensions;

builder.Services.AddMassTransitMessaging(options =>
{
    options.BindOptions(builder.Configuration);
    options.AddConsumersFromAssembly(typeof(Program).Assembly);

    // Opsional: Transactional outbox
    options.UseEntityFrameworkOutbox<AppDbContext>();
});
```

### JWT İstifadəçi İdentifikasiyası (Identity Token)

```csharp
using MW.Identity.Token.DependencyInjection;

// Program.cs — servislərin qeydiyyatı
builder.Services.AddUserTokenManager();
```

```csharp
using MW.Identity.Token.Contracts;
using MW.Identity.Token.Constants;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ICurrentUser _currentUser;

    public ProductsController(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (!_currentUser.IsAuthenticated)
            throw new UnauthorizedAccessException();

        if (!_currentUser.IsInRole(SystemRole.SuperAdmin))
            return Forbid();

        // ... məhsulu sil
        return NoContent();
    }
}
```

## 🏗️ Layihə Strukturu

```
MW.Extensions/
├── MW.Core/                              # Domain Layer
│   ├── Entities/                         # Entity base class və interfeyslər
│   ├── AggregateRoots/                   # Aggregate Root abstraksiyaları
│   ├── ValueObjects/                     # Value Object base class
│   ├── Events/                           # Domain Event infrastrukturu
│   ├── Rules/                            # Business Rule interfeysi
│   ├── Auditing/                         # Audit interfeyslər
│   ├── MultiTenancy/                     # Multi-tenant dəstəyi
│   ├── Exceptions/                       # Domain exception-lar
│   ├── Abstractions/                     # Ümumi abstraksiyalar
│   ├── Concretes/                        # Enumeration, StandardFilter
│   └── Models/                           # Model abstraksiyaları
│
├── MW.Application.Abstractions/          # Application Layer
│   ├── CQRS/                             # ICommand, IQuery
│   ├── Errors/                           # Tip əsaslı xəta modelləri
│   ├── Authorization/                    # Avtorizasiya markerləri
│   ├── Caching/                          # ICacheableQuery
│   ├── Validation/                       # IRequestValidator
│   ├── Behaviors/                        # ITransactionalRequest
│   ├── Pagination/                       # SortRequest, SourcePaged
│   ├── Context/                          # ICurrentUser, IRequestContext
│   ├── Time/                             # IClock
│   └── Constants/                        # ApplicationConstants
│
├── MW.Persistence.Abstractions/          # Persistence Contracts
│   ├── Repositories/                     # 5 repository interfeysi
│   ├── UnitOfWork/                       # IUnitOfWork
│   ├── Specifications/                   # ISpecification, IQuerySpecification
│   ├── Queries/                          # IQueryOptions, ISoftDeleteFilter
│   └── Transactions/                     # ITransactionManager, ITransactionScope
│
├── MW.Persistence.EntityFrameworkCore/   # EF Core Infrastructure
│   ├── Repositories/                     # 5 EF Core repository implementasiyası
│   ├── UnitOfWork/                       # EfUnitOfWork
│   ├── Specifications/                   # BaseSpecification, QuerySpecification
│   ├── Evaluators/                       # SpecificationEvaluator
│   ├── Transactions/                     # EfTransactionManager, EfTransactionScope
│   ├── Querying/                         # QueryOptions, SoftDeleteFilter
│   ├── Extensions/                       # Extension metodları
│   └── Internal/                         # Daxili köməkçilər
│
├── MW.Persistence.DependencyInjection/   # Persistence DI Composition
│   ├── Extensions/                       # AddEfCorePersistence extension metodu
│   ├── Options/                          # PersistenceOptions, DatabaseProvider
│   ├── Registrations/                    # Servis qeydiyyat məntiqi
│   ├── Providers/                        # Database provider konfiqurasiyası
│   └── Internal/                         # Daxili implementasiya detalları
│
├── MW.Messaging.Abstractions/            # Messaging Contracts
│   ├── Contracts/                        # IIntegrationEvent, IntegrationEvent
│   ├── Messaging/                        # EventMetadata, ServiceIdentity
│   ├── Correlation/                      # ICorrelationContext
│   ├── Observability/                    # MessageLogContext, ObservabilityFields
│   ├── MassTransit/                      # Observer interfeyslər, IMessageHeaderMapper
│   ├── Headers/                          # MessageHeaders sabitləri
│   ├── Context/                          # IMessageContextAccessor, IMessageExecutionContext
│   ├── Identity/                         # IServiceIdentityProvider
│   ├── Publishing/                       # IIntegrationEventPublisher
│   ├── Validation/                       # IIntegrationEventValidator, IEventNamingConvention
│   ├── Audit/                            # EventAuditRecord
│   ├── Constants/                        # EventDirections, EventStatuses
│   └── Docs/                             # Konvensiya sənədləri
│
├── MW.Messaging.MassTransit/             # MassTransit Infrastructure
│   ├── Context/                          # ScopedMessageContextAccessor, DefaultPublishContextProvider
│   ├── Extensions/                       # AddMassTransitMessaging extension metodu
│   ├── Filters/                          # HeaderEnrichmentPublishFilter, MessageContextConsumeFilter
│   ├── Health/                           # MassTransitBusHealthCheck
│   ├── Identity/                         # ConfigurationServiceIdentityProvider
│   ├── Naming/                           # ServiceEndpointNameFormatter
│   ├── Observers/                        # Publish, Consume, Send, Bus lifecycle observer-lər
│   ├── Options/                          # MassTransitOptions, RabbitMqOptions, RetryOptions
│   ├── Publishing/                       # MassTransitIntegrationEventPublisher
│   ├── Tracing/                          # MessagingActivitySource
│   └── Docs/                             # İnteqrasiya və arxitektura sənədləri
│
├── MW.Identity.Token/                    # JWT Identity Management
│   ├── Constants/                        # ClaimConstants, SystemRole sabitləri
│   ├── Contracts/                        # ICurrentUser interfeysi
│   ├── DependencyInjection/              # AddUserTokenManager() extension
│   ├── Extensions/                       # ClaimsPrincipal extension metodları
│   └── Services/                         # CurrentUser implementasiyası
│
└── tests/                                # Test Layihələri
    ├── MW.Persistence.Tests.Architecture/ # Arxitektura testləri
    ├── MW.Persistence.Tests.Unit/         # Persistence unit testləri
    ├── MW.Persistence.Tests.Integration/  # Persistence inteqrasiya testləri
    ├── MW.Persistence.Tests.Shared/       # Paylaşılan test infrastrukturu
    ├── MW.Identity.Token.Tests/           # Identity token testləri
    └── MW.Messaging.MassTransit.Tests/    # MassTransit mesajlaşma testləri
```

## 📋 Asılılıqlar

| Paket | Versiya | İstifadə Edən |
|-------|---------|---------------|
| Microsoft.EntityFrameworkCore | 8.0.4 | MW.Persistence.EntityFrameworkCore, MW.Application.Abstractions |
| Npgsql.EntityFrameworkCore.PostgreSQL | 8.0.4 | MW.Persistence.DependencyInjection |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.4 | MW.Persistence.DependencyInjection |
| MediatR | 12.1.1 | MW.Application.Abstractions |
| MediatR.Contracts | 2.0.1 | MW.Core |
| CSharpFunctionalExtensions | 3.7.0 | MW.Application.Abstractions |
| Dr.Pagination | 1.0.1 | MW.Application.Abstractions |
| MassTransit.RabbitMQ | 8.2.5 | MW.Messaging.MassTransit |
| MassTransit.EntityFrameworkCore | 8.2.5 | MW.Messaging.MassTransit |
| AspNetCore.HealthChecks.Rabbitmq | 8.0.0 | MW.Messaging.MassTransit |
| Newtonsoft.Json | 13.0.3 | MW.Identity.Token |
| Microsoft.AspNetCore.App | framework | MW.Identity.Token |

## 🔧 Build və Test

```bash
dotnet build MW.Extensions.sln
dotnet test MW.Extensions.sln
```

## 🔧 Tələblər

- .NET 8.0+
- C# 12+

## 📄 Lisenziya

Bu layihə MIT lisenziyası altında paylanır.