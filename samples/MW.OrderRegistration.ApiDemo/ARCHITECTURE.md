# Architecture Note — MW.OrderRegistration.ApiDemo

## Intended Role

This ASP.NET Core Web API application is a **sample/integration demo host**. Its purpose is to:

- **Validate shared infrastructure packages together** — ensuring `MW.Persistence.*`, `MW.Messaging.*`, and `MW.Saga.*` packages integrate correctly in an HTTP-hosted application.
- **Serve as a reference executable API sample** — showing how to register and use the shared packages in an ASP.NET Core host.
- **Demonstrate saga + messaging + persistence integration** — exercising a realistic long-running order registration process triggered through HTTP.
- **Replace the console demo** as the primary executable sample for the MW.Extensions shared infrastructure.

## What This Is NOT

- ❌ A production microservice
- ❌ A template to copy directly into production
- ❌ A full-featured order management system
- ❌ An API service that should be deployed to staging/production environments

## Design Decisions

- **Reuses ConsoleDemo types** — domain models, integration events, saga state/machine, consumers, and application services are reused from `MW.OrderRegistration.ConsoleDemo` via project reference. This avoids code duplication and ensures both demos exercise the same core flow logic.
- **API-specific DTOs** — HTTP request/response contracts are separate from internal domain entities. The API does not expose internal entities directly.
- **Scenario-driven behavior** — consumer behavior is controlled by a configurable scenario (via request body or config), not production business logic.
- **Single host** — all consumers, the saga, and the API endpoints run in a single process for demo convenience.
- **EnsureCreated for database** — database initialization uses `EnsureCreated` instead of migrations for simplicity.
- **Structured logging** — Serilog provides readable, structured output with request metadata for observing end-to-end flow.
- **Swagger/OpenAPI** — Swagger UI is available at the root URL for interactive API exploration.

## Relationship to Console Demo

The API demo reuses all business and infrastructure code from the console demo:

```
MW.OrderRegistration.ConsoleDemo (reusable types)
├── Domain/Entities/     → reused via project reference
├── Domain/Enums/        → reused via project reference
├── Events/              → reused via project reference
├── Saga/                → reused via project reference
├── Consumers/           → reused via project reference
├── Services/            → reused via project reference
├── Infrastructure/      → reused via project reference
├── Configuration/       → reused via project reference
└── Program.cs           → console-only (not used by API)

MW.OrderRegistration.ApiDemo (this project)
├── Contracts/           → API-specific DTOs
├── Controllers/         → API endpoints
├── Program.cs           → ASP.NET Core host setup
└── appsettings.json     → API-specific configuration
```

## Future Guidance

If the shared packages change, this API demo should be updated to reflect the new registration patterns and APIs. The demo should remain:

- Simple to run locally
- Focused on infrastructure validation
- Free of production business logic complexity
- The primary executable sample for the MW.Extensions infrastructure
