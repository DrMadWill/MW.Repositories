using MW.Core.Abstractions;
using MW.Core.Entities;

namespace MW.Persistence.Tests.Shared.Entities;

/// <summary>
/// A test entity implementing IHasConcurrencyToken for concurrency handling validation.
/// </summary>
public class ConcurrentEntity : Entity<Guid>, IHasConcurrencyToken
{
    public string Name { get; set; } = string.Empty;
    public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
}
