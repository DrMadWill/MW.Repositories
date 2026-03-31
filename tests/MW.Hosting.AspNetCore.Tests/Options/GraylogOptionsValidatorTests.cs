using FluentAssertions;
using MW.Hosting.AspNetCore.Options;

namespace MW.Hosting.AspNetCore.Tests.Options;

public class GraylogOptionsValidatorTests
{
    private readonly GraylogOptionsValidator _validator = new();

    [Fact]
    public void Validate_DisabledGraylog_ReturnsSuccess()
    {
        var options = new GraylogOptions { Enabled = false };

        var result = _validator.Validate(null, options);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_ValidEnabledGraylog_ReturnsSuccess()
    {
        var options = new GraylogOptions
        {
            Enabled = true,
            Host = "graylog.local",
            Port = 12201,
            Facility = "my-service"
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_EnabledMissingHost_ReturnsFail()
    {
        var options = new GraylogOptions
        {
            Enabled = true,
            Host = "",
            Port = 12201
        };

        var result = _validator.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("Host");
    }

    [Fact]
    public void Validate_EnabledZeroPort_ReturnsFail()
    {
        var options = new GraylogOptions
        {
            Enabled = true,
            Host = "graylog.local",
            Port = 0
        };

        var result = _validator.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("Port");
    }

    [Fact]
    public void Validate_EnabledNegativePort_ReturnsFail()
    {
        var options = new GraylogOptions
        {
            Enabled = true,
            Host = "graylog.local",
            Port = -1
        };

        var result = _validator.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("Port");
    }

    [Fact]
    public void Validate_EnabledMultipleFailures_ReturnsAllMessages()
    {
        var options = new GraylogOptions
        {
            Enabled = true,
            Host = "",
            Port = 0
        };

        var result = _validator.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("Host");
        result.FailureMessage.Should().Contain("Port");
    }
}
