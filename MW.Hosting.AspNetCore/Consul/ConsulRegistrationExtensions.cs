using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MW.Hosting.AspNetCore.Options;

namespace MW.Hosting.AspNetCore.Consul;

public static class ConsulRegistrationExtensions
{
    public static IServiceCollection AddConsulRegistration(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "Consul")
    {
        var section = configuration.GetSection(sectionName);

        var consulOptions = new ConsulOptions();
        section.Bind(consulOptions);

        services.Configure<ConsulOptions>(section);
        services.AddSingleton<IValidateOptions<ConsulOptions>, ConsulOptionsValidator>();

        if (!consulOptions.Enabled)
            return services;

        services.AddHostedService<ConsulRegistrationHostedService>();

        return services;
    }
}
