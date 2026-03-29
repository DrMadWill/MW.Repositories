using Microsoft.Extensions.Logging;
using MW.OrderRegistration.ConsoleDemo.Domain.Entities;
using MW.OrderRegistration.ConsoleDemo.Domain.Enums;
using MW.Persistence.Abstractions.Repositories;
using MW.Persistence.Abstractions.UnitOfWork;

namespace MW.OrderRegistration.ConsoleDemo.Services;

/// <summary>
/// Application service for inventory reservation through repository abstractions.
/// </summary>
public class InventoryService
{
    private readonly IRepository<InventoryReservation, Guid> _reservationRepository;
    private readonly IRepository<Order, Guid> _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(
        IRepository<InventoryReservation, Guid> reservationRepository,
        IRepository<Order, Guid> orderRepository,
        IUnitOfWork unitOfWork,
        ILogger<InventoryService> logger)
    {
        _reservationRepository = reservationRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<InventoryReservation> ReserveInventoryAsync(Guid orderId, bool shouldSucceed)
    {
        var reservation = new InventoryReservation
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            IsReserved = shouldSucceed,
            FailureReason = shouldSucceed ? null : "Insufficient inventory (simulated)",
            CreatedAt = DateTime.UtcNow
        };

        await _reservationRepository.AddAsync(reservation);

        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order != null)
        {
            order.Status = shouldSucceed ? OrderStatus.AwaitingPayment : OrderStatus.Failed;
            if (!shouldSucceed)
            {
                order.FailedAt = DateTime.UtcNow;
                order.FailureReason = reservation.FailureReason;
            }
            _orderRepository.Update(order);
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation(
            "[InventoryService] Reservation {Result} — OrderId={OrderId}, ReservationId={ReservationId}",
            shouldSucceed ? "succeeded" : "failed", orderId, reservation.Id);

        return reservation;
    }
}
