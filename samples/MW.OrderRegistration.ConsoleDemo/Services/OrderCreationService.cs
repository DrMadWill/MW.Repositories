using Microsoft.Extensions.Logging;
using MW.OrderRegistration.ConsoleDemo.Domain.Entities;
using MW.OrderRegistration.ConsoleDemo.Domain.Enums;
using MW.Persistence.Abstractions.Repositories;
using MW.Persistence.Abstractions.UnitOfWork;

namespace MW.OrderRegistration.ConsoleDemo.Services;

/// <summary>
/// Application service for creating a demo order through repository abstractions.
/// </summary>
public class OrderCreationService
{
    private readonly IRepository<Order, Guid> _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrderCreationService> _logger;

    public OrderCreationService(
        IRepository<Order, Guid> orderRepository,
        IUnitOfWork unitOfWork,
        ILogger<OrderCreationService> logger)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Order> CreateOrderAsync(string buyerId, List<(string ProductName, int Quantity, decimal UnitPrice)> items)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            BuyerId = buyerId,
            Status = OrderStatus.Pending,
            TotalAmount = items.Sum(i => i.Quantity * i.UnitPrice),
            CreatedAt = DateTime.UtcNow,
            Items = items.Select(i => new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        await _orderRepository.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation(
            "[OrderService] Order created — OrderId={OrderId}, BuyerId={BuyerId}, Total={TotalAmount:C}",
            order.Id, order.BuyerId, order.TotalAmount);

        return order;
    }
}
