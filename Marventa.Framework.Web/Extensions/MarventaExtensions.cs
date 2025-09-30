using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Marventa.Framework.Core.Models;
using Marventa.Framework.Web.Middleware;

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

    private static MarventaFrameworkOptions ConfigureFrameworkOptions(Action<MarventaFrameworkOptions> configureOptions)
    {
        var options = new MarventaFrameworkOptions();
        configureOptions(options);
        return options;
    }
}
