# Architecture Note — MW.OrderRegistration.ConsoleDemo

## Intended Role

This console application is a **sample/integration demo host**. Its purpose is to:

- **Validate shared infrastructure packages together** — ensuring `MW.Persistence.*`, `MW.Messaging.*`, and `MW.Saga.*` packages integrate correctly in a real executable host.
- **Serve as a reference executable sample** — showing how to register and use the shared packages in a .NET host.
- **Demonstrate saga + messaging + persistence integration** — exercising a realistic long-running order registration process.

## What This Is NOT

- ❌ A production microservice
- ❌ A template to copy directly into production
- ❌ A full-featured order management system
- ❌ An API or service that should be deployed

## Design Decisions

- **Minimal domain model** — entities are deliberately simple and focused on the demo scenario.
- **Scenario-driven behavior** — consumer behavior is controlled by a configurable scenario, not production business logic.
- **Single host** — all consumers, the saga, and the publisher run in a single process for demo convenience.
- **EnsureCreated for database** — database initialization uses `EnsureCreated` instead of migrations for simplicity.
- **Structured logging** — Serilog provides readable, structured output for observing infrastructure behavior.

## Future Guidance

If the shared packages change, this demo should be updated to reflect the new registration patterns and APIs. The demo should remain:

- Simple to run locally
- Focused on infrastructure validation
- Free of production business logic complexity
