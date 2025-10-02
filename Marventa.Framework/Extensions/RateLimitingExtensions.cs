using Marventa.Framework.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marventa.Framework.Extensions;

/// <summary>
/// Provides extension methods for configuring rate limiting services.
/// Supports IP-based, user-based, API key-based, and custom header-based rate limiting strategies.
/// </summary>
public static class RateLimitingExtensions
{
    /// <summary>
    /// Adds rate limiting services with configurable strategies and thresholds.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddMarventaRateLimiting(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (!configuration.HasSection(ConfigurationKeys.RateLimiting))
        {
            return services;
        }

        var options = configuration.GetConfigurationSection<RateLimitingOptions>(
            RateLimitingOptions.SectionName);

        services.Configure<RateLimitingOptions>(
            configuration.GetSection(RateLimitingOptions.SectionName));

        // Memory cache is required for rate limiting
        services.AddMemoryCache();

        return services;
    }
}
