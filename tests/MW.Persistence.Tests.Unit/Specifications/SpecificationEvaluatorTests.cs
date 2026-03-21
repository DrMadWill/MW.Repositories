using FluentAssertions;
using MW.Persistence.EntityFrameworkCore.Evaluators;
using MW.Persistence.EntityFrameworkCore.Specifications;
using MW.Persistence.Tests.Shared.Entities;
using MW.Persistence.Tests.Shared.Specifications;

namespace MW.Persistence.Tests.Unit.Specifications;

/// <summary>
/// PTST-014: Unit tests for SpecificationEvaluator.
/// Tests specification evaluation against in-memory queryable sources.
/// </summary>
public class SpecificationEvaluatorTests
{
    private static readonly List<TestEntity> _testData = new()
    {
        new TestEntity { Id = Guid.NewGuid(), Name = "Alpha", Value = 10 },
        new TestEntity { Id = Guid.NewGuid(), Name = "Beta", Value = 20 },
        new TestEntity { Id = Guid.NewGuid(), Name = "Charlie", Value = 30 },
        new TestEntity { Id = Guid.NewGuid(), Name = "Delta", Value = 40 },
        new TestEntity { Id = Guid.NewGuid(), Name = "Echo", Value = 50 }
    };

    [Fact]
    public void GetQuery_Should_ApplyCriteriaFilter()
    {
        var spec = new TestEntityByNameSpecification("Beta");

        var result = SpecificationEvaluator.GetQuery(_testData.AsQueryable(), spec).ToList();

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Beta");
    }

    [Fact]
    public void GetQuery_Should_ApplyPaging()
    {
        var spec = new TestEntityPagedSpecification(skip: 1, take: 2);

        var result = SpecificationEvaluator.GetQuery(_testData.AsQueryable(), spec).ToList();

        result.Should().HaveCount(2);
    }

    [Fact]
    public void GetQuery_Should_ApplyOrderByAscending()
    {
        var spec = new TestEntityPagedSpecification(skip: 0, take: 5);

        var result = SpecificationEvaluator.GetQuery(_testData.AsQueryable(), spec).ToList();

        result.Should().BeInAscendingOrder(e => e.Name);
    }

    [Fact]
    public void GetQuery_Should_ApplyOrderByDescending()
    {
        var spec = new TestEntityOrderedDescSpecification();

        var result = SpecificationEvaluator.GetQuery(_testData.AsQueryable(), spec).ToList();

        result.Should().BeInDescendingOrder(e => e.Value);
    }

    [Fact]
    public void GetQuery_Should_CombineCriteriaAndPaging()
    {
        var spec = new TestEntityFilteredPagedSpecification(minValue: 20, skip: 0, take: 2);

        var result = SpecificationEvaluator.GetQuery(_testData.AsQueryable(), spec).ToList();

        result.Should().HaveCount(2);
        result.Should().OnlyContain(e => e.Value >= 20);
    }

    [Fact]
    public void GetQuery_Should_ReturnAll_WhenSpecHasNoCriteria()
    {
        var spec = new TestEntityAllSpecification();

        var result = SpecificationEvaluator.GetQuery(_testData.AsQueryable(), spec).ToList();

        result.Should().HaveCount(5);
    }

    [Fact]
    public void GetQuery_Should_ApplySkipOnly()
    {
        var spec = new TestEntityFilteredPagedSpecification(minValue: 0, skip: 3, take: 100);

        var result = SpecificationEvaluator.GetQuery(_testData.AsQueryable(), spec).ToList();

        result.Should().HaveCount(2);
    }
}

/// <summary>
/// PTST-014 (extended): Tests for BaseSpecification behavior.
/// </summary>
public class BaseSpecificationTests
{
    [Fact]
    public void IsSatisfiedBy_Should_ReturnTrue_WhenEntityMatchesCriteria()
    {
        var spec = new TestEntityByNameSpecification("Alpha");
        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Alpha", Value = 10 };

        spec.IsSatisfiedBy(entity).Should().BeTrue();
    }

    [Fact]
    public void IsSatisfiedBy_Should_ReturnFalse_WhenEntityDoesNotMatch()
    {
        var spec = new TestEntityByNameSpecification("Alpha");
        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Beta", Value = 20 };

        spec.IsSatisfiedBy(entity).Should().BeFalse();
    }

    [Fact]
    public void IsSatisfiedBy_Should_ReturnTrue_WhenNoCriteria()
    {
        var spec = new TestEntityAllSpecification();
        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Any", Value = 99 };

        spec.IsSatisfiedBy(entity).Should().BeTrue();
    }

    [Fact]
    public void Specification_Should_ExposeProperties()
    {
        var spec = new TestEntityFilteredPagedSpecification(minValue: 10, skip: 5, take: 20);

        spec.Criteria.Should().NotBeNull();
        spec.OrderBy.Should().NotBeNull();
        spec.Skip.Should().Be(5);
        spec.Take.Should().Be(20);
    }

    [Fact]
    public void Specification_WithoutCriteria_Should_HaveNullCriteria()
    {
        var spec = new TestEntityAllSpecification();

        spec.Criteria.Should().BeNull();
    }
}
