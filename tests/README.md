# MW.Persistence Tests

Comprehensive test suite for the MW.Persistence layer, validating contracts, EF Core implementation, dependency injection, and runtime behavior.

## Test Project Structure

```
tests/
├── MW.Persistence.Tests.Shared/           # Shared test utilities
│   ├── Entities/                           # Test entities (TestEntity, SoftDeletableEntity, TestAggregate, etc.)
│   ├── Specifications/                     # Test specifications for repository queries
│   ├── Builders/                           # Test data builders with fluent API
│   └── Infrastructure/                     # TestDbContext, TestDbContextFactory (SQLite in-memory)
│
├── MW.Persistence.Tests.Unit/             # Unit tests
│   ├── Repositories/                       # EfReadRepository, EfWriteRepository, EfRepository, etc.
│   ├── UnitOfWork/                         # EfUnitOfWork behavior
│   ├── Specifications/                     # SpecificationEvaluator, BaseSpecification
│   ├── Querying/                           # QueryOptions, SoftDeleteFilter
│   ├── Transactions/                       # EfTransactionManager
│   └── DomainEvents/                       # Domain event collection
│
├── MW.Persistence.Tests.Integration/      # Integration tests (SQLite in-memory)
│   ├── Repositories/                       # Read, Write, SoftDelete, Concurrency
│   ├── UnitOfWork/                         # UnitOfWork commit behavior
│   ├── Specifications/                     # Specification queries against real DB
│   ├── Querying/                           # NoTracking policy validation
│   ├── Transactions/                       # Commit, Rollback, Auto-rollback
│   ├── DependencyInjection/                # Service registration, lifetimes, providers
│   └── Performance/                        # Performance smoke tests
│
└── MW.Persistence.Tests.Architecture/     # Architecture validation tests
    ├── ContractShape/                      # Interface shapes, async contracts, generic constraints
    ├── NamingConventions/                  # Naming rules (I-prefix, Ef-prefix, suffixes)
    └── ForbiddenPatterns/                  # Dependency rules, virtual methods, no SaveChanges in repos
```

## Running Tests

### Run all tests
```bash
dotnet test MW.Repositories.sln
```

### Run specific test project
```bash
dotnet test tests/MW.Persistence.Tests.Unit
dotnet test tests/MW.Persistence.Tests.Integration
dotnet test tests/MW.Persistence.Tests.Architecture
```

### Run with verbose output
```bash
dotnet test MW.Repositories.sln --verbosity normal
```

### Run with coverage
```bash
dotnet test MW.Repositories.sln --collect:"XPlat Code Coverage"
```

## Test Categories

### Architecture Tests
Validate structural rules without running any persistence operations:
- **Contract Shape**: All interfaces declare expected methods and properties
- **Async-First**: All async methods return Task/Task<T> and accept CancellationToken
- **Generic Constraints**: TEntity constrained to `class, IEntity<TId>`, TAggregate to `IAggregateRoot<TId>`
- **Naming Conventions**: I-prefix for interfaces, Ef-prefix for implementations
- **Forbidden Patterns**: Abstractions don't reference EF Core, repos don't call SaveChanges

### Unit Tests
Test individual component behavior in isolation:
- **Read Repository**: GetByIdAsync, GetAllAsync, FindAsync, ExistsAsync, CountAsync
- **Write Repository**: AddAsync, AddRangeAsync, Update, Remove, RemoveRange
- **Combined Repository**: Read + Write operations through single dependency
- **Aggregate Repository**: IAggregateRoot constraint validation
- **Projection Repository**: ProjectAsync, ProjectByIdAsync with selectors
- **UnitOfWork**: SaveChangesAsync, Dispose, idempotent disposal
- **Specifications**: Criteria evaluation, ordering, paging, IsSatisfiedBy
- **Query Options**: Default values, Tracked mode, SoftDeleteFilter presets
- **Transactions**: BeginTransaction, Commit, Rollback
- **Domain Events**: Add, Remove, Clear events on entities and aggregates
- **Concurrency**: ConcurrencyToken prevents stale updates
- **Exception Handling**: ArgumentNullException on null DbContext, ObjectDisposedException

### Integration Tests
Test real EF Core behavior using SQLite in-memory database:
- **Read/Write**: Full CRUD operations persisted to database
- **UnitOfWork**: Multi-repository atomic commits
- **Specifications**: Query specification evaluation against SQL
- **Soft Delete**: Query filter excludes deleted, IgnoreQueryFilters includes
- **Transactions**: Commit persists, Rollback discards, dispose without commit rolls back
- **Concurrency**: DbUpdateConcurrencyException on conflicting updates
- **DI Registration**: All 7+ services registered as Scoped, correct implementations
- **Provider Config**: SqlServer/PostgreSql options, interceptors, health checks
- **Performance**: Smoke tests for bulk operations within time bounds

## Test Infrastructure

### TestDbContext
SQLite in-memory database with entities:
- `TestEntity` — Simple entity with Name, Value, CreatedAt
- `SoftDeletableEntity` — Entity with ISoftDelete, global query filter
- `TestAggregate` — AggregateRoot with Title, Description
- `ConcurrentEntity` — Entity with IHasConcurrencyToken

### TestDbContextFactory
Creates isolated in-memory databases per test class using a shared SQLite connection.

### Test Data Builders
Fluent builders for all test entities:
```csharp
var entity = TestEntityBuilder.Default()
    .WithName("Sample")
    .WithValue(42)
    .Build();
```

## Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| xunit | 2.5.3 | Test framework |
| FluentAssertions | 6.12.2 | Assertion library |
| Moq | 4.20.72 | Mocking framework |
| Microsoft.EntityFrameworkCore.Sqlite | 8.0.4 | In-memory test database |
| Microsoft.NET.Test.Sdk | 17.8.0 | Test SDK |
| coverlet.collector | 6.0.0 | Code coverage |
