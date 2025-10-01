using Marventa.Framework.Caching.Abstractions;
using Marventa.Framework.Caching.Distributed;
using Marventa.Framework.Caching.Hybrid;
using Marventa.Framework.Caching.InMemory;
using Marventa.Framework.Configuration;
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

namespace Marventa.Framework.Extensions;

/// <summary>
/// 🚀 MARVENTA - Convention over Configuration
/// Smart configuration of the entire framework with a single method
/// </summary>
public static class MarventaExtensions
{
    /// <summary>
    /// 🎯 ONE METHOD - Intelligently configures the entire framework
    /// Auto-activates features based on appsettings.json configuration
    /// </summary>
    public static IServiceCollection AddMarventa(this IServiceCollection services, IConfiguration configuration, IHostEnvironment? environment = null)
    {
        var autoConfig = new MarventaAutoConfiguration(configuration, environment);

        // Always required
        services.AddHttpContextAccessor();

        // Exception Handling Options
        services.Configure<ExceptionHandlingOptions>(configuration.GetSection(ExceptionHandlingOptions.SectionName));

        // JWT Authentication (if configured)
        if (autoConfig.HasJwtConfiguration())
        {
            ConfigureJwtAuthentication(services, configuration);
        }

        // Caching (smart selection)
        ConfigureCaching(services, configuration, autoConfig);

        // Multi-Tenancy (if configured)
        if (autoConfig.HasMultiTenancyConfiguration())
        {
            ConfigureMultiTenancy(services, configuration);
        }

        // Rate Limiting (if configured)
        if (autoConfig.HasRateLimitingConfiguration())
        {
            ConfigureRateLimiting(services, configuration);
        }

        // RabbitMQ (if configured)
        if (autoConfig.HasRabbitMqConfiguration())
        {
            ConfigureRabbitMq(services, configuration);
        }

        // Kafka (if configured)
        if (autoConfig.HasKafkaConfiguration())
        {
            ConfigureKafka(services, configuration);
        }

        // Elasticsearch (if configured)
        if (autoConfig.HasElasticsearchConfiguration())
        {
            ConfigureElasticsearch(services, configuration);
        }

        // MongoDB (if configured)
        if (autoConfig.HasMongoDbConfiguration())
        {
            ConfigureMongoDB(services, configuration);
        }

        // Azure Storage (if configured)
        if (autoConfig.HasAzureStorageConfiguration())
        {
            ConfigureAzureStorage(services, configuration);
        }

        // AWS Storage (if configured)
        if (autoConfig.HasAwsStorageConfiguration())
        {
            ConfigureAwsStorage(services, configuration);
        }

        // Local Storage (if configured)
        if (autoConfig.HasLocalStorageConfiguration())
        {
            services.AddMarventaLocalStorage(configuration);
        }

        // MassTransit (if configured)
        if (autoConfig.HasMassTransitConfiguration())
        {
            services.AddMarventaMassTransit(configuration);
        }

        // Health Checks (if configured)
        if (autoConfig.HasHealthChecksConfiguration())
        {
            services.AddMarventaHealthChecks(configuration);
        }

        return services;
    }

    /// <summary>
    /// 🎯 ONE METHOD - Intelligently configures all middleware
    /// Automatically builds middleware pipeline based on registered services
    /// </summary>
    public static IApplicationBuilder UseMarventa(this IApplicationBuilder app, IHostEnvironment? environment = null)
    {
        var config = app.ApplicationServices.GetService<IConfiguration>();
        if (config == null) return app;

        var autoConfig = new MarventaAutoConfiguration(config, environment);

        // 1. Exception Handling (must be first)
        if (autoConfig.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseMiddleware<ExceptionHandling.ExceptionMiddleware>();
        }

        // 2. HTTPS Redirection (security)
        app.UseHttpsRedirection();

        // 3. Routing (must be before Authentication)
        app.UseRouting();

        // 4. CORS (if needed, before Authentication)
        // User can add: app.UseCors("PolicyName");

        // 5. Authentication & Authorization
        if (autoConfig.HasJwtConfiguration())
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        // 6. Multi-Tenancy (after Authentication, to read tenant from claims)
        if (autoConfig.HasMultiTenancyConfiguration())
        {
            app.UseMiddleware<TenantMiddleware>();
        }

        // 7. Rate Limiting (after Authentication, for user-based limiting)
        if (autoConfig.HasRateLimitingConfiguration())
        {
            app.UseMiddleware<Security.RateLimiting.RateLimiterMiddleware>();
        }

        // 8. Health Checks endpoint (if configured)
        if (autoConfig.HasHealthChecksConfiguration())
        {
            app.UseHealthChecks("/health");
        }

        return app;
    }

    /// <summary>
    /// For advanced users: Customizable configuration
    /// </summary>
    public static IServiceCollection AddMarventa(this IServiceCollection services, IConfiguration configuration, Action<MarventaOptions> configure)
    {
        var options = new MarventaOptions();
        configure(options);

        services.AddHttpContextAccessor();

        if (options.UseAuthentication)
        {
            ConfigureJwtAuthentication(services, configuration);
        }

        if (options.UseCache)
        {
            var autoConfig = new MarventaAutoConfiguration(configuration);
            ConfigureCaching(services, configuration, autoConfig);
        }

        if (options.UseMultiTenancy)
        {
            ConfigureMultiTenancy(services, configuration);
        }

        if (options.UseRateLimiting)
        {
            ConfigureRateLimiting(services, configuration);
        }

        if (options.UseRabbitMQ)
        {
            ConfigureRabbitMq(services, configuration);
        }

        if (options.UseKafka)
        {
            ConfigureKafka(services, configuration);
        }

        if (options.UseElasticsearch)
        {
            ConfigureElasticsearch(services, configuration);
        }

        if (options.UseMongoDB)
        {
            ConfigureMongoDB(services, configuration);
        }

        return services;
    }

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

    private static void ConfigureCaching(IServiceCollection services, IConfiguration configuration, MarventaAutoConfiguration autoConfig)
    {
        var cacheType = autoConfig.GetCacheType();

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

    #endregion
}

/// <summary>
/// Customization options for advanced users
/// </summary>
public class MarventaOptions
{
    public bool UseAuthentication { get; set; } = true;
    public bool UseCache { get; set; } = true;
    public bool UseMultiTenancy { get; set; } = false;
    public bool UseRateLimiting { get; set; } = false;
    public bool UseRabbitMQ { get; set; } = false;
    public bool UseKafka { get; set; } = false;
    public bool UseElasticsearch { get; set; } = false;
    public bool UseMongoDB { get; set; } = false;
}
