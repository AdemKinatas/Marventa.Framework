using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using StackExchange.Redis;
using Marventa.Framework.Core.Interfaces.Caching;
using Marventa.Framework.Core.Interfaces.Data;
using Marventa.Framework.Core.Interfaces.MultiTenancy;
using Marventa.Framework.Core.Interfaces.Sagas;
using Marventa.Framework.Core.Interfaces.Storage;
using Marventa.Framework.Core.Interfaces.Services;
using Marventa.Framework.Core.Interfaces.Security;
using Marventa.Framework.Core.Interfaces.MachineLearning;
using Marventa.Framework.Core.Models;
using Marventa.Framework.Core.Configuration;
using Marventa.Framework.Web.Models;
using Marventa.Framework.Infrastructure.Caching;
using Marventa.Framework.Infrastructure.Services;
using Marventa.Framework.Infrastructure.Services.Security;
using Marventa.Framework.Infrastructure.Storage;
using Marventa.Framework.Infrastructure.Services.FileServices;
using Marventa.Framework.Infrastructure.Sagas;
using Marventa.Framework.Infrastructure.Multitenancy;
using Marventa.Framework.Infrastructure.Authorization;
using Marventa.Framework.Web.Middleware;
using FluentValidation;

namespace Marventa.Framework.Web.Extensions;

/// <summary>
/// Marventa Framework - Enterprise-grade modular framework extensions
/// Implements Clean Architecture and SOLID principles
/// </summary>
public static class MarventaExtensions
{
    /// <summary>
    /// Adds Marventa Framework services with clean architecture approach
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <param name="configureOptions">Framework options configuration</param>
    /// <returns>Service collection for chaining</returns>
    /// <exception cref="ArgumentNullException">Thrown when required parameters are null</exception>
    public static IServiceCollection AddMarventaFramework(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<MarventaFrameworkOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(configureOptions);

        var options = ConfigureFrameworkOptions(configureOptions);
        services.AddSingleton(options);
        services.AddHttpContextAccessor();

        return services
            .AddCoreInfrastructure(configuration, options)
            .AddSecurityServices(configuration, options)
            .AddCommunicationServices(configuration, options)
            .AddDataStorageServices(configuration, options)
            .AddApiManagementServices(configuration, options)
            .AddPerformanceServices(configuration, options)
            .AddMonitoringServices(configuration, options)
            .AddBackgroundProcessingServices(configuration, options)
            .AddEnterpriseArchitectureServices(configuration, options)
            .AddSearchAndAiServices(configuration, options)
            .AddBusinessServices(configuration, options);
    }

    /// <summary>
    /// Configures Marventa Framework middleware pipeline with clean architecture approach
    /// </summary>
    /// <param name="app">Application builder</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>Application builder for chaining</returns>
    /// <exception cref="ArgumentNullException">Thrown when required parameters are null</exception>
    public static IApplicationBuilder UseMarventaFramework(
        this IApplicationBuilder app,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(configuration);

        var options = app.ApplicationServices.GetRequiredService<MarventaFrameworkOptions>();
        var logger = app.ApplicationServices.GetRequiredService<ILogger<MarventaUnifiedMiddleware>>();

        return app.ConfigureMiddlewarePipeline(configuration, options, logger);
    }

    #region Private Configuration Methods

    private static MarventaFrameworkOptions ConfigureFrameworkOptions(Action<MarventaFrameworkOptions> configureOptions)
    {
        var options = new MarventaFrameworkOptions();
        configureOptions(options);
        return options;
    }

    #endregion

    #region Core Infrastructure Services

    private static IServiceCollection AddCoreInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        return services
            .AddLoggingServices(configuration, options)
            .AddCachingServices(configuration, options)
            .AddValidationServices(configuration, options)
            .AddHealthCheckServices(configuration, options)
            .AddCompressionServices(configuration, options)
            .AddExceptionHandlingServices(configuration, options);
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
        var redisConnectionString = GetRedisConnectionString(configuration, options);

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

        services.AddResponseCompression(opts => opts.EnableForHttps = true);
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

    #endregion

    #region Security Services

