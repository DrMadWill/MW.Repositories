using Microsoft.Extensions.Options;

namespace MW.Hosting.AspNetCore.Options;

public class GraylogOptionsValidator : IValidateOptions<GraylogOptions>
{
    public ValidateOptionsResult Validate(string? name, GraylogOptions options)
    {
        if (!options.Enabled)
            return ValidateOptionsResult.Success;

        var failures = new List<string>();

        if (string.IsNullOrEmpty(options.Host))
            failures.Add($"{nameof(GraylogOptions.Host)} is required when Graylog is enabled.");

        if (options.Port <= 0)
            failures.Add($"{nameof(GraylogOptions.Port)} must be greater than 0 when Graylog is enabled.");

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
