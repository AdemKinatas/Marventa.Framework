using Marventa.Framework.Configuration;
using Marventa.Framework.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marventa.Framework.Extensions;

/// <summary>
/// Extension methods for configuring request/response logging.
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Adds request/response logging services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRequestResponseLogging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<LoggingOptions>(configuration.GetSection("Logging:RequestResponse"));
        return services;
    }

    /// <summary>
    /// Adds request/response logging services with custom options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Action to configure logging options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRequestResponseLogging(
        this IServiceCollection services,
        Action<LoggingOptions> configureOptions)
    {
        services.Configure(configureOptions);
        return services;
    }

    /// <summary>
    /// Adds the request/response logging middleware to the application pipeline.
    /// This middleware should be added early in the pipeline to capture all requests/responses.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}
