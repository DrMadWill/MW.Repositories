namespace MW.OrderRegistration.ApiDemo.Contracts;

/// <summary>
/// HTTP response contract for business-level order status.
/// </summary>
public class OrderStatusResponse
{
    /// <summary>The unique order identifier.</summary>
    public Guid OrderId { get; init; }

    /// <summary>Buyer/customer identifier.</summary>
    public string BuyerId { get; init; } = string.Empty;

    /// <summary>Total order amount.</summary>
    public decimal TotalAmount { get; init; }

    /// <summary>Current business order status.</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>When the order was created.</summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>When the order was completed (if applicable).</summary>
    public DateTime? CompletedAt { get; init; }

    /// <summary>When the order failed (if applicable).</summary>
    public DateTime? FailedAt { get; init; }

    /// <summary>Failure reason (if applicable).</summary>
    public string? FailureReason { get; init; }

    /// <summary>Inventory reservation result.</summary>
    public InventoryReservationResult? Reservation { get; init; }

    /// <summary>Payment attempt result.</summary>
    public PaymentAttemptResult? Payment { get; init; }

    /// <summary>Order line items.</summary>
    public List<OrderItemResult> Items { get; init; } = new();
}

/// <summary>
/// Inventory reservation result within order status.
/// </summary>
public class InventoryReservationResult
{
    public Guid ReservationId { get; init; }
    public bool IsReserved { get; init; }
    public string? FailureReason { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Payment attempt result within order status.
/// </summary>
public class PaymentAttemptResult
{
    public Guid PaymentId { get; init; }
    public decimal Amount { get; init; }
    public bool IsSuccessful { get; init; }
    public string? FailureReason { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Order line item result within order status.
/// </summary>
public class OrderItemResult
{
    public Guid ItemId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}
