using MW.Core.Entities;

namespace MW.Persistence.Tests.Shared.Entities;

/// <summary>
/// A simple test entity for repository validation.
/// </summary>
public class TestEntity : Entity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
