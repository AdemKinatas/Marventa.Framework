using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Marventa.Framework.Core.Interfaces.Caching;
using Marventa.Framework.Core.Interfaces.MultiTenancy;
using Marventa.Framework.Infrastructure.Caching;

namespace Marventa.Framework.Infrastructure.Extensions;

public static class CacheServiceRegistrationExtensions
{
    /// <summary>
    /// Adds in-memory caching
    /// </summary>
    public static IServiceCollection AddMemoryCache(this IServiceCollection services, Action<TenantCacheOptions>? configure = null)
    {
        services.AddMemoryCache();

        if (configure != null)
            services.Configure(configure);

        services.AddScoped<ICacheService, MemoryCacheService>();
        return services;
    }

    /// <summary>
    /// Adds Redis caching with StackExchange.Redis
    /// </summary>
    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisOptions = configuration.GetSection("Redis").Get<RedisCacheOptions>() ?? new RedisCacheOptions();
        return services.AddRedisCache(redisOptions);
    }

    /// <summary>
    /// Adds Redis caching with custom options
    /// </summary>
    public static IServiceCollection AddRedisCache(this IServiceCollection services, RedisCacheOptions options)
    {
        services.Configure<RedisCacheOptions>(opt =>
        {
            opt.ConnectionString = options.ConnectionString;
            opt.DefaultExpiration = options.DefaultExpiration;
            opt.EnableMultiTenancy = options.EnableMultiTenancy;
            opt.KeyPrefix = options.KeyPrefix;
            opt.Database = options.Database;
            opt.ThrowOnError = options.ThrowOnError;
            opt.AllowFlushDatabase = options.AllowFlushDatabase;
        });

        // Register Redis connection
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configOptions = ConfigurationOptions.Parse(options.ConnectionString);
            configOptions.DefaultDatabase = options.Database;

            var logger = sp.GetRequiredService<ILogger<RedisCacheService>>();
            logger.LogInformation("Connecting to Redis: {Endpoint}", configOptions.EndPoints[0]);

            return ConnectionMultiplexer.Connect(configOptions);
        });

        // Register cache service
        services.AddScoped<ICacheService, RedisCacheService>();

        return services;
    }

    /// <summary>
    /// Adds distributed caching using Microsoft.Extensions.Caching.Distributed
    /// </summary>
    public static IServiceCollection AddDistributedCache(this IServiceCollection services, Action<IServiceCollection> configure)
    {
        // Let the caller configure IDistributedCache (Redis, SQL Server, etc.)
        configure(services);

        // Register our wrapper
        services.AddScoped<ICacheService, DistributedCacheService>();

        return services;
    }
}