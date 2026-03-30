namespace MW.OrderRegistration.ApiDemo.Contracts;

/// <summary>
/// HTTP response contract returned after starting an order registration process.
/// </summary>
public class RegisterOrderResponse
{
    /// <summary>The unique order identifier.</summary>
    public Guid OrderId { get; init; }

    /// <summary>Correlation identifier used for saga tracking.</summary>
    public Guid CorrelationId { get; init; }

    /// <summary>Initial order status.</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>Demo scenario that was applied.</summary>
    public string Scenario { get; init; } = string.Empty;

    /// <summary>URL to check business order status.</summary>
    public string? OrderStatusUrl { get; init; }

    /// <summary>URL to check saga/process status.</summary>
    public string? ProcessStatusUrl { get; init; }
}
