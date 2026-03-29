# MW.OrderRegistration.ConsoleDemo

An executable integration demo for the shared MW infrastructure packages, using an order registration scenario as the test flow.

## Purpose

This console application is a **reference/demo host** — not a production microservice. It validates the following shared packages working together:

- `MW.Messaging.Abstractions` + `MW.Messaging.MassTransit` — messaging infrastructure
- `MW.Saga.Abstractions` + `MW.Saga.MassTransit` — saga state machine coordination
- `MW.Persistence.Abstractions` + `MW.Persistence.EntityFrameworkCore` + `MW.Persistence.DependencyInjection` — repository/unit-of-work persistence

## Supported Scenarios

| Scenario         | Description                                                      | Expected Outcome      |
|------------------|------------------------------------------------------------------|-----------------------|
| `success`        | Full happy path: order → inventory → payment → completed         | Order completed       |
| `inventory-fail` | Inventory reservation fails early                                | Order failed          |
| `payment-fail`   | Inventory succeeds, payment fails                                | Order failed          |
| `timeout`        | Inventory succeeds, payment response never arrives (timeout)     | Order timed out       |

## Required Infrastructure

- **RabbitMQ** — message transport
- **SQL Server** — database for business data, saga state, and outbox

## Quick Start

### 1. Start Local Infrastructure

```bash
cd samples/MW.OrderRegistration.ConsoleDemo
docker-compose up -d
```

This starts RabbitMQ (ports 5672/15672) and SQL Server (port 1433).

### 2. Run the Demo

```bash
# Success scenario (default)
dotnet run --project samples/MW.OrderRegistration.ConsoleDemo

# Specific scenarios
dotnet run --project samples/MW.OrderRegistration.ConsoleDemo -- --scenario=success
dotnet run --project samples/MW.OrderRegistration.ConsoleDemo -- --scenario=inventory-fail
dotnet run --project samples/MW.OrderRegistration.ConsoleDemo -- --scenario=payment-fail
dotnet run --project samples/MW.OrderRegistration.ConsoleDemo -- --scenario=timeout
```

You can also set the scenario in `appsettings.json` under `Demo:Scenario`.

### 3. Expected Output

The demo will:
1. Create an order using repository abstractions and unit-of-work
2. Publish `OrderRegistrationStarted` to start the saga
3. Saga coordinates: inventory reservation → payment → completion/failure/timeout
4. Consumers use repository abstractions for business data persistence
5. Final summary output shows order status, saga state, and business data

Example (success):
```
═══════════════════════════════════════════════════════════
                   ORDER REGISTRATION SUMMARY
═══════════════════════════════════════════════════════════
  Order ID          : 3f2504e0-4f89-11d3-9a0c-0305e82c3301
  Buyer ID          : demo-buyer-001
  Total Amount      : $109.97
  Order Status      : Completed
  ...
  Saga State        : OrderCompleted
  Saga Status       : Completed
  ...
═══════════════════════════════════════════════════════════
```

## Configuration

Settings are in `appsettings.json`:

| Setting                         | Description                                      | Default                                    |
|---------------------------------|--------------------------------------------------|--------------------------------------------|
| `ConnectionStrings:DemoDb`      | SQL Server connection string                     | localhost,1433/mw_order_demo               |
| `Messaging:RabbitMq:Host`       | RabbitMQ host                                    | localhost                                  |
| `Messaging:ServiceName`         | Service name for endpoint naming                 | order-registration-demo                    |
| `Demo:Scenario`                 | Active scenario                                  | success                                    |
| `Demo:PaymentTimeoutSeconds`    | Timeout duration for payment stage               | 30                                         |
| `Demo:ResultQueryDelaySeconds`  | Delay before querying results                    | 10                                         |

## Shared Packages Exercised

| Package                           | Usage in Demo                                              |
|-----------------------------------|------------------------------------------------------------|
| `MW.Persistence.DependencyInjection` | `AddEfCorePersistence<DemoDbContext>(...)` registration  |
| `MW.Persistence.Abstractions`     | `IRepository<T, TId>`, `IUnitOfWork` in services           |
| `MW.Persistence.EntityFrameworkCore` | EF Core repository and unit-of-work implementations     |
| `MW.Messaging.MassTransit`       | `AddMassTransitMessaging(...)` with RabbitMQ + outbox      |
| `MW.Messaging.Abstractions`      | `IntegrationEvent` base, `IIntegrationEventPublisher`      |
| `MW.Saga.MassTransit`            | `AddSagaMassTransitInfrastructure(...)`, state machine     |
| `MW.Saga.Abstractions`           | `SagaStateBase`, `ISagaState`, `SagaStatus`                |

## Stopping Infrastructure

```bash
docker-compose down -v
```
