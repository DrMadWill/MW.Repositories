using FluentAssertions;
using MW.Saga.Context;
using MW.Saga.MassTransit.Context;
using MW.Saga.Models;

namespace MW.Saga.MassTransit.Tests.Context;

public class MutableSagaContextTests
{
    [Fact]
    public void ShouldImplementISagaContext()
    {
        var context = new MutableSagaContext();

        context.Should().BeAssignableTo<ISagaContext>();
    }

    [Fact]
    public void DefaultValues_ShouldBeCorrect()
    {
        var context = new MutableSagaContext();

        context.CorrelationId.Should().Be(Guid.Empty);
        context.CurrentState.Should().BeEmpty();
        context.Status.Should().Be(SagaStatus.NotStarted);
        context.SourceService.Should().BeNull();
        context.StartedByEvent.Should().BeNull();
        context.TraceId.Should().BeNull();
        context.SagaName.Should().BeEmpty();
    }

    [Fact]
    public void CorrelationId_CanBeSetAndRead()
    {
        var context = new MutableSagaContext();
        var id = Guid.NewGuid();

        context.CorrelationId = id;

        context.CorrelationId.Should().Be(id);
    }

    [Fact]
    public void CurrentState_CanBeSetAndRead()
    {
        var context = new MutableSagaContext();

        context.CurrentState = "Processing";

        context.CurrentState.Should().Be("Processing");
    }

    [Fact]
    public void Status_CanBeSetAndRead()
    {
        var context = new MutableSagaContext();

        context.Status = SagaStatus.Running;

        context.Status.Should().Be(SagaStatus.Running);
    }

    [Fact]
    public void SourceService_CanBeSetAndRead()
    {
        var context = new MutableSagaContext();

        context.SourceService = "OrderService";

        context.SourceService.Should().Be("OrderService");
    }

    [Fact]
    public void StartedByEvent_CanBeSetAndRead()
    {
        var context = new MutableSagaContext();

        context.StartedByEvent = "OrderCreated";

        context.StartedByEvent.Should().Be("OrderCreated");
    }

    [Fact]
    public void TraceId_CanBeSetAndRead()
    {
        var context = new MutableSagaContext();

        context.TraceId = "trace-123";

        context.TraceId.Should().Be("trace-123");
    }

    [Fact]
    public void SagaName_InternalProperty_CanBeSetAndRead()
    {
        var context = new MutableSagaContext();

        context.SagaName = "OrderSaga";

        context.SagaName.Should().Be("OrderSaga");
    }

    [Fact]
    public void AllProperties_CanBeSetTogether()
    {
        var id = Guid.NewGuid();
        var context = new MutableSagaContext
        {
            CorrelationId = id,
            CurrentState = "Active",
            Status = SagaStatus.Completed,
            SourceService = "PaymentService",
            StartedByEvent = "PaymentInitiated",
            TraceId = "trace-456",
            SagaName = "PaymentSaga"
        };

        context.CorrelationId.Should().Be(id);
        context.CurrentState.Should().Be("Active");
        context.Status.Should().Be(SagaStatus.Completed);
        context.SourceService.Should().Be("PaymentService");
        context.StartedByEvent.Should().Be("PaymentInitiated");
        context.TraceId.Should().Be("trace-456");
        context.SagaName.Should().Be("PaymentSaga");
    }
}
