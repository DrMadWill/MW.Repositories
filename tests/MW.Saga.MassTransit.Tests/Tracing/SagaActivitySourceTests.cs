using System.Diagnostics;
using FluentAssertions;
using MW.Saga.MassTransit.Tracing;

namespace MW.Saga.MassTransit.Tests.Tracing;

public class SagaActivitySourceTests
{
    [Fact]
    public void SourceName_Should_BeCorrect()
    {
        SagaActivitySource.SourceName.Should().Be("MW.Saga");
    }

    [Fact]
    public void Source_Should_NotBeNull()
    {
        SagaActivitySource.Source.Should().NotBeNull();
    }

    [Fact]
    public void Source_Should_HaveCorrectName()
    {
        SagaActivitySource.Source.Name.Should().Be("MW.Saga");
    }

    [Fact]
    public void Source_Should_HaveCorrectVersion()
    {
        SagaActivitySource.Source.Version.Should().Be("1.0.0");
    }
}
