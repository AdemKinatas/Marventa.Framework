using Microsoft.Extensions.DependencyInjection;

namespace Marventa.Framework.Web.Extensions;

/// <summary>
/// Core framework service registration extensions
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds minimal Marventa Framework core services (alias for AddMarventaCore)
    /// </summary>
    public static IServiceCollection AddMarventaFramework(this IServiceCollection services)
    {
        // Basic HTTP context accessor - required for most features
        services.AddHttpContextAccessor();
        services.AddMemoryCache();

        return services;
    }
}