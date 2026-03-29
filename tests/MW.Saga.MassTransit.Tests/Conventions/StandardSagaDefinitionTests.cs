using FluentAssertions;
using MassTransit;
using MW.Saga.MassTransit.Conventions;
using MW.Saga.MassTransit.Options;

namespace MW.Saga.MassTransit.Tests.Conventions;

public class TestSaga : ISaga
{
    public Guid CorrelationId { get; set; }
}

public class StandardSagaDefinitionTests
{
    [Fact]
    public void Constructor_Should_ThrowOnNullOptions()
    {
        var act = () => new StandardSagaDefinition<TestSaga>(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("options");
    }

    [Fact]
    public void Constructor_Default_Should_NotThrow()
    {
        var act = () => new StandardSagaDefinition<TestSaga>();

        act.Should().NotThrow();
    }

    [Fact]
    public void Should_InheritFromSagaDefinition()
    {
        typeof(StandardSagaDefinition<TestSaga>).BaseType
            .Should().Be(typeof(SagaDefinition<TestSaga>));
    }
}
