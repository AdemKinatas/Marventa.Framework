using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Marventa.Framework.Core.Interfaces;
using Marventa.Framework.Infrastructure.Entities;
using Marventa.Framework.Infrastructure.Idempotency;
using Marventa.Framework.Infrastructure.Messaging;
using Marventa.Framework.Infrastructure.Multitenancy;
using Marventa.Framework.Infrastructure.Observability;
using Marventa.Framework.Infrastructure.Sagas;
using Marventa.Framework.Infrastructure.Projections;
using MongoDB.Driver;
using Marventa.Framework.Web.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using FluentValidation;
using MediatR;
using System.Reflection;
using Marventa.Framework.Application.Behaviors;

namespace Marventa.Framework.Web.Extensions;

public static class MarventaExtensions
{
    /// <summary>
    /// Adds Multi-tenancy support with configurable tenant resolution strategies
    /// </summary>
    public static IServiceCollection AddMarventaMultitenancy(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<TenantOptions>(configuration.GetSection(TenantOptions.SectionName));

        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<ITenantResolver<HttpContext>, TenantResolver>();
        services.AddScoped<ITenantStore, TenantStore>();

        return services;
    }

    /// <summary>
    /// Adds Multi-tenancy support with custom configuration
    /// </summary>
    public static IServiceCollection AddMarventaMultitenancy(
        this IServiceCollection services,
        Action<TenantOptions> configure)
    {
        services.Configure(configure);

        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<ITenantResolver<HttpContext>, TenantResolver>();
        services.AddScoped<ITenantStore, TenantStore>();

        return services;
    }

    /// <summary>
    /// Adds Outbox/Inbox pattern for transactional messaging
    /// </summary>
    public static IServiceCollection AddMarventaTransactionalMessaging(
        this IServiceCollection services)
    {
        services.AddScoped<IOutboxService, OutboxService>();
        services.AddScoped<IInboxService, InboxService>();
        services.AddScoped<ITransactionalMessageService, TransactionalMessageService>();

        return services;
    }

    /// <summary>
    /// Adds HTTP idempotency support
    /// </summary>
    public static IServiceCollection AddMarventaIdempotency(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<IdempotencyOptions>(configuration.GetSection(IdempotencyOptions.SectionName));
        services.AddScoped<IIdempotencyService, IdempotencyService>();

        return services;
    }

    /// <summary>
    /// Adds HTTP idempotency support with custom configuration
    /// </summary>
    public static IServiceCollection AddMarventaIdempotency(
        this IServiceCollection services,
        Action<IdempotencyOptions> configure)
    {
        services.Configure(configure);
        services.AddScoped<IIdempotencyService, IdempotencyService>();

        return services;
    }

