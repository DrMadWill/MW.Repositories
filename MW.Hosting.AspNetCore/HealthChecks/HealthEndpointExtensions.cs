using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MW.Hosting.AspNetCore.Options;

namespace MW.Hosting.AspNetCore.HealthChecks;

public static class HealthEndpointExtensions
{
    public static WebApplication MapDefaultHealthEndpoints(this WebApplication app)
    {
        var options = app.Services.GetService<IOptions<HealthEndpointOptions>>()?.Value
            ?? new HealthEndpointOptions();

        app.MapHealthChecks(options.Path, new HealthCheckOptions
        {
            ResponseWriter = options.UsePlainTextResponse
                ? WritePlainTextResponse(options.PlainText)
                : WriteJsonResponse,
            AllowCachingResponses = false
        });

        if (!string.IsNullOrEmpty(options.ReadinessPath))
        {
            app.MapHealthChecks(options.ReadinessPath, new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready"),
                ResponseWriter = options.UsePlainTextResponse
                    ? WritePlainTextResponse(options.PlainText)
                    : WriteJsonResponse,
                AllowCachingResponses = false
            });
        }

        if (!string.IsNullOrEmpty(options.LivenessPath))
        {
            app.MapHealthChecks(options.LivenessPath, new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("live"),
                ResponseWriter = options.UsePlainTextResponse
                    ? WritePlainTextResponse(options.PlainText)
                    : WriteJsonResponse,
                AllowCachingResponses = false
            });
        }

        return app;
    }

    private static Func<HttpContext, HealthReport, Task> WritePlainTextResponse(string plainText)
    {
        return (context, report) =>
        {
            context.Response.ContentType = "text/plain";
            return context.Response.WriteAsync(
                report.Status == HealthStatus.Healthy ? plainText : report.Status.ToString());
        };
    }

    private static Task WriteJsonResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            entries = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.ToString()
            })
        };
        return context.Response.WriteAsJsonAsync(result);
    }
}
