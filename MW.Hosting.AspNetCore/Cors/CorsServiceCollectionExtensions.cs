using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MW.Hosting.AspNetCore.Options;

namespace MW.Hosting.AspNetCore.Cors;

public static class CorsServiceCollectionExtensions
{
    public static IServiceCollection AddDefaultCors(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "Cors")
    {
        var section = configuration.GetSection(sectionName);
        services.Configure<CorsOptions>(section);
        services.AddSingleton<IValidateOptions<CorsOptions>, CorsOptionsValidator>();

        var corsOptions = new CorsOptions();
        section.Bind(corsOptions);

        services.AddCors(options =>
        {
            options.AddPolicy(corsOptions.PolicyName, builder =>
            {
                builder.WithOrigins(corsOptions.AllowedOrigins);

                if (corsOptions.AllowAnyHeader)
                    builder.AllowAnyHeader();
                if (corsOptions.AllowAnyMethod)
                    builder.AllowAnyMethod();
                if (corsOptions.AllowCredentials)
                    builder.AllowCredentials();
                if (corsOptions.ExposedHeaders is { Length: > 0 })
                    builder.WithExposedHeaders(corsOptions.ExposedHeaders);
            });
        });

        return services;
    }

    public static IServiceCollection AddDefaultCors(
        this IServiceCollection services,
        Action<CorsOptions> configure)
    {
        var corsOptions = new CorsOptions();
        configure(corsOptions);

        services.Configure(configure);
        services.AddSingleton<IValidateOptions<CorsOptions>, CorsOptionsValidator>();

        services.AddCors(options =>
        {
            options.AddPolicy(corsOptions.PolicyName, builder =>
            {
                builder.WithOrigins(corsOptions.AllowedOrigins);

                if (corsOptions.AllowAnyHeader)
                    builder.AllowAnyHeader();
                if (corsOptions.AllowAnyMethod)
                    builder.AllowAnyMethod();
                if (corsOptions.AllowCredentials)
                    builder.AllowCredentials();
                if (corsOptions.ExposedHeaders is { Length: > 0 })
                    builder.WithExposedHeaders(corsOptions.ExposedHeaders);
            });
        });

        return services;
    }
}
