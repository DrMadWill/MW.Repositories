# MW.Persistence.Abstractions

Persistence-related contracts used by application and infrastructure layers.

## Purpose

This project separates repository and persistence abstractions from concrete implementation (e.g., EF Core).

## Architecture Rules

- This project contains **contracts only** — no implementation logic
- **No EF Core** references (`Microsoft.EntityFrameworkCore`)
- **No DbContext** types
- **No DI registration** logic
- **No reflection-based** service resolution
- **No manual Dispose** patterns (except `IUnitOfWork : IDisposable` contract)
- **No provider-specific** libraries (SQL Server, PostgreSQL, etc.)

## Allowed Dependencies

- `MW.Core`
- Base .NET libraries

## Structure

```
MW.Persistence.Abstractions/
├── Repositories/
│   ├── IReadRepository.cs          # Read-only repository
│   ├── IWriteRepository.cs         # Write-only repository
│   ├── IRepository.cs              # Combined read + write
│   ├── IAggregateRepository.cs     # DDD aggregate root repository
│   └── IProjectionReadRepository.cs # Projection-based queries
├── UnitOfWork/
│   └── IUnitOfWork.cs              # Commit coordination
├── Specifications/
│   ├── ISpecification.cs           # Reusable query specification
│   └── IQuerySpecification.cs      # Query-side specification with projection
├── Queries/
│   ├── IQueryOptions.cs            # Non-business query behaviors
│   └── ISoftDeleteFilter.cs        # Soft-delete aware querying
└── Transactions/
    ├── ITransactionManager.cs      # Transaction lifecycle
    └── ITransactionScope.cs        # Transaction commit/rollback
```

## Design Decisions

### IQueryable Exposure Policy

Repository abstractions **do not expose** `IQueryable<T>`. All query logic is expressed through methods, predicates, and specifications. This prevents ORM leakage into application layers.

### Generic Constraints

- Repository contracts use `where TEntity : class, IEntity<TId>`
- Aggregate repository uses `where TAggregate : class, IAggregateRoot<TId>`
- Specification contracts use `where TEntity : class`

### Async-First

All persistence operations are async-first. Synchronous variants are intentionally excluded. All async methods accept `CancellationToken cancellationToken = default`.

### Naming Standard

| Contract | Purpose |
|---|---|
| `IReadRepository<TEntity, TId>` | Read-only queries |
| `IWriteRepository<TEntity, TId>` | Mutation operations |
| `IRepository<TEntity, TId>` | Combined read + write |
| `IAggregateRepository<TAggregate, TId>` | DDD aggregate repository |
| `IUnitOfWork` | Commit coordination |
| `ISpecification<TEntity>` | Reusable query criteria |
