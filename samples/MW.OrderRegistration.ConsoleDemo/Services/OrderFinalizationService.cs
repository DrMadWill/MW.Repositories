using Microsoft.Extensions.Logging;
using MW.OrderRegistration.ConsoleDemo.Domain.Entities;
using MW.OrderRegistration.ConsoleDemo.Domain.Enums;
using MW.Persistence.Abstractions.Repositories;
using MW.Persistence.Abstractions.UnitOfWork;

namespace MW.OrderRegistration.ConsoleDemo.Services;

/// <summary>
/// Application service for finalizing order state on saga completion/failure/timeout.
/// </summary>
public class OrderFinalizationService
{
    private readonly IRepository<Order, Guid> _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrderFinalizationService> _logger;

    public OrderFinalizationService(
        IRepository<Order, Guid> orderRepository,
        IUnitOfWork unitOfWork,
        ILogger<OrderFinalizationService> logger)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task FinalizeOrderAsync(Guid orderId, OrderStatus status, string? reason = null)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            _logger.LogWarning("[OrderFinalization] Order not found — OrderId={OrderId}", orderId);
            return;
        }

        order.Status = status;
        order.FailureReason = reason;

        switch (status)
        {
            case OrderStatus.Completed:
                order.CompletedAt = DateTime.UtcNow;
                break;
            case OrderStatus.Failed:
            case OrderStatus.TimedOut:
                order.FailedAt = DateTime.UtcNow;
                break;
        }

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation(
            "[OrderFinalization] Order finalized — OrderId={OrderId}, Status={Status}, Reason={Reason}",
            orderId, status, reason ?? "N/A");
    }
}
