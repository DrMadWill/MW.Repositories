using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MW.Hosting.AspNetCore.Options;

namespace MW.Hosting.AspNetCore.Cors;

public static class CorsApplicationBuilderExtensions
{
    public static IApplicationBuilder UseDefaultCors(
        this IApplicationBuilder app,
        string? policyName = null)
    {
        if (string.IsNullOrEmpty(policyName))
        {
            var options = app.ApplicationServices
                .GetRequiredService<IOptions<CorsOptions>>().Value;
            policyName = options.PolicyName;
        }

        app.UseCors(policyName);
        return app;
    }
}
