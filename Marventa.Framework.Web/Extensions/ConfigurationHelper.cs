using Microsoft.Extensions.Configuration;
using Marventa.Framework.Core.Models;
using Marventa.Framework.Core.Configuration;
using Marventa.Framework.Web.Models;

namespace Marventa.Framework.Web.Extensions;

/// <summary>
/// Helper class for extracting configuration values
/// </summary>
internal static class ConfigurationHelper
{
    internal static string? GetRedisConnectionString(IConfiguration configuration, MarventaFrameworkOptions options)
    {
        return configuration.GetConnectionString("Redis") ??
               configuration["Marventa:ConnectionStrings:Redis"] ??
               options.CachingOptions.ConnectionString;
    }

    internal static string GetStorageProvider(IConfiguration configuration, MarventaFrameworkOptions options)
    {
        return configuration["Marventa:Storage:Provider"] ?? options.StorageOptions.Provider;
    }

    internal static string? GetStorageConnectionString(IConfiguration configuration, MarventaFrameworkOptions options)
    {
        return configuration.GetConnectionString("Storage") ??
               configuration["Marventa:ConnectionStrings:Storage"] ??
               options.StorageOptions.ConnectionString;
    }

    internal static string GetStorageBasePath(IConfiguration configuration, MarventaFrameworkOptions options)
    {
        return configuration["Marventa:Storage:BasePath"] ??
               options.StorageOptions.BasePath ??
               "uploads";
    }

    internal static StorageOptions CreateStorageOptions(
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

    internal static RateLimitOptions GetRateLimitConfiguration(IConfiguration configuration, MarventaFrameworkOptions options)
    {
        var maxRequests = configuration.GetValue<int?>("Marventa:RateLimit:MaxRequests") ?? options.MiddlewareOptions.RateLimiting.MaxRequests;
        var windowMinutes = configuration.GetValue<int?>("Marventa:RateLimit:WindowMinutes") ?? options.MiddlewareOptions.RateLimiting.WindowMinutes;

        return new RateLimitOptions
        {
            MaxRequests = maxRequests,
            Window = TimeSpan.FromMinutes(windowMinutes)
        };
    }
}
