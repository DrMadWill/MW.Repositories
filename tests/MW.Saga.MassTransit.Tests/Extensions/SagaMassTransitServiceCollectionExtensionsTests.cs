using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MW.Saga.Context;
using MW.Saga.MassTransit.Context;
using MW.Saga.MassTransit.Extensions;
using MW.Saga.MassTransit.Options;
using MW.Saga.Observability;

namespace MW.Saga.MassTransit.Tests.Extensions;

public class SagaMassTransitServiceCollectionExtensionsTests
{
    [Fact]
    public void AddSagaMassTransitInfrastructure_Should_ThrowOnNullServices()
    {
        IServiceCollection services = null!;

        var act = () => services.AddSagaMassTransitInfrastructure(_ => { });

        act.Should().Throw<ArgumentNullException>().WithParameterName("services");
    }

    [Fact]
    public void AddSagaMassTransitInfrastructure_Should_ThrowOnNullConfigure()
    {
        var services = new ServiceCollection();

        var act = () => services.AddSagaMassTransitInfrastructure(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("configure");
    }

    [Fact]
    public void AddSagaMassTransitInfrastructure_Should_RegisterScopedSagaContextAccessor()
    {
        var services = new ServiceCollection();

        services.AddSagaMassTransitInfrastructure(_ => { });

        var descriptor = services.Should().ContainSingle(
            d => d.ServiceType == typeof(ScopedSagaContextAccessor)).Subject;
        descriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddSagaMassTransitInfrastructure_Should_RegisterISagaContextAccessor()
    {
        var services = new ServiceCollection();

        services.AddSagaMassTransitInfrastructure(_ => { });

        var descriptor = services.Should().ContainSingle(
            d => d.ServiceType == typeof(ISagaContextAccessor)).Subject;
        descriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddSagaMassTransitInfrastructure_Should_RegisterISagaExecutionContext()
    {
        var services = new ServiceCollection();

        services.AddSagaMassTransitInfrastructure(_ => { });

        var descriptor = services.Should().ContainSingle(
            d => d.ServiceType == typeof(ISagaExecutionContext)).Subject;
        descriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddSagaMassTransitInfrastructure_Should_RegisterISagaObserver()
    {
        var services = new ServiceCollection();

        services.AddSagaMassTransitInfrastructure(_ => { });

        var descriptor = services.Should().ContainSingle(
            d => d.ServiceType == typeof(ISagaObserver)).Subject;
        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }

    [Fact]
    public void BindOptions_Should_ThrowOnNullConfiguration()
    {
        var registrationOptions = new SagaMassTransitRegistrationOptions();

        var act = () => registrationOptions.BindOptions(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("configuration");
    }

    [Fact]
    public void BindOptions_Should_BindOptionsFromConfiguration()
    {
        var registrationOptions = new SagaMassTransitRegistrationOptions();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "SagaMassTransit:EndpointPrefix", "test-prefix" }
            })
            .Build();

        registrationOptions.BindOptions(configuration);

        registrationOptions.Options.EndpointPrefix.Should().Be("test-prefix");
    }

    [Fact]
    public void BindOptions_Should_SupportCustomSectionName()
    {
        var registrationOptions = new SagaMassTransitRegistrationOptions();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "CustomSection:EndpointPrefix", "custom-prefix" }
            })
            .Build();

        registrationOptions.BindOptions(configuration, "CustomSection");

        registrationOptions.Options.EndpointPrefix.Should().Be("custom-prefix");
    }

    [Fact]
    public void SagaMassTransitRegistrationOptions_Should_HaveDefaultOptions()
    {
        var registrationOptions = new SagaMassTransitRegistrationOptions();

        registrationOptions.Options.Should().NotBeNull();
        registrationOptions.Options.Should().BeOfType<SagaMassTransitOptions>();
    }
}
