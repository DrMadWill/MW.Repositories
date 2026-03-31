using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using MW.Hosting.AspNetCore.Abstractions;

namespace MW.Hosting.AspNetCore.Logging;

public class HttpRequestTraceAccessor : IRequestTraceAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpRequestTraceAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? TraceId
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            if (context is null) return null;

            var activityTraceId = Activity.Current?.TraceId.ToString();
            if (!string.IsNullOrEmpty(activityTraceId))
                return activityTraceId;

            return context.TraceIdentifier;
        }
    }

    public string? CorrelationId
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            if (context is null) return null;

            if (context.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId)
                && !string.IsNullOrEmpty(correlationId))
            {
                return correlationId!;
            }

            return TraceId;
        }
    }
}
