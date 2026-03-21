# MW.Persistence.DependencyInjection

Composition layer for persistence service registration using dependency injection.

## Purpose

`MW.Persistence.DependencyInjection` is the centralized DI composition layer that wires together all persistence-related services. It provides a single entry point for microservices to register EF Core persistence infrastructure.

## What Belongs Here

- DbContext registration and provider configuration
- Repository abstraction → EF Core implementation registrations
- UnitOfWork registration
- Transaction manager registration
- Specification evaluator registration (if non-static)
- EF Core interceptor registration hooks
- Health check registration for database connectivity
- Provider-specific configuration (SQL Server, PostgreSQL)

## What Does NOT Belong Here

- Business logic
- Controller logic
- Application service logic
- Direct domain logic
- Custom repository implementations (those belong in the consuming project)

## Relationship to Other Projects

| Project | Relationship |
|---|---|
| `MW.Persistence.Abstractions` | Provides the interfaces registered here |
| `MW.Persistence.EntityFrameworkCore` | Provides the implementations registered here |
| `MW.Core` | Provides entity/aggregate root contracts (transitive dependency) |

## Usage

```csharp
using MW.Persistence.DependencyInjection.Extensions;
using MW.Persistence.DependencyInjection.Options;

services.AddEfCorePersistence<AppDbContext>(options =>
{
    options.ConnectionString = configuration.GetConnectionString("Default")!;
    options.Provider = DatabaseProvider.PostgreSql;
    options.MigrationAssembly = "MyApp.Infrastructure";
    options.EnableSensitiveDataLogging = builder.Environment.IsDevelopment();
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.EnableHealthCheck = true;
});
```

## Registered Services

All services are registered with **scoped** lifetime to align with DbContext lifecycle.

| Abstraction | Implementation | Lifetime |
|---|---|---|
| `DbContext` | `TDbContext` | Scoped |
| `IReadRepository<TEntity, TId>` | `EfReadRepository<TEntity, TId>` | Scoped |
| `IWriteRepository<TEntity, TId>` | `EfWriteRepository<TEntity, TId>` | Scoped |
| `IRepository<TEntity, TId>` | `EfRepository<TEntity, TId>` | Scoped |
| `IAggregateRepository<TAggregate, TId>` | `EfAggregateRepository<TAggregate, TId>` | Scoped |
| `IProjectionReadRepository<TEntity, TId>` | `EfProjectionReadRepository<TEntity, TId>` | Scoped |
| `IUnitOfWork` | `EfUnitOfWork` | Scoped |
| `ITransactionManager` | `EfTransactionManager` | Scoped |

## Supported Providers

| Provider | Enum Value |
|---|---|
| SQL Server | `DatabaseProvider.SqlServer` |
| PostgreSQL | `DatabaseProvider.PostgreSql` |

## EF Core Interceptors

Interceptors can be registered through `PersistenceOptions`:

```csharp
services.AddEfCorePersistence<AppDbContext>(options =>
{
    options.ConnectionString = connectionString;
    options.AddInterceptor(new AuditSaveChangesInterceptor());
    options.AddInterceptor(new SoftDeleteInterceptor());
});
```

## Health Checks

Optional database health check registration:

```csharp
services.AddEfCorePersistence<AppDbContext>(options =>
{
    options.ConnectionString = connectionString;
    options.EnableHealthCheck = true;
    options.HealthCheckName = "database";
});
```

## Lifetime Policy

| Service Type | Lifetime | Reason |
|---|---|---|
| DbContext | Scoped | EF Core standard; one context per request |
| Repositories | Scoped | Depend on scoped DbContext |
| UnitOfWork | Scoped | Must share DbContext scope with repositories |
| Transaction Manager | Scoped | Must share DbContext scope |

> ⚠️ No Singleton repository or UnitOfWork registrations exist. This prevents lifetime mismatches.

## Design Principles

- **Explicit registrations** — no reflection-based or name-convention-based service resolution
- **Scoped lifetimes** — all persistence services align with DbContext lifecycle
- **Composition only** — this project contains no business logic
- **Provider agnostic** — supports SQL Server and PostgreSQL through configuration

## Folder Structure

```
MW.Persistence.DependencyInjection
├── Extensions/        — IServiceCollection extension methods
├── Options/           — Configuration models (PersistenceOptions, DatabaseProvider)
├── Registrations/     — Explicit service registration logic
├── Providers/         — Database provider configuration
└── Internal/          — Reserved for internal implementation details
```
