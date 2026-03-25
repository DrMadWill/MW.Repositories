# Architecture Notes — Future Abstraction Evaluation

## Message Envelope Abstraction (`MessageEnvelope<T>`)

### Status: **Postponed** — Not needed at this stage.

### Analysis

A `MessageEnvelope<T>` could group metadata + payload into a single wrapper:

```csharp
public class MessageEnvelope<T> where T : IIntegrationEvent
{
    public EventMetadata Metadata { get; set; }
    public T Payload { get; set; }
}
```

**Potential use cases:**
- Audit logging (metadata + payload together)
- Message replay (store and re-publish complete envelopes)
- Testing (create test envelopes with controlled metadata)
- Metadata + payload grouping for transport-level concerns

**Recommendation:**
At this stage, `IIntegrationEvent` already carries its own metadata (`EventId`, `OccurredOn`, `CorrelationId`, etc.) and `PublishContextModel` / `ConsumerContextModel` handle transport-level metadata separately. Adding an envelope would create redundancy with the existing model structure.

If future use cases (e.g., message replay, audit storage) clearly require a unified envelope, it can be introduced without breaking existing contracts.

---

## Endpoint Naming Abstraction

### Status: **Postponed** — Belongs in `MW.Messaging.MassTransit`, not in abstractions.

### Analysis

Queue/endpoint naming concerns include:
- Service prefixing (e.g., `order-service-order-created`)
- Kebab-case formatting
- Environment-based naming (e.g., `dev-order-created`)
- Versioned endpoints

**Recommendation:**
Endpoint naming is a transport-level concern. It depends on the broker topology (RabbitMQ exchanges/queues, Kafka topics) and framework conventions (MassTransit's `KebabCaseEndpointNameFormatter`). Including this in abstractions would:

1. Create coupling to transport-specific naming patterns
2. Duplicates what MassTransit already provides via `IEndpointNameFormatter`
3. Force abstractions to know about broker topology

If a common naming contract is needed across multiple transport implementations in the future, a minimal `IEndpointNamingConvention` can be added to abstractions at that point.

---

## Context Overlap with `MW.Application.Abstractions`

### Status: **Noted** — Clear separation exists, adapter may be needed later.

### Analysis

**Messaging context** (`IMessageExecutionContext`, `IMessageContextAccessor`) provides:
- Message-driven flow metadata (MessageId, CorrelationId, CausationId, etc.)
- Consumer-scoped context within message handling

**Application context** (`MW.Application.Abstractions`) provides:
- CQRS command/query contracts (`ICommand`, `IQuery<T>`)
- MediatR-based request/response patterns

**Current separation:**
- Messaging context is specific to message bus flows (consumer pipelines)
- Application context is specific to request handling (HTTP/CQRS flows)
- No overlap exists at this time

**Future consideration:**
When a service handles both HTTP requests and message consumers, an adapter or unified correlation provider may be needed to bridge `ICorrelationContext` from both entry points. This adapter would belong in the service composition layer, not in either abstractions package.
