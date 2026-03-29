using MW.Core.Entities;

namespace MW.OrderRegistration.ConsoleDemo.Domain.Entities;

/// <summary>
/// Minimal demo order item entity.
/// </summary>
public class OrderItem : Entity<Guid>
{
    public Guid OrderId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public Order? Order { get; set; }
}
