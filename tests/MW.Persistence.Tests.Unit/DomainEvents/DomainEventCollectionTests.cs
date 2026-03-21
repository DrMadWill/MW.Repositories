using FluentAssertions;
using MW.Core.Events;
using MW.Persistence.Tests.Shared.Entities;

namespace MW.Persistence.Tests.Unit.DomainEvents;

/// <summary>
/// PTST-022: Unit tests for domain event collection on entities.
/// </summary>
public class DomainEventCollectionTests
{
    [Fact]
    public void Entity_Should_StartWithNoDomainEvents()
    {
        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test" };

        entity.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void AddDomainEvent_Should_AddToCollection()
    {
        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test" };
        var domainEvent = new TestDomainEvent("created");

        entity.AddDomainEvent(domainEvent);

        entity.DomainEvents.Should().HaveCount(1);
        entity.DomainEvents.Should().Contain(domainEvent);
    }

    [Fact]
    public void AddMultipleDomainEvents_Should_AddAll()
    {
        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test" };

        entity.AddDomainEvent(new TestDomainEvent("event1"));
        entity.AddDomainEvent(new TestDomainEvent("event2"));
        entity.AddDomainEvent(new TestDomainEvent("event3"));

        entity.DomainEvents.Should().HaveCount(3);
    }

    [Fact]
    public void RemoveDomainEvent_Should_RemoveSpecificEvent()
    {
        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test" };
        var event1 = new TestDomainEvent("keep");
        var event2 = new TestDomainEvent("remove");

        entity.AddDomainEvent(event1);
        entity.AddDomainEvent(event2);
        entity.RemoveDomainEvent(event2);

        entity.DomainEvents.Should().HaveCount(1);
        entity.DomainEvents.Should().Contain(event1);
    }

    [Fact]
    public void ClearDomainEvents_Should_RemoveAll()
    {
        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test" };
        entity.AddDomainEvent(new TestDomainEvent("event1"));
        entity.AddDomainEvent(new TestDomainEvent("event2"));

        entity.ClearDomainEvents();

        entity.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void DomainEvent_Should_HaveOccurredOnTimestamp()
    {
        var before = DateTimeOffset.UtcNow;
        var domainEvent = new TestDomainEvent("test");
        var after = DateTimeOffset.UtcNow;

        domainEvent.OccurredOn.Should().BeOnOrAfter(before);
        domainEvent.OccurredOn.Should().BeOnOrBefore(after);
    }

    [Fact]
    public void AggregateRoot_Should_SupportDomainEvents()
    {
        var aggregate = new TestAggregate { Id = Guid.NewGuid(), Title = "Test" };
        var domainEvent = new TestDomainEvent("aggregate-event");

        aggregate.AddDomainEvent(domainEvent);

        aggregate.DomainEvents.Should().HaveCount(1);
        aggregate.DomainEvents.Should().Contain(domainEvent);
    }
}
