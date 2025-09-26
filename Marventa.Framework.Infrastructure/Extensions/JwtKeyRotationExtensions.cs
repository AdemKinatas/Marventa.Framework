using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Marventa.Framework.Core.Interfaces;
using Marventa.Framework.Infrastructure.Security;
using Marventa.Framework.Infrastructure.Services;

namespace Marventa.Framework.Infrastructure.Extensions;

public static class JwtKeyRotationExtensions
{
    /// <summary>
    /// Adds JWT Key Rotation services with in-memory key store
    /// </summary>
    public static IServiceCollection AddJwtKeyRotation(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtKeyRotationOptions>(
            configuration.GetSection(JwtKeyRotationOptions.SectionName));

        services.AddSingleton<IJwtKeyStore, InMemoryJwtKeyStore>();
        services.AddScoped<IJwtKeyRotationService, JwtKeyRotationService>();
        services.AddHostedService<JwtKeyRotationHostedService>();

        return services;
    }

    /// <summary>
    /// Adds JWT Key Rotation services with custom configuration
    /// </summary>
    public static IServiceCollection AddJwtKeyRotation(
        this IServiceCollection services,
        Action<JwtKeyRotationOptions> configure)
    {
        services.Configure(configure);

        services.AddSingleton<IJwtKeyStore, InMemoryJwtKeyStore>();
        services.AddScoped<IJwtKeyRotationService, JwtKeyRotationService>();
        services.AddHostedService<JwtKeyRotationHostedService>();

        return services;
    }

    /// <summary>
    /// Adds JWT Key Rotation services with custom key store
    /// </summary>
    public static IServiceCollection AddJwtKeyRotation<TKeyStore>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TKeyStore : class, IJwtKeyStore
    {
        services.Configure<JwtKeyRotationOptions>(
            configuration.GetSection(JwtKeyRotationOptions.SectionName));

        services.AddScoped<IJwtKeyStore, TKeyStore>();
        services.AddScoped<IJwtKeyRotationService, JwtKeyRotationService>();
        services.AddHostedService<JwtKeyRotationHostedService>();

        return services;
    }
}