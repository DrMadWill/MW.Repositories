using MW.Core.Entities;

namespace MW.OrderRegistration.ConsoleDemo.Domain.Entities;

/// <summary>
/// Simple demo entity used by debug/test API endpoints for validating
/// repository and unit-of-work abstractions independently.
/// Not a business entity — exists solely for package-level testing.
/// </summary>
public class TestItem : Entity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
