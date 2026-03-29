namespace MW.OrderRegistration.ConsoleDemo.Domain.Enums;

/// <summary>
/// Supported demo execution scenarios.
/// Each scenario exercises a different infrastructure path.
/// </summary>
public enum DemoScenario
{
    /// <summary>Full success path: order → inventory reserved → payment succeeded → completed.</summary>
    Success,

    /// <summary>Inventory reservation fails early.</summary>
    InventoryFail,

    /// <summary>Inventory succeeds but payment fails.</summary>
    PaymentFail,

    /// <summary>Inventory succeeds but payment response never arrives (timeout).</summary>
    Timeout
}
