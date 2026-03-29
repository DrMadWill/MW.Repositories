using MassTransit;
using Microsoft.Extensions.Logging;
using MW.OrderRegistration.ConsoleDemo.Events;
using MW.Saga.Models;

namespace MW.OrderRegistration.ConsoleDemo.Saga;

/// <summary>
/// MassTransit saga state machine that coordinates the order registration process.
/// Transitions: start → awaiting inventory → awaiting payment → completed/failed/timed out.
/// </summary>
public class OrderRegistrationStateMachine : MassTransitStateMachine<OrderRegistrationSagaState>
{
    private readonly ILogger<OrderRegistrationStateMachine> _logger;

    public State AwaitingInventory { get; private set; } = null!;
    public State AwaitingPayment { get; private set; } = null!;
    public State OrderCompleted { get; private set; } = null!;
    public State OrderFailed { get; private set; } = null!;
    public State OrderTimedOut { get; private set; } = null!;

    public Event<OrderRegistrationStarted> OrderRegistrationStartedEvent { get; private set; } = null!;
    public Event<InventoryReserved> InventoryReservedEvent { get; private set; } = null!;
    public Event<InventoryReservationFailed> InventoryReservationFailedEvent { get; private set; } = null!;
    public Event<PaymentSucceeded> PaymentSucceededEvent { get; private set; } = null!;
    public Event<PaymentFailed> PaymentFailedEvent { get; private set; } = null!;
    public Event<PaymentTimeoutExpired> PaymentTimeoutExpiredEvent { get; private set; } = null!;

    public Schedule<OrderRegistrationSagaState, PaymentTimeoutExpired> PaymentTimeoutSchedule { get; private set; } = null!;

