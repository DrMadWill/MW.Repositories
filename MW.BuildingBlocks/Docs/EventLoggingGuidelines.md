# Event Logging Guidelines

## Overview

Since the platform relies on MassTransit transactional outbox for reliability,
event visibility comes from structured logging and tracing rather than manual
Inbox/Outbox audit persistence. All services must follow these logging guidelines
to ensure event search and diagnostics are consistent.

## When to Log

| Phase                    | Required | Description                                    |
|--------------------------|----------|------------------------------------------------|
| Before publish           | Yes      | Log intent to publish with event metadata      |
| After publish            | Yes      | Log successful publish confirmation             |
| Before consume           | Yes      | Log receipt of message with consumer metadata  |
| After successful consume | Yes      | Log successful processing                       |
| After failed consume     | Yes      | Log failure with exception details              |

## Required Log Fields

Every event-related log entry must include the following fields when available:

| Field              | Type              | Description                                       |
|--------------------|-------------------|---------------------------------------------------|
| `message_id`       | `Guid`            | Unique identifier of the message                  |
| `event_name`       | `string`          | Explicit event name (e.g., `order.created.v1`)    |
| `event_version`    | `string`          | Event contract version                            |
| `correlation_id`   | `string`          | Distributed tracing correlation identifier        |
| `causation_id`     | `string`          | Identifier of the command/event that caused this  |
| `trace_id`         | `string`          | OpenTelemetry/distributed trace id                |
| `source_service`   | `string`          | Name of the service that published the event      |
| `consumer`         | `string`          | Name of the consumer processing the message       |
| `endpoint`         | `string`          | Endpoint where the message was received           |
| `status`           | `string`          | Processing status (e.g., Published, Consumed)     |
| `duration_ms`      | `long`            | Processing duration in milliseconds               |

## Log Level Guidelines

| Scenario                 | Log Level   |
|--------------------------|-------------|
| Before publish           | Information |
| After successful publish | Information |
| Before consume           | Information |
| After successful consume | Information |
| After failed consume     | Error       |
| Retry attempt            | Warning     |

## Structured Logging Example

```csharp
logger.LogInformation(
    "Event published: {EventName} MessageId={MessageId} CorrelationId={CorrelationId} SourceService={SourceService}",
    logContext.EventName,
    logContext.MessageId,
    logContext.CorrelationId,
    logContext.SourceService);
```

## Integration with Observability Fields

Use the constants from `ObservabilityFields` to ensure field names match
Grafana/Graylog/Loki/Tempo query expectations. See `ObservabilityFields.cs`
for the standard field name list.

## Summary

- All services must log events at publish and consume time.
- Log entries must use the standard field set defined above.
- Field names must match `ObservabilityFields` constants for queryability.
- Error logs must include exception details and stack traces.
- Duration tracking is required for consume operations.
