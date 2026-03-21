using FluentAssertions;
using MW.Persistence.EntityFrameworkCore.Querying;

namespace MW.Persistence.Tests.Unit.Querying;

/// <summary>
/// PTST-015: Unit tests for QueryOptions mapping behavior.
/// </summary>
public class QueryOptionsTests
{
    [Fact]
    public void Default_Should_HaveAsNoTrackingTrue()
    {
        var options = QueryOptions.Default;

        options.AsNoTracking.Should().BeTrue();
    }

    [Fact]
    public void Default_Should_HaveIgnoreQueryFiltersFalse()
    {
        var options = QueryOptions.Default;

        options.IgnoreQueryFilters.Should().BeFalse();
    }

    [Fact]
    public void Default_Should_HaveIncludeSoftDeletedFalse()
    {
        var options = QueryOptions.Default;

        options.IncludeSoftDeleted.Should().BeFalse();
    }

    [Fact]
    public void Tracked_Should_HaveAsNoTrackingFalse()
    {
        var options = QueryOptions.Tracked;

        options.AsNoTracking.Should().BeFalse();
    }

    [Fact]
    public void CustomOptions_Should_SupportInit()
    {
        var options = new QueryOptions
        {
            AsNoTracking = false,
            IgnoreQueryFilters = true,
            IncludeSoftDeleted = true
        };

        options.AsNoTracking.Should().BeFalse();
        options.IgnoreQueryFilters.Should().BeTrue();
        options.IncludeSoftDeleted.Should().BeTrue();
    }
}

/// <summary>
/// PTST-016: Unit tests for SoftDeleteFilter behavior.
/// </summary>
public class SoftDeleteFilterTests
{
    [Fact]
    public void Default_Should_ExcludeDeleted()
    {
        var filter = SoftDeleteFilter.Default;

        filter.IncludeDeleted.Should().BeFalse();
        filter.OnlyDeleted.Should().BeFalse();
    }

    [Fact]
    public void WithDeleted_Should_IncludeDeleted()
    {
        var filter = SoftDeleteFilter.WithDeleted;

        filter.IncludeDeleted.Should().BeTrue();
        filter.OnlyDeleted.Should().BeFalse();
    }

    [Fact]
    public void DeletedOnly_Should_ReturnOnlyDeleted()
    {
        var filter = SoftDeleteFilter.DeletedOnly;

        filter.OnlyDeleted.Should().BeTrue();
    }

    [Fact]
    public void CustomFilter_Should_SupportInit()
    {
        var filter = new SoftDeleteFilter
        {
            IncludeDeleted = true,
            OnlyDeleted = true
        };

        filter.IncludeDeleted.Should().BeTrue();
        filter.OnlyDeleted.Should().BeTrue();
    }
}
