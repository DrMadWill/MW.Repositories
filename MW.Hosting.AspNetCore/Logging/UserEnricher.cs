using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MW.Hosting.AspNetCore.Abstractions;
using Serilog.Core;
using Serilog.Events;

namespace MW.Hosting.AspNetCore.Logging;

public class UserEnricher(IHttpContextAccessor httpContextAccessor) : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = httpContextAccessor.HttpContext;

        var accessor = httpContext?.RequestServices.GetService<ICurrentUserAccessor>();

        var displayName = (accessor is not null && accessor.IsAuthenticated)
            ? (!string.IsNullOrWhiteSpace(accessor.UserName)
                ? accessor.UserName
                : accessor.UserId ?? "Authenticated")
            : "Anonymous";

        logEvent.AddPropertyIfAbsent(
            propertyFactory.CreateProperty("UserName", displayName));

        if (accessor is not null && accessor.IsAuthenticated && !string.IsNullOrWhiteSpace(accessor.UserId))
        {
            logEvent.AddPropertyIfAbsent(
                propertyFactory.CreateProperty("UserId", accessor.UserId));
        }

        var traceId = httpContext?.TraceIdentifier;
        var activityTraceId = Activity.Current?.TraceId.ToString();
        var spanId = Activity.Current?.SpanId.ToString();

        logEvent.AddPropertyIfAbsent(
            propertyFactory.CreateProperty("STraceId", traceId ?? string.Empty));

        logEvent.AddPropertyIfAbsent(
            propertyFactory.CreateProperty("SActivityId", activityTraceId ?? string.Empty));

        logEvent.AddPropertyIfAbsent(
            propertyFactory.CreateProperty("SSpanId", spanId ?? string.Empty));
    }
}
