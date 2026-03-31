using Microsoft.Extensions.Options;

namespace MW.Hosting.AspNetCore.Options;

public class CorsOptionsValidator : IValidateOptions<CorsOptions>
{
    public ValidateOptionsResult Validate(string? name, CorsOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrEmpty(options.PolicyName))
            failures.Add($"{nameof(CorsOptions.PolicyName)} must not be null or empty.");

        if (options.AllowedOrigins is null || options.AllowedOrigins.Length == 0)
            failures.Add($"{nameof(CorsOptions.AllowedOrigins)} must not be null or empty.");
        else
        {
            for (var i = 0; i < options.AllowedOrigins.Length; i++)
            {
                if (!Uri.TryCreate(options.AllowedOrigins[i], UriKind.Absolute, out _))
                    failures.Add($"{nameof(CorsOptions.AllowedOrigins)}[{i}] ('{options.AllowedOrigins[i]}') is not a valid absolute URI.");
            }
        }

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
