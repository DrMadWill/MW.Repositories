using MassTransit;
using MW.Saga.Models;

namespace MW.OrderRegistration.ConsoleDemo.Saga;

/// <summary>
/// Saga state model for the order registration workflow.
/// Tracks the long-running process data for the demo scenario.
/// </summary>
public class OrderRegistrationSagaState : SagaStateBase, SagaStateMachineInstance
{
    public Guid OrderId { get; set; }
    public string BuyerId { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public Guid? InventoryReservationId { get; set; }
    public Guid? PaymentAttemptId { get; set; }
    public string? FailureReason { get; set; }
    public SagaStatus Status { get; set; } = SagaStatus.NotStarted;

    /// <summary>
    /// Timeout token for the payment stage scheduler.
    /// </summary>
    public Guid? PaymentTimeoutTokenId { get; set; }
}
