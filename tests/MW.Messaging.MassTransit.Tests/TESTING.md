# MW.Messaging.MassTransit — Testing Guide

This document describes the test structure and conventions for `MW.Messaging.MassTransit.Tests`.

## Test Categories

### Pure Unit Tests

Tests that validate a single class in isolation using mocks for all dependencies.

**Location:** Folders matching the production code layout (`Context/`, `Filters/`, `Publishing/`, `Identity/`, `Naming/`, `Observers/`)

**Examples:**
- `DefaultMessageHeaderMapperTests` — header mapping in both directions
- `ScopedMessageContextAccessorTests` — set/clear/null behavior
- `MassTransitIntegrationEventPublisherTests` — publisher orchestration and validation

**Guidelines:**
- Use `Moq` for all external dependencies
- Use `FluentAssertions` for readable assertions
- One concern per test method
- Name tests: `Method_Should_ExpectedBehavior` or `Method_Should_ExpectedBehavior_WhenCondition`

### DI Registration Tests

Tests that verify service registration wiring without starting the MassTransit bus.

**Location:** `Extensions/`

**Examples:**
- `MassTransitServiceCollectionExtensionsTests` — verifies required service registrations
- `OutboxRegistrationTests` — verifies outbox configurator wiring
- `ConfigurationBindingTests` — verifies options binding from `IConfiguration`

**Guidelines:**
- Use `ServiceCollection` directly (not a full host)
- Verify service descriptors (type, lifetime) rather than resolving services
- Keep tests focused on wiring, not business behavior

### Filter and Observer Tests

Tests that validate publish/consume filter behavior and observer adapter forwarding.

**Location:** `Filters/`, `Observers/`

**Examples:**
- `HeaderEnrichmentPublishFilterTests` — header enrichment and pipeline continuation
- `MessageContextConsumeFilterTests` — context propagation and cleanup
- `ObserverAdapterDetailedTests` — metadata forwarding and status mapping

**Guidelines:**
- Mock `PublishContext<T>`, `ConsumeContext<T>`, `SendContext<T>` from MassTransit
- Verify header values via captured dictionaries or mock callbacks
- Test both success and failure paths (especially context cleanup)

### Health Check Tests

Tests for health check logic and health check registration.

**Location:** `Health/`

**Examples:**
- `MassTransitBusHealthCheckTests` — healthy/degraded/unhealthy states
- `HealthCheckRegistrationTests` — DI registration of health checks

### Configuration and Options Tests

Tests for options default values and configuration binding.

**Location:** `Options/`

**Examples:**
- `OptionsTests` — default values for all option classes
- `RetryExceptionFilteringTests` — exception type filter behavior

### Harness-Based Integration Tests

End-to-end tests using MassTransit's in-memory test harness.

**Location:** `Harness/`

**Examples:**
- `EndToEndHarnessTests` — full publish/consume flow with filter enrichment

**Guidelines:**
- Use `MassTransit.Testing.ITestHarness` with in-memory transport
- Register filters and observers as they would be in production
- Keep test consumers generic and infrastructure-focused
- Use `IAsyncLifetime` for setup/teardown

## Adding New Tests

1. Identify the category (unit, registration, filter, harness)
2. Place the file in the matching folder
3. Follow the naming convention: `{ClassName}Tests.cs`
4. Use existing test files as reference for mock setup patterns
5. Run tests with: `dotnet test tests/MW.Messaging.MassTransit.Tests`

## Test Dependencies

| Package | Purpose |
|---------|---------|
| xUnit | Test framework |
| FluentAssertions | Readable assertions |
| Moq | Mocking |
| MassTransit.TestFramework | In-memory test harness |
| Microsoft.Extensions.Configuration | Configuration binding tests |
| Microsoft.Extensions.DependencyInjection | DI registration tests |
