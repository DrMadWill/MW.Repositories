using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MW.Hosting.AspNetCore.Abstractions;
using Serilog.Core;
using Serilog.Events;

namespace MW.Hosting.AspNetCore.Logging;

public class UserEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var accessor = _httpContextAccessor.HttpContext?.RequestServices
            .GetService<ICurrentUserAccessor>();

        if (accessor is null || !accessor.IsAuthenticated)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserName", "Anonymous"));
            return;
        }

        var displayName = !string.IsNullOrEmpty(accessor.UserName)
            ? accessor.UserName
            : accessor.UserId ?? "Anonymous";

        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserName", displayName));

        if (!string.IsNullOrEmpty(accessor.UserId))
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserId", accessor.UserId));
    }
}
