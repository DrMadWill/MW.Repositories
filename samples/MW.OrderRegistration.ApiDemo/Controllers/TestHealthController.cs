using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MW.OrderRegistration.ConsoleDemo.Infrastructure.Persistence;

namespace MW.OrderRegistration.ApiDemo.Controllers;

/// <summary>
/// Debug/test health check endpoint for quick infrastructure verification.
/// Development-only — not exposed in production.
/// </summary>
[ApiController]
[Route("api/test/health")]
[Produces("application/json")]
public class TestHealthController : ControllerBase
{
    private readonly DemoDbContext _dbContext;

    public TestHealthController(DemoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Returns a simple health check indicating whether the debug/test infrastructure is reachable.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Health(CancellationToken ct)
    {
        var dbOk = false;
        try
        {
            dbOk = await _dbContext.Database.CanConnectAsync(ct);
        }
        catch
        {
            // Swallow — report as not connected
        }

        return Ok(new
        {
            Status = dbOk ? "Healthy" : "Degraded",
            DatabaseConnected = dbOk,
            CheckedAt = DateTimeOffset.UtcNow
        });
    }
}
