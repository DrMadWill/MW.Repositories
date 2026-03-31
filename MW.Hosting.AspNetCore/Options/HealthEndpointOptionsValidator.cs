using Microsoft.Extensions.Options;

namespace MW.Hosting.AspNetCore.Options;

public class HealthEndpointOptionsValidator : IValidateOptions<HealthEndpointOptions>
{
    public ValidateOptionsResult Validate(string? name, HealthEndpointOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrEmpty(options.Path))
            failures.Add($"{nameof(HealthEndpointOptions.Path)} must not be null or empty.");
        else if (!options.Path.StartsWith("/"))
            failures.Add($"{nameof(HealthEndpointOptions.Path)} must start with '/'.");

        if (!string.IsNullOrEmpty(options.ReadinessPath) && !options.ReadinessPath.StartsWith("/"))
            failures.Add($"{nameof(HealthEndpointOptions.ReadinessPath)} must start with '/'.");

        if (!string.IsNullOrEmpty(options.LivenessPath) && !options.LivenessPath.StartsWith("/"))
            failures.Add($"{nameof(HealthEndpointOptions.LivenessPath)} must start with '/'.");

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
