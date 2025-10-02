using Marventa.Framework.Configuration;

using Marventa.Framework.Features.Caching.Abstractions;
using Marventa.Framework.Features.Caching.Distributed;
using Marventa.Framework.Features.Caching.Hybrid;
using Marventa.Framework.Features.Caching.InMemory;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marventa.Framework.Extensions;

/// <summary>
/// Provides extension methods for configuring caching services.
/// Supports in-memory, Redis, and hybrid caching strategies.
/// </summary>
public static class CachingExtensions
{
    /// <summary>
    /// Adds caching services based on configuration.
    /// Automatically detects cache type from configuration and registers appropriate services.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddMarventaCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var cacheType = DetermineCacheType(configuration);

        return cacheType switch
        {
            CacheType.Redis => services.AddRedisCaching(configuration),
            CacheType.Hybrid => services.AddHybridCaching(configuration),
            _ => services.AddInMemoryCaching(configuration)
        };
    }

    /// <summary>
    /// Adds in-memory caching services with optional configuration.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">Optional configuration for memory cache settings.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddInMemoryCaching(
        this IServiceCollection services,
        IConfiguration? configuration = null)
    {
        var cacheOptions = configuration?.GetConfigurationSection<MemoryCacheConfiguration>(MemoryCacheConfiguration.SectionName)
            ?? new MemoryCacheConfiguration();

        services.AddMemoryCache(options =>
        {
            options.SizeLimit = cacheOptions.SizeLimit;
            options.CompactionPercentage = cacheOptions.CompactionPercentage;

            if (cacheOptions.ExpirationScanFrequency.HasValue)
            {
                options.ExpirationScanFrequency = cacheOptions.ExpirationScanFrequency.Value;
            }
        });

        services.AddSingleton<ICacheService, MemoryCacheService>();

        return services;
    }

    /// <summary>
    /// Adds Redis distributed caching services.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when Redis configuration is missing.</exception>
    public static IServiceCollection AddRedisCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var redisConfig = configuration.GetConfigurationSection<RedisCacheConfiguration>(ConfigurationKeys.Redis);

        if (string.IsNullOrWhiteSpace(redisConfig.ConnectionString))
        {
            throw new InvalidOperationException(
                "Redis connection string is required when using Redis caching. " +
                $"Please configure '{ConfigurationKeys.RedisConnectionString}' in your settings.");
        }

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConfig.ConnectionString;
            options.InstanceName = redisConfig.InstanceName;
        });

        services.AddSingleton<ICacheService, RedisCache>();

        return services;
    }

    /// <summary>
    /// Adds hybrid caching services combining in-memory and Redis caching.
    /// Provides two-level caching with L1 (memory) and L2 (Redis) cache layers.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when Redis configuration is missing.</exception>
    public static IServiceCollection AddHybridCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var redisConfig = configuration.GetConfigurationSection<RedisCacheConfiguration>(ConfigurationKeys.Redis);

        if (string.IsNullOrWhiteSpace(redisConfig.ConnectionString))
        {
            throw new InvalidOperationException(
                "Redis connection string is required when using hybrid caching. " +
                $"Please configure '{ConfigurationKeys.RedisConnectionString}' in your settings.");
        }

        // Register both memory and Redis cache
        var cacheOptions = configuration.GetConfigurationSection<MemoryCacheConfiguration>(MemoryCacheConfiguration.SectionName);

        services.AddMemoryCache(options =>
        {
            options.SizeLimit = cacheOptions.SizeLimit;
            options.CompactionPercentage = cacheOptions.CompactionPercentage;

            if (cacheOptions.ExpirationScanFrequency.HasValue)
            {
                options.ExpirationScanFrequency = cacheOptions.ExpirationScanFrequency.Value;
            }
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConfig.ConnectionString;
            options.InstanceName = redisConfig.InstanceName;
        });

        // Register individual cache implementations
        services.AddSingleton<MemoryCacheService>();
        services.AddSingleton<RedisCache>();

        // Register hybrid cache as the main ICacheService
        services.AddSingleton<ICacheService, HybridCacheService>(serviceProvider =>
        {
            var memoryCache = serviceProvider.GetRequiredService<MemoryCacheService>();
            var redisCache = serviceProvider.GetRequiredService<RedisCache>();
            return new HybridCacheService(memoryCache, redisCache);
        });

        return services;
    }

    /// <summary>
    /// Determines the cache type from configuration.
    /// </summary>
    private static CacheType DetermineCacheType(IConfiguration configuration)
    {
        // Check explicit caching configuration
        var cachingSection = configuration.GetSection(ConfigurationKeys.Caching);
        if (cachingSection.Exists())
        {
            var typeValue = cachingSection["Type"];
            if (!string.IsNullOrEmpty(typeValue) &&
                Enum.TryParse<CacheType>(typeValue, ignoreCase: true, out var explicitCacheType))
            {
                return explicitCacheType;
            }
        }

        // Fall back to Redis section detection
        var redisSection = configuration.GetSection(ConfigurationKeys.Redis);
        var hasRedisConfig = redisSection.Exists() &&
                            !string.IsNullOrEmpty(redisSection[ConfigurationKeys.ConnectionString]);

        return hasRedisConfig ? CacheType.Redis : CacheType.InMemory;
    }

    /// <summary>
    /// Adds ASP.NET Core output caching middleware.
    /// Requires .NET 7 or higher.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddMarventaOutputCache(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (!configuration.IsSectionEnabled(OutputCacheOptions.SectionName))
        {
            return services;
        }

        var outputCacheOptions = configuration.GetConfigurationSection<OutputCacheOptions>(
            OutputCacheOptions.SectionName);

        services.AddOutputCache(options =>
        {
            options.AddBasePolicy(builder =>
            {
                builder.Cache();
                builder.Expire(TimeSpan.FromSeconds(outputCacheOptions.DefaultExpirationSeconds));

                if (outputCacheOptions.VaryByQuery)
                {
                    builder.SetVaryByQuery("*");
                }

                if (outputCacheOptions.VaryByHeader && outputCacheOptions.VaryByHeaderNames.Length > 0)
                {
                    builder.SetVaryByHeader(outputCacheOptions.VaryByHeaderNames);
                }
            });
        });

        return services;
    }
}
