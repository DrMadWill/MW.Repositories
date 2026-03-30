namespace MW.OrderRegistration.ApiDemo.Contracts;

// ── Repository Test Contracts ────────────────────────────────────────────────

public class TestItemResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public class SaveTestItemRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}

public class UpdateTestItemRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}

// ── Messaging Test Contracts ─────────────────────────────────────────────────

public class PublishTestEventRequest
{
    public string? Payload { get; init; }
}

public class PublishTestEventResponse
{
    public string EventName { get; init; } = string.Empty;
    public string? CorrelationId { get; init; }
    public string SourceService { get; init; } = string.Empty;
    public bool Accepted { get; init; }
    public DateTimeOffset PublishedAt { get; init; }
}

public class PublishWithContextRequest
{
    public string? Payload { get; init; }
    public string? CorrelationId { get; init; }
    public string? CausationId { get; init; }
    public string? SourceService { get; init; }
    public string? TraceId { get; init; }
    public Guid? TenantId { get; init; }
    public Guid? UserId { get; init; }
}

public class ConsumedEventResponse
{
    public string CorrelationId { get; init; } = string.Empty;
    public string EventName { get; init; } = string.Empty;
    public string EventVersion { get; init; } = string.Empty;
    public string SourceService { get; init; } = string.Empty;
    public string? CausationId { get; init; }
    public string? TraceId { get; init; }
    public string Payload { get; init; } = string.Empty;
    public DateTimeOffset ConsumedAt { get; init; }
    public Guid EventId { get; init; }
    public DateTimeOffset OccurredOn { get; init; }
}

// ── Saga Test Contracts ──────────────────────────────────────────────────────

public class StartTestSagaRequest
{
    public string? BuyerId { get; init; }
    public decimal TotalAmount { get; init; } = 99.99m;
}

public class SagaStartResponse
{
    public Guid CorrelationId { get; init; }
    public string InitialState { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime StartedAt { get; init; }
}

public class SagaStateResponse
{
    public Guid CorrelationId { get; init; }
    public string CurrentState { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public DateTime? FailedAt { get; init; }
    public string? FailureReason { get; init; }
    public Guid OrderId { get; init; }
    public Guid? InventoryReservationId { get; init; }
    public Guid? PaymentAttemptId { get; init; }
}

public class SagaTransitionRequest
{
    /// <summary>
    /// The event to publish for advancing the saga.
    /// Supported: inventory-reserved, inventory-failed, payment-succeeded, payment-failed.
    /// </summary>
    public string TransitionEvent { get; init; } = string.Empty;
}

public class SagaTransitionResponse
{
    public Guid CorrelationId { get; init; }
    public string PublishedEvent { get; init; } = string.Empty;
    public bool Accepted { get; init; }
    public DateTimeOffset TriggeredAt { get; init; }
}

// ── Persistence Test Contracts ───────────────────────────────────────────────

public class PersistencePingResponse
{
    public bool Connected { get; init; }
    public string? Provider { get; init; }
    public string? DatabaseName { get; init; }
    public DateTimeOffset CheckedAt { get; init; }
}

public class OutboxDebugResponse
{
    public int PendingOutboxMessages { get; init; }
    public int ProcessedOutboxMessages { get; init; }
    public int InboxStateCount { get; init; }
    public DateTimeOffset CheckedAt { get; init; }
}

// ── Integration Test Contracts ───────────────────────────────────────────────

public class SaveAndPublishRequest
{
    public string? ItemName { get; init; }
    public string? EventPayload { get; init; }
}

public class SaveAndPublishResponse
{
    public Guid SavedItemId { get; init; }
    public string? ItemName { get; init; }
    public string? EventCorrelationId { get; init; }
    public string EventName { get; init; } = string.Empty;
    public bool SaveCommitted { get; init; }
    public bool PublishAccepted { get; init; }
    public DateTimeOffset Timestamp { get; init; }
}

// ── Summary Contract ─────────────────────────────────────────────────────────

public class DebugSummaryResponse
{
    public bool DatabaseConnected { get; init; }
    public bool MessagingRegistered { get; init; }
    public bool SagaRegistered { get; init; }
    public Guid? LastTestItemId { get; init; }
    public string? LastTestEventCorrelationId { get; init; }
    public int TestItemCount { get; init; }
    public int ConsumedEventCount { get; init; }
    public DateTimeOffset CheckedAt { get; init; }
}
