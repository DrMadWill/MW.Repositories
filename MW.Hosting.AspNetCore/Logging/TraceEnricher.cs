using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MW.Hosting.AspNetCore.Abstractions;
using Serilog.Core;
using Serilog.Events;

namespace MW.Hosting.AspNetCore.Logging;

public class TraceEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TraceEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var accessor = _httpContextAccessor.HttpContext?.RequestServices
            .GetService<IRequestTraceAccessor>();

        if (accessor is null)
            return;

        if (!string.IsNullOrEmpty(accessor.TraceId))
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", accessor.TraceId));

        if (!string.IsNullOrEmpty(accessor.CorrelationId))
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("CorrelationId", accessor.CorrelationId));
    }
}
