using Marventa.Framework.Caching.Abstractions;
using Marventa.Framework.Caching.Distributed;
using Marventa.Framework.Caching.Hybrid;
using Marventa.Framework.Caching.InMemory;
using Marventa.Framework.EventBus.Abstractions;
using Marventa.Framework.EventBus.RabbitMQ;
using Marventa.Framework.MultiTenancy;
using Marventa.Framework.Search.Elasticsearch;
using Marventa.Framework.Security.Authentication;
using Marventa.Framework.Security.Authorization;
using Marventa.Framework.Security.Encryption;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Nest;
using RabbitMQ.Client;
using System.Text;

namespace Marventa.Framework.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMarventaFramework(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        return services;
    }

    public static IServiceCollection AddMarventaJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfig = configuration.GetSection("Jwt").Get<JwtConfiguration>() ?? new JwtConfiguration();
        services.Configure<JwtConfiguration>(configuration.GetSection("Jwt"));
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidAudience = jwtConfig.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret)),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

        return services;
    }

    public static IServiceCollection AddMarventaCaching(this IServiceCollection services, IConfiguration configuration, CacheType cacheType = CacheType.InMemory)
    {
        switch (cacheType)
        {
            case CacheType.InMemory:
                services.AddMemoryCache();
                services.AddSingleton<ICacheService, MemoryCacheService>();
                break;

            case CacheType.Redis:
                var redisConfig = configuration.GetSection("Redis").Get<RedisCacheConfiguration>() ?? new RedisCacheConfiguration();
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConfig.ConnectionString;
                    options.InstanceName = redisConfig.InstanceName;
                });
                services.AddSingleton<ICacheService, RedisCache>();
                break;

            case CacheType.Hybrid:
                var redisHybridConfig = configuration.GetSection("Redis").Get<RedisCacheConfiguration>() ?? new RedisCacheConfiguration();
                services.AddMemoryCache();
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisHybridConfig.ConnectionString;
                    options.InstanceName = redisHybridConfig.InstanceName;
                });
                services.AddSingleton<MemoryCacheService>();
                services.AddSingleton<RedisCache>();
                services.AddSingleton<ICacheService, HybridCacheService>(sp =>
                {
                    var memoryCache = sp.GetRequiredService<MemoryCacheService>();
                    var redisCache = sp.GetRequiredService<RedisCache>();
                    return new HybridCacheService(memoryCache, redisCache);
                });
                break;
        }

        return services;
    }

    public static IServiceCollection AddMarventaRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqHost = configuration["RabbitMQ:Host"] ?? "localhost";
        var rabbitMqUsername = configuration["RabbitMQ:Username"] ?? "guest";
        var rabbitMqPassword = configuration["RabbitMQ:Password"] ?? "guest";

        services.AddSingleton<IConnectionFactory>(sp => new ConnectionFactory
        {
            HostName = rabbitMqHost,
            UserName = rabbitMqUsername,
            Password = rabbitMqPassword
        });

        services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
        services.AddSingleton<IEventBus, RabbitMqEventBus>();

        return services;
    }

    public static IServiceCollection AddMarventaElasticsearch(this IServiceCollection services, IConfiguration configuration)
    {
        var elasticsearchUri = configuration["Elasticsearch:Uri"] ?? "http://localhost:9200";
        var settings = new ConnectionSettings(new Uri(elasticsearchUri))
            .DefaultIndex("default-index");

        services.AddSingleton<IElasticClient>(new ElasticClient(settings));
        services.AddScoped<IElasticsearchService, ElasticsearchService>();

        return services;
    }

    public static IServiceCollection AddMarventaMultiTenancy(this IServiceCollection services)
    {
        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<TenantResolver>();
        return services;
    }
}

public enum CacheType
{
    InMemory,
    Redis,
    Hybrid
}
