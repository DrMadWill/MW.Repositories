using MW.Core.Entities;

namespace MW.OrderRegistration.ConsoleDemo.Domain.Entities;

/// <summary>
/// Minimal demo inventory reservation entity.
/// </summary>
public class InventoryReservation : Entity<Guid>
{
    public Guid OrderId { get; set; }
    public bool IsReserved { get; set; }
    public string? FailureReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
