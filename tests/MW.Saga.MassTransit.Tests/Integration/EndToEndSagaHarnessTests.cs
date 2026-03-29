using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using MW.Saga.Contracts;

namespace MW.Saga.MassTransit.Tests.Integration;

public class SampleSagaState : SagaStateMachineInstance, ISagaState
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = string.Empty;
}

public class SampleSagaStarted
{
    public Guid CorrelationId { get; set; }
}

public class SampleSagaCompleted
{
    public Guid CorrelationId { get; set; }
}

public class SampleStateMachine : MassTransitStateMachine<SampleSagaState>
{
    public State? Processing { get; private set; }
    public State? Done { get; private set; }
    public Event<SampleSagaStarted>? StartEvent { get; private set; }
    public Event<SampleSagaCompleted>? CompleteEvent { get; private set; }

    public SampleStateMachine()
    {
        InstanceState(x => x.CurrentState);
        Event(() => StartEvent, e => e.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => CompleteEvent, e => e.CorrelateById(ctx => ctx.Message.CorrelationId));

        Initially(When(StartEvent).TransitionTo(Processing));
        During(Processing, When(CompleteEvent).TransitionTo(Done).Finalize());

        SetCompletedWhenFinalized();
    }
}

public class EndToEndSagaHarnessTests : IAsyncLifetime
{
    private ServiceProvider _provider = null!;
    private ITestHarness _harness = null!;

    public async Task InitializeAsync()
    {
        var services = new ServiceCollection();
        services.AddMassTransitTestHarness(cfg =>
        {
            cfg.AddSagaStateMachine<SampleStateMachine, SampleSagaState>()
                .InMemoryRepository();
        });
        _provider = services.BuildServiceProvider(true);
        _harness = _provider.GetRequiredService<ITestHarness>();
        await _harness.Start();
    }

    public async Task DisposeAsync()
    {
        await _harness.Stop();
        await _provider.DisposeAsync();
    }

    [Fact]
    public async Task SagaFlow_Should_TransitionThroughStates()
    {
        var correlationId = Guid.NewGuid();

        await _harness.Bus.Publish(new SampleSagaStarted { CorrelationId = correlationId });

        var sagaHarness = _harness.GetSagaStateMachineHarness<SampleStateMachine, SampleSagaState>();

        (await sagaHarness.Created.Any(x => x.CorrelationId == correlationId)).Should().BeTrue();

        (await sagaHarness.Consumed.Any<SampleSagaStarted>(x =>
            x.Context.Message.CorrelationId == correlationId)).Should().BeTrue();

        var instance = sagaHarness.Sagas.ContainsInState(correlationId, sagaHarness.StateMachine, sagaHarness.StateMachine.Processing!);
        instance.Should().NotBeNull();
    }

    [Fact]
    public async Task SagaFlow_Should_CompleteSaga()
    {
        var correlationId = Guid.NewGuid();

        await _harness.Bus.Publish(new SampleSagaStarted { CorrelationId = correlationId });

        var sagaHarness = _harness.GetSagaStateMachineHarness<SampleStateMachine, SampleSagaState>();

        (await sagaHarness.Created.Any(x => x.CorrelationId == correlationId)).Should().BeTrue();

        await _harness.Bus.Publish(new SampleSagaCompleted { CorrelationId = correlationId });

        (await sagaHarness.Consumed.Any<SampleSagaCompleted>(x =>
            x.Context.Message.CorrelationId == correlationId)).Should().BeTrue();

        var exists = sagaHarness.Sagas.ContainsInState(correlationId, sagaHarness.StateMachine, sagaHarness.StateMachine.Final);
        exists.Should().NotBeNull();
    }

    [Fact]
    public async Task SagaFlow_Should_CorrelateByCorrelationId()
    {
        var correlationId = Guid.NewGuid();
        var otherCorrelationId = Guid.NewGuid();

        await _harness.Bus.Publish(new SampleSagaStarted { CorrelationId = correlationId });
        await _harness.Bus.Publish(new SampleSagaStarted { CorrelationId = otherCorrelationId });

        var sagaHarness = _harness.GetSagaStateMachineHarness<SampleStateMachine, SampleSagaState>();

        (await sagaHarness.Created.Any(x => x.CorrelationId == correlationId)).Should().BeTrue();
        (await sagaHarness.Created.Any(x => x.CorrelationId == otherCorrelationId)).Should().BeTrue();

        var targetInstance = sagaHarness.Sagas.ContainsInState(correlationId, sagaHarness.StateMachine, sagaHarness.StateMachine.Processing!);
        targetInstance.Should().NotBeNull();
        targetInstance!.CorrelationId.Should().Be(correlationId);

        var otherInstance = sagaHarness.Sagas.ContainsInState(otherCorrelationId, sagaHarness.StateMachine, sagaHarness.StateMachine.Processing!);
        otherInstance.Should().NotBeNull();
        otherInstance!.CorrelationId.Should().Be(otherCorrelationId);
    }
}
