using Microsoft.AspNetCore.Mvc;
using MW.Messaging.Messaging;
using MW.Messaging.Publishing;
using MW.OrderRegistration.ApiDemo.Contracts;
using MW.OrderRegistration.ApiDemo.TestInfrastructure;

namespace MW.OrderRegistration.ApiDemo.Controllers;

/// <summary>
/// Debug/test endpoints for validating messaging publish and consume abstractions.
/// Development-only — not exposed in production.
/// </summary>
[ApiController]
[Route("api/test/messaging")]
[Produces("application/json")]
public class TestMessagingController : ControllerBase
{
    private readonly IIntegrationEventPublisher _publisher;
    private readonly TestConsumedEventStore _consumedStore;
    private readonly ILogger<TestMessagingController> _logger;

    public TestMessagingController(
        IIntegrationEventPublisher publisher,
        TestConsumedEventStore consumedStore,
        ILogger<TestMessagingController> logger)
    {
        _publisher = publisher;
        _consumedStore = consumedStore;
        _logger = logger;
    }

    /// <summary>
    /// Publishes a simple test integration event through IIntegrationEventPublisher.
    /// </summary>
    [HttpPost("publish")]
    [ProducesResponseType(typeof(PublishTestEventResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Publish([FromBody] PublishTestEventRequest request, CancellationToken ct)
    {
        var correlationId = Guid.NewGuid().ToString();
        var testEvent = new TestIntegrationEvent
        {
            CorrelationId = correlationId,
            Payload = request.Payload ?? $"test-payload-{DateTime.UtcNow:HHmmss}"
        };

        await _publisher.PublishAsync(testEvent, ct);

        _logger.LogInformation(
            "[TestMessaging] Published — CorrelationId={CorrelationId}, EventName={EventName}",
            correlationId, testEvent.EventName);

        return Ok(new PublishTestEventResponse
        {
            EventName = testEvent.EventName,
            CorrelationId = correlationId,
            SourceService = testEvent.SourceService,
            Accepted = true,
            PublishedAt = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Publishes a test event with explicit publish-context metadata for debugging header enrichment.
    /// </summary>
    [HttpPost("publish-with-context")]
    [ProducesResponseType(typeof(PublishTestEventResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> PublishWithContext([FromBody] PublishWithContextRequest request, CancellationToken ct)
    {
        var correlationId = request.CorrelationId ?? Guid.NewGuid().ToString();
        var testEvent = new TestIntegrationEvent
        {
            CorrelationId = correlationId,
            CausationId = request.CausationId,
            Payload = request.Payload ?? $"context-test-{DateTime.UtcNow:HHmmss}"
        };

        var context = new PublishContextModel
        {
            CorrelationId = correlationId,
            CausationId = request.CausationId,
            SourceService = request.SourceService ?? testEvent.SourceService,
            TraceId = request.TraceId,
            TenantId = request.TenantId,
            UserId = request.UserId
        };

        await _publisher.PublishAsync(testEvent, context, ct);

        _logger.LogInformation(
            "[TestMessaging] Published with context — CorrelationId={CorrelationId}, TraceId={TraceId}",
            correlationId, request.TraceId);

        return Ok(new PublishTestEventResponse
        {
            EventName = testEvent.EventName,
            CorrelationId = correlationId,
            SourceService = context.SourceService,
            Accepted = true,
            PublishedAt = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Verifies whether a published test event was consumed by checking the in-memory debug store.
    /// </summary>
    [HttpGet("consumed/{correlationId}")]
    [ProducesResponseType(typeof(ConsumedEventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetConsumed(string correlationId)
    {
        var record = _consumedStore.Get(correlationId);
        if (record == null)
            return NotFound(new { Message = $"No consumed event found for correlationId '{correlationId}'. The event may not have been consumed yet." });

        return Ok(new ConsumedEventResponse
        {
            CorrelationId = record.CorrelationId,
            EventName = record.EventName,
            EventVersion = record.EventVersion,
            SourceService = record.SourceService,
            CausationId = record.CausationId,
            TraceId = record.TraceId,
            Payload = record.Payload,
            ConsumedAt = record.ConsumedAt,
            EventId = record.EventId,
            OccurredOn = record.OccurredOn
        });
    }

    /// <summary>
    /// Returns messaging metadata for the last processed or specified test event for debugging.
    /// </summary>
    [HttpGet("metadata/{correlationId}")]
    [ProducesResponseType(typeof(ConsumedEventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetMetadata(string correlationId)
    {
        var record = _consumedStore.Get(correlationId);
        if (record == null)
            return NotFound(new { Message = $"No metadata found for correlationId '{correlationId}'." });

        return Ok(new ConsumedEventResponse
        {
            CorrelationId = record.CorrelationId,
            EventName = record.EventName,
            EventVersion = record.EventVersion,
            SourceService = record.SourceService,
            CausationId = record.CausationId,
            TraceId = record.TraceId,
            Payload = record.Payload,
            ConsumedAt = record.ConsumedAt,
            EventId = record.EventId,
            OccurredOn = record.OccurredOn
        });
    }
}
