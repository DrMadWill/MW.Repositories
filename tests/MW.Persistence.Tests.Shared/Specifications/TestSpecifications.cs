using MW.Persistence.EntityFrameworkCore.Specifications;
using MW.Persistence.Tests.Shared.Entities;

namespace MW.Persistence.Tests.Shared.Specifications;

/// <summary>
/// A test specification that filters TestEntity by name.
/// </summary>
public class TestEntityByNameSpecification : BaseSpecification<TestEntity>
{
    public TestEntityByNameSpecification(string name) : base(e => e.Name == name)
    {
    }
}

/// <summary>
/// A test specification that applies ordering and paging.
/// </summary>
public class TestEntityPagedSpecification : BaseSpecification<TestEntity>
{
    public TestEntityPagedSpecification(int skip, int take)
    {
        ApplyOrderBy(e => e.Name);
        ApplyPaging(skip, take);
    }
}

/// <summary>
/// A test specification with descending ordering.
/// </summary>
public class TestEntityOrderedDescSpecification : BaseSpecification<TestEntity>
{
    public TestEntityOrderedDescSpecification()
    {
        ApplyOrderByDescending(e => e.Value);
    }
}

/// <summary>
/// A test specification with criteria, ordering and paging combined.
/// </summary>
public class TestEntityFilteredPagedSpecification : BaseSpecification<TestEntity>
{
    public TestEntityFilteredPagedSpecification(int minValue, int skip, int take)
        : base(e => e.Value >= minValue)
    {
        ApplyOrderBy(e => e.Name);
        ApplyPaging(skip, take);
    }
}

/// <summary>
/// A test specification with no criteria (should return all).
/// </summary>
public class TestEntityAllSpecification : BaseSpecification<TestEntity>
{
    public TestEntityAllSpecification()
    {
    }
}
