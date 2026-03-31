using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MW.OrderRegistration.ApiDemo.Contracts;
using MW.OrderRegistration.ConsoleDemo.Configuration;
using MW.OrderRegistration.ConsoleDemo.Domain.Enums;
using MW.OrderRegistration.ConsoleDemo.Events;
using MW.OrderRegistration.ConsoleDemo.Infrastructure.Persistence;
using MW.OrderRegistration.ConsoleDemo.Services;
using MW.Persistence.Abstractions.UnitOfWork;

namespace MW.OrderRegistration.ApiDemo.Controllers;

/// <summary>
/// API controller for order registration demo endpoints.
/// Provides HTTP access to the order registration saga workflow.
/// </summary>
[ApiController]
[Route("api/orders")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly OrderCreationService _orderCreationService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly DemoSettings _demoSettings;
    private readonly DemoDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        OrderCreationService orderCreationService,
        IPublishEndpoint publishEndpoint,
        DemoSettings demoSettings,
        DemoDbContext dbContext,
        IUnitOfWork unitOfWork,
        ILogger<OrdersController> logger)
    {
        _orderCreationService = orderCreationService;
        _publishEndpoint = publishEndpoint;
        _demoSettings = demoSettings;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Starts the order registration process.
    /// Creates the initial order and triggers the saga workflow.
    /// </summary>
    /// <remarks>
    /// The scenario override mutates a singleton DemoSettings instance. This is acceptable for
    /// sequential demo usage but is NOT safe for concurrent requests with different scenarios.
    /// For production use, scenario context should be propagated through message headers.
    /// </remarks>
    /// <param name="request">Order registration request with buyer info, items, and optional scenario.</param>
    /// <returns>202 Accepted with order tracking information.</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterOrderResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterOrder([FromBody] RegisterOrderRequest request)
    {
        // Apply scenario override if provided in request.
        // Note: This mutates singleton state — safe for sequential demo use only.
        var scenario = ResolveScenario(request.Scenario);
        _demoSettings.ResolvedScenario = scenario;

        _logger.LogInformation(
            "[API] Order registration requested — BuyerId={BuyerId}, Items={ItemCount}, Scenario={Scenario}",
            request.BuyerId, request.Items.Count, scenario);

        // Create order through shared service
        var items = request.Items
            .Select(i => (i.ProductName, i.Quantity, i.UnitPrice))
            .ToList();

        var order = await _orderCreationService.CreateOrderAsync(request.BuyerId, items);

        // Publish saga-starting event
        await _publishEndpoint.Publish(new OrderRegistrationStarted
        {
            CorrelationId = order.Id.ToString(),
            OrderId = order.Id,
            BuyerId = order.BuyerId,
            TotalAmount = order.TotalAmount
        });
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation(
            "[API] OrderRegistrationStarted published — OrderId={OrderId}, CorrelationId={CorrelationId}, Scenario={Scenario}",
            order.Id, order.Id, scenario);

        var response = new RegisterOrderResponse
        {
            OrderId = order.Id,
            CorrelationId = order.Id,
            Status = order.Status.ToString(),
            Scenario = scenario.ToString(),
            OrderStatusUrl = Url.Action(nameof(GetOrderStatus), new { orderId = order.Id }),
            ProcessStatusUrl = Url.Action(nameof(GetProcessStatus), new { orderId = order.Id })
        };

        return Accepted(response);
    }

    /// <summary>
    /// Gets the business-level order status.
    /// Returns final/ongoing business process state including reservation and payment results.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    [HttpGet("{orderId:guid}")]
    [ProducesResponseType(typeof(OrderStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderStatus(Guid orderId)
    {
        var order = await _dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            return NotFound(new { Message = $"Order {orderId} not found." });

        var reservation = await _dbContext.InventoryReservations
            .FirstOrDefaultAsync(r => r.OrderId == orderId);

        var payment = await _dbContext.PaymentAttempts
            .FirstOrDefaultAsync(p => p.OrderId == orderId);

        var response = new OrderStatusResponse
        {
            OrderId = order.Id,
            BuyerId = order.BuyerId,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            CompletedAt = order.CompletedAt,
            FailedAt = order.FailedAt,
            FailureReason = order.FailureReason,
            Reservation = reservation != null ? new InventoryReservationResult
            {
                ReservationId = reservation.Id,
                IsReserved = reservation.IsReserved,
                FailureReason = reservation.FailureReason,
                CreatedAt = reservation.CreatedAt
            } : null,
            Payment = payment != null ? new PaymentAttemptResult
            {
                PaymentId = payment.Id,
                Amount = payment.Amount,
                IsSuccessful = payment.IsSuccessful,
                FailureReason = payment.FailureReason,
                CreatedAt = payment.CreatedAt
            } : null,
            Items = order.Items.Select(i => new OrderItemResult
            {
                ItemId = i.Id,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        return Ok(response);
    }

    /// <summary>
    /// Gets the saga/process-level status for an order.
    /// Returns workflow orchestration state independently from business order data.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    [HttpGet("{orderId:guid}/process")]
    [ProducesResponseType(typeof(ProcessStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProcessStatus(Guid orderId)
    {
        var sagaState = await _dbContext.OrderRegistrationSagaStates
            .FirstOrDefaultAsync(s => s.OrderId == orderId);

        if (sagaState == null)
            return NotFound(new { Message = $"Process state for order {orderId} not found. The saga may have been finalized." });

        var response = new ProcessStatusResponse
        {
            CorrelationId = sagaState.CorrelationId,
            OrderId = sagaState.OrderId,
            CurrentState = sagaState.CurrentState ?? "Unknown",
            ProcessStatus = sagaState.Status.ToString(),
            StartedAt = sagaState.CreatedAt,
            CompletedAt = sagaState.CompletedAt,
            FailedAt = sagaState.FailedAt,
            FailureReason = sagaState.FailureReason,
            InventoryReservationId = sagaState.InventoryReservationId,
            PaymentAttemptId = sagaState.PaymentAttemptId
        };

        return Ok(response);
    }

    private static DemoScenario ResolveScenario(string? scenarioName)
    {
        if (string.IsNullOrWhiteSpace(scenarioName))
            return DemoScenario.Success;

        return scenarioName.ToLowerInvariant() switch
        {
            "success" => DemoScenario.Success,
            "inventory-fail" => DemoScenario.InventoryFail,
            "payment-fail" => DemoScenario.PaymentFail,
            "timeout" => DemoScenario.Timeout,
            _ => throw new ArgumentException($"Unknown scenario: '{scenarioName}'. Supported: success, inventory-fail, payment-fail, timeout")
        };
    }
}
