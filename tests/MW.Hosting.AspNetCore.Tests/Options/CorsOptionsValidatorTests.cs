using FluentAssertions;
using MW.Hosting.AspNetCore.Options;

namespace MW.Hosting.AspNetCore.Tests.Options;

public class CorsOptionsValidatorTests
{
    private readonly CorsOptionsValidator _validator = new();

    [Fact]
    public void Validate_ValidOptions_ReturnsSuccess()
    {
        var options = new CorsOptions
        {
            PolicyName = "TestPolicy",
            AllowedOrigins = new[] { "https://example.com" },
            AllowAnyHeader = true,
            AllowAnyMethod = true
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyPolicyName_ReturnsFail()
    {
        var options = new CorsOptions
        {
            PolicyName = "",
            AllowedOrigins = new[] { "https://example.com" }
        };

        var result = _validator.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("PolicyName");
    }

    [Fact]
    public void Validate_NullAllowedOrigins_ReturnsFail()
    {
        var options = new CorsOptions
        {
            PolicyName = "TestPolicy",
            AllowedOrigins = null!
        };

        var result = _validator.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("AllowedOrigins");
    }

    [Fact]
    public void Validate_EmptyAllowedOrigins_ReturnsFail()
    {
        var options = new CorsOptions
        {
            PolicyName = "TestPolicy",
            AllowedOrigins = Array.Empty<string>()
        };

        var result = _validator.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("AllowedOrigins");
    }

    [Fact]
    public void Validate_InvalidOriginUri_ReturnsFail()
    {
        var options = new CorsOptions
        {
            PolicyName = "TestPolicy",
            AllowedOrigins = new[] { "not-a-valid-uri" }
        };

        var result = _validator.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("not a valid absolute URI");
    }

    [Fact]
    public void Validate_MultipleOrigins_AllValid_ReturnsSuccess()
    {
        var options = new CorsOptions
        {
            PolicyName = "TestPolicy",
            AllowedOrigins = new[] { "https://example.com", "http://localhost:3000" }
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_MultipleFailures_ReturnsAllMessages()
    {
        var options = new CorsOptions
        {
            PolicyName = "",
            AllowedOrigins = null!
        };

        var result = _validator.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("PolicyName");
        result.FailureMessage.Should().Contain("AllowedOrigins");
    }
}
