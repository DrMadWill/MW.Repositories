# MW.Core

**MW.Core** — Domain-Driven Design (DDD) prinsiplərinə əsaslanan .NET 8.0 kitabxanasıdır. Bu kitabxana entity, value object, aggregate root, domain event və business rule kimi əsas DDD konseptlərini təmin edir.

## 📦 Quraşdırma

### NuGet Package Manager

```bash
Install-Package MW.Core
```

### .NET CLI

```bash
dotnet add package MW.Core
```

### PackageReference

```xml
<PackageReference Include="MW.Core" Version="1.0.0" />
```

## 🏗️ Struktur

```
MW.Core/
├── Abstractions/        # Base interfeyslər (IBaseDto, ILang, IHasConcurrencyToken)
├── AggregateRoots/      # Aggregate Root base class və interfeys
├── Auditing/            # Audit interfeyslər (yaradılma, yenilənmə, silinmə)
├── Concretes/           # Concrete implementasiyalar (Enumeration, StandardFilter)
├── Entities/            # Entity base class və interfeyslər
├── Events/              # Domain Event infrastrukturu
├── Exceptions/          # Business Rule exception
├── Models/              # Model abstraksiyaları
├── MultiTenancy/        # Multi-tenant dəstəyi
├── Rules/               # Business Rule interfeysi
└── ValueObjects/        # Value Object base class
```

## 🚀 İstifadə

### Entity

Entity base class identifikator, bərabərlik və domain event dəstəyi təmin edir:

```csharp
public class Product : Entity<Guid>
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    
    public Product(string name, decimal price)
    {
        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        
        // Domain event əlavə et
        AddDomainEvent(new ProductCreatedEvent(Id, name));
    }
}
```

### Aggregate Root

Aggregate Root, entity-nin genişləndirilmiş versiyasıdır:

```csharp
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

### Value Object

Value Object-lər dəyərləri ilə müqayisə olunur:

```csharp
public class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }
    
    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
```

### Domain Events

Domain event-lər entity daxilində baş verən hadisələri izləyir:

```csharp
public class ProductCreatedEvent : DomainEventBase
{
    public Guid ProductId { get; }
    public string ProductName { get; }
    
    public ProductCreatedEvent(Guid productId, string productName)
    {
        ProductId = productId;
        ProductName = productName;
    }
}
```

### Business Rules

Business rule-lar domain qaydalarını təmin edir:

```csharp
public class ProductNameMustBeUniqueRule : IBusinessRule
{
    private readonly string _name;
    private readonly IProductRepository _repository;
    
    public string Message => "Məhsul adı unikal olmalıdır.";
    
    public ProductNameMustBeUniqueRule(string name, IProductRepository repository)
    {
        _name = name;
        _repository = repository;
    }
    
    public bool IsBroken() => _repository.ExistsByName(_name);
}

// İstifadə
public void CheckRule(IBusinessRule rule)
{
    if (rule.IsBroken())
        throw new BusinessRuleValidationException(rule);
}
```

### Auditing

Audit interfeyslər entity-lərin yaradılma, yenilənmə və silinmə tarixçəsini izləyir:

```csharp
public class AuditableProduct : Entity<Guid>, IAuditableEntity, ISoftDelete
{
    public string Name { get; set; }
    
    // ICreationAudited
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    
    // IUpdateAudited
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    // ISoftDelete
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
```

### Multi-Tenancy

Çox kiracılı sistemlər üçün `IHasTenant` interfeysini istifadə edin:

```csharp
public class TenantProduct : Entity<Guid>, IHasTenant
{
    public string Name { get; set; }
    public Guid TenantId { get; set; }
}
```

### Enumeration Pattern

Type-safe enum pattern:

```csharp
public class OrderStatus : Enumeration
{
    public static readonly OrderStatus Pending = new(1, "Gözləyir");
    public static readonly OrderStatus Processing = new(2, "İşlənir");
    public static readonly OrderStatus Completed = new(3, "Tamamlandı");
    public static readonly OrderStatus Cancelled = new(4, "Ləğv edildi");
    
    private OrderStatus(int id, string name) : base(id, name) { }
}

// İstifadə
var allStatuses = Enumeration.GetAll<OrderStatus>();
var status = Enumeration.FromId<OrderStatus>(1);
```

### Filtering

StandardFilter ilə filterleme:

```csharp
public class ProductFilter : StandardFilter<Product>
{
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    
    public override IQueryable<Product> ApplyFilter(IQueryable<Product> source)
    {
        if (MinPrice.HasValue)
            source = source.Where(x => x.Price >= MinPrice);
            
        if (MaxPrice.HasValue)
            source = source.Where(x => x.Price <= MaxPrice);
            
        return source;
    }
}

// Extension method istifadəsi:
IQueryable<Product> query = dbContext.Products.AsQueryable();
var filter = new ProductFilter { MinPrice = 10, MaxPrice = 100 };
query = query.FilterBy(filter);
```

## 📋 Interface-lər

| Interface | Təsvir |
|-----------|--------|
| `IEntity<TId>` | Entity identifikatoru |
| `IAggregateRoot<TId>` | Aggregate root marker |
| `IHasDomainEvents` | Domain event dəstəyi |
| `IDomainEvent` | Domain event interfeysi |
| `IBusinessRule` | Business rule interfeysi |
| `IAuditableEntity` | Yaradılma və yenilənmə audit |
| `ISoftDelete` | Soft delete dəstəyi |
| `IHasTenant` | Multi-tenancy dəstəyi |
| `IBaseDto<T>` | DTO base interfeysi |
| `IHasConcurrencyToken` | Concurrency control |

## 🔧 Tələblər

- .NET 8.0+
- C# 12+

## 📄 Lisenziya

Bu layihə MIT lisenziyası altında paylanır.