    public OrderRegistrationStateMachine(ILogger<OrderRegistrationStateMachine> logger)
    {
        _logger = logger;

        InstanceState(x => x.CurrentState);

        // Correlation: all events correlate on OrderId → CorrelationId
        Event(() => OrderRegistrationStartedEvent, e => e.CorrelateById(ctx => ctx.Message.OrderId));
        Event(() => InventoryReservedEvent, e => e.CorrelateById(ctx => ctx.Message.OrderId));
        Event(() => InventoryReservationFailedEvent, e => e.CorrelateById(ctx => ctx.Message.OrderId));
        Event(() => PaymentSucceededEvent, e => e.CorrelateById(ctx => ctx.Message.OrderId));
        Event(() => PaymentFailedEvent, e => e.CorrelateById(ctx => ctx.Message.OrderId));
        Event(() => PaymentTimeoutExpiredEvent, e => e.CorrelateById(ctx => ctx.Message.OrderId));

        // Payment timeout schedule
        Schedule(() => PaymentTimeoutSchedule, instance => instance.PaymentTimeoutTokenId, s =>
        {
            s.Delay = TimeSpan.FromSeconds(30);
            s.Received = r => r.CorrelateById(ctx => ctx.Message.OrderId);
        });

        // Initial state → AwaitingInventory
        Initially(
            When(OrderRegistrationStartedEvent)
                .Then(ctx =>
                {
                    ctx.Saga.OrderId = ctx.Message.OrderId;
                    ctx.Saga.BuyerId = ctx.Message.BuyerId;
                    ctx.Saga.TotalAmount = ctx.Message.TotalAmount;
                    ctx.Saga.CreatedAt = DateTime.UtcNow;
                    ctx.Saga.UpdatedAt = DateTime.UtcNow;
                    ctx.Saga.Status = SagaStatus.Running;
                    ctx.Saga.CorrelationId = ctx.Message.OrderId;

                    _logger.LogInformation(
                        "[Saga] Started — OrderId={OrderId}, BuyerId={BuyerId}, CorrelationId={CorrelationId}",
                        ctx.Message.OrderId, ctx.Message.BuyerId, ctx.Saga.CorrelationId);
                })
                .Publish(ctx => new InventoryReservationRequested
                {
                    CorrelationId = ctx.Saga.CorrelationId.ToString(),
                    OrderId = ctx.Saga.OrderId
                })
                .TransitionTo(AwaitingInventory)
        );

        // AwaitingInventory → AwaitingPayment or Failed
        During(AwaitingInventory,
            When(InventoryReservedEvent)
                .Then(ctx =>
                {
                    ctx.Saga.InventoryReservationId = ctx.Message.ReservationId;
                    ctx.Saga.UpdatedAt = DateTime.UtcNow;

                    _logger.LogInformation(
                        "[Saga] Inventory reserved — OrderId={OrderId}, ReservationId={ReservationId}",
                        ctx.Saga.OrderId, ctx.Message.ReservationId);
                })
                .Publish(ctx => new PaymentRequested
                {
                    CorrelationId = ctx.Saga.CorrelationId.ToString(),
                    OrderId = ctx.Saga.OrderId,
                    Amount = ctx.Saga.TotalAmount
                })
                .Schedule(PaymentTimeoutSchedule, ctx => new PaymentTimeoutExpired
                {
                    CorrelationId = ctx.Saga.CorrelationId.ToString(),
                    OrderId = ctx.Saga.OrderId
                })
                .TransitionTo(AwaitingPayment),

            When(InventoryReservationFailedEvent)
                .Then(ctx =>
                {
                    ctx.Saga.FailureReason = ctx.Message.Reason;
                    ctx.Saga.FailedAt = DateTime.UtcNow;
                    ctx.Saga.UpdatedAt = DateTime.UtcNow;
                    ctx.Saga.Status = SagaStatus.Failed;

                    _logger.LogWarning(
                        "[Saga] Inventory reservation failed — OrderId={OrderId}, Reason={Reason}",
                        ctx.Saga.OrderId, ctx.Message.Reason);
                })
                .Publish(ctx => new OrderRegistrationFailed
                {
                    CorrelationId = ctx.Saga.CorrelationId.ToString(),
                    OrderId = ctx.Saga.OrderId,
                    Reason = ctx.Message.Reason
                })
                .TransitionTo(OrderFailed)
                .Finalize()
        );

        // AwaitingPayment → Completed, Failed, or TimedOut
        During(AwaitingPayment,
            When(PaymentSucceededEvent)
                .Unschedule(PaymentTimeoutSchedule)
                .Then(ctx =>
                {
                    ctx.Saga.PaymentAttemptId = ctx.Message.PaymentId;
                    ctx.Saga.CompletedAt = DateTime.UtcNow;
                    ctx.Saga.UpdatedAt = DateTime.UtcNow;
                    ctx.Saga.Status = SagaStatus.Completed;

                    _logger.LogInformation(
                        "[Saga] Payment succeeded — OrderId={OrderId}, PaymentId={PaymentId}",
                        ctx.Saga.OrderId, ctx.Message.PaymentId);
                })
                .Publish(ctx => new OrderRegistrationCompleted
                {
                    CorrelationId = ctx.Saga.CorrelationId.ToString(),
                    OrderId = ctx.Saga.OrderId
                })
                .TransitionTo(OrderCompleted)
                .Finalize(),

            When(PaymentFailedEvent)
                .Unschedule(PaymentTimeoutSchedule)
                .Then(ctx =>
                {
                    ctx.Saga.FailureReason = ctx.Message.Reason;
                    ctx.Saga.FailedAt = DateTime.UtcNow;
                    ctx.Saga.UpdatedAt = DateTime.UtcNow;
                    ctx.Saga.Status = SagaStatus.Failed;

                    _logger.LogWarning(
                        "[Saga] Payment failed — OrderId={OrderId}, Reason={Reason}",
                        ctx.Saga.OrderId, ctx.Message.Reason);
                })
                .Publish(ctx => new OrderRegistrationFailed
                {
                    CorrelationId = ctx.Saga.CorrelationId.ToString(),
                    OrderId = ctx.Saga.OrderId,
                    Reason = ctx.Message.Reason
                })
                .TransitionTo(OrderFailed)
                .Finalize(),

            When(PaymentTimeoutSchedule!.Received)
                .Then(ctx =>
                {
                    ctx.Saga.FailureReason = "Payment response timed out";
                    ctx.Saga.FailedAt = DateTime.UtcNow;
                    ctx.Saga.UpdatedAt = DateTime.UtcNow;
                    ctx.Saga.Status = SagaStatus.TimedOut;

                    _logger.LogWarning(
                        "[Saga] Payment timed out — OrderId={OrderId}",
                        ctx.Saga.OrderId);
                })
                .Publish(ctx => new OrderRegistrationTimedOut
                {
                    CorrelationId = ctx.Saga.CorrelationId.ToString(),
                    OrderId = ctx.Saga.OrderId
                })
                .TransitionTo(OrderTimedOut)
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }
}
