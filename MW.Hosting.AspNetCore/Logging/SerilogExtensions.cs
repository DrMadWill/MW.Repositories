using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MW.Hosting.AspNetCore.Options;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Graylog;
using Serilog.Sinks.Graylog.Core.Transport;

namespace MW.Hosting.AspNetCore.Logging;

public static class SerilogExtensions
{
    public static WebApplicationBuilder ConfigureDefaultSerilog(
        this WebApplicationBuilder builder,
        string graylogSectionName = "Graylog")
    {
        var configuration = builder.Configuration;

        var graylogOptions = new GraylogOptions();
        configuration.GetSection(graylogSectionName).Bind(graylogOptions);

        builder.Host.UseSerilog((context, services, loggerConfig) =>
        {
            loggerConfig
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("MachineName", Environment.MachineName)
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                .WriteTo.Console();

            if (graylogOptions.Enabled)
            {
                var transportType = Enum.TryParse<TransportType>(graylogOptions.TransportType, true, out var parsed)
                    ? parsed
                    : TransportType.Udp;

                loggerConfig.WriteTo.Graylog(new GraylogSinkOptions
                {
                    HostnameOrAddress = graylogOptions.Host,
                    Port = graylogOptions.Port,
                    Facility = graylogOptions.Facility,
                    TransportType = transportType
                });
            }

            var healthPath = configuration.GetValue<string>("HealthEndpoints:Path") ?? "/api/health";
            loggerConfig.Filter.ByExcluding(logEvent =>
                logEvent.Properties.TryGetValue("RequestPath", out var pathValue)
                && pathValue is ScalarValue { Value: string pathString }
                && pathString == healthPath);
        });

        return builder;
    }
}
