using MW.Core.Entities;

namespace MW.OrderRegistration.ConsoleDemo.Domain.Entities;

/// <summary>
/// Minimal demo payment attempt entity.
/// </summary>
public class PaymentAttempt : Entity<Guid>
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public bool IsSuccessful { get; set; }
    public string? FailureReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
