using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Marventa.Framework.Core.Models;
using Marventa.Framework.Web.Models;

namespace Marventa.Framework.Web.Extensions;

/// <summary>
/// API management service extensions (Versioning, Rate Limiting, CORS, Idempotency)
/// </summary>
internal static class ServiceCollectionExtensionsApiManagement
{
    internal static IServiceCollection AddApiManagementServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        return services
            .AddVersioningServices(configuration, options)
            .AddRateLimitingServices(configuration, options)
            .AddIdempotencyServices(configuration, options)
            .AddCorsServices(configuration, options);
    }

    private static IServiceCollection AddVersioningServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableVersioning) return services;

        services.AddApiVersioning();
        return services;
    }

    private static IServiceCollection AddRateLimitingServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableRateLimiting) return services;

        var rateLimitConfig = ConfigurationHelper.GetRateLimitConfiguration(configuration, options);
        services.AddSingleton(rateLimitConfig);
        services.AddMemoryCache();
        return services;
    }

    private static IServiceCollection AddIdempotencyServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableIdempotency) return services;

        // Add idempotency services
        return services;
    }

    private static IServiceCollection AddCorsServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        var corsOrigins = configuration.GetSection("Cors:Origins").Get<string[]>();
        if (corsOrigins?.Length > 0)
        {
            services.AddCors(opts =>
            {
                opts.AddPolicy("MarventaPolicy", builder =>
                {
                    builder.WithOrigins(corsOrigins)
                           .AllowAnyMethod()
                           .AllowAnyHeader();

                    if (!corsOrigins.Contains("*"))
                    {
                        builder.AllowCredentials();
                    }
                });
            });
        }

        return services;
    }
}
