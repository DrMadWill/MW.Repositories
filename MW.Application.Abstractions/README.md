# MW.Application.Abstractions

**MW.Application.Abstractions** — Clean Architecture prinsiplərinə əsaslanan .NET 8.0 kitabxanasıdır. Bu kitabxana CQRS, xəta idarəetmə, avtorizasiya, keşləmə, validasiya, pagination və digər application-layer abstraksiyalarını təmin edir.

## 📦 Quraşdırma

### NuGet Package Manager

```bash
Install-Package MW.Application.Abstractions
```

### .NET CLI

```bash
dotnet add package MW.Application.Abstractions
```

### PackageReference

```xml
<PackageReference Include="MW.Application.Abstractions" Version="1.0.0" />
```

## 🏗️ Struktur

```
MW.Application.Abstractions/
├── Authorization/       # Avtorizasiya marker interfeyslər (rol, icazə)
├── Behaviors/           # Pipeline davranış markerləri (tranzaksiya)
├── Caching/             # Keşləmə abstraksiyası
├── Constants/           # Ümumi sabitlər (pagination, keş, xəta kodları)
├── Context/             # Request kontekst və istifadəçi məlumatları
├── CQRS/                # Command və Query interfeyslər (MediatR əsaslı)
├── Errors/              # Tip əsaslı xəta modelləri
├── Pagination/          # Pagination və sıralama
├── Time/                # Vaxt abstraksiyası
└── Validation/          # Request validasiya interfeysi
```

## 🚀 İstifadə

### CQRS — Command və Query

CQRS pattern MediatR və CSharpFunctionalExtensions əsasında qurulub. Command-lar dəyişiklik edir, Query-lər məlumat qaytarır:

#### Command (dəyər qaytarmayan)

```csharp
using MW.Application.Abstractions.CQRS;

public record CreateProductCommand(string Name, decimal Price) : ICommand;
```

#### Command (dəyər qaytaran)

```csharp
using MW.Application.Abstractions.CQRS;

public record CreateProductCommand(string Name, decimal Price) : ICommand<Guid>;
```

#### Query

```csharp
using MW.Application.Abstractions.CQRS;

public record GetProductByIdQuery(Guid Id) : IQuery<ProductDto>;
```

#### Handler nümunəsi

```csharp
using CSharpFunctionalExtensions;
using MediatR;
using MW.Application.Abstractions.CQRS;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        // Biznes məntiqi
        var id = Guid.NewGuid();
        return Result.Success(id);
    }
}

public class GetProductHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        // Məlumat oxuma məntiqi
        return Result.Success(new ProductDto());
    }
}
```

### Xəta İdarəetmə (Errors)

Tip əsaslı xəta modelləri müxtəlif xəta ssenariləri üçün nəzərdə tutulub:

```csharp
using MW.Application.Abstractions.Errors;

// Resurs tapılmadı
var notFound = new NotFoundError("Məhsul tapılmadı.");

// Validasiya xətası
var validation = new ValidationError("Ad sahəsi boş ola bilməz.");

// Biznes qaydası pozuntusu
var businessRule = new BusinessRuleError("Stokda kifayət qədər məhsul yoxdur.");

// Konflikt xətası
var conflict = new ConflictError("Bu adda məhsul artıq mövcuddur.");

// İcazəsiz giriş
var unauthorized = new UnauthorizedError("Giriş tələb olunur.");

// Qadağan edilmiş əməliyyat
var forbidden = new ForbiddenError("Bu əməliyyat üçün icazəniz yoxdur.");
```

Xüsusi xəta kodu ilə istifadə:

```csharp
var error = new NotFoundError("PRODUCT_NOT_FOUND", "Məhsul tapılmadı.");
Console.WriteLine(error.ToString()); // "PRODUCT_NOT_FOUND: Məhsul tapılmadı."
```

### Avtorizasiya (Authorization)

Pipeline davranışları ilə avtorizasiya qaydalarını tətbiq etmək üçün marker interfeyslər:

