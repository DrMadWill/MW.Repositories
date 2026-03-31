using Microsoft.AspNetCore.Builder;
using MW.Hosting.AspNetCore.Cors;
using MW.Hosting.AspNetCore.HealthChecks;

namespace MW.Hosting.AspNetCore.Extensions;

public static class HostingApplicationBuilderExtensions
{
    public static WebApplication UseMyBidHostingDefaults(this WebApplication app)
    {
        // 1. CORS middleware
        app.UseDefaultCors();

        // 2. Health endpoints
        app.MapDefaultHealthEndpoints();

        return app;
    }
}
