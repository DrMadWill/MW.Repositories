using MW.Core.Entities;

namespace MW.Persistence.Tests.Shared.Entities;

/// <summary>
/// A test entity implementing ISoftDelete for soft-delete behavior validation.
/// </summary>
public class SoftDeletableEntity : Entity<Guid>, ISoftDelete
{
    public string Name { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
