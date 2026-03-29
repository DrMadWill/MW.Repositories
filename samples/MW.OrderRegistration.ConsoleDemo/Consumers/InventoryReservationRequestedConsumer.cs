using MassTransit;
using Microsoft.Extensions.Logging;
using MW.OrderRegistration.ConsoleDemo.Configuration;
using MW.OrderRegistration.ConsoleDemo.Domain.Enums;
using MW.OrderRegistration.ConsoleDemo.Events;
using MW.OrderRegistration.ConsoleDemo.Services;

namespace MW.OrderRegistration.ConsoleDemo.Consumers;

/// <summary>
/// Demo consumer that reacts to inventory reservation requests.
/// Behavior is driven by the configured DemoScenario.
/// </summary>
public class InventoryReservationRequestedConsumer : IConsumer<InventoryReservationRequested>
{
    private readonly InventoryService _inventoryService;
    private readonly DemoScenario _scenario;
    private readonly ILogger<InventoryReservationRequestedConsumer> _logger;

    public InventoryReservationRequestedConsumer(
        InventoryService inventoryService,
        DemoSettings settings,
        ILogger<InventoryReservationRequestedConsumer> logger)
    {
        _inventoryService = inventoryService;
        _scenario = settings.ResolvedScenario;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<InventoryReservationRequested> context)
    {
        var orderId = context.Message.OrderId;
        var shouldSucceed = _scenario != DemoScenario.InventoryFail;

        _logger.LogInformation(
            "[Consumer] InventoryReservationRequested — OrderId={OrderId}, Scenario={Scenario}, WillSucceed={WillSucceed}",
            orderId, _scenario, shouldSucceed);

        var reservation = await _inventoryService.ReserveInventoryAsync(orderId, shouldSucceed);

        if (shouldSucceed)
        {
            await context.Publish(new InventoryReserved
            {
                CorrelationId = context.Message.CorrelationId,
                OrderId = orderId,
                ReservationId = reservation.Id
            });
        }
        else
        {
            await context.Publish(new InventoryReservationFailed
            {
                CorrelationId = context.Message.CorrelationId,
                OrderId = orderId,
                Reason = reservation.FailureReason ?? "Inventory reservation failed"
            });
        }
    }
}
