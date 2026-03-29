using FluentAssertions;
using Moq;
using MW.Saga.Context;
using MW.Saga.MassTransit.Context;
using MW.Saga.Models;

namespace MW.Saga.MassTransit.Tests.Context;

public class ScopedSagaContextAccessorTests
{
    [Fact]
    public void Current_Initially_ShouldBeNull()
    {
        var accessor = new ScopedSagaContextAccessor();

        accessor.Current.Should().BeNull();
    }

    [Fact]
    public void SetContext_ShouldSetCurrent()
    {
        var accessor = new ScopedSagaContextAccessor();
        var context = new MutableSagaContext { CorrelationId = Guid.NewGuid() };

        accessor.SetContext(context);

        accessor.Current.Should().BeSameAs(context);
    }

    [Fact]
    public void SetContext_WithNull_ShouldThrowArgumentNullException()
    {
        var accessor = new ScopedSagaContextAccessor();

        var act = () => accessor.SetContext(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("context");
    }

    [Fact]
    public void ClearContext_ShouldSetCurrentToNull()
    {
        var accessor = new ScopedSagaContextAccessor();
        accessor.SetContext(new MutableSagaContext());

        accessor.ClearContext();

        accessor.Current.Should().BeNull();
    }

    [Fact]
    public void SetContext_ThenClearContext_ShouldLeaveCurrentNull()
    {
        var accessor = new ScopedSagaContextAccessor();
        var context = new MutableSagaContext { CorrelationId = Guid.NewGuid() };

        accessor.SetContext(context);
        accessor.ClearContext();

        accessor.Current.Should().BeNull();
    }

    [Fact]
    public void SetContext_AfterClear_ShouldNotRetainStaleState()
    {
        var accessor = new ScopedSagaContextAccessor();
        var first = new MutableSagaContext { CorrelationId = Guid.NewGuid() };
        var second = new MutableSagaContext { CorrelationId = Guid.NewGuid() };

        accessor.SetContext(first);
        accessor.ClearContext();
        accessor.SetContext(second);

        accessor.Current.Should().BeSameAs(second);
        accessor.Current!.CorrelationId.Should().Be(second.CorrelationId);
    }
}