```csharp
using MW.Application.Abstractions.Authorization;
using MW.Application.Abstractions.CQRS;

// Sadə avtorizasiya tələbi
public record DeleteProductCommand(Guid Id) : ICommand, IAuthorizableRequest;

// Rol əsaslı avtorizasiya
public record ManageUsersQuery : IQuery<List<UserDto>>, IRoleProtectedRequest
{
    public IEnumerable<string> RequiredRoles => new[] { "Admin", "Manager" };
}

// İcazə əsaslı avtorizasiya
public record ExportDataCommand : ICommand, IPermissionProtectedRequest
{
    public IEnumerable<string> RequiredPermissions => new[] { "Data.Export", "Reports.View" };
}
```

### Kontekst (Context)

#### ICurrentUser — Cari İstifadəçi

Cari autentifikasiya olunmuş istifadəçi məlumatlarına giriş:

```csharp
using MW.Application.Abstractions.Context;

public class OrderService
{
    private readonly ICurrentUser _currentUser;

    public OrderService(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public void CreateOrder()
    {
        if (!_currentUser.IsAuthenticated)
            throw new UnauthorizedAccessException();

        var userId = _currentUser.UserId;
        var userName = _currentUser.UserName;
        var roles = _currentUser.Roles;
    }
}
```

#### IRequestContext — Request Kontekst

Distributed tracing, multi-tenancy və diagnostika üçün request-səviyyəli metadata:

```csharp
using MW.Application.Abstractions.Context;

public class AuditService
{
    private readonly IRequestContext _context;

    public AuditService(IRequestContext context)
    {
        _context = context;
    }

    public void LogAction(string action)
    {
        var correlationId = _context.CorrelationId;
        var requestId = _context.RequestId;
        var tenantId = _context.TenantId;
        var culture = _context.Culture;
        var clientIp = _context.ClientIp;
    }
}
```

### Pagination və Sıralama

#### SortRequest

Sıralama parametrlərini təyin etmək üçün:

```csharp
using MW.Application.Abstractions.Pagination;

var sort = new SortRequest
{
    SortBy = "Name",
    SortDirection = SortDirection.Descending
};
```

#### SourcePaged — Səhifələmə

`SourcePaged<T>` EF Core ilə asinxron səhifələmə təmin edir:

```csharp
using MW.Application.Abstractions.Pagination;
using Dr.Pagination;

// IQueryable-dan səhifələnmiş nəticə
var req = new PageReq { Page = 1, PerPage = 20 };
var pagedResult = await SourcePaged<Product>.PagedAsync(queryableSource, req);

// Extension metod ilə
var pagedResult = await queryableSource.ToPagedAsync(req);

// Nəticə üzərində transformasiya ilə
var pagedResult = await queryableSource.ToPagedAsync(req, products =>
    products.Select(p => new ProductDto(p.Name, p.Price)).ToList()
);
```

### Keşləmə (Caching)

`ICacheableQuery` interfeysi pipeline əsaslı keşləmə strategiyaları üçün:

```csharp
using MW.Application.Abstractions.Caching;
using MW.Application.Abstractions.CQRS;

public record GetProductByIdQuery(Guid Id) : IQuery<ProductDto>, ICacheableQuery
{
    public string CacheKey => $"product:{Id}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
}

// Default keş müddəti ilə (null = standart siyasət)
public record GetCategoriesQuery : IQuery<List<CategoryDto>>, ICacheableQuery
{
    public string CacheKey => "categories:all";
    public TimeSpan? Expiration => null;
}
```

### Validasiya (Validation)

`IRequestValidator<T>` interfeysi request-lərin icrasından əvvəl validasiya etmək üçün:

```csharp
using CSharpFunctionalExtensions;
using MW.Application.Abstractions.Validation;

public class CreateProductValidator : IRequestValidator<CreateProductCommand>
{
    public Task<Result> ValidateAsync(CreateProductCommand request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Task.FromResult(Result.Failure("Məhsul adı boş ola bilməz."));

        if (request.Price <= 0)
            return Task.FromResult(Result.Failure("Qiymət sıfırdan böyük olmalıdır."));

        return Task.FromResult(Result.Success());
    }
}
```

### Davranışlar (Behaviors)

`ITransactionalRequest` markeri pipeline davranışlarında tranzaksiya idarəetməsi üçün:

```csharp
using MW.Application.Abstractions.Behaviors;
using MW.Application.Abstractions.CQRS;

// Bu command avtomatik olaraq tranzaksiya daxilində icra olunacaq
public record TransferMoneyCommand(Guid FromAccount, Guid ToAccount, decimal Amount)
    : ICommand, ITransactionalRequest;
```

