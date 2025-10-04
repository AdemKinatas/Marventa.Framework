using FluentValidation;
using Mapster;
using Marventa.Framework.Configuration;

using Marventa.Framework.Infrastructure.Environment;
using Marventa.Framework.Infrastructure.MultiTenancy;
using Marventa.Framework.Infrastructure.Seeding;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Marventa.Framework.Extensions;

/// <summary>
/// Provides the main entry point extension methods for configuring Marventa Framework services.
/// This is the primary API for bootstrapping all framework features.
/// </summary>
public static class MarventaExtensions
{
    /// <summary>
    /// Adds all Marventa Framework services to the service collection.
    /// This is the main extension method that configures all framework features based on configuration.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="assemblies">Assemblies to scan for MediatR handlers, validators, and Mapster configurations.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddMarventa(
        this IServiceCollection services,
        IConfiguration configuration,
        params System.Reflection.Assembly[] assemblies)
    {
        // Configure JSON serialization and HTTP context
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            });

        services.AddHttpContextAccessor();

        // Configure exception handling
        services.Configure<ExceptionHandlingOptions>(
            configuration.GetSection(ExceptionHandlingOptions.SectionName));

        // Determine assemblies to scan
        var assembliesToScan = assemblies.Length > 0
            ? assemblies
            : new[] { System.Reflection.Assembly.GetCallingAssembly() };

        // Add MediatR, FluentValidation, and Mapster
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assembliesToScan));
        services.AddValidatorsFromAssemblies(assembliesToScan);
        services.AddMapster();

        foreach (var assembly in assembliesToScan)
        {
            TypeAdapterConfig.GlobalSettings.Scan(assembly);
        }

        // Add modular services using dedicated extension methods
        services.AddMarventaApiServices(configuration);
        services.AddMarventaAuthenticationAndAuthorization(configuration);
        services.AddMarventaCaching(configuration);
        services.AddMarventaMultiTenancy(configuration);
        services.AddMarventaRateLimiting(configuration);
        services.AddMarventaEventBus(configuration, assembliesToScan);
        services.AddMarventaDataServices(configuration);
        services.AddMarventaStorage(configuration);
        services.AddMarventaObservability(configuration);

        // Add data seeding infrastructure
        services.AddScoped<DataSeederRunner>();

        return services;
    }

    /// <summary>
    /// Configures the Marventa Framework middleware pipeline.
    /// Sets up exception handling, authentication, multi-tenancy, rate limiting, and Swagger UI.
    /// </summary>
    /// <param name="app">The application builder to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="env">Optional hosting environment for environment-specific configuration.</param>
    /// <returns>The application builder for method chaining.</returns>
    public static IApplicationBuilder UseMarventa(
        this IApplicationBuilder app,
        IConfiguration configuration,
        IWebHostEnvironment? env = null)
    {
        // Exception handling (must be first)
        app.UseMiddleware<Middleware.ExceptionMiddleware>();

        // Standard ASP.NET Core middleware
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        // CORS
        var corsOrigins = configuration.GetSection(ConfigurationKeys.CorsAllowedOrigins).Get<string[]>();
        if (corsOrigins?.Length > 0)
        {
            app.UseCors();
        }

        // Performance tracking
        app.UseMiddleware<Middleware.PerformanceMiddleware>();

        // Authentication
        if (configuration.HasSection(ConfigurationKeys.Jwt))
        {
            app.UseAuthentication();
        }

        // Multi-tenancy (after authentication)
        if (configuration.HasSection(ConfigurationKeys.MultiTenancy))
        {
            app.UseMiddleware<TenantMiddleware>();
        }

        // Authorization
        if (configuration.HasSection(ConfigurationKeys.Jwt))
        {
            app.UseAuthorization();
        }

        // Rate limiting
        if (configuration.HasSection(ConfigurationKeys.RateLimiting))
        {
            app.UseMiddleware<Security.RateLimiting.RateLimiterMiddleware>();
        }

        // Endpoints
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            // Health checks
            if (configuration.IsSectionEnabled(ConfigurationKeys.HealthChecks))
            {
                endpoints.MapHealthChecks("/health");
            }
        });

        // Swagger UI (development environment)
        ConfigureSwaggerUI(app, configuration, env);

        return app;
    }

    /// <summary>
    /// Configures Swagger UI with environment restrictions and API versioning support.
    /// </summary>
    private static void ConfigureSwaggerUI(
        IApplicationBuilder app,
        IConfiguration configuration,
        IWebHostEnvironment? env)
    {
        if (env == null || !configuration.HasSection(ConfigurationKeys.Swagger))
        {
            return;
        }

        var swaggerOptions = configuration.GetConfigurationSection<SwaggerOptions>(SwaggerOptions.SectionName);

        if (!swaggerOptions.Enabled)
        {
            return;
        }

        // Check environment restrictions
        var shouldEnable = EnvironmentHelper.ShouldEnableFeature(
            swaggerOptions.EnvironmentRestriction,
            env.EnvironmentName);

        if (!shouldEnable)
        {
            return;
        }

        app.UseSwagger();
        app.UseSwaggerUI(uiOptions =>
        {
            // Configure Swagger endpoints based on API versioning
            if (configuration.HasSection(ConfigurationKeys.ApiVersioning))
            {
                var apiVersionProvider = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();

                if (apiVersionProvider != null)
                {
                    foreach (var description in apiVersionProvider.ApiVersionDescriptions)
                    {
                        uiOptions.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            $"{swaggerOptions.Title} {description.GroupName.ToUpperInvariant()}");
                    }
                }
                else
                {
                    uiOptions.SwaggerEndpoint(
                        $"/swagger/{swaggerOptions.Version}/swagger.json",
                        $"{swaggerOptions.Title} {swaggerOptions.Version}");
                }
            }
            else
            {
                uiOptions.SwaggerEndpoint(
                    $"/swagger/{swaggerOptions.Version}/swagger.json",
                    $"{swaggerOptions.Title} {swaggerOptions.Version}");
            }
        });
    }

}
