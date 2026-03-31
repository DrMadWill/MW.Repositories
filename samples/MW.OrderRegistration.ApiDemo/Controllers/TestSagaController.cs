using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MW.OrderRegistration.ApiDemo.Contracts;
using MW.OrderRegistration.ConsoleDemo.Domain.Entities;
using MW.OrderRegistration.ConsoleDemo.Events;
using MW.OrderRegistration.ConsoleDemo.Infrastructure.Persistence;
using MW.OrderRegistration.ConsoleDemo.Saga;
using MW.OrderRegistration.ConsoleDemo.Services;
using MW.Persistence.Abstractions.Repositories;
using MW.Persistence.Abstractions.UnitOfWork;

namespace MW.OrderRegistration.ApiDemo.Controllers;

/// <summary>
/// Debug/test endpoints for validating saga start, inspection, transition, and timeout behavior.
/// Development-only — not exposed in production.
/// </summary>
[ApiController]
[Route("api/test/saga")]
[Produces("application/json")]
public class TestSagaController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly DemoDbContext _dbContext;
    private readonly OrderCreationService _orderCreationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TestSagaController> _logger;

    public TestSagaController(
        IPublishEndpoint publishEndpoint,
        DemoDbContext dbContext,
        OrderCreationService orderCreationService,
        IUnitOfWork unitOfWork,
        ILogger<TestSagaController> logger)
    {
        _publishEndpoint = publishEndpoint;
        _dbContext = dbContext;
        _orderCreationService = orderCreationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Starts a minimal demo saga by creating an order and publishing OrderRegistrationStarted.
    /// </summary>
    [HttpPost("start")]
    [ProducesResponseType(typeof(SagaStartResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Start([FromBody] StartTestSagaRequest request, CancellationToken ct)
    {
        var buyerId = request.BuyerId ?? $"test-buyer-{DateTime.UtcNow:HHmmss}";
        var items = new List<(string ProductName, int Quantity, decimal UnitPrice)>
        {
            ("TestProduct", 1, request.TotalAmount)
        };

        var order = await _orderCreationService.CreateOrderAsync(buyerId, items);

        await _publishEndpoint.Publish(new OrderRegistrationStarted
        {
            CorrelationId = order.Id.ToString(),
            OrderId = order.Id,
            BuyerId = buyerId,
            TotalAmount = order.TotalAmount
        }, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation(
            "[TestSaga] Started — OrderId={OrderId}, CorrelationId={CorrelationId}",
            order.Id, order.Id);

        return Ok(new SagaStartResponse
        {
            CorrelationId = order.Id,
            InitialState = "AwaitingInventory",
            Status = "Running",
            StartedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Returns current saga state data for step-by-step debugging.
    /// </summary>
    [HttpGet("{correlationId:guid}")]
    [ProducesResponseType(typeof(SagaStateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetState(Guid correlationId, CancellationToken ct)
    {
        var saga = await _dbContext.OrderRegistrationSagaStates
            .FirstOrDefaultAsync(s => s.CorrelationId == correlationId, ct);

        if (saga == null)
            return NotFound(new { Message = $"Saga state for correlationId '{correlationId}' not found." });

        return Ok(MapToResponse(saga));
    }

    /// <summary>
    /// Publishes the next expected saga event manually to advance the saga one step at a time.
    /// </summary>
    [HttpPost("{correlationId:guid}/transition")]
    [ProducesResponseType(typeof(SagaTransitionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Transition(Guid correlationId, [FromBody] SagaTransitionRequest request, CancellationToken ct)
    {
        var saga = await _dbContext.OrderRegistrationSagaStates
            .FirstOrDefaultAsync(s => s.CorrelationId == correlationId, ct);

        if (saga == null)
            return NotFound(new { Message = $"Saga state for correlationId '{correlationId}' not found." });

        var eventName = request.TransitionEvent?.ToLowerInvariant();

        switch (eventName)
        {
            case "inventory-reserved":
                var reservationId = Guid.NewGuid();
                await _publishEndpoint.Publish(new InventoryReserved
                {
                    CorrelationId = correlationId.ToString(),
                    OrderId = saga.OrderId,
                    ReservationId = reservationId
                }, ct);
                await _unitOfWork.SaveChangesAsync(ct);
                return Ok(new SagaTransitionResponse
                {
                    CorrelationId = correlationId,
                    PublishedEvent = "InventoryReserved",
                    Accepted = true,
                    TriggeredAt = DateTimeOffset.UtcNow
                });

            case "inventory-failed":
                await _publishEndpoint.Publish(new InventoryReservationFailed
                {
                    CorrelationId = correlationId.ToString(),
                    OrderId = saga.OrderId,
                    Reason = "Debug/test: intentional inventory failure"
                }, ct);
                await _unitOfWork.SaveChangesAsync(ct);
                return Ok(new SagaTransitionResponse
                {
                    CorrelationId = correlationId,
                    PublishedEvent = "InventoryReservationFailed",
                    Accepted = true,
                    TriggeredAt = DateTimeOffset.UtcNow
                });

            case "payment-succeeded":
                var paymentId = Guid.NewGuid();
                await _publishEndpoint.Publish(new PaymentSucceeded
                {
                    CorrelationId = correlationId.ToString(),
                    OrderId = saga.OrderId,
                    PaymentId = paymentId
                }, ct);
                await _unitOfWork.SaveChangesAsync(ct);
                return Ok(new SagaTransitionResponse
                {
                    CorrelationId = correlationId,
                    PublishedEvent = "PaymentSucceeded",
                    Accepted = true,
                    TriggeredAt = DateTimeOffset.UtcNow
                });

            case "payment-failed":
                await _publishEndpoint.Publish(new PaymentFailed
                {
                    CorrelationId = correlationId.ToString(),
                    OrderId = saga.OrderId,
                    Reason = "Debug/test: intentional payment failure"
                }, ct);
                await _unitOfWork.SaveChangesAsync(ct);
                return Ok(new SagaTransitionResponse
                {
                    CorrelationId = correlationId,
                    PublishedEvent = "PaymentFailed",
                    Accepted = true,
                    TriggeredAt = DateTimeOffset.UtcNow
                });

            default:
                return BadRequest(new
                {
                    Message = $"Unknown transition event: '{request.TransitionEvent}'. " +
                              "Supported: inventory-reserved, inventory-failed, payment-succeeded, payment-failed."
                });
        }
    }

    /// <summary>
    /// Simulates a saga timeout by publishing PaymentTimeoutExpired explicitly.
    /// </summary>
    [HttpPost("{correlationId:guid}/timeout")]
    [ProducesResponseType(typeof(SagaTransitionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SimulateTimeout(Guid correlationId, CancellationToken ct)
    {
        var saga = await _dbContext.OrderRegistrationSagaStates
            .FirstOrDefaultAsync(s => s.CorrelationId == correlationId, ct);

        if (saga == null)
            return NotFound(new { Message = $"Saga state for correlationId '{correlationId}' not found." });

        await _publishEndpoint.Publish(new PaymentTimeoutExpired
        {
            CorrelationId = correlationId.ToString(),
            OrderId = saga.OrderId
        }, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogWarning(
            "[TestSaga] Timeout simulated — CorrelationId={CorrelationId}, OrderId={OrderId}",
            correlationId, saga.OrderId);

        return Ok(new SagaTransitionResponse
        {
            CorrelationId = correlationId,
            PublishedEvent = "PaymentTimeoutExpired",
            Accepted = true,
            TriggeredAt = DateTimeOffset.UtcNow
        });
    }

    private static SagaStateResponse MapToResponse(OrderRegistrationSagaState saga) => new()
    {
        CorrelationId = saga.CorrelationId,
        CurrentState = saga.CurrentState ?? "Unknown",
        Status = saga.Status.ToString(),
        CreatedAt = saga.CreatedAt,
        UpdatedAt = saga.UpdatedAt,
        CompletedAt = saga.CompletedAt,
        FailedAt = saga.FailedAt,
        FailureReason = saga.FailureReason,
        OrderId = saga.OrderId,
        InventoryReservationId = saga.InventoryReservationId,
        PaymentAttemptId = saga.PaymentAttemptId
    };
}
