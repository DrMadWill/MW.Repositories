using MassTransit;
using Microsoft.Extensions.Logging;
using MW.OrderRegistration.ConsoleDemo.Configuration;
using MW.OrderRegistration.ConsoleDemo.Domain.Enums;
using MW.OrderRegistration.ConsoleDemo.Events;
using MW.OrderRegistration.ConsoleDemo.Services;

namespace MW.OrderRegistration.ConsoleDemo.Consumers;

/// <summary>
/// Demo consumer that reacts to payment requests.
/// In the Timeout scenario, this consumer does not respond, allowing the saga to time out.
/// </summary>
public class PaymentRequestedConsumer : IConsumer<PaymentRequested>
{
    private readonly PaymentService _paymentService;
    private readonly DemoScenario _scenario;
    private readonly ILogger<PaymentRequestedConsumer> _logger;

    public PaymentRequestedConsumer(
        PaymentService paymentService,
        DemoSettings settings,
        ILogger<PaymentRequestedConsumer> logger)
    {
        _paymentService = paymentService;
        _scenario = settings.ResolvedScenario;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentRequested> context)
    {
        var orderId = context.Message.OrderId;
        var amount = context.Message.Amount;

        _logger.LogInformation(
            "[Consumer] PaymentRequested — OrderId={OrderId}, Amount={Amount:C}, Scenario={Scenario}",
            orderId, amount, _scenario);

        // In the timeout scenario, do not respond — let the saga timeout fire
        if (_scenario == DemoScenario.Timeout)
        {
            _logger.LogWarning(
                "[Consumer] Simulating payment timeout — OrderId={OrderId} (no response will be sent)",
                orderId);
            return;
        }

        var shouldSucceed = _scenario != DemoScenario.PaymentFail;
        var payment = await _paymentService.ProcessPaymentAsync(orderId, amount, shouldSucceed);

        if (shouldSucceed)
        {
            await context.Publish(new PaymentSucceeded
            {
                CorrelationId = context.Message.CorrelationId,
                OrderId = orderId,
                PaymentId = payment.Id
            });
        }
        else
        {
            await context.Publish(new PaymentFailed
            {
                CorrelationId = context.Message.CorrelationId,
                OrderId = orderId,
                Reason = payment.FailureReason ?? "Payment failed"
            });
        }
    }
}
