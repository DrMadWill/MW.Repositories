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
- Outbox integration hooks (infrastructure-level only)
- Health check registration

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
