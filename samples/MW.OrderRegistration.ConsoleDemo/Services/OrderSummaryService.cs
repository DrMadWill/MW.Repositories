using Microsoft.Extensions.Logging;
using MW.OrderRegistration.ConsoleDemo.Domain.Entities;
using MW.OrderRegistration.ConsoleDemo.Saga;
using MW.Persistence.Abstractions.Repositories;

namespace MW.OrderRegistration.ConsoleDemo.Services;

/// <summary>
/// Queries final business and saga data and outputs a deterministic summary.
/// </summary>
public class OrderSummaryService
{
    private readonly IReadRepository<Order, Guid> _orderRepository;
    private readonly IReadRepository<InventoryReservation, Guid> _reservationRepository;
    private readonly IReadRepository<PaymentAttempt, Guid> _paymentRepository;
    private readonly ILogger<OrderSummaryService> _logger;

    public OrderSummaryService(
        IReadRepository<Order, Guid> orderRepository,
        IReadRepository<InventoryReservation, Guid> reservationRepository,
        IReadRepository<PaymentAttempt, Guid> paymentRepository,
        ILogger<OrderSummaryService> logger)
    {
        _orderRepository = orderRepository;
        _reservationRepository = reservationRepository;
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public async Task PrintSummaryAsync(Guid orderId, OrderRegistrationSagaState? sagaState)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        var reservations = await _reservationRepository.FindAsync(r => r.OrderId == orderId);
        var payments = await _paymentRepository.FindAsync(p => p.OrderId == orderId);

        Console.WriteLine();
        Console.WriteLine("═══════════════════════════════════════════════════════════");
        Console.WriteLine("                   ORDER REGISTRATION SUMMARY              ");
        Console.WriteLine("═══════════════════════════════════════════════════════════");

        if (order != null)
        {
            Console.WriteLine($"  Order ID          : {order.Id}");
            Console.WriteLine($"  Buyer ID          : {order.BuyerId}");
            Console.WriteLine($"  Total Amount      : {order.TotalAmount:C}");
            Console.WriteLine($"  Order Status      : {order.Status}");
            Console.WriteLine($"  Created At        : {order.CreatedAt:O}");
            Console.WriteLine($"  Completed At      : {order.CompletedAt?.ToString("O") ?? "N/A"}");
            Console.WriteLine($"  Failed At         : {order.FailedAt?.ToString("O") ?? "N/A"}");
            Console.WriteLine($"  Failure Reason    : {order.FailureReason ?? "N/A"}");
        }
        else
        {
            Console.WriteLine("  Order: NOT FOUND");
        }

        Console.WriteLine("───────────────────────────────────────────────────────────");

        if (sagaState != null)
        {
            Console.WriteLine($"  Correlation ID    : {sagaState.CorrelationId}");
            Console.WriteLine($"  Saga State        : {sagaState.CurrentState}");
            Console.WriteLine($"  Saga Status       : {sagaState.Status}");
            Console.WriteLine($"  Reservation ID    : {sagaState.InventoryReservationId?.ToString() ?? "N/A"}");
            Console.WriteLine($"  Payment ID        : {sagaState.PaymentAttemptId?.ToString() ?? "N/A"}");
            Console.WriteLine($"  Saga Created At   : {sagaState.CreatedAt:O}");
            Console.WriteLine($"  Saga Completed At : {sagaState.CompletedAt?.ToString("O") ?? "N/A"}");
            Console.WriteLine($"  Saga Failed At    : {sagaState.FailedAt?.ToString("O") ?? "N/A"}");
            Console.WriteLine($"  Saga Failure      : {sagaState.FailureReason ?? "N/A"}");
        }
        else
        {
            Console.WriteLine("  Saga State: NOT FOUND (may have been finalized/removed)");
        }

        Console.WriteLine("───────────────────────────────────────────────────────────");

        if (reservations.Count > 0)
        {
            foreach (var r in reservations)
            {
                Console.WriteLine($"  Inventory Reservation: {r.Id} — Reserved={r.IsReserved}, Reason={r.FailureReason ?? "N/A"}");
            }
        }
        else
        {
            Console.WriteLine("  Inventory Reservations: NONE");
        }

        if (payments.Count > 0)
        {
            foreach (var p in payments)
            {
                Console.WriteLine($"  Payment Attempt: {p.Id} — Success={p.IsSuccessful}, Amount={p.Amount:C}, Reason={p.FailureReason ?? "N/A"}");
            }
        }
        else
        {
            Console.WriteLine("  Payment Attempts: NONE");
        }

        Console.WriteLine("═══════════════════════════════════════════════════════════");
        Console.WriteLine();

        _logger.LogInformation(
            "[Summary] OrderId={OrderId}, OrderStatus={OrderStatus}, SagaState={SagaState}",
            orderId, order?.Status.ToString() ?? "N/A", sagaState?.CurrentState ?? "N/A");
    }
}
