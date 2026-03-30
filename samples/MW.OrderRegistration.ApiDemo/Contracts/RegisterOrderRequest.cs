using System.ComponentModel.DataAnnotations;

namespace MW.OrderRegistration.ApiDemo.Contracts;

/// <summary>
/// HTTP request contract for registering a new order.
/// </summary>
public class RegisterOrderRequest
{
    /// <summary>Buyer/customer identifier.</summary>
    [Required(ErrorMessage = "BuyerId is required.")]
    [StringLength(256, MinimumLength = 1, ErrorMessage = "BuyerId must be between 1 and 256 characters.")]
    public string BuyerId { get; init; } = string.Empty;

    /// <summary>Order line items.</summary>
    [Required(ErrorMessage = "At least one item is required.")]
    [MinLength(1, ErrorMessage = "At least one item is required.")]
    public List<OrderItemRequest> Items { get; init; } = new();

    /// <summary>
    /// Optional demo scenario override. Supported: success, inventory-fail, payment-fail, timeout.
    /// If not provided, the default scenario from configuration is used.
    /// </summary>
    public string? Scenario { get; init; }
}

/// <summary>
/// HTTP request contract for an order line item.
/// </summary>
public class OrderItemRequest
{
    /// <summary>Product name.</summary>
    [Required(ErrorMessage = "ProductName is required.")]
    [StringLength(256, MinimumLength = 1, ErrorMessage = "ProductName must be between 1 and 256 characters.")]
    public string ProductName { get; init; } = string.Empty;

    /// <summary>Quantity to order.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; init; }

    /// <summary>Unit price per item.</summary>
    [Range(0.01, double.MaxValue, ErrorMessage = "UnitPrice must be greater than zero.")]
    public decimal UnitPrice { get; init; }
}
