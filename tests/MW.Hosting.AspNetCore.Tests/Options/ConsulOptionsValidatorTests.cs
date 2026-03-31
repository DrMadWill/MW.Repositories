using FluentAssertions;
using MW.Hosting.AspNetCore.Options;

namespace MW.Hosting.AspNetCore.Tests.Options;

public class ConsulOptionsValidatorTests
{
    private readonly ConsulOptionsValidator _validator = new();

    [Fact]
    public void Validate_DisabledConsul_ReturnsSuccess()
    {
        var options = new ConsulOptions { Enabled = false };

        var result = _validator.Validate(null, options);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_ValidEnabledConsul_ReturnsSuccess()
    {
        var options = new ConsulOptions
        {
            Enabled = true,
            ServiceId = "order-service-1",
            ServiceName = "order-service",
            ConsulAddress = "http://localhost:8500",
            ServiceAddress = "http://localhost:5000",
            HealthCheckPath = "/api/health"
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_EnabledMissingServiceId_ReturnsFail()
    {
        var options = new ConsulOptions
        {
            Enabled = true,
            ServiceId = "",
            ServiceName = "order-service",
            ConsulAddress = "http://localhost:8500",
            ServiceAddress = "http://localhost:5000",
            HealthCheckPath = "/api/health"
        };

        var result = _validator.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("ServiceId");
    }

    [Fact]
    public void Validate_EnabledMissingServiceName_ReturnsFail()
    {
        var options = new ConsulOptions
        {
            Enabled = true,
            ServiceId = "order-service-1",
            ServiceName = "",
            ConsulAddress = "http://localhost:8500",
            ServiceAddress = "http://localhost:5000",
            HealthCheckPath = "/api/health"
        };

        var result = _validator.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("ServiceName");
    }

    [Fact]
    public void Validate_EnabledMissingConsulAddress_ReturnsFail()
    {
        var options = new ConsulOptions
        {
            Enabled = true,
            ServiceId = "order-service-1",
            ServiceName = "order-service",
            ConsulAddress = "",
            ServiceAddress = "http://localhost:5000",
            HealthCheckPath = "/api/health"
        };

        var result = _validator.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("ConsulAddress");
    }

    [Fact]
    public void Validate_EnabledMissingServiceAddress_ReturnsFail()
    {
        var options = new ConsulOptions
        {
            Enabled = true,
            ServiceId = "order-service-1",
            ServiceName = "order-service",
            ConsulAddress = "http://localhost:8500",
            ServiceAddress = "",
            HealthCheckPath = "/api/health"
        };

        var result = _validator.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("ServiceAddress");
    }

    [Fact]
    public void Validate_EnabledMissingHealthCheckPath_ReturnsFail()
    {
        var options = new ConsulOptions
        {
            Enabled = true,
            ServiceId = "order-service-1",
            ServiceName = "order-service",
            ConsulAddress = "http://localhost:8500",
            ServiceAddress = "http://localhost:5000",
            HealthCheckPath = ""
        };

        var result = _validator.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("HealthCheckPath");
    }

    [Fact]
    public void Validate_EnabledAllMissing_ReturnsMultipleFailures()
    {
        var options = new ConsulOptions
        {
            Enabled = true,
            ServiceId = "",
            ServiceName = "",
            ConsulAddress = "",
            ServiceAddress = "",
            HealthCheckPath = ""
        };

        var result = _validator.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("ServiceId");
        result.FailureMessage.Should().Contain("ServiceName");
        result.FailureMessage.Should().Contain("ConsulAddress");
        result.FailureMessage.Should().Contain("ServiceAddress");
        result.FailureMessage.Should().Contain("HealthCheckPath");
    }
}
