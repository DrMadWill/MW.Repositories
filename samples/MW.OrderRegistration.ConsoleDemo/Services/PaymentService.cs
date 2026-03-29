using Microsoft.Extensions.Logging;
using MW.OrderRegistration.ConsoleDemo.Domain.Entities;
using MW.OrderRegistration.ConsoleDemo.Domain.Enums;
using MW.Persistence.Abstractions.Repositories;
using MW.Persistence.Abstractions.UnitOfWork;

namespace MW.OrderRegistration.ConsoleDemo.Services;

/// <summary>
/// Application service for payment processing through repository abstractions.
/// </summary>
public class PaymentService
{
    private readonly IRepository<PaymentAttempt, Guid> _paymentRepository;
    private readonly IRepository<Order, Guid> _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        IRepository<PaymentAttempt, Guid> paymentRepository,
        IRepository<Order, Guid> orderRepository,
        IUnitOfWork unitOfWork,
        ILogger<PaymentService> logger)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PaymentAttempt> ProcessPaymentAsync(Guid orderId, decimal amount, bool shouldSucceed)
    {
        var payment = new PaymentAttempt
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            Amount = amount,
            IsSuccessful = shouldSucceed,
            FailureReason = shouldSucceed ? null : "Payment declined (simulated)",
            CreatedAt = DateTime.UtcNow
        };

        await _paymentRepository.AddAsync(payment);

        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order != null)
        {
            order.Status = shouldSucceed ? OrderStatus.Completed : OrderStatus.Failed;
            if (shouldSucceed)
                order.CompletedAt = DateTime.UtcNow;
            else
            {
                order.FailedAt = DateTime.UtcNow;
                order.FailureReason = payment.FailureReason;
            }
            _orderRepository.Update(order);
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation(
            "[PaymentService] Payment {Result} — OrderId={OrderId}, PaymentId={PaymentId}, Amount={Amount:C}",
            shouldSucceed ? "succeeded" : "failed", orderId, payment.Id, amount);

        return payment;
    }
}