### Vaxt Abstraksiyası (Time)

`IClock` interfeysi vaxt asılılığını abstrakt edir və test edilə bilən kod yazmağa imkan verir:

```csharp
using MW.Application.Abstractions.Time;

public class OrderService
{
    private readonly IClock _clock;

    public OrderService(IClock clock)
    {
        _clock = clock;
    }

    public void ProcessOrder()
    {
        var now = _clock.Now;
        // DateTimeOffset.UtcNow əvəzinə IClock istifadə edin
    }
}
```

### Sabitlər (Constants)

`ApplicationConstants` ümumi dəyərləri mərkəzləşdirir:

```csharp
using MW.Application.Abstractions.Constants;

// Pagination
int defaultPage = ApplicationConstants.Pagination.DefaultPage;         // 1
int defaultSize = ApplicationConstants.Pagination.DefaultPageSize;     // 20
int maxSize = ApplicationConstants.Pagination.MaxPageSize;             // 100

// Keş
int defaultExpiry = ApplicationConstants.Cache.DefaultExpirationMinutes;   // 5
int extendedExpiry = ApplicationConstants.Cache.ExtendedExpirationMinutes; // 30

// Xəta kodları
string notFound = ApplicationConstants.ErrorCodes.NotFound;         // "NotFound"
string validation = ApplicationConstants.ErrorCodes.Validation;     // "Validation"
string conflict = ApplicationConstants.ErrorCodes.Conflict;         // "Conflict"
string unauthorized = ApplicationConstants.ErrorCodes.Unauthorized; // "Unauthorized"
string forbidden = ApplicationConstants.ErrorCodes.Forbidden;       // "Forbidden"
string businessRule = ApplicationConstants.ErrorCodes.BusinessRule; // "BusinessRule"
```

## 📋 Interface-lər

| Interface | Təsvir |
|-----------|--------|
| `ICommand` | Dəyər qaytarmayan command (Result qaytarır) |
| `ICommand<TResult>` | Dəyər qaytaran command (Result\<TResult\> qaytarır) |
| `IQuery<TResult>` | Məlumat oxuyan query (Result\<TResult\> qaytarır) |
| `IAuthorizableRequest` | Avtorizasiya tələb edən request markeri |
| `IRoleProtectedRequest` | Rol əsaslı avtorizasiya markeri |
| `IPermissionProtectedRequest` | İcazə əsaslı avtorizasiya markeri |
| `ITransactionalRequest` | Tranzaksiya daxilində icra markeri |
| `ICacheableQuery` | Keşlənə bilən query interfeysi |
| `ICurrentUser` | Cari istifadəçi məlumatları |
| `IRequestContext` | Request kontekst metadata |
| `IClock` | Vaxt abstraksiyası |
| `IRequestValidator<T>` | Request validasiya interfeysi |

## 📋 Xəta Modelləri

| Model | Xəta Kodu | Təsvir |
|-------|-----------|--------|
| `Error` | (xüsusi) | Baza xəta tipi |
| `NotFoundError` | `NotFound` | Resurs tapılmadı |
| `ValidationError` | `Validation` | Validasiya xətası |
| `BusinessRuleError` | `BusinessRule` | Biznes qaydası pozuntusu |
| `ConflictError` | `Conflict` | Resurs konflikti |
| `UnauthorizedError` | `Unauthorized` | Autentifikasiya tələb olunur |
| `ForbiddenError` | `Forbidden` | İcazə yoxdur |

## 📦 Asılılıqlar

| Paket | Versiya | Təsvir |
|-------|---------|--------|
| [MediatR](https://www.nuget.org/packages/MediatR) | 12.1.1 | CQRS mediator pattern |
| [CSharpFunctionalExtensions](https://www.nuget.org/packages/CSharpFunctionalExtensions) | 3.7.0 | Railway-oriented programming (Result\<T\>) |
| [Dr.Pagination](https://www.nuget.org/packages/Dr.Pagination) | 1.0.1 | Pagination utiliti |
| [Microsoft.EntityFrameworkCore](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore) | 8.0.4 | EF Core (SourcePaged üçün) |

## 🔧 Tələblər

- .NET 8.0+
- C# 12+

## 📄 Lisenziya

Bu layihə MIT lisenziyası altında paylanır.
