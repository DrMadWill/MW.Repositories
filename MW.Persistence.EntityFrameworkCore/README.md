# MW.Persistence.EntityFrameworkCore

Concrete EF Core implementations of the persistence abstractions defined in `MW.Persistence.Abstractions`.

## Purpose

This project provides production-ready Entity Framework Core implementations for all repository, unit of work, specification, transaction, and query contracts. It bridges domain-layer abstractions with EF Core infrastructure.

## Dependencies

- `MW.Core` — Domain entities, aggregate roots, value objects
- `MW.Persistence.Abstractions` — Persistence contracts (interfaces only)
- `Microsoft.EntityFrameworkCore` 8.0.4

## Structure

```
MW.Persistence.EntityFrameworkCore/
├── Repositories/
│   ├── EfReadRepository.cs              # IReadRepository — read-only queries
│   ├── EfWriteRepository.cs             # IWriteRepository — mutation operations
│   ├── EfRepository.cs                  # IRepository — combined read + write
│   ├── EfAggregateRepository.cs         # IAggregateRepository — DDD aggregate root
│   └── EfProjectionReadRepository.cs    # IProjectionReadRepository — projection queries
├── UnitOfWork/
│   └── EfUnitOfWork.cs                  # IUnitOfWork — SaveChangesAsync coordination
├── Specifications/
│   ├── BaseSpecification.cs             # ISpecification — reusable query criteria
│   └── QuerySpecification.cs            # IQuerySpecification — specification with projection
├── Evaluators/
│   └── SpecificationEvaluator.cs        # Applies specifications to IQueryable
├── Transactions/
│   ├── EfTransactionManager.cs          # ITransactionManager — begins transactions
│   └── EfTransactionScope.cs            # ITransactionScope — commit/rollback
├── Querying/
│   ├── QueryOptions.cs                  # IQueryOptions — tracking, filters, soft-delete
│   └── SoftDeleteFilter.cs              # ISoftDeleteFilter — soft-delete aware filtering
├── Extensions/                          # Reserved for query/DI extension methods
└── Internal/                            # Reserved for internal helpers
```

## Abstraction Coverage

| Abstraction | Implementation | Status |
|---|---|---|
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

## Design Decisions

### Repository Inheritance

```
EfReadRepository<TEntity, TId>          ← IReadRepository
  └── EfRepository<TEntity, TId>        ← IRepository (read + write)
        └── EfAggregateRepository<T, TId> ← IAggregateRepository (DDD)

EfWriteRepository<TEntity, TId>         ← IWriteRepository (standalone)
EfProjectionReadRepository<TEntity, TId> ← IProjectionReadRepository (standalone)
```

### No-Tracking by Default

All read operations use `AsNoTracking()` to optimize query performance. Write operations use tracked entities via `DbSet` mutations.

### Specification Pattern

`BaseSpecification<TEntity>` provides a builder-style API for composable queries:
- `Criteria` — filter expression
- `ApplyOrderBy` / `ApplyOrderByDescending` — sorting
- `ApplyPaging` — skip/take pagination
- `IsSatisfiedBy` — in-memory evaluation

`SpecificationEvaluator` applies specifications to `IQueryable<T>` sources.

### Unit of Work

`EfUnitOfWork` wraps `DbContext.SaveChangesAsync()`. Repository methods do not auto-save — all commits go through `IUnitOfWork`.

### Transaction Management

`EfTransactionManager` provides explicit transaction control:
```csharp
await using var scope = await transactionManager.BeginTransactionAsync();
// ... perform operations ...
await scope.CommitAsync();
```

### Query Options

`QueryOptions` and `SoftDeleteFilter` provide intention-based query configuration with convenient static factory properties (`Default`, `ReadOnly`, `Tracked`, `WithDeleted`, `DeletedOnly`).

### Extension Points

All public methods are `virtual` for override in derived classes. Constructor-injected `DbContext` enables testability.

