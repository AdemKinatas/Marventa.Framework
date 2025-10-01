using Amazon.S3;
using FluentValidation;
using Mapster;
using Marventa.Framework.Configuration;
using Marventa.Framework.Features.Caching.Abstractions;
using Marventa.Framework.Features.Caching.Distributed;
using Marventa.Framework.Features.Caching.Hybrid;
using Marventa.Framework.Features.Caching.InMemory;
using Marventa.Framework.Features.EventBus.Abstractions;
using Marventa.Framework.Features.EventBus.Kafka;
using Marventa.Framework.Features.EventBus.RabbitMQ;
using Marventa.Framework.Features.Logging;
using Marventa.Framework.Features.Search.Elasticsearch;
using Marventa.Framework.Features.Storage.Abstractions;
using Marventa.Framework.Features.Storage.AWS;
using Marventa.Framework.Features.Storage.Azure;
using Marventa.Framework.Infrastructure.MultiTenancy;
using Marventa.Framework.Security.Authentication;
using Marventa.Framework.Security.Authorization;
using Marventa.Framework.Security.Encryption;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Nest;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using RabbitMQ.Client;
using Serilog;
using System.Text;

namespace Marventa.Framework.Extensions;

public static class MarventaExtensions
{
    public static IServiceCollection AddMarventa(
        this IServiceCollection services,
        IConfiguration configuration,
        params System.Reflection.Assembly[] assemblies)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            });

        services.AddEndpointsApiExplorer();
        services.AddHttpContextAccessor();
        services.Configure<ExceptionHandlingOptions>(configuration.GetSection(ExceptionHandlingOptions.SectionName));

        var corsOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        if (corsOrigins?.Length > 0)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder => builder
                    .WithOrigins(corsOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
        }

        var assembliesToScan = assemblies.Length > 0
            ? assemblies
            : new[] { System.Reflection.Assembly.GetCallingAssembly() };

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assembliesToScan));
        services.AddValidatorsFromAssemblies(assembliesToScan);
        services.AddMapster();
        foreach (var assembly in assembliesToScan)
        {
            TypeAdapterConfig.GlobalSettings.Scan(assembly);
        }

        if (HasSection(configuration, "Jwt"))
            ConfigureJwtAuthentication(services, configuration);

        ConfigureCaching(services, configuration);

        if (HasSection(configuration, "MultiTenancy"))
            ConfigureMultiTenancy(services, configuration);

        if (HasSection(configuration, "RateLimiting"))
            ConfigureRateLimiting(services, configuration);

        if (HasSection(configuration, "RabbitMQ"))
            ConfigureRabbitMq(services, configuration);

        if (HasSection(configuration, "Kafka"))
            ConfigureKafka(services, configuration);

        if (configuration.GetSection("MassTransit")["Enabled"] == "true")
            ConfigureMassTransit(services, configuration, assembliesToScan);

        if (HasSection(configuration, "Elasticsearch"))
            ConfigureElasticsearch(services, configuration);

        if (HasSection(configuration, "MongoDB"))
            ConfigureMongoDB(services, configuration);

        if (HasSection(configuration, "Azure:Storage"))
            ConfigureAzureStorage(services, configuration);

        if (HasSection(configuration, "AWS"))
            ConfigureAwsStorage(services, configuration);

        if (HasSection(configuration, "LocalStorage"))
            ConfigureLocalStorage(services, configuration);

        if (HasSection(configuration, "HealthChecks") && configuration.GetSection("HealthChecks")["Enabled"] != "false")
            ConfigureHealthChecks(services, configuration);

        if (HasSection(configuration, "Serilog"))
            ConfigureSerilog(configuration);

        if (HasSection(configuration, "OpenTelemetry"))
            ConfigureOpenTelemetry(services, configuration);

        return services;
    }

    public static IApplicationBuilder UseMarventa(this IApplicationBuilder app, IConfiguration configuration)
    {
        app.UseMiddleware<Middleware.ExceptionMiddleware>();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        var corsOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        if (corsOrigins?.Length > 0)
            app.UseCors();

        if (HasSection(configuration, "Jwt"))
            app.UseAuthentication();

        if (HasSection(configuration, "MultiTenancy"))
            app.UseMiddleware<TenantMiddleware>();

        if (HasSection(configuration, "Jwt"))
            app.UseAuthorization();

        if (HasSection(configuration, "RateLimiting"))
            app.UseMiddleware<Security.RateLimiting.RateLimiterMiddleware>();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            if (HasSection(configuration, "HealthChecks") && configuration.GetSection("HealthChecks")["Enabled"] != "false")
                endpoints.MapHealthChecks("/health");
        });

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

            default:
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

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(connectionString))
            healthChecks.AddSqlServer(connectionString, name: "database");

        var redisConnection = configuration["Redis:ConnectionString"];
        if (!string.IsNullOrEmpty(redisConnection))
            healthChecks.AddRedis(redisConnection, name: "redis");

        var rabbitMqHost = configuration["RabbitMQ:Host"];
        if (!string.IsNullOrEmpty(rabbitMqHost))
            healthChecks.AddCheck<Infrastructure.HealthChecks.RabbitMqHealthCheck>("rabbitmq");
    }

    private static void ConfigureMassTransit(IServiceCollection services, IConfiguration configuration, params System.Reflection.Assembly[] assemblies)
    {
        var rabbitMqHost = configuration["RabbitMQ:Host"] ?? "localhost";
        var rabbitMqVirtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/";
        var rabbitMqUsername = configuration["RabbitMQ:Username"] ?? "guest";
        var rabbitMqPassword = configuration["RabbitMQ:Password"] ?? "guest";

        var assembliesToScan = assemblies.Length > 0
            ? assemblies
            : new[] { System.Reflection.Assembly.GetCallingAssembly() };

        services.AddMassTransit(x =>
        {
            foreach (var assembly in assembliesToScan)
                x.AddConsumers(assembly);

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