    private static IServiceCollection AddSecurityServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        return services
            .AddCoreSecurityServices(configuration, options)
            .AddJwtServices(configuration, options)
            .AddApiKeyServices(configuration, options)
            .AddEncryptionServices(configuration, options);
    }

    private static IServiceCollection AddCoreSecurityServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableSecurity) return services;

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        return services;
    }

    private static IServiceCollection AddJwtServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableJWT) return services;

        services.AddScoped<ITokenService, TokenService>();
        services.AddAuthentication();
        services.AddAuthorization();
        return services;
    }

    private static IServiceCollection AddApiKeyServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableApiKeys) return services;

        // API Key validation is handled in middleware
        return services;
    }

    private static IServiceCollection AddEncryptionServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableEncryption) return services;

        services.AddScoped<IEncryptionService, EncryptionService>();
        return services;
    }

    #endregion

    #region Communication Services

    private static IServiceCollection AddCommunicationServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        return services
            .AddEmailServices(configuration, options)
            .AddSmsServices(configuration, options)
            .AddHttpClientServices(configuration, options);
    }

    private static IServiceCollection AddEmailServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableEmail) return services;

        services.AddScoped<IEmailService, EmailService>();
        return services;
    }

    private static IServiceCollection AddSmsServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableSMS) return services;

        services.AddScoped<ISmsService, SmsService>();
        return services;
    }

    private static IServiceCollection AddHttpClientServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableHttpClient) return services;

        services.AddHttpClient();
        return services;
    }

    #endregion

    #region Data Storage Services

    private static IServiceCollection AddDataStorageServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        return services
            .AddStorageServices(configuration, options)
            .AddFileProcessorServices(configuration, options)
            .AddMetadataServices(configuration, options)
            .AddCdnServices(configuration, options);
    }

    private static IServiceCollection AddStorageServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableStorage) return services;

        var storageProvider = GetStorageProvider(configuration, options);
        var storageConnectionString = GetStorageConnectionString(configuration, options);

        if (storageProvider == "Cloud")
        {
            var storageOptions = CreateStorageOptions(configuration, options, storageProvider, storageConnectionString);
            services.AddSingleton(storageOptions);
            services.AddScoped<IStorageService, CloudStorageService>();
            services.AddScoped<IMarventaStorage, MockStorageService>();
        }
        else
        {
            var basePath = GetStorageBasePath(configuration, options);
            services.AddScoped<IStorageService>(serviceProvider =>
                new LocalFileStorageService(serviceProvider.GetRequiredService<ILogger<LocalFileStorageService>>(), basePath));
            services.AddScoped<IMarventaStorage, MockStorageService>();
        }

        return services;
    }

    private static IServiceCollection AddFileProcessorServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableFileProcessor) return services;

        // File processor services would be implemented here
        // services.AddScoped<IMarventaFileProcessor, FileProcessorService>();
        return services;
    }

    private static IServiceCollection AddMetadataServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableMetadata) return services;

        services.AddScoped<IMarventaFileMetadata, MockMetadataService>();
        return services;
    }

    private static IServiceCollection AddCdnServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableCDN) return services;

        // Register HttpClient for CDN services
        services.AddHttpClient<AzureCDNService>();
        services.AddHttpClient<AwsCDNService>();
        services.AddHttpClient<CloudFlareCDNService>();

        // Register CDN service based on configuration
        var cdnProvider = configuration.GetValue<string>("Marventa:CDN:Provider") ?? "Mock";

        services.AddScoped<IMarventaCDN>(provider => cdnProvider.ToLowerInvariant() switch
        {
            "azure" => provider.GetRequiredService<AzureCDNService>(),
            "aws" => provider.GetRequiredService<AwsCDNService>(),
            "cloudflare" => provider.GetRequiredService<CloudFlareCDNService>(),
            _ => provider.GetRequiredService<MockCDNService>()
        });

        // Register all CDN services for factory pattern usage
        services.AddScoped<MockCDNService>();
        services.AddScoped<AzureCDNService>();
        services.AddScoped<AwsCDNService>();
        services.AddScoped<CloudFlareCDNService>();

        return services;
    }

    #endregion

    #region API Management Services

    private static IServiceCollection AddApiManagementServices(
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

        var rateLimitConfig = GetRateLimitConfiguration(configuration, options);
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

    #endregion

    #region Placeholder Service Groups

    private static IServiceCollection AddPerformanceServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        // Performance services like distributed locking, circuit breaker, batch operations
        return services;
    }

    private static IServiceCollection AddMonitoringServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        // Monitoring services like analytics, observability, tracking
        return services;
    }

    private static IServiceCollection AddBackgroundProcessingServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        // Background processing services like jobs, messaging, dead letter queue
        return services;
    }

    private static IServiceCollection AddEnterpriseArchitectureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        // Saga Pattern Implementation
        if (options.EnableSagas)
        {
            services.AddScoped<ISagaManager, SagaManager>();
            services.AddScoped(typeof(ISagaRepository<>), typeof(SimpleSagaRepository<>));

            // Register example saga orchestrator
            services.AddScoped<ISagaOrchestrator<OrderSaga>, OrderSagaOrchestrator>();
        }

        // Multi-tenancy services
        if (options.EnableMultiTenancy)
        {
            services.AddScoped<ITenantContext, TenantContext>();
            services.AddScoped<ITenantResolver<HttpContext>, TenantResolver>();
            services.AddScoped<ITenantStore, TenantStore>();
            services.AddScoped<ITenantAuthorization, TenantAuthorizationService>();
        }

        // CQRS pattern with MediatR and Pipeline Behaviors
        if (options.EnableCQRS)
        {
            services.AddCqrsServices(options);
        }

        // Event-driven architecture
        if (options.EnableEventDriven)
        {
            // Event bus and handlers would go here
        }

        return services;
    }

    private static IServiceCollection AddCqrsServices(
        this IServiceCollection services,
        MarventaFrameworkOptions options)
    {
        var cqrsOptions = options.CqrsOptions;

        if (cqrsOptions.Assemblies == null || !cqrsOptions.Assemblies.Any())
        {
            throw new InvalidOperationException(
                "CQRS is enabled but no assemblies are configured. " +
                "Please add assemblies to CqrsOptions.Assemblies in your configuration.");
        }

        // Add MediatR with handlers from configured assemblies
        services.AddMediatR(cfg =>
        {
            foreach (var assembly in cqrsOptions.Assemblies)
            {
                cfg.RegisterServicesFromAssembly(assembly);
            }

            // Add Marventa pipeline behaviors based on configuration
            if (cqrsOptions.EnableValidationBehavior)
            {
                cfg.AddOpenBehavior(typeof(Marventa.Framework.Application.Behaviors.ValidationBehavior<,>));
            }

            if (cqrsOptions.EnableLoggingBehavior)
            {
                cfg.AddOpenBehavior(typeof(Marventa.Framework.Application.Behaviors.LoggingBehavior<,>));
            }

            if (cqrsOptions.EnableTransactionBehavior)
            {
                cfg.AddOpenBehavior(typeof(Marventa.Framework.Application.Behaviors.TransactionBehavior<,>));
            }
        });

        // Add FluentValidation validators if ValidationBehavior is enabled
        if (cqrsOptions.EnableValidationBehavior)
        {
            foreach (var assembly in cqrsOptions.Assemblies)
            {
                services.AddValidatorsFromAssembly(assembly);
            }
        }

        // Add Unit of Work for transaction management
        services.AddScoped<IUnitOfWork, Marventa.Framework.Infrastructure.Data.UnitOfWork>();

        return services;
    }

    private static IServiceCollection AddSearchAndAiServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableML) return services;

        services.AddScoped<IMarventaML, MockMLService>();
        return services;
    }

    private static IServiceCollection AddBusinessServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        // Business services like e-commerce, payments, shipping
        return services;
    }

    #endregion

    #region Middleware Configuration

    private static IApplicationBuilder ConfigureMiddlewarePipeline(
        this IApplicationBuilder app,
        IConfiguration configuration,
        MarventaFrameworkOptions options,
        Microsoft.Extensions.Logging.ILogger logger)
    {
        if (options.MiddlewareOptions.UseUnifiedMiddleware)
        {
            app.UseMiddleware<MarventaUnifiedMiddleware>();
            logger.LogInformation("Marventa Unified Middleware activated (High Performance Mode)");
        }
        else
        {
            app.ConfigureModularMiddleware(options, logger);
        }

        return app.ConfigureStandardPipeline(configuration, options);
    }

    private static IApplicationBuilder ConfigureModularMiddleware(
        this IApplicationBuilder app,
        MarventaFrameworkOptions options,
        Microsoft.Extensions.Logging.ILogger logger)
    {
        logger.LogInformation("Marventa Modular Middleware activated (Customization Mode)");

        app.UseMiddleware<CorrelationIdMiddleware>();

        if (options.EnableExceptionHandling)
            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

        if (options.EnableLogging)
            app.UseMiddleware<RequestResponseLoggingMiddleware>();

        if (options.EnableRateLimiting)
            app.UseMiddleware<RateLimitingMiddleware>();

        app.Use(async (context, next) =>
        {
            AddSecurityHeaders(context);
            await next();
        });

        return app;
    }

    private static IApplicationBuilder ConfigureStandardPipeline(
        this IApplicationBuilder app,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        app.UseHttpsRedirection();

        if (options.EnableCompression)
            app.UseResponseCompression();

        if (options.EnableCaching)
            app.UseResponseCaching();

        app.UseStaticFiles();
        app.UseRouting();

        var corsOrigins = configuration.GetSection("Cors:Origins").Get<string[]>();
        if (corsOrigins?.Length > 0)
            app.UseCors("MarventaPolicy");

        if (!options.MiddlewareOptions.UseUnifiedMiddleware && options.EnableApiKeys)
            app.UseMiddleware<ApiKeyAuthenticationMiddleware>();

        if (options.EnableJWT || options.EnableApiKeys)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        if (options.EnableLogging)
        {
            var serilogLogger = app.ApplicationServices.GetService<Serilog.ILogger>();
            if (serilogLogger != null)
            {
                app.UseSerilogRequestLogging(opts =>
                {
                    opts.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                    {
                        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value ?? "unknown");
                        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                        var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                        if (!string.IsNullOrEmpty(userAgent))
                        {
                            diagnosticContext.Set("UserAgent", userAgent);
                        }
                    };
                });
            }
        }

        if (options.EnableHealthChecks)
            app.UseHealthChecks("/health");

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            if (options.EnableHealthChecks)
                endpoints.MapHealthChecks("/health");

            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Marventa Framework is running!", context.RequestAborted);
            });
        });

        return app;
    }

    private static void AddSecurityHeaders(HttpContext context)
    {
        var headers = context.Response.Headers;
        headers["X-Content-Type-Options"] = "nosniff";
        headers["X-Frame-Options"] = "DENY";
        headers["X-XSS-Protection"] = "1; mode=block";
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        headers["X-Powered-By"] = "Marventa.Framework";
    }

    #endregion

    #region Helper Methods

    private static string? GetRedisConnectionString(IConfiguration configuration, MarventaFrameworkOptions options)
    {
        return configuration.GetConnectionString("Redis") ??
               configuration["Marventa:ConnectionStrings:Redis"] ??
               options.CachingOptions.ConnectionString;
    }

    private static string GetStorageProvider(IConfiguration configuration, MarventaFrameworkOptions options)
    {
        return configuration["Marventa:Storage:Provider"] ?? options.StorageOptions.Provider;
    }

    private static string? GetStorageConnectionString(IConfiguration configuration, MarventaFrameworkOptions options)
    {
        return configuration.GetConnectionString("Storage") ??
               configuration["Marventa:ConnectionStrings:Storage"] ??
               options.StorageOptions.ConnectionString;
    }

    private static string GetStorageBasePath(IConfiguration configuration, MarventaFrameworkOptions options)
    {
        return configuration["Marventa:Storage:BasePath"] ??
               options.StorageOptions.BasePath ??
               "uploads";
    }

    private static StorageOptions CreateStorageOptions(
        IConfiguration configuration,
        MarventaFrameworkOptions options,
        string provider,
        string? connectionString)
    {
        return new StorageOptions
        {
            Provider = provider,
            ConnectionString = connectionString ?? options.StorageOptions.ConnectionString,
            BasePath = GetStorageBasePath(configuration, options)
        };
    }

    private static RateLimitOptions GetRateLimitConfiguration(IConfiguration configuration, MarventaFrameworkOptions options)
    {
        var maxRequests = configuration.GetValue<int?>("Marventa:RateLimit:MaxRequests") ?? options.MiddlewareOptions.RateLimiting.MaxRequests;
        var windowMinutes = configuration.GetValue<int?>("Marventa:RateLimit:WindowMinutes") ?? options.MiddlewareOptions.RateLimiting.WindowMinutes;

        return new RateLimitOptions
        {
            MaxRequests = maxRequests,
            Window = TimeSpan.FromMinutes(windowMinutes)
        };
    }

    #endregion
}