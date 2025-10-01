using Marventa.Framework.Features.Caching.Abstractions;
using Marventa.Framework.Features.Caching.Distributed;
using Marventa.Framework.Features.Caching.Hybrid;
using Marventa.Framework.Features.Caching.InMemory;
using Marventa.Framework.Configuration;
using Marventa.Framework.Features.EventBus.Abstractions;
using Marventa.Framework.Features.EventBus.Kafka;
using Marventa.Framework.Features.EventBus.RabbitMQ;
using Marventa.Framework.Features.Logging;
using Marventa.Framework.Infrastructure.MultiTenancy;
using Marventa.Framework.Features.Search.Elasticsearch;
using Marventa.Framework.Security.Authentication;
using Marventa.Framework.Security.Authorization;
using Marventa.Framework.Security.Encryption;
using Marventa.Framework.Features.Storage.Abstractions;
using Marventa.Framework.Features.Storage.AWS;
using Marventa.Framework.Features.Storage.Azure;
using Marventa.Framework.Features.Storage.Local;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Nest;
using RabbitMQ.Client;
using Serilog;
using System.Text;
using Amazon.S3;
using MassTransit;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

namespace Marventa.Framework.Extensions;

/// <summary>
/// 🚀 MARVENTA - Convention over Configuration
/// Smart configuration of the entire framework with a single method
/// </summary>
public static class MarventaExtensions
{
    /// <summary>
    /// 🎯 ONE METHOD - Configures all Marventa.Framework services
    /// Auto-activates features based on appsettings.json configuration
    /// </summary>
    public static IServiceCollection AddMarventa(this IServiceCollection services, IConfiguration configuration)
    {
        // Always required
        services.AddHttpContextAccessor();
        services.Configure<ExceptionHandlingOptions>(configuration.GetSection(ExceptionHandlingOptions.SectionName));

        // JWT Authentication (if configured)
        if (HasSection(configuration, "Jwt"))
            ConfigureJwtAuthentication(services, configuration);

        // Caching (smart selection)
        ConfigureCaching(services, configuration);

        // Multi-Tenancy (if configured)
        if (HasSection(configuration, "MultiTenancy"))
            ConfigureMultiTenancy(services, configuration);

        // Rate Limiting (if configured)
        if (HasSection(configuration, "RateLimiting"))
            ConfigureRateLimiting(services, configuration);

        // RabbitMQ (if configured)
        if (HasSection(configuration, "RabbitMQ"))
            ConfigureRabbitMq(services, configuration);

        // Kafka (if configured)
        if (HasSection(configuration, "Kafka"))
            ConfigureKafka(services, configuration);

        // MassTransit (if configured)
        if (configuration.GetSection("MassTransit")["Enabled"] == "true")
            ConfigureMassTransit(services, configuration);

        // Elasticsearch (if configured)
        if (HasSection(configuration, "Elasticsearch"))
            ConfigureElasticsearch(services, configuration);

        // MongoDB (if configured)
        if (HasSection(configuration, "MongoDB"))
            ConfigureMongoDB(services, configuration);

        // Azure Storage (if configured)
        if (HasSection(configuration, "Azure:Storage"))
            ConfigureAzureStorage(services, configuration);

        // AWS Storage (if configured)
        if (HasSection(configuration, "AWS"))
            ConfigureAwsStorage(services, configuration);

        // Local Storage (if configured)
        if (HasSection(configuration, "LocalStorage"))
            ConfigureLocalStorage(services, configuration);

        // Health Checks (if configured)
        if (HasSection(configuration, "HealthChecks") && configuration.GetSection("HealthChecks")["Enabled"] != "false")
            ConfigureHealthChecks(services, configuration);

        // Serilog Logging (if configured)
        if (HasSection(configuration, "Serilog"))
            ConfigureSerilog(configuration);

        // OpenTelemetry (if configured)
        if (HasSection(configuration, "OpenTelemetry"))
            ConfigureOpenTelemetry(services, configuration);

        return services;
    }

    /// <summary>
    /// 🎯 ONE METHOD - Configures all Marventa.Framework middleware
    /// Automatically builds middleware pipeline based on registered services
    /// </summary>
    public static IApplicationBuilder UseMarventa(this IApplicationBuilder app, IConfiguration configuration)
    {
        // 1. Exception Handling (must be first)
        app.UseMiddleware<Middleware.ExceptionMiddleware>();

        // 2. HTTPS Redirection (security)
        app.UseHttpsRedirection();

        // 3. Routing (must be before Authentication)
        app.UseRouting();

        // 4. Authentication & Authorization
        if (HasSection(configuration, "Jwt"))
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        // 5. Multi-Tenancy (after Authentication, to read tenant from claims)
        if (HasSection(configuration, "MultiTenancy"))
            app.UseMiddleware<TenantMiddleware>();

        // 6. Rate Limiting (after Authentication, for user-based limiting)
        if (HasSection(configuration, "RateLimiting"))
            app.UseMiddleware<Security.RateLimiting.RateLimiterMiddleware>();

        // 7. Health Checks endpoint (if configured)
        if (HasSection(configuration, "HealthChecks") && configuration.GetSection("HealthChecks")["Enabled"] != "false")
            app.UseHealthChecks("/health");

        return app;
    }

    #region Helper Methods

    private static bool HasSection(IConfiguration configuration, string sectionName)
    {
        return configuration.GetSection(sectionName).Exists();
    }

    #endregion

    #region Private Configuration Methods

