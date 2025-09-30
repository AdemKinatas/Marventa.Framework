using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using StackExchange.Redis;
using Marventa.Framework.Core.Interfaces.Caching;
using Marventa.Framework.Core.Interfaces.Data;
using Marventa.Framework.Core.Models;
using Marventa.Framework.Infrastructure.Caching;
using Marventa.Framework.Infrastructure.Data;

namespace Marventa.Framework.Web.Extensions;

/// <summary>
/// Core infrastructure service extensions
/// </summary>
internal static class ServiceCollectionExtensionsCore
{
    internal static IServiceCollection AddCoreInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        return services
            .AddRepositoryPattern(configuration, options)
            .AddLoggingServices(configuration, options)
            .AddCachingServices(configuration, options)
            .AddValidationServices(configuration, options)
            .AddHealthCheckServices(configuration, options)
            .AddCompressionServices(configuration, options)
            .AddExceptionHandlingServices(configuration, options);
    }

    private static IServiceCollection AddRepositoryPattern(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        // Register generic repository pattern
        services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
        return services;
    }

    private static IServiceCollection AddLoggingServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableLogging) return services;

        services.AddSingleton<Serilog.ILogger>(serviceProvider =>
        {
            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console();

            if (options.LoggingOptions.Provider == "Serilog")
            {
                loggerConfig.ReadFrom.Configuration(configuration);
            }

            return loggerConfig.CreateLogger();
        });

        services.AddLogging(builder => builder.AddSerilog());
        return services;
    }

    private static IServiceCollection AddCachingServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableCaching) return services;

        var cachingProvider = configuration["Marventa:Caching:Provider"] ?? options.CachingOptions.Provider;
        var redisConnectionString = ConfigurationHelper.GetRedisConnectionString(configuration, options);

        if (cachingProvider == "Redis" && !string.IsNullOrEmpty(redisConnectionString))
        {
            services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
            {
                var configOptions = ConfigurationOptions.Parse(redisConnectionString);
                configOptions.AbortOnConnectFail = false;
                return ConnectionMultiplexer.Connect(configOptions);
            });
            services.AddScoped<ICacheService, RedisCacheService>();
        }
        else
        {
            services.AddMemoryCache();
            services.AddScoped<ICacheService, MemoryCacheService>();
        }

        services.AddResponseCaching();
        return services;
    }

    private static IServiceCollection AddValidationServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableValidation) return services;

        // Add validation services here
        return services;
    }

    private static IServiceCollection AddHealthCheckServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableHealthChecks) return services;

        services.AddHealthChecks();
        return services;
    }

    private static IServiceCollection AddCompressionServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableCompression) return services;

        // ResponseCompression services would be configured here
        // Requires Microsoft.AspNetCore.ResponseCompression package
        return services;
    }

    private static IServiceCollection AddExceptionHandlingServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableExceptionHandling) return services;

        // Exception handling services are registered via middleware
        return services;
    }
}
