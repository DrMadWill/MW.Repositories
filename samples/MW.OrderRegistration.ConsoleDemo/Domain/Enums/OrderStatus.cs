namespace MW.OrderRegistration.ConsoleDemo.Domain.Enums;

/// <summary>
/// Business-level order status for the demo flow.
/// Tracked independently from saga lifecycle status.
/// </summary>
public enum OrderStatus
{
    Pending = 0,
    AwaitingInventory = 1,
    AwaitingPayment = 2,
    Completed = 3,
    Failed = 4,
    TimedOut = 5
}
