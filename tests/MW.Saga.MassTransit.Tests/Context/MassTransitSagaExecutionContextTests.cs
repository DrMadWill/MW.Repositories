using FluentAssertions;
using Moq;
using MW.Saga.Context;
using MW.Saga.MassTransit.Context;
using MW.Saga.Models;

namespace MW.Saga.MassTransit.Tests.Context;

public class MassTransitSagaExecutionContextTests
{
    [Fact]
    public void Constructor_WithNullAccessor_ShouldThrowArgumentNullException()
    {
        var act = () => new MassTransitSagaExecutionContext(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("accessor");
    }

    [Fact]
    public void ShouldImplementISagaExecutionContext()
    {
        var accessor = new ScopedSagaContextAccessor();

        var executionContext = new MassTransitSagaExecutionContext(accessor);

        executionContext.Should().BeAssignableTo<ISagaExecutionContext>();
    }

    [Fact]
    public void WhenNoContextSet_ShouldReturnDefaults()
    {
        var accessor = new ScopedSagaContextAccessor();
        var executionContext = new MassTransitSagaExecutionContext(accessor);

        executionContext.CorrelationId.Should().Be(Guid.Empty);
        executionContext.CausationId.Should().BeNull();
        executionContext.TraceId.Should().BeNull();
        executionContext.SagaName.Should().BeEmpty();
        executionContext.CurrentState.Should().BeEmpty();
        executionContext.Status.Should().Be(SagaStatus.NotStarted);
        executionContext.SourceService.Should().BeNull();
    }

    [Fact]
    public void ShouldMapAllFieldsFromMutableSagaContext()
    {
        var accessor = new ScopedSagaContextAccessor();
        var id = Guid.NewGuid();
        var context = new MutableSagaContext
        {
            CorrelationId = id,
            CurrentState = "Processing",
            Status = SagaStatus.Running,
            SourceService = "OrderService",
            TraceId = "trace-abc",
            SagaName = "OrderSaga"
        };
        accessor.SetContext(context);

        var executionContext = new MassTransitSagaExecutionContext(accessor);

        executionContext.CorrelationId.Should().Be(id);
        executionContext.TraceId.Should().Be("trace-abc");
        executionContext.SagaName.Should().Be("OrderSaga");
        executionContext.CurrentState.Should().Be("Processing");
        executionContext.Status.Should().Be(SagaStatus.Running);
        executionContext.SourceService.Should().Be("OrderService");
    }

    [Fact]
    public void CausationId_ShouldAlwaysBeNull()
    {
        var accessor = new ScopedSagaContextAccessor();
        var context = new MutableSagaContext
        {
            CorrelationId = Guid.NewGuid(),
            CurrentState = "Active",
            Status = SagaStatus.Completed
        };
        accessor.SetContext(context);

        var executionContext = new MassTransitSagaExecutionContext(accessor);

        executionContext.CausationId.Should().BeNull();
    }

    [Fact]
    public void ShouldReflectContextChanges_WhenAccessorContextChanges()
    {
        var accessor = new ScopedSagaContextAccessor();
        var executionContext = new MassTransitSagaExecutionContext(accessor);

        var firstId = Guid.NewGuid();
        accessor.SetContext(new MutableSagaContext
        {
            CorrelationId = firstId,
            CurrentState = "First",
            Status = SagaStatus.Running
        });

        executionContext.CorrelationId.Should().Be(firstId);
        executionContext.CurrentState.Should().Be("First");
        executionContext.Status.Should().Be(SagaStatus.Running);

        var secondId = Guid.NewGuid();
        accessor.ClearContext();
        accessor.SetContext(new MutableSagaContext
        {
            CorrelationId = secondId,
            CurrentState = "Second",
            Status = SagaStatus.Completed
        });

        executionContext.CorrelationId.Should().Be(secondId);
        executionContext.CurrentState.Should().Be("Second");
        executionContext.Status.Should().Be(SagaStatus.Completed);
    }

    [Fact]
    public void SagaName_WhenContextIsMutableSagaContext_ShouldReturnSagaName()
    {
        var accessor = new ScopedSagaContextAccessor();
        accessor.SetContext(new MutableSagaContext { SagaName = "PaymentSaga" });

        var executionContext = new MassTransitSagaExecutionContext(accessor);

        executionContext.SagaName.Should().Be("PaymentSaga");
    }

    [Fact]
    public void SagaName_WhenContextIsNotMutableSagaContext_ShouldReturnEmpty()
    {
        var mockContext = new Mock<ISagaContext>();
        mockContext.Setup(c => c.CorrelationId).Returns(Guid.NewGuid());
        mockContext.Setup(c => c.CurrentState).Returns("Active");

        var accessor = new ScopedSagaContextAccessor();
        accessor.SetContext(mockContext.Object);

        var executionContext = new MassTransitSagaExecutionContext(accessor);

        executionContext.SagaName.Should().BeEmpty();
    }
}
