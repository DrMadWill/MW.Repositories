using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MW.OrderRegistration.ApiDemo.Contracts;
using MW.OrderRegistration.ConsoleDemo.Infrastructure.Persistence;

namespace MW.OrderRegistration.ApiDemo.Controllers;

/// <summary>
/// Debug/test endpoints for validating persistence connectivity and outbox behavior.
/// Development-only — not exposed in production.
/// </summary>
[ApiController]
[Route("api/test/persistence")]
[Produces("application/json")]
public class TestPersistenceController : ControllerBase
{
    private readonly DemoDbContext _dbContext;
    private readonly ILogger<TestPersistenceController> _logger;

    public TestPersistenceController(DemoDbContext dbContext, ILogger<TestPersistenceController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Verifies database connectivity and basic DbContext health.
    /// </summary>
    [HttpGet("ping")]
    [ProducesResponseType(typeof(PersistencePingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Ping(CancellationToken ct)
    {
        try
        {
            var canConnect = await _dbContext.Database.CanConnectAsync(ct);
            var databaseName = _dbContext.Database.GetDbConnection().Database;
            var providerName = _dbContext.Database.ProviderName;

            var response = new PersistencePingResponse
            {
                Connected = canConnect,
                Provider = providerName,
                DatabaseName = databaseName,
                CheckedAt = DateTimeOffset.UtcNow
            };

            if (!canConnect)
            {
                _logger.LogWarning("[TestPersistence] Database ping failed — could not connect.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, response);
            }

            _logger.LogInformation("[TestPersistence] Database ping OK — Provider={Provider}, Database={Database}",
                providerName, databaseName);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TestPersistence] Database ping error.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new PersistencePingResponse
            {
                Connected = false,
                CheckedAt = DateTimeOffset.UtcNow
            });
        }
    }

    /// <summary>
    /// Returns lightweight debug information about the MassTransit outbox tables.
    /// </summary>
    [HttpGet("outbox")]
    [ProducesResponseType(typeof(OutboxDebugResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> OutboxStatus(CancellationToken ct)
    {
        try
        {
            // Query MassTransit outbox tables for debug information
            var totalOutbox = await _dbContext.Set<MassTransit.EntityFrameworkCoreIntegration.OutboxMessage>()
                .CountAsync(ct);

            var processedOutbox = await _dbContext.Set<MassTransit.EntityFrameworkCoreIntegration.OutboxState>()
                .CountAsync(ct);

            var inboxCount = await _dbContext.Set<MassTransit.EntityFrameworkCoreIntegration.InboxState>()
                .CountAsync(ct);

            _logger.LogInformation(
                "[TestPersistence] Outbox — Total={Total}, OutboxStates={OutboxStates}, Inbox={Inbox}",
                totalOutbox, processedOutbox, inboxCount);

            return Ok(new OutboxDebugResponse
            {
                PendingOutboxMessages = totalOutbox - processedOutbox,
                ProcessedOutboxMessages = processedOutbox,
                InboxStateCount = inboxCount,
                CheckedAt = DateTimeOffset.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[TestPersistence] Outbox query failed — tables may not exist yet.");
            return Ok(new OutboxDebugResponse
            {
                PendingOutboxMessages = -1,
                ProcessedOutboxMessages = -1,
                InboxStateCount = -1,
                CheckedAt = DateTimeOffset.UtcNow
            });
        }
    }
}
