# MW.OrderRegistration.ApiDemo

An ASP.NET Core Web API demo host for the shared MW infrastructure packages, using the same order registration scenario as the console demo — but exposed through HTTP endpoints.

## Purpose

This API application is a **reference/demo host** — not a production microservice. It replaces the console demo as the primary executable sample, validating the following shared packages working together through HTTP:

- `MW.Messaging.Abstractions` + `MW.Messaging.MassTransit` — messaging infrastructure
- `MW.Saga.Abstractions` + `MW.Saga.MassTransit` — saga state machine coordination
- `MW.Persistence.Abstractions` + `MW.Persistence.EntityFrameworkCore` + `MW.Persistence.DependencyInjection` — repository/unit-of-work persistence

## Migration from Console Demo

This API demo replaces `MW.OrderRegistration.ConsoleDemo` as the main sample entry point.

| Aspect                  | Console Demo                     | API Demo (this project)               |
|-------------------------|----------------------------------|---------------------------------------|
| **Entry point**         | Command-line execution           | HTTP API endpoints                    |
| **Scenario selection**  | CLI args / config                | Request body field / config           |
| **Status checking**     | Console summary output           | `GET /api/orders/{id}` endpoints      |
| **Reusable code**       | All domain/saga/messaging logic  | Reused via project reference          |
| **Infrastructure**      | Same (RabbitMQ + PostgreSQL)     | Same (RabbitMQ + PostgreSQL)          |

The console demo remains available as a reference during migration but is no longer the primary sample.

## API Endpoints

| Method | Path                              | Description                        | Response   |
|--------|-----------------------------------|------------------------------------|------------|
| POST   | `/api/orders/register`            | Start order registration process   | 202 Accepted |
| GET    | `/api/orders/{orderId}`           | Get business order status          | 200 OK     |
| GET    | `/api/orders/{orderId}/process`   | Get saga/process status            | 200 OK     |

### POST /api/orders/register

**Request Body:**
```json
{
  "buyerId": "demo-buyer-001",
  "items": [
    { "productName": "Widget A", "quantity": 2, "unitPrice": 29.99 },
    { "productName": "Widget B", "quantity": 1, "unitPrice": 49.99 }
  ],
  "scenario": "success"
}
```

**Supported Scenarios:**

| Scenario         | Description                                                      | Expected Outcome      |
|------------------|------------------------------------------------------------------|-----------------------|
| `success`        | Full happy path: order → inventory → payment → completed         | Order completed       |
| `inventory-fail` | Inventory reservation fails early                                | Order failed          |
| `payment-fail`   | Inventory succeeds, payment fails                                | Order failed          |
| `timeout`        | Inventory succeeds, payment response never arrives (timeout)     | Order timed out       |

> **Note:** Scenario selection mutates shared singleton state. Run different scenarios sequentially, not concurrently. This is a demo limitation, not a production pattern.

**Response (202 Accepted):**
```json
{
  "orderId": "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
  "correlationId": "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
  "status": "Pending",
  "scenario": "Success",
  "orderStatusUrl": "/api/orders/3f2504e0-4f89-11d3-9a0c-0305e82c3301",
  "processStatusUrl": "/api/orders/3f2504e0-4f89-11d3-9a0c-0305e82c3301/process"
}
```

### GET /api/orders/{orderId}

Returns business-level order data including reservation and payment results.

### GET /api/orders/{orderId}/process

Returns saga/process-level state including current state, timestamps, and failure info.

## Required Infrastructure

- **RabbitMQ** — message transport
- **PostgreSQL** — database for business data, saga state, and outbox

## Quick Start

### 1. Start Local Infrastructure

```bash
cd samples/MW.OrderRegistration.ApiDemo
docker-compose up -d
```

This starts RabbitMQ (ports 5672/15672) and PostgreSQL (port 5432).

### 2. Run the API

```bash
dotnet run --project samples/MW.OrderRegistration.ApiDemo
```

### 3. Open Swagger UI

Navigate to the application URL (default: `http://localhost:5000` or as shown in the console output) to explore endpoints interactively via Swagger UI.

### 4. Test a Scenario

Using curl:
```bash
# Success scenario
curl -X POST http://localhost:5000/api/orders/register \
  -H "Content-Type: application/json" \
  -d '{"buyerId":"demo-buyer-001","items":[{"productName":"Widget A","quantity":2,"unitPrice":29.99}],"scenario":"success"}'

# Check order status (use orderId from response)
curl http://localhost:5000/api/orders/{orderId}

# Check process/saga status
curl http://localhost:5000/api/orders/{orderId}/process
```

## Configuration

Settings are in `appsettings.json`:

| Setting                         | Description                                      | Default                                    |
|---------------------------------|--------------------------------------------------|--------------------------------------------|
| `ConnectionStrings:DemoDb`      | PostgreSQL connection string                     | localhost:5432/mw_order_demo               |
| `Messaging:RabbitMq:Host`       | RabbitMQ host                                    | localhost                                  |
| `Messaging:ServiceName`         | Service name for endpoint naming                 | order-registration-api-demo                |
| `Demo:Scenario`                 | Default scenario (overridden by request)         | success                                    |
| `Demo:PaymentTimeoutSeconds`    | Timeout duration for payment stage               | 30                                         |

## Shared Packages Exercised

| Package                           | Usage in API Demo                                          |
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
