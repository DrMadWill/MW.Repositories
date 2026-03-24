# Fault Event Handling Guideline

## Overview

MassTransit produces `Fault<T>` messages when consumers throw exceptions.
These fault events are critical for diagnostics and monitoring and should be
part of the platform's event observability strategy.

## Should You Consume `Fault<T>`?

| Scenario                                    | Recommendation |
|---------------------------------------------|----------------|
| General observability / logging             | Yes            |
| Triggering compensating actions             | Yes            |
| Sending alerts for critical failures        | Yes            |
| Replacing proper error handling in consumers| No             |

### When to Consume `Fault<T>`

- When you need to observe failures that happen outside the consumer (e.g., serialization errors).
- When a downstream service needs to react to a failure in another service.
- When you want to centralize failure logging for dashboards and alerting.

### When NOT to Consume `Fault<T>`

- As a substitute for proper try/catch and error handling inside consumers.
- For retry logic — use MassTransit retry policies instead.

## How to Log Fault Events

Fault event logs must include:

| Field              | Source                                      |
|--------------------|---------------------------------------------|
| `message_id`       | Original message id from fault context      |
| `correlation_id`   | Original correlation id                     |
| `causation_id`     | Original causation id                       |
| `event_name`       | Original event name from headers            |
| `source_service`   | Service that published the original event   |
| `consumer`         | Consumer that faulted                       |
| `error`            | Exception message and type                  |
| `trace_id`         | Distributed trace id for linking            |
| `status`           | `Failed`                                    |

## Linking Faults to Original Messages

Use `MessageId` and `CorrelationId` from the fault context to trace back to
the original published event. The `CausationId` links to the command or event
that triggered the original message.

## When to Raise Alerts

| Condition                                     | Action                  |
|-----------------------------------------------|-------------------------|
| Fault count exceeds threshold in time window  | Raise alert             |
| Specific critical event type faults           | Raise immediate alert   |
| Repeated faults for same CorrelationId        | Investigate and alert   |
| Fault after all retries exhausted             | Raise alert             |

## Summary

- Consume `Fault<T>` for observability and compensating actions.
- Always log fault events with full correlation metadata.
- Use `MessageId` and `CorrelationId` to link faults to original events.
- Configure alerts based on fault frequency and severity.
- Do not use `Fault<T>` as a replacement for in-consumer error handling.