    private static void ConfigureJwtAuthentication(IServiceCollection services, IConfiguration configuration)
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
    }

    private static void ConfigureCaching(IServiceCollection services, IConfiguration configuration)
    {
        var cacheType = GetCacheType(configuration);

        switch (cacheType)
        {
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

            default: // InMemory
                services.AddMemoryCache();
                services.AddSingleton<ICacheService, MemoryCacheService>();
                break;
        }
    }

    private static CacheType GetCacheType(IConfiguration configuration)
    {
        var cachingSection = configuration.GetSection("Caching");
        if (cachingSection.Exists())
        {
            var typeValue = cachingSection["Type"];
            if (Enum.TryParse<CacheType>(typeValue, true, out var cacheType))
                return cacheType;
        }

        var redisSection = configuration.GetSection("Redis");
        return redisSection.Exists() && !string.IsNullOrEmpty(redisSection["ConnectionString"])
            ? CacheType.Redis
            : CacheType.InMemory;
    }

    private static void ConfigureMultiTenancy(IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection(MultiTenancyOptions.SectionName).Get<MultiTenancyOptions>()
            ?? new MultiTenancyOptions();

        services.Configure<MultiTenancyOptions>(configuration.GetSection(MultiTenancyOptions.SectionName));
        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<TenantResolver>();
    }

    private static void ConfigureRateLimiting(IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection(RateLimitingOptions.SectionName).Get<RateLimitingOptions>()
            ?? new RateLimitingOptions();

        services.Configure<RateLimitingOptions>(configuration.GetSection(RateLimitingOptions.SectionName));
        services.AddMemoryCache();
    }

    private static void ConfigureRabbitMq(IServiceCollection services, IConfiguration configuration)
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
    }

    private static void ConfigureKafka(IServiceCollection services, IConfiguration configuration)
    {
        var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
        var groupId = configuration["Kafka:GroupId"] ?? "default-group";

        services.AddSingleton<IKafkaProducer>(sp => new KafkaProducer(bootstrapServers));
        services.AddSingleton<IKafkaConsumer>(sp =>
        {
            var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<KafkaConsumer>>();
            return new KafkaConsumer(bootstrapServers, groupId, logger);
        });
    }

    private static void ConfigureElasticsearch(IServiceCollection services, IConfiguration configuration)
    {
        var elasticsearchUri = configuration["Elasticsearch:Uri"] ?? "http://localhost:9200";
        var settings = new ConnectionSettings(new Uri(elasticsearchUri))
            .DefaultIndex("default-index");

        services.AddSingleton<IElasticClient>(new ElasticClient(settings));
        services.AddScoped<IElasticsearchService, ElasticsearchService>();
    }

    private static void ConfigureMongoDB(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["MongoDB:ConnectionString"] ?? "mongodb://localhost:27017";
        var databaseName = configuration["MongoDB:DatabaseName"] ?? "default";

        services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
        services.AddScoped(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(databaseName);
        });
    }

    private static void ConfigureAzureStorage(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["Azure:Storage:ConnectionString"]
            ?? throw new InvalidOperationException("Azure Storage connection string is not configured");
        var containerName = configuration["Azure:Storage:ContainerName"] ?? "default";

        services.AddSingleton<IStorageService>(new AzureBlobStorage(connectionString, containerName));
    }

    private static void ConfigureAwsStorage(IServiceCollection services, IConfiguration configuration)
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
    }

    private static void ConfigureLocalStorage(IServiceCollection services, IConfiguration configuration)
    {
        var basePath = configuration["LocalStorage:BasePath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        var baseUrl = configuration["LocalStorage:BaseUrl"];

        services.AddSingleton<IStorageService>(new Features.Storage.Local.LocalFileStorage(basePath, baseUrl));
    }

    private static void ConfigureHealthChecks(IServiceCollection services, IConfiguration configuration)
    {
        var healthChecks = services.AddHealthChecks();

        // Add database health check if connection string exists
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(connectionString))
        {
            healthChecks.AddSqlServer(connectionString, name: "database");
        }

        // Add Redis health check if configured
        var redisConnection = configuration["Redis:ConnectionString"];
        if (!string.IsNullOrEmpty(redisConnection))
        {
            healthChecks.AddRedis(redisConnection, name: "redis");
        }

        // Add RabbitMQ health check if configured
        var rabbitMqHost = configuration["RabbitMQ:Host"];
        if (!string.IsNullOrEmpty(rabbitMqHost))
        {
            healthChecks.AddCheck<Infrastructure.HealthChecks.RabbitMqHealthCheck>("rabbitmq");
        }
    }

    private static void ConfigureMassTransit(IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqHost = configuration["RabbitMQ:Host"] ?? "localhost";
        var rabbitMqVirtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/";
        var rabbitMqUsername = configuration["RabbitMQ:Username"] ?? "guest";
        var rabbitMqPassword = configuration["RabbitMQ:Password"] ?? "guest";

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqHost, rabbitMqVirtualHost, h =>
                {
                    h.Username(rabbitMqUsername);
                    h.Password(rabbitMqPassword);
                });

                cfg.ConfigureEndpoints(context);
            });
        });
    }

    private static void ConfigureSerilog(IConfiguration configuration)
    {
        Log.Logger = SerilogConfiguration.ConfigureSerilog(configuration, configuration["ApplicationName"] ?? "Marventa");
    }

    private static void ConfigureOpenTelemetry(IServiceCollection services, IConfiguration configuration)
    {
        var serviceName = configuration["OpenTelemetry:ServiceName"] ?? configuration["ApplicationName"] ?? "Marventa";
        var otlpEndpoint = configuration["OpenTelemetry:OtlpEndpoint"];

        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder
                    .AddSource(serviceName)
                    .ConfigureResource(resource => resource.AddService(serviceName))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                if (!string.IsNullOrEmpty(otlpEndpoint))
                {
                    builder.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otlpEndpoint);
                    });
                }
            });
    }

    #endregion
}
