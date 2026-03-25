# Event Naming and Versioning Convention

## Event Naming

All integration events must follow a consistent naming pattern:

```
{domain}.{action}.{version}
```

### Examples

- `order.created.v1`
- `payment.completed.v1`
- `auction.bid-placed.v1`
- `user.registered.v1`
- `inventory.stock-updated.v1`

### Rules

1. **Domain** — lowercase name of the bounded context or aggregate (e.g., `order`, `payment`, `auction`)
2. **Action** — past-tense verb describing what happened, using kebab-case for multi-word actions (e.g., `created`, `bid-placed`, `stock-updated`)
3. **Version** — prefixed with `v` followed by an integer (e.g., `v1`, `v2`)

## Event Versioning

### Strategy

- Every event contract starts at `v1`.
- When a breaking change is introduced, a new version is created (`v2`, `v3`, etc.).
- Non-breaking changes (adding optional fields) do not require a new version.

### Backward Compatibility Rules

- **Additive changes** (new optional properties) — no version bump required.
- **Removing a property** — new version required.
- **Renaming a property** — new version required.
- **Changing a property type** — new version required.

### Namespace Convention

Event contracts should be organized under a namespace that includes the domain and version:

```
MW.BuildingBlocks.Contracts.Orders.V1.OrderCreatedEvent
MW.BuildingBlocks.Contracts.Payments.V1.PaymentCompletedEvent
```

### Event Name Property

The `EventName` property on every integration event must match the naming convention:

```csharp
public override string EventName => "order.created.v1";
public override string EventVersion => "v1";
```

## Summary

| Aspect              | Convention                                    |
|---------------------|-----------------------------------------------|
| Naming format       | `{domain}.{action}.{version}`                 |
| Domain casing       | lowercase                                     |
| Action casing       | kebab-case, past tense                        |
| Version prefix      | `v` + integer                                 |
| Non-breaking change | No version bump                               |
| Breaking change     | New version required                          |
| Namespace           | `{Root}.Contracts.{Domain}.V{N}.{EventClass}` |
