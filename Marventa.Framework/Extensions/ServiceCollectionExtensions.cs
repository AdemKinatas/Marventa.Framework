using Amazon.S3;
using Azure.Storage.Blobs;
using Marventa.Framework.Caching.Abstractions;
using Marventa.Framework.Caching.Distributed;
using Marventa.Framework.Caching.Hybrid;
using Marventa.Framework.Caching.InMemory;
using Marventa.Framework.EventBus.Abstractions;
using Marventa.Framework.EventBus.Kafka;
using Marventa.Framework.EventBus.RabbitMQ;
using Marventa.Framework.Logging;
using Marventa.Framework.MultiTenancy;
using Marventa.Framework.Search.Elasticsearch;
using Marventa.Framework.Security.Authentication;
using Marventa.Framework.Security.Authorization;
using Marventa.Framework.Security.Encryption;
using Marventa.Framework.Storage.Abstractions;
using Marventa.Framework.Storage.AWS;
using Marventa.Framework.Storage.Azure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Nest;
using RabbitMQ.Client;
using Serilog;
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

    public static IServiceCollection AddMarventaLogging(this IServiceCollection services, IConfiguration configuration, string applicationName)
    {
        Log.Logger = SerilogConfiguration.ConfigureSerilog(configuration, applicationName);
        services.AddSerilog();
        return services;
    }

    public static IServiceCollection AddMarventaAzureStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["Azure:Storage:ConnectionString"]
            ?? throw new InvalidOperationException("Azure Storage connection string is not configured");
        var containerName = configuration["Azure:Storage:ContainerName"] ?? "default";

        services.AddSingleton<IStorageService>(new AzureBlobStorage(connectionString, containerName));
        return services;
    }

    public static IServiceCollection AddMarventaAwsStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var accessKey = configuration["AWS:AccessKey"]
            ?? throw new InvalidOperationException("AWS AccessKey is not configured");
        var secretKey = configuration["AWS:SecretKey"]
            ?? throw new InvalidOperationException("AWS SecretKey is not configured");
        var region = configuration["AWS:Region"] ?? "us-east-1";
        var bucketName = configuration["AWS:BucketName"] ?? "default";

        var s3Client = new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.GetBySystemName(region));
        services.AddSingleton<IAmazonS3>(s3Client);
        services.AddSingleton<IStorageService>(new S3Storage(s3Client, bucketName));
        return services;
    }

    public static IServiceCollection AddMarventaMongoDB(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["MongoDB:ConnectionString"] ?? "mongodb://localhost:27017";
        var databaseName = configuration["MongoDB:DatabaseName"] ?? "default";

        services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
        services.AddScoped(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(databaseName);
        });
        return services;
    }

    public static IServiceCollection AddMarventaKafka(this IServiceCollection services, IConfiguration configuration)
    {
        var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
        var groupId = configuration["Kafka:GroupId"] ?? "default-group";

        services.AddSingleton<IKafkaProducer>(sp => new KafkaProducer(bootstrapServers));
        services.AddSingleton<IKafkaConsumer>(sp =>
        {
            var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<KafkaConsumer>>();
            return new KafkaConsumer(bootstrapServers, groupId, logger);
        });

        return services;
    }
}

public enum CacheType
{
    InMemory,
    Redis,
    Hybrid
}
