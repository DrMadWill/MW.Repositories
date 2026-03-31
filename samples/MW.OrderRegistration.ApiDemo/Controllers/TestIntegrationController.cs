using Microsoft.AspNetCore.Mvc;
using MW.Messaging.Publishing;
using MW.OrderRegistration.ApiDemo.Contracts;
using MW.OrderRegistration.ApiDemo.TestInfrastructure;
using MW.OrderRegistration.ConsoleDemo.Domain.Entities;
using MW.Persistence.Abstractions.Repositories;
using MW.Persistence.Abstractions.UnitOfWork;

namespace MW.OrderRegistration.ApiDemo.Controllers;

/// <summary>
/// Debug/test endpoints for combined persistence + messaging integration testing.
/// Development-only — not exposed in production.
/// </summary>
[ApiController]
[Route("api/test/integration")]
[Produces("application/json")]
public class TestIntegrationController : ControllerBase
{
    private readonly IRepository<TestItem, Guid> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIntegrationEventPublisher _publisher;
    private readonly ILogger<TestIntegrationController> _logger;

    public TestIntegrationController(
        IRepository<TestItem, Guid> repository,
        IUnitOfWork unitOfWork,
        IIntegrationEventPublisher publisher,
        ILogger<TestIntegrationController> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _publisher = publisher;
        _logger = logger;
    }

    /// <summary>
    /// Saves demo data using repository/unit-of-work and then publishes a test event.
    /// Validates the full persistence + messaging path together.
    /// </summary>
    [HttpPost("save-and-publish")]
    [ProducesResponseType(typeof(SaveAndPublishResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> SaveAndPublish([FromBody] SaveAndPublishRequest request, CancellationToken ct)
    {
        // Step 1: Save through repository
        var entity = new TestItem
        {
            Id = Guid.NewGuid(),
            Name = request.ItemName ?? $"IntegrationTest-{DateTime.UtcNow:HHmmss}",
            Description = "Created by save-and-publish integration test endpoint.",
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        // Step 2: Publish integration event
        var correlationId = Guid.NewGuid().ToString();
        var testEvent = new TestIntegrationEvent
        {
            CorrelationId = correlationId,
            Payload = request.EventPayload ?? $"integration-test-{entity.Id}"
        };

        await _publisher.PublishAsync(testEvent, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation(
            "[TestIntegration] Save+Publish — ItemId={ItemId}, CorrelationId={CorrelationId}",
            entity.Id, correlationId);

        return Ok(new SaveAndPublishResponse
        {
            SavedItemId = entity.Id,
            ItemName = entity.Name,
            EventCorrelationId = correlationId,
            EventName = testEvent.EventName,
            SaveCommitted = true,
            PublishAccepted = true,
            Timestamp = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Performs a save and prepares a publish, then intentionally fails.
    /// Useful for observing transaction rollback/consistency behavior.
    /// </summary>
    [HttpPost("fail-after-save")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> FailAfterSave([FromBody] SaveAndPublishRequest request, CancellationToken ct)
    {
        var entity = new TestItem
        {
            Id = Guid.NewGuid(),
            Name = request.ItemName ?? $"FailTest-{DateTime.UtcNow:HHmmss}",
            Description = "Created by fail-after-save test — should verify rollback behavior.",
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogWarning(
            "[TestIntegration] Fail-after-save — entity saved, now throwing before publish. Id={Id}",
            entity.Id);

        // Intentional failure after save, before publish
        throw new InvalidOperationException(
            $"[Debug/Test] Intentional failure after save. " +
            $"Entity Id={entity.Id} was saved. " +
            $"This endpoint tests whether the publish step failing after a commit is observable.");
    }
}
