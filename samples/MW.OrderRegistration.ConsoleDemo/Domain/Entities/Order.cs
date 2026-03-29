using MW.Core.Entities;
using MW.OrderRegistration.ConsoleDemo.Domain.Enums;

namespace MW.OrderRegistration.ConsoleDemo.Domain.Entities;

/// <summary>
/// Minimal demo order entity for the order registration scenario.
/// </summary>
public class Order : Entity<Guid>
{
    public string BuyerId { get; set; } = string.Empty;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public string? FailureReason { get; set; }

    public List<OrderItem> Items { get; set; } = new();
}
