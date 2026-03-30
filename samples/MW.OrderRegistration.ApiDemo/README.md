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

## Debug/Test Endpoints

The API host includes a dedicated set of debug/test endpoints for validating each shared infrastructure package individually. These endpoints are **development-only** and are not part of the business API.

### Enabling Test Endpoints

Test endpoints are automatically enabled when:
- `ASPNETCORE_ENVIRONMENT` is set to `Development` (the default for local `dotnet run`)
- Or `TestEndpoints:Enabled` is set to `true` in configuration

When disabled, all `/api/test/*` routes return 404.

### Route Groups

| Route Group              | Purpose                                                      |
|--------------------------|--------------------------------------------------------------|
| `/api/test/repository/*` | Validate repository abstractions and unit-of-work (CRUD)     |
| `/api/test/messaging/*`  | Validate messaging publish/consume and header propagation    |
| `/api/test/saga/*`       | Validate saga start, state inspection, transitions, timeout  |
| `/api/test/persistence/*`| Validate DB connectivity and outbox behavior                 |
| `/api/test/integration/*`| Validate combined persistence + messaging flows              |
| `/api/test/health`       | Quick health check for debug infrastructure                  |
| `/api/test/summary`      | High-level summary of all registered infrastructure          |

### Repository Endpoints

| Method | Path                            | Description                                          |
|--------|---------------------------------|------------------------------------------------------|
| POST   | `/api/test/repository/save`     | Save a demo entity using repository + unit of work   |
| GET    | `/api/test/repository/{id}`     | Get a demo entity by id                              |
| PUT    | `/api/test/repository/{id}`     | Update a demo entity                                 |
| DELETE | `/api/test/repository/{id}`     | Delete a demo entity                                 |
| GET    | `/api/test/repository/list`     | List all demo entities                               |
| POST   | `/api/test/repository/rollback` | Intentional failure to observe rollback behavior     |

### Messaging Endpoints

| Method | Path                                           | Description                                     |
|--------|-------------------------------------------------|-------------------------------------------------|
| POST   | `/api/test/messaging/publish`                  | Publish a test integration event                 |
| POST   | `/api/test/messaging/publish-with-context`     | Publish with explicit metadata/context           |
| GET    | `/api/test/messaging/consumed/{correlationId}` | Check if a test event was consumed               |
| GET    | `/api/test/messaging/metadata/{correlationId}` | Inspect messaging metadata for a consumed event  |

### Saga Endpoints

| Method | Path                                           | Description                                     |
|--------|-------------------------------------------------|-------------------------------------------------|
| POST   | `/api/test/saga/start`                         | Start a demo saga flow                           |
| GET    | `/api/test/saga/{correlationId}`               | Inspect current saga state                       |
| POST   | `/api/test/saga/{correlationId}/transition`    | Manually trigger next saga transition            |
| POST   | `/api/test/saga/{correlationId}/timeout`       | Simulate saga timeout                            |

### Persistence Endpoints

| Method | Path                          | Description                                       |
|--------|-------------------------------|---------------------------------------------------|
| GET    | `/api/test/persistence/ping`  | Verify database connectivity                      |
| GET    | `/api/test/persistence/outbox`| Inspect MassTransit outbox status                 |

### Integration Endpoints

| Method | Path                                     | Description                                      |
|--------|------------------------------------------|--------------------------------------------------|
| POST   | `/api/test/integration/save-and-publish` | Combined repository save + event publish          |
| POST   | `/api/test/integration/fail-after-save`  | Intentional failure after save for debugging      |

### Other Endpoints

| Method | Path                  | Description                                            |
|--------|-----------------------|--------------------------------------------------------|
| GET    | `/api/test/health`    | Quick health check                                     |
| GET    | `/api/test/summary`   | Debug summary with DB, messaging, saga status          |

### Example Usage

```bash
# Save a test item
curl -X POST http://localhost:5000/api/test/repository/save \
  -H "Content-Type: application/json" \
  -d '{"name":"My Test Item","description":"Testing repository save"}'

# Get the saved item
curl http://localhost:5000/api/test/repository/{id}

# Publish a test event
curl -X POST http://localhost:5000/api/test/messaging/publish \
  -H "Content-Type: application/json" \
  -d '{"payload":"hello from test"}'

# Check if event was consumed
curl http://localhost:5000/api/test/messaging/consumed/{correlationId}

# Start a test saga
curl -X POST http://localhost:5000/api/test/saga/start \
  -H "Content-Type: application/json" \
  -d '{"buyerId":"test-buyer","totalAmount":99.99}'

# Check saga state
curl http://localhost:5000/api/test/saga/{correlationId}

# Get debug summary
curl http://localhost:5000/api/test/summary
```

### Swagger Grouping

Debug/test endpoints are grouped under separate tags in Swagger UI (e.g., `Debug: Repository`, `Debug: Messaging`, `Debug: Saga`) so they are clearly separated from business endpoints.
