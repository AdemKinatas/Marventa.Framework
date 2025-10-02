using Marventa.Framework.Configuration;

using Marventa.Framework.Infrastructure.MultiTenancy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marventa.Framework.Extensions;

/// <summary>
/// Provides extension methods for configuring multi-tenancy services.
/// Supports tenant resolution via headers, query parameters, subdomains, claims, or custom strategies.
/// </summary>
public static class MultiTenancyExtensions
{
    /// <summary>
    /// Adds multi-tenancy services with tenant resolution and context management.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddMarventaMultiTenancy(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (!configuration.HasSection(ConfigurationKeys.MultiTenancy))
        {
            return services;
        }

        var options = configuration.GetConfigurationSection<MultiTenancyOptions>(
            MultiTenancyOptions.SectionName);

        services.Configure<MultiTenancyOptions>(
            configuration.GetSection(MultiTenancyOptions.SectionName));

        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<TenantResolver>();

        return services;
    }
}
