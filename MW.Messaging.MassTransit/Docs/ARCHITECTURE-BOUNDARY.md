# MW.Messaging.MassTransit — Architecture Boundary

## Purpose

This project is the **concrete infrastructure implementation** of the messaging abstractions
defined in `MW.Messaging.Abstractions`, using MassTransit and RabbitMQ as the transport layer.

## Scope

This project **MUST** contain only:

- Transport-specific implementations (MassTransit filters, observers, adapters)
- Configuration options for RabbitMQ, retry, redelivery
- DI registration extensions for messaging infrastructure
- Endpoint naming conventions
- Outbox and inbox integration hooks (infrastructure-level only)
- Health check registration
- Tracing instrumentation using standard .NET primitives

## Boundary Rules

This project **MUST NOT** contain:

- Repository-driven sync or repair logic
- AutoMapper-driven entity synchronization
- Static repair registries
- Service-specific replay logic
- Projection rebuild logic
- Business-specific consumer logic
- Domain event handlers
- Any direct dependency on application/domain projects

## Dependency Direction

```
[Application Code] --> [MW.Messaging.Abstractions] <-- [MW.Messaging.MassTransit]
```

Application code depends on abstractions. This infrastructure project implements those abstractions.
No reverse dependency is allowed.

## Rationale

Keeping the messaging infrastructure free of business logic ensures:

1. **Reusability** — Any service can adopt this package without inheriting unrelated logic
2. **Testability** — Infrastructure behavior can be tested in isolation
3. **Maintainability** — Changes to business rules do not affect transport infrastructure
4. **Clean Architecture** — Dependency inversion is preserved

---

## Open Architectural Boundaries

### Publish-Context Ownership

- **`HeaderEnrichmentPublishFilter`** is the sole owner of header enrichment.
- `MassTransitIntegrationEventPublisher` is orchestration-only: it validates and delegates to MassTransit.
- The `PublishContextModel` overload on `IIntegrationEventPublisher` exists for API symmetry
  but does not directly inject headers — the filter always creates its own context from `IPublishContextProvider`.

### Service Identity (`IServiceIdentityProvider`)

- A **safe default is always registered** during DI setup via `ConfigurationServiceIdentityProvider`.
- When `ServiceName` is configured, the provider returns the configured value.
- When `ServiceName` is empty or missing, the provider returns an empty `ServiceIdentity`
  so that downstream code (`DefaultPublishContextProvider`) never encounters `null`.
- Service teams should always configure `ServiceName` in their `Messaging` configuration section.

### Consumer Outbox / InboxState

- Publisher-side outbox is supported via `UseEntityFrameworkOutbox<TDbContext>()`.
- Consumer-side inbox/outbox is supported via `UseEntityFrameworkInboxOutbox<TDbContext>()`.
- Both hooks are infrastructure-only — no business-specific idempotency logic is embedded.
- The inbox/outbox feature enables `QueryDelay` and `DuplicateDetectionWindow` defaults
  that can be overridden through the configuration callback.

### Distributed Tracing

- Tracing is based on standard `System.Diagnostics.Activity` and `ActivitySource` primitives.
- The `MessagingActivitySource` ("MW.Messaging") is used internally by publish and consume filters.
- Publish filter starts a `Producer` activity; consume filter starts a `Consumer` activity.
- No vendor-specific APM dependency is required for the core implementation.
- Services can subscribe to the "MW.Messaging" source via OpenTelemetry or any compatible listener.

### SSL/TLS

- SSL/TLS configuration has been removed from `RabbitMqOptions`.
- The current deployment model uses internal Docker networks without SSL.
- If SSL support is needed in the future, it should be reintroduced as a separate,
  explicit configuration extension rather than unused default properties.