    /// <summary>
    /// Adds observability with OpenTelemetry, correlation context, and structured logging
    /// </summary>
    public static IServiceCollection AddMarventaObservability(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName,
        string? serviceVersion = null)
    {
        services.Configure<CorrelationOptions>(configuration.GetSection(CorrelationOptions.SectionName));

        services.AddSingleton<ICorrelationContext, CorrelationContext>();
        services.AddSingleton<IActivityService, ActivityService>();

        // Add OpenTelemetry
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(serviceName, serviceVersion: serviceVersion)
                .AddAttributes(new Dictionary<string, object>
                {
                    ["service.name"] = serviceName,
                    ["service.version"] = serviceVersion ?? "unknown",
                    ["deployment.environment"] = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "production"
                }))
            .WithTracing(tracing => tracing
                .AddSource("Marventa.Framework")
                .AddSource(serviceName)
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.RecordException = true;
                    options.EnrichWithHttpRequest = EnrichWithHttpRequest;
                    options.EnrichWithHttpResponse = EnrichWithHttpResponse;
                })
                .AddHttpClientInstrumentation(options =>
                {
                    options.RecordException = true;
                })
                .AddSqlClientInstrumentation(options =>
                {
                    options.SetDbStatementForText = true;
                    options.RecordException = true;
                })
                .AddOtlpExporter())
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter());

        return services;
    }

    /// <summary>
    /// Configures Entity Framework for multi-tenant scenarios
    /// </summary>
    public static IServiceCollection AddMarventaEntityFramework<TContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringName = "DefaultConnection")
        where TContext : DbContext
    {
        services.AddDbContext<TContext>((serviceProvider, options) =>
        {
            var tenantContext = serviceProvider.GetService<ITenantContext>();
            var connectionString = tenantContext?.CurrentTenant?.ConnectionString
                                 ?? configuration.GetConnectionString(connectionStringName);

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException($"Connection string '{connectionStringName}' not found");

            options.UseSqlServer(connectionString);

            // Add tenant-aware query filters
            options.EnableServiceProviderCaching(false);
        });

        // Configure tenant-aware entities
        services.AddScoped<DbContext>(provider => provider.GetRequiredService<TContext>());

        return services;
    }

    /// <summary>
    /// Uses Marventa middleware pipeline in the correct order
    /// </summary>
    public static IApplicationBuilder UseMarventaMiddleware(this IApplicationBuilder app)
    {
        // Order is important!
        app.UseMarventaValidation();
        app.UseMiddleware<CorrelationMiddleware>();
        app.UseMiddleware<TenantMiddleware>();
        app.UseMiddleware<IdempotencyMiddleware>();

        return app;
    }

    /// <summary>
    /// Adds Saga/Process Manager support
    /// </summary>
    public static IServiceCollection AddMarventaSagas(
        this IServiceCollection services)
    {
        services.AddScoped(typeof(ISagaRepository<>), typeof(SimpleSagaRepository<>));
        services.AddScoped<ISagaManager, SagaManager>();

        return services;
    }

    /// <summary>
    /// Adds Read Model Projections with MongoDB support
    /// </summary>
    public static IServiceCollection AddMarventaProjections(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<MongoProjectionOptions>(configuration.GetSection(MongoProjectionOptions.SectionName));

        // Register MongoDB client and database
        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<MongoProjectionOptions>>().Value;
            return new MongoClient(options.ConnectionString);
        });

        services.AddScoped<IMongoDatabase>(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            var options = serviceProvider.GetRequiredService<IOptions<MongoProjectionOptions>>().Value;
            return client.GetDatabase(options.DatabaseName);
        });

        // Register projection services
        services.AddScoped(typeof(IProjectionRepository<>), typeof(MongoProjectionRepository<>));
        services.AddScoped<IProjectionManager, ProjectionManager>();
        services.AddScoped<IEventStore, SqlServerEventStore>();

        return services;
    }

    /// <summary>
    /// Adds Read Model Projections with custom MongoDB configuration
    /// </summary>
    public static IServiceCollection AddMarventaProjections(
        this IServiceCollection services,
        Action<MongoProjectionOptions> configure)
    {
        services.Configure(configure);

        // Register MongoDB client and database
        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<MongoProjectionOptions>>().Value;
            return new MongoClient(options.ConnectionString);
        });

        services.AddScoped<IMongoDatabase>(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            var options = serviceProvider.GetRequiredService<IOptions<MongoProjectionOptions>>().Value;
            return client.GetDatabase(options.DatabaseName);
        });

        // Register projection services
        services.AddScoped(typeof(IProjectionRepository<>), typeof(MongoProjectionRepository<>));
        services.AddScoped<IProjectionManager, ProjectionManager>();
        services.AddScoped<IEventStore, SqlServerEventStore>();

        return services;
    }

    /// <summary>
    /// Adds FluentValidation pipeline with MediatR behaviors
    /// </summary>
    public static IServiceCollection AddMarventaValidation(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        // Register FluentValidation
        services.AddValidatorsFromAssemblies(assemblies);

        // Register MediatR with validation behavior
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        return services;
    }

    /// <summary>
    /// Uses Marventa validation middleware
    /// </summary>
    public static IApplicationBuilder UseMarventaValidation(this IApplicationBuilder app)
    {
        app.UseMiddleware<ValidationExceptionMiddleware>();
        return app;
    }

    /// <summary>
    /// Adds complete Marventa v1.3 infrastructure
    /// </summary>
    public static IServiceCollection AddMarventaV13(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName,
        string? serviceVersion = null)
    {
        // v1.2 features
        services.AddMarventaMultitenancy(configuration);
        services.AddMarventaTransactionalMessaging();
        services.AddMarventaIdempotency(configuration);
        services.AddMarventaObservability(configuration, serviceName, serviceVersion);

        // v1.3 features
        services.AddMarventaSagas();
        services.AddMarventaProjections(configuration);

        return services;
    }

    /// <summary>
    /// Adds complete Marventa v1.2 infrastructure
    /// </summary>
    public static IServiceCollection AddMarventaV12(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName,
        string? serviceVersion = null)
    {
        // Core features
        services.AddMarventaMultitenancy(configuration);
        services.AddMarventaTransactionalMessaging();
        services.AddMarventaIdempotency(configuration);
        services.AddMarventaObservability(configuration, serviceName, serviceVersion);

        return services;
    }

    private static void EnrichWithHttpRequest(System.Diagnostics.Activity activity, HttpRequest httpRequest)
    {
        activity.SetTag("http.method", httpRequest.Method);
        activity.SetTag("http.scheme", httpRequest.Scheme);
        activity.SetTag("http.host", httpRequest.Host.ToString());
        activity.SetTag("http.path", httpRequest.Path);
        activity.SetTag("http.query_string", httpRequest.QueryString.ToString());
        activity.SetTag("http.user_agent", httpRequest.Headers.UserAgent.ToString());

        if (httpRequest.Headers.TryGetValue("X-Tenant-Id", out var tenantId))
        {
            activity.SetTag("tenant.id", tenantId.ToString());
        }

        if (httpRequest.Headers.TryGetValue("X-Correlation-ID", out var correlationId))
        {
            activity.SetTag("correlation.id", correlationId.ToString());
        }
    }

    private static void EnrichWithHttpResponse(System.Diagnostics.Activity activity, HttpResponse httpResponse)
    {
        activity.SetTag("http.status_code", httpResponse.StatusCode);

        if (httpResponse.Headers.TryGetValue("X-Correlation-ID", out var correlationId))
        {
            activity.SetTag("correlation.id", correlationId.ToString());
        }
    }
}