using System.Collections.Concurrent;

namespace MW.OrderRegistration.ApiDemo.TestInfrastructure;

/// <summary>
/// In-memory store for tracking consumed test events during development/debug sessions.
/// Not intended for production use — exists solely for debugging consumer flow.
/// </summary>
public class TestConsumedEventStore
{
    private readonly ConcurrentDictionary<string, ConsumedEventRecord> _records = new();

    /// <summary>
    /// Records a consumed test event.
    /// </summary>
    public void Record(ConsumedEventRecord record)
    {
        _records[record.CorrelationId] = record;
    }

    /// <summary>
    /// Gets a consumed event record by correlation id.
    /// </summary>
    public ConsumedEventRecord? Get(string correlationId)
    {
        _records.TryGetValue(correlationId, out var record);
        return record;
    }

    /// <summary>
    /// Gets all consumed event records.
    /// </summary>
    public IReadOnlyList<ConsumedEventRecord> GetAll()
    {
        return _records.Values.ToList().AsReadOnly();
    }

    /// <summary>
    /// Gets the most recently consumed event record, or null if none.
    /// </summary>
    public ConsumedEventRecord? GetLatest()
    {
        return _records.Values
            .OrderByDescending(r => r.ConsumedAt)
            .FirstOrDefault();
    }
}

/// <summary>
/// Represents a single consumed test event record for debugging.
/// </summary>
public class ConsumedEventRecord
{
    public string CorrelationId { get; init; } = string.Empty;
    public string EventName { get; init; } = string.Empty;
    public string EventVersion { get; init; } = string.Empty;
    public string SourceService { get; init; } = string.Empty;
    public string? CausationId { get; init; }
    public string? TraceId { get; init; }
    public string Payload { get; init; } = string.Empty;
    public DateTimeOffset ConsumedAt { get; init; } = DateTimeOffset.UtcNow;
    public Guid EventId { get; init; }
    public DateTimeOffset OccurredOn { get; init; }
}
