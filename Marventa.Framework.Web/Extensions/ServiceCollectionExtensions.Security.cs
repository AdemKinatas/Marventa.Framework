using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Marventa.Framework.Core.Interfaces.Security;
using Marventa.Framework.Core.Interfaces.Services;
using Marventa.Framework.Core.Models;
using Marventa.Framework.Infrastructure.Services.Security;

namespace Marventa.Framework.Web.Extensions;

/// <summary>
/// Security service extensions
/// </summary>
internal static class ServiceCollectionExtensionsSecurity
{
    internal static IServiceCollection AddSecurityServices(
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
}
