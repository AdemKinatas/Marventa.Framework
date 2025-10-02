using Marventa.Framework.Configuration;

using Marventa.Framework.Features.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace Marventa.Framework.Extensions;

/// <summary>
/// Provides extension methods for configuring observability services.
/// Includes logging (Serilog), health checks, and distributed tracing (OpenTelemetry).
/// </summary>
public static class ObservabilityExtensions
{
    /// <summary>
    /// Configures Serilog logging from configuration.
    /// Sets up the global Serilog logger with settings from the Serilog configuration section.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="applicationName">Optional application name for logging context. Defaults to configuration or "Marventa".</param>
    public static void AddMarventaSerilog(
        this IConfiguration configuration,
        string? applicationName = null)
    {
        if (!configuration.HasSection(ConfigurationKeys.Serilog))
        {
            return;
        }

        var appName = applicationName
            ?? configuration[ConfigurationKeys.ApplicationName]
            ?? ConfigurationKeys.Defaults.ApplicationName;

        Log.Logger = SerilogConfiguration.ConfigureSerilog(configuration, appName);
    }

    /// <summary>
    /// Adds OpenTelemetry distributed tracing with ASP.NET Core and HTTP client instrumentation.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddMarventaOpenTelemetry(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (!configuration.HasSection(ConfigurationKeys.OpenTelemetry))
        {
            return services;
        }

        var serviceName = configuration.GetValueOrDefault(
            ConfigurationKeys.OpenTelemetryServiceName,
            configuration[ConfigurationKeys.ApplicationName]
                ?? ConfigurationKeys.Defaults.ApplicationName);

        var otlpEndpoint = configuration[ConfigurationKeys.OpenTelemetryOtlpEndpoint];

        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder
                    .AddSource(serviceName)
                    .ConfigureResource(resource => resource.AddService(serviceName))
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                    })
                    .AddHttpClientInstrumentation(options =>
                    {
                        options.RecordException = true;
                    });

                // Add OTLP exporter if endpoint is configured
                if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                {
                    builder.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otlpEndpoint);
                    });
                }
            });

        return services;
    }

    /// <summary>
    /// Adds health check services with optional SQL Server and Redis health checks.
    /// Automatically configures health checks based on available configuration sections.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddMarventaHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (!configuration.IsSectionEnabled(ConfigurationKeys.HealthChecks))
        {
            return services;
        }

        var healthChecksBuilder = services.AddHealthChecks();

        // Add SQL Server health check if connection string is configured
        var sqlConnectionString = configuration.GetConnectionString(ConfigurationKeys.DefaultConnection);
        if (!string.IsNullOrWhiteSpace(sqlConnectionString))
        {
            healthChecksBuilder.AddSqlServer(
                connectionString: sqlConnectionString,
                name: "database",
                tags: new[] { "db", "sql", "sqlserver" });
        }

        // Add Redis health check if Redis is configured
        var redisConnectionString = configuration[ConfigurationKeys.RedisConnectionString];
        if (!string.IsNullOrWhiteSpace(redisConnectionString))
        {
            healthChecksBuilder.AddRedis(
                redisConnectionString: redisConnectionString,
                name: "redis",
                tags: new[] { "cache", "redis" });
        }

        // Add RabbitMQ health check if RabbitMQ is configured
        var rabbitMqHost = configuration[ConfigurationKeys.RabbitMQHost];
        if (!string.IsNullOrWhiteSpace(rabbitMqHost))
        {
            healthChecksBuilder.AddCheck<Infrastructure.HealthChecks.RabbitMqHealthCheck>(
                name: "rabbitmq",
                tags: new[] { "messaging", "rabbitmq" });
        }

        return services;
    }

    /// <summary>
    /// Adds all observability services including logging, health checks, and tracing.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="applicationName">Optional application name for logging context.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddMarventaObservability(
        this IServiceCollection services,
        IConfiguration configuration,
        string? applicationName = null)
    {
        configuration.AddMarventaSerilog(applicationName);
        services.AddMarventaOpenTelemetry(configuration);
        services.AddMarventaHealthChecks(configuration);

        return services;
    }
}
