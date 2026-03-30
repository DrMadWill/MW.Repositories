using System.Diagnostics;
using MassTransit;

namespace MW.OrderRegistration.ApiDemo.TestInfrastructure;

/// <summary>
/// MassTransit consumer that handles <see cref="TestIntegrationEvent"/> messages
/// and records them in the <see cref="TestConsumedEventStore"/> for debug inspection.
/// </summary>
public class TestEventConsumer : IConsumer<TestIntegrationEvent>
{
    private readonly TestConsumedEventStore _store;
    private readonly ILogger<TestEventConsumer> _logger;

    public TestEventConsumer(TestConsumedEventStore store, ILogger<TestEventConsumer> logger)
    {
        _store = store;
        _logger = logger;
    }

    public Task Consume(ConsumeContext<TestIntegrationEvent> context)
    {
        var message = context.Message;

        var record = new ConsumedEventRecord
        {
            CorrelationId = message.CorrelationId ?? message.EventId.ToString(),
            EventName = message.EventName,
            EventVersion = message.EventVersion,
            SourceService = message.SourceService,
            CausationId = message.CausationId,
            TraceId = Activity.Current?.TraceId.ToString(),
            Payload = message.Payload,
            EventId = message.EventId,
            OccurredOn = message.OccurredOn,
            ConsumedAt = DateTimeOffset.UtcNow
        };

        _store.Record(record);

        _logger.LogInformation(
            "[TestConsumer] Consumed test event — CorrelationId={CorrelationId}, EventName={EventName}, Payload={Payload}",
            record.CorrelationId, record.EventName, record.Payload);

        return Task.CompletedTask;
    }
}
