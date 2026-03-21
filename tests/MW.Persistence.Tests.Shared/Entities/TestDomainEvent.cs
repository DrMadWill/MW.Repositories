using MW.Core.Events;

namespace MW.Persistence.Tests.Shared.Entities;

/// <summary>
/// A test domain event for domain event collection validation.
/// </summary>
public class TestDomainEvent : DomainEventBase
{
    public string EventData { get; }

    public TestDomainEvent(string eventData)
    {
        EventData = eventData;
    }
}
