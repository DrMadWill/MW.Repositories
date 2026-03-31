using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MW.Hosting.AspNetCore.Options;

namespace MW.Hosting.AspNetCore.Consul;

public static class ConsulServiceCollectionExtensions
{
    public static IServiceCollection AddConsulClient(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "Consul")
    {
        var section = configuration.GetSection(sectionName);
        services.Configure<ConsulOptions>(section);

        var consulOptions = new ConsulOptions();
        section.Bind(consulOptions);

        if (!consulOptions.Enabled)
            return services;

        services.AddSingleton<IConsulClient>(_ => new ConsulClient(cfg =>
        {
            cfg.Address = new Uri(consulOptions.ConsulAddress);
        }));

        return services;
    }
}
