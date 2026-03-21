using MW.Persistence.Tests.Shared.Entities;

namespace MW.Persistence.Tests.Shared.Builders;

/// <summary>
/// Builder for creating TestEntity instances with fluent API.
/// </summary>
public class TestEntityBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _name = "Test Entity";
    private int _value;
    private DateTimeOffset _createdAt = DateTimeOffset.UtcNow;

    public TestEntityBuilder WithId(Guid id) { _id = id; return this; }
    public TestEntityBuilder WithName(string name) { _name = name; return this; }
    public TestEntityBuilder WithValue(int value) { _value = value; return this; }
    public TestEntityBuilder WithCreatedAt(DateTimeOffset createdAt) { _createdAt = createdAt; return this; }

    public TestEntity Build() => new()
    {
        Id = _id,
        Name = _name,
        Value = _value,
        CreatedAt = _createdAt
    };

    public static TestEntityBuilder Default() => new();
}

/// <summary>
/// Builder for creating SoftDeletableEntity instances with fluent API.
/// </summary>
public class SoftDeletableEntityBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _name = "Soft Deletable";
    private bool _isDeleted;
    private DateTimeOffset? _deletedAt;
    private Guid? _deletedBy;

    public SoftDeletableEntityBuilder WithId(Guid id) { _id = id; return this; }
    public SoftDeletableEntityBuilder WithName(string name) { _name = name; return this; }
    public SoftDeletableEntityBuilder AsDeleted(Guid? deletedBy = null)
    {
        _isDeleted = true;
        _deletedAt = DateTimeOffset.UtcNow;
        _deletedBy = deletedBy ?? Guid.NewGuid();
        return this;
    }

    public SoftDeletableEntity Build() => new()
    {
        Id = _id,
        Name = _name,
        IsDeleted = _isDeleted,
        DeletedAt = _deletedAt,
        DeletedBy = _deletedBy
    };

    public static SoftDeletableEntityBuilder Default() => new();
}

/// <summary>
/// Builder for creating TestAggregate instances with fluent API.
/// </summary>
public class TestAggregateBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _title = "Test Aggregate";
    private string _description = "Description";

    public TestAggregateBuilder WithId(Guid id) { _id = id; return this; }
    public TestAggregateBuilder WithTitle(string title) { _title = title; return this; }
    public TestAggregateBuilder WithDescription(string description) { _description = description; return this; }

    public TestAggregate Build() => new()
    {
        Id = _id,
        Title = _title,
        Description = _description
    };

    public static TestAggregateBuilder Default() => new();
}
