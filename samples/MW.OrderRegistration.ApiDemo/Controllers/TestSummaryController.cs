using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MW.OrderRegistration.ApiDemo.Contracts;
using MW.OrderRegistration.ApiDemo.TestInfrastructure;
using MW.OrderRegistration.ConsoleDemo.Infrastructure.Persistence;

namespace MW.OrderRegistration.ApiDemo.Controllers;

/// <summary>
/// Debug summary endpoint returning high-level infrastructure status.
/// Development-only — not exposed in production.
/// </summary>
[ApiController]
[Route("api/test")]
[Produces("application/json")]
public class TestSummaryController : ControllerBase
{
    private readonly DemoDbContext _dbContext;
    private readonly TestConsumedEventStore _consumedStore;
    private readonly ILogger<TestSummaryController> _logger;

    public TestSummaryController(
        DemoDbContext dbContext,
        TestConsumedEventStore consumedStore,
        ILogger<TestSummaryController> logger)
    {
        _dbContext = dbContext;
        _consumedStore = consumedStore;
        _logger = logger;
    }

    /// <summary>
    /// Returns a high-level debug summary of registered infrastructure and recent test state.
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(DebugSummaryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Summary(CancellationToken ct)
    {
        var dbConnected = false;
        try
        {
            dbConnected = await _dbContext.Database.CanConnectAsync(ct);
        }
        catch
        {
            // Connectivity check failed — report as not connected
        }

        var lastTestItem = await _dbContext.TestItems
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync(ct);

        var testItemCount = await _dbContext.TestItems.CountAsync(ct);

        var latestConsumed = _consumedStore.GetLatest();
        var consumedCount = _consumedStore.GetAll().Count;

        // Check if MassTransit bus is registered (IBus resolves if messaging is configured)
        var messagingRegistered = HttpContext.RequestServices.GetService<IBus>() != null;

        // Check if saga state machine is registered (saga states exist in DbContext)
        var sagaRegistered = _dbContext.Model.FindEntityType(typeof(MW.OrderRegistration.ConsoleDemo.Saga.OrderRegistrationSagaState)) != null;

        var response = new DebugSummaryResponse
        {
            DatabaseConnected = dbConnected,
            MessagingRegistered = messagingRegistered,
            SagaRegistered = sagaRegistered,
            LastTestItemId = lastTestItem?.Id,
            LastTestEventCorrelationId = latestConsumed?.CorrelationId,
            TestItemCount = testItemCount,
            ConsumedEventCount = consumedCount,
            CheckedAt = DateTimeOffset.UtcNow
        };

        _logger.LogInformation(
            "[TestSummary] DB={DbConnected}, Messaging={Messaging}, Saga={Saga}, TestItems={TestItems}, ConsumedEvents={Consumed}",
            dbConnected, messagingRegistered, sagaRegistered, testItemCount, consumedCount);

        return Ok(response);
    }
}
