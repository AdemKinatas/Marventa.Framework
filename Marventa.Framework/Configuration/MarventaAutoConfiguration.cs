using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Marventa.Framework.Configuration;

/// <summary>
/// Smart auto-configuration - detects and activates features from appsettings.json
/// </summary>
public class MarventaAutoConfiguration
{
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment? _environment;

    public MarventaAutoConfiguration(IConfiguration configuration, IHostEnvironment? environment = null)
    {
        _configuration = configuration;
        _environment = environment;
    }

    public bool HasJwtConfiguration()
    {
        var jwtSection = _configuration.GetSection("Jwt");
        return jwtSection.Exists() && !string.IsNullOrEmpty(jwtSection["Secret"]);
    }

    public bool HasRateLimitingConfiguration()
    {
        return _configuration.GetSection("RateLimiting").Exists();
    }

    public bool HasMultiTenancyConfiguration()
    {
        return _configuration.GetSection("MultiTenancy").Exists();
    }

    public bool HasCachingConfiguration()
    {
        return _configuration.GetSection("Caching").Exists();
    }

    public bool HasRedisConfiguration()
    {
        var redisSection = _configuration.GetSection("Redis");
        return redisSection.Exists() && !string.IsNullOrEmpty(redisSection["ConnectionString"]);
    }

    public bool HasRabbitMqConfiguration()
    {
        return _configuration.GetSection("RabbitMQ").Exists();
    }

    public bool HasElasticsearchConfiguration()
    {
        return _configuration.GetSection("Elasticsearch").Exists();
    }

    public bool HasMongoDbConfiguration()
    {
        return _configuration.GetSection("MongoDB").Exists();
    }

    public bool HasAzureStorageConfiguration()
    {
        var azureSection = _configuration.GetSection("Azure:Storage");
        return azureSection.Exists() && !string.IsNullOrEmpty(azureSection["ConnectionString"]);
    }

    public bool HasAwsStorageConfiguration()
    {
        var awsSection = _configuration.GetSection("AWS");
        return awsSection.Exists() && !string.IsNullOrEmpty(awsSection["AccessKey"]);
    }

    public bool HasLocalStorageConfiguration()
    {
        var localStorageSection = _configuration.GetSection("LocalStorage");
        return localStorageSection.Exists();
    }

    public bool HasKafkaConfiguration()
    {
        return _configuration.GetSection("Kafka").Exists();
    }

    public bool IsDevelopment()
    {
        return _environment?.IsDevelopment() ?? false;
    }

    public bool IsProduction()
    {
        return _environment?.IsProduction() ?? false;
    }

    public CacheType GetCacheType()
    {
        var cachingSection = _configuration.GetSection("Caching");
        if (cachingSection.Exists())
        {
            var typeValue = cachingSection["Type"];
            if (Enum.TryParse<CacheType>(typeValue, true, out var cacheType))
            {
                return cacheType;
            }
        }

        // Fallback: Redis varsa Redis, yoksa InMemory
        return HasRedisConfiguration() ? CacheType.Redis : CacheType.InMemory;
    }

    public TenantResolutionStrategy GetTenantStrategy()
    {
        var tenancySection = _configuration.GetSection("MultiTenancy");
        if (tenancySection.Exists())
        {
            var strategyValue = tenancySection["Strategy"];
            if (Enum.TryParse<TenantResolutionStrategy>(strategyValue, true, out var strategy))
            {
                return strategy;
            }
        }

        return TenantResolutionStrategy.Header;
    }

    public RateLimitStrategy GetRateLimitStrategy()
    {
        var rateLimitSection = _configuration.GetSection("RateLimiting");
        if (rateLimitSection.Exists())
        {
            var strategyValue = rateLimitSection["Strategy"];
            if (Enum.TryParse<RateLimitStrategy>(strategyValue, true, out var strategy))
            {
                return strategy;
            }
        }

        return RateLimitStrategy.IpAddress;
    }

    public bool HasMassTransitConfiguration()
    {
        var massTransitSection = _configuration.GetSection("MassTransit");
        return massTransitSection.Exists() && massTransitSection["Enabled"] == "true";
    }

    public bool HasHealthChecksConfiguration()
    {
        var healthChecksSection = _configuration.GetSection("HealthChecks");
        return healthChecksSection.Exists() && healthChecksSection["Enabled"] != "false";
    }
}
