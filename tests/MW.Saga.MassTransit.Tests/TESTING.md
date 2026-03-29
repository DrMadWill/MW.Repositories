# MW.Saga.MassTransit — Testing Guide

This document describes the test structure and conventions for `MW.Saga.MassTransit.Tests`.

## Test Categories

### Pure Unit Tests

Tests that validate a single class in isolation using mocks for all dependencies.

**Location:** Folders matching the production code layout (`Context/`, `Filters/`, `Observers/`, `Naming/`, `Correlation/`, `Tracing/`)

**Examples:**
- `ScopedSagaContextAccessorTests` — set/clear/null behavior
- `MutableSagaContextTests` — property assignment and defaults
- `MassTransitSagaExecutionContextTests` — execution context metadata mapping
- `SagaObserverAdapterTests` — lifecycle event forwarding with context verification
- `StructuredSagaLoggerTests` — structured observability and no-throw behavior

**Guidelines:**
- Use `Moq` for all external dependencies
- Use `FluentAssertions` for readable assertions
- One concern per test method
- Name tests: `Method_Should_ExpectedBehavior` or `Method_Should_ExpectedBehavior_WhenCondition`

### DI Registration Tests

Tests that verify service registration wiring without starting the MassTransit bus.

**Location:** `Extensions/`

**Examples:**
- `SagaMassTransitServiceCollectionExtensionsTests` — verifies required saga service registrations (context accessor, execution context, observer)

**Guidelines:**
- Use `ServiceCollection` directly (not a full host)
- Verify service descriptors (type, lifetime) rather than resolving services
- Keep tests focused on wiring, not business behavior

### Configuration and Options Tests

Tests for options default values and configuration binding.

**Location:** `Options/`

**Examples:**
- `SagaMassTransitOptionsTests` — default values, configuration binding from `IConfiguration`

### Filter and Context Tests

Tests that validate saga pipeline filter behavior, context population, and cleanup.

**Location:** `Filters/`

**Examples:**
- `SagaContextPopulationFilterTests` — context population and cleanup during saga execution
- `SagaFaultFilterTests` — fault-path invocation and rethrow behavior
- `SagaRetryPolicyConfiguratorTests` — retry policy application

**Guidelines:**
- Mock `SagaConsumeContext<TSaga, TMessage>` from MassTransit
- Verify context values via captured references or mock callbacks
- Test both success and failure paths (especially context cleanup)

### Correlation Tests

Tests for saga correlation bridge and resolver integration.

**Location:** `Correlation/`

**Examples:**
- `SagaCorrelationBridgeTests` — bridges abstraction-layer correlation into MassTransit

### Observer and Logging Tests

Tests for saga lifecycle observability.

**Location:** `Observers/`

**Examples:**
- `SagaObserverAdapterTests` — lifecycle metadata forwarding (started, transitioned, completed, failed, timed out)
- `StructuredSagaLoggerTests` — structured log output behavior

### Timeout and Scheduler Tests

Tests for scheduler configuration and timeout conventions.

**Location:** `Scheduling/`

**Examples:**
- `SagaSchedulerConfiguratorTests` — scheduler setup behavior
- `SagaTimeoutConventionsTests` — timeout naming and duration conventions

### Convention and Definition Tests

Tests for shared saga definition/convention behavior.

**Location:** `Conventions/`

**Examples:**
- `StandardSagaDefinitionTests` — definition-level configuration and type validation

### Persistence Tests

Tests for EF Core persistence helpers and saga state mapping.

**Location:** `Persistence/`

**Examples:**
- `SagaStateBaseMapTests` — common saga field mapping (CorrelationId, CurrentState, timestamps, concurrency)
- `SagaEfCorePersistenceHelperTests` — persistence registration null guards

### Tracing Tests

Tests for distributed tracing infrastructure.

**Location:** `Tracing/`

**Examples:**
- `SagaActivitySourceTests` — activity source creation and metadata

### Harness-Based Integration Tests

End-to-end tests using MassTransit's in-memory test harness.

**Location:** `Integration/`

**Examples:**
- `EndToEndSagaHarnessTests` — full saga flow with state transitions, completion, and correlation
- `SagaContextCleanupRegressionTests` — regression tests for context cleanup on failure paths

**Guidelines:**
- Use `MassTransit.Testing.ITestHarness` with in-memory transport
- Register state machines and saga infrastructure as they would be in production
- Keep test sagas generic and infrastructure-focused
- Use `IAsyncLifetime` for setup/teardown

### Test Harness Helper Tests

Tests for the saga test harness helper utility.

**Location:** `Testing/`

**Examples:**
- `SagaTestHarnessHelperTests` — harness setup, configuration callback, and registration

## Adding New Tests

1. Identify the category (unit, registration, filter, observer, integration)
2. Place the file in the matching folder
3. Follow the naming convention: `{ClassName}Tests.cs`
4. Use existing test files as reference for mock setup patterns
5. Run tests with: `dotnet test tests/MW.Saga.MassTransit.Tests`

## Test Dependencies

| Package | Purpose |
|---------|---------|
| xUnit | Test framework |
| FluentAssertions | Readable assertions |
| Moq | Mocking |
| MassTransit.TestFramework | In-memory test harness |
| Microsoft.EntityFrameworkCore.InMemory | EF Core persistence tests |
| Microsoft.Extensions.Configuration | Configuration binding tests |
| Microsoft.Extensions.DependencyInjection | DI registration tests |
| Microsoft.Extensions.Logging | Logging tests |
