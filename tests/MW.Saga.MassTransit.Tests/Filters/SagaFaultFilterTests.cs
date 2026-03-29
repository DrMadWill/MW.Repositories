using FluentAssertions;
using Moq;
using MassTransit;
using MW.Saga.MassTransit.Filters;

namespace MW.Saga.MassTransit.Tests.Filters;

public class SagaFaultFilterTests
{
    private static Mock<SagaConsumeContext<TestSagaState, TestSagaMessage>> CreateContextMock()
    {
        return new Mock<SagaConsumeContext<TestSagaState, TestSagaMessage>>();
    }

    private static Mock<IPipe<SagaConsumeContext<TestSagaState, TestSagaMessage>>> CreateNextPipeMock(
        Exception? throwOnSend = null)
    {
        var pipeMock = new Mock<IPipe<SagaConsumeContext<TestSagaState, TestSagaMessage>>>();

        if (throwOnSend is not null)
        {
            pipeMock
                .Setup(p => p.Send(It.IsAny<SagaConsumeContext<TestSagaState, TestSagaMessage>>()))
                .ThrowsAsync(throwOnSend);
        }
        else
        {
            pipeMock
                .Setup(p => p.Send(It.IsAny<SagaConsumeContext<TestSagaState, TestSagaMessage>>()))
                .Returns(Task.CompletedTask);
        }

        return pipeMock;
    }

    [Fact]
    public async Task Send_Should_InvokeOnFault_WhenExceptionOccurs()
    {
        Exception? capturedEx = null;
        SagaConsumeContext<TestSagaState, TestSagaMessage>? capturedCtx = null;

        var filter = new SagaFaultFilter<TestSagaState, TestSagaMessage>((ctx, ex) =>
        {
            capturedCtx = ctx;
            capturedEx = ex;
        });

        var contextMock = CreateContextMock();
        var expectedException = new InvalidOperationException("test fault");
        var pipeMock = CreateNextPipeMock(throwOnSend: expectedException);

        var act = () => filter.Send(contextMock.Object, pipeMock.Object);

        await act.Should().ThrowAsync<InvalidOperationException>();
        capturedEx.Should().BeSameAs(expectedException);
        capturedCtx.Should().BeSameAs(contextMock.Object);
    }

    [Fact]
    public async Task Send_Should_RethrowException()
    {
        var filter = new SagaFaultFilter<TestSagaState, TestSagaMessage>((_, _) => { });
        var contextMock = CreateContextMock();
        var expectedException = new InvalidOperationException("rethrow test");
        var pipeMock = CreateNextPipeMock(throwOnSend: expectedException);

        var act = () => filter.Send(contextMock.Object, pipeMock.Object);

        (await act.Should().ThrowAsync<InvalidOperationException>())
            .Which.Should().BeSameAs(expectedException);
    }

    [Fact]
    public async Task Send_Should_NotInvokeOnFault_WhenNoException()
    {
        var faultInvoked = false;
        var filter = new SagaFaultFilter<TestSagaState, TestSagaMessage>((_, _) => faultInvoked = true);
        var contextMock = CreateContextMock();
        var pipeMock = CreateNextPipeMock();

        await filter.Send(contextMock.Object, pipeMock.Object);

        faultInvoked.Should().BeFalse();
    }

    [Fact]
    public async Task Send_Should_HandleNullOnFault_GracefullyOnException()
    {
        var filter = new SagaFaultFilter<TestSagaState, TestSagaMessage>(onFault: null);
        var contextMock = CreateContextMock();
        var pipeMock = CreateNextPipeMock(throwOnSend: new InvalidOperationException("null callback"));

        var act = () => filter.Send(contextMock.Object, pipeMock.Object);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public void Probe_Should_CreateFilterScope()
    {
        var filter = new SagaFaultFilter<TestSagaState, TestSagaMessage>();
        var scopeMock = new Mock<ProbeContext>();
        var probeMock = new Mock<ProbeContext>();
        probeMock.Setup(p => p.CreateScope(It.IsAny<string>())).Returns(scopeMock.Object);

        filter.Probe(probeMock.Object);

        probeMock.Verify(p => p.CreateScope("filters"), Times.Once);
        scopeMock.Verify(s => s.Add("filterType", "sagaFaultHandling"), Times.Once);
    }
}
