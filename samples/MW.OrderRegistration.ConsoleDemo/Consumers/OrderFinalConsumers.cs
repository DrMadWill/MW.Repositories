using MassTransit;
using Microsoft.Extensions.Logging;
using MW.OrderRegistration.ConsoleDemo.Domain.Enums;
using MW.OrderRegistration.ConsoleDemo.Events;
using MW.OrderRegistration.ConsoleDemo.Services;

namespace MW.OrderRegistration.ConsoleDemo.Consumers;

/// <summary>
/// Consumer that finalizes order as completed when saga succeeds.
/// </summary>
public class OrderRegistrationCompletedConsumer : IConsumer<OrderRegistrationCompleted>
{
    private readonly OrderFinalizationService _finalizationService;
    private readonly ILogger<OrderRegistrationCompletedConsumer> _logger;

    public OrderRegistrationCompletedConsumer(
        OrderFinalizationService finalizationService,
        ILogger<OrderRegistrationCompletedConsumer> logger)
    {
        _finalizationService = finalizationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderRegistrationCompleted> context)
    {
        _logger.LogInformation("[Consumer] OrderRegistrationCompleted — OrderId={OrderId}", context.Message.OrderId);
        await _finalizationService.FinalizeOrderAsync(context.Message.OrderId, OrderStatus.Completed);
    }
}

/// <summary>
/// Consumer that finalizes order as failed when saga fails.
/// </summary>
public class OrderRegistrationFailedConsumer : IConsumer<OrderRegistrationFailed>
{
    private readonly OrderFinalizationService _finalizationService;
    private readonly ILogger<OrderRegistrationFailedConsumer> _logger;

    public OrderRegistrationFailedConsumer(
        OrderFinalizationService finalizationService,
        ILogger<OrderRegistrationFailedConsumer> logger)
    {
        _finalizationService = finalizationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderRegistrationFailed> context)
    {
        _logger.LogWarning("[Consumer] OrderRegistrationFailed — OrderId={OrderId}, Reason={Reason}",
            context.Message.OrderId, context.Message.Reason);
        await _finalizationService.FinalizeOrderAsync(context.Message.OrderId, OrderStatus.Failed, context.Message.Reason);
    }
}

/// <summary>
/// Consumer that finalizes order as timed out when saga times out.
/// </summary>
public class OrderRegistrationTimedOutConsumer : IConsumer<OrderRegistrationTimedOut>
{
    private readonly OrderFinalizationService _finalizationService;
    private readonly ILogger<OrderRegistrationTimedOutConsumer> _logger;

    public OrderRegistrationTimedOutConsumer(
        OrderFinalizationService finalizationService,
        ILogger<OrderRegistrationTimedOutConsumer> logger)
    {
        _finalizationService = finalizationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderRegistrationTimedOut> context)
    {
        _logger.LogWarning("[Consumer] OrderRegistrationTimedOut — OrderId={OrderId}", context.Message.OrderId);
        await _finalizationService.FinalizeOrderAsync(context.Message.OrderId, OrderStatus.TimedOut, "Payment response timed out");
    }
}
