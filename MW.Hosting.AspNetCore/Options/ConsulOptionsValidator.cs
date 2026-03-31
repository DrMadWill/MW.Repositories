using Microsoft.Extensions.Options;

namespace MW.Hosting.AspNetCore.Options;

public class ConsulOptionsValidator : IValidateOptions<ConsulOptions>
{
    public ValidateOptionsResult Validate(string? name, ConsulOptions options)
    {
        if (!options.Enabled)
            return ValidateOptionsResult.Success;

        var failures = new List<string>();

        if (string.IsNullOrEmpty(options.ServiceId))
            failures.Add($"{nameof(ConsulOptions.ServiceId)} is required when Consul is enabled.");

        if (string.IsNullOrEmpty(options.ServiceName))
            failures.Add($"{nameof(ConsulOptions.ServiceName)} is required when Consul is enabled.");

        if (string.IsNullOrEmpty(options.ConsulAddress))
            failures.Add($"{nameof(ConsulOptions.ConsulAddress)} is required when Consul is enabled.");

        if (string.IsNullOrEmpty(options.ServiceAddress))
            failures.Add($"{nameof(ConsulOptions.ServiceAddress)} is required when Consul is enabled.");

        if (string.IsNullOrEmpty(options.HealthCheckPath))
            failures.Add($"{nameof(ConsulOptions.HealthCheckPath)} is required when Consul is enabled.");

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
