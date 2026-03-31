using FluentAssertions;
using MW.Hosting.AspNetCore.Options;

namespace MW.Hosting.AspNetCore.Tests.Options;

public class HealthEndpointOptionsValidatorTests
{
    private readonly HealthEndpointOptionsValidator _validator = new();

    [Fact]
    public void Validate_ValidDefaults_ReturnsSuccess()
    {
        var options = new HealthEndpointOptions();

        var result = _validator.Validate(null, options);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyPath_ReturnsFail()
    {
        var options = new HealthEndpointOptions { Path = "" };

        var result = _validator.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("Path");
    }

    [Fact]
    public void Validate_PathWithoutLeadingSlash_ReturnsFail()
    {
        var options = new HealthEndpointOptions { Path = "api/health" };

        var result = _validator.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("Path");
    }

    [Fact]
    public void Validate_ReadinessPathWithoutSlash_ReturnsFail()
    {
        var options = new HealthEndpointOptions { ReadinessPath = "ready" };

        var result = _validator.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("ReadinessPath");
    }

    [Fact]
    public void Validate_LivenessPathWithoutSlash_ReturnsFail()
    {
        var options = new HealthEndpointOptions { LivenessPath = "live" };

        var result = _validator.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("LivenessPath");
    }

    [Fact]
    public void Validate_ValidReadinessAndLivenessPaths_ReturnsSuccess()
    {
        var options = new HealthEndpointOptions
        {
            ReadinessPath = "/api/ready",
            LivenessPath = "/api/live"
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_NullOptionalPaths_ReturnsSuccess()
    {
        var options = new HealthEndpointOptions
        {
            ReadinessPath = null,
            LivenessPath = null
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.Should().BeTrue();
    }
}
