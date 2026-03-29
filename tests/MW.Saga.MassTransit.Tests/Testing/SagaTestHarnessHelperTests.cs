using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using MW.Saga.MassTransit.Testing;

namespace MW.Saga.MassTransit.Tests.Testing;

public class TestHarnessState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = string.Empty;
}

public class TestHarnessStarted
{
    public Guid CorrelationId { get; set; }
}

public class TestHarnessStateMachine : MassTransitStateMachine<TestHarnessState>
{
    public State? Processing { get; private set; }
    public Event<TestHarnessStarted>? Started { get; private set; }

    public TestHarnessStateMachine()
    {
        InstanceState(x => x.CurrentState);
        Event(() => Started, e => e.CorrelateById(ctx => ctx.Message.CorrelationId));
        Initially(When(Started).TransitionTo(Processing));
    }
}

public class SagaTestHarnessHelperTests
{
    [Fact]
    public async Task AddSagaTestHarness_Should_RegisterHarness()
    {
        var services = new ServiceCollection();

        services.AddSagaTestHarness<TestHarnessState, TestHarnessStateMachine>();

        var provider = services.BuildServiceProvider(true);
        try
        {
            var harness = provider.GetService<ITestHarness>();
            harness.Should().NotBeNull();
        }
        finally
        {
            await provider.DisposeAsync();
        }
    }

    [Fact]
    public async Task AddSagaTestHarness_Should_InvokeConfigureCallback()
    {
        var services = new ServiceCollection();
        var callbackInvoked = false;

        services.AddSagaTestHarness<TestHarnessState, TestHarnessStateMachine>(cfg =>
        {
            callbackInvoked = true;
        });

        var provider = services.BuildServiceProvider(true);
        try
        {
            // Resolve harness to trigger configuration
            provider.GetService<ITestHarness>();
            callbackInvoked.Should().BeTrue();
        }
        finally
        {
            await provider.DisposeAsync();
        }
    }

    [Fact]
    public async Task AddSagaTestHarness_Should_WorkWithoutConfigureCallback()
    {
        var services = new ServiceCollection();

        var result = services.AddSagaTestHarness<TestHarnessState, TestHarnessStateMachine>();

        result.Should().BeSameAs(services);

        var provider = services.BuildServiceProvider(true);
        try
        {
            var harness = provider.GetService<ITestHarness>();
            harness.Should().NotBeNull();
        }
        finally
        {
            await provider.DisposeAsync();
        }
    }
}
