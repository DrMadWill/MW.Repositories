using MW.Core.AggregateRoots;

namespace MW.Persistence.Tests.Shared.Entities;

/// <summary>
/// A test aggregate root for aggregate repository validation.
/// </summary>
public class TestAggregate : AggregateRoot<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
