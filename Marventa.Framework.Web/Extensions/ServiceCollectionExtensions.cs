using Marventa.Framework.Core.Interfaces;
using Marventa.Framework.Core.Security;
using Marventa.Framework.Infrastructure.Caching;
using Marventa.Framework.Infrastructure.Configuration;
using Marventa.Framework.Infrastructure.Data;
using Marventa.Framework.Infrastructure.Handlers;
using Marventa.Framework.Infrastructure.HealthChecks;
using Marventa.Framework.Infrastructure.Http;
using Marventa.Framework.Infrastructure.Security;
using Marventa.Framework.Infrastructure.Services;
using Marventa.Framework.Web.RateLimiting;
using Marventa.Framework.Web.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace Marventa.Framework.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMarventaFramework(this IServiceCollection services)
    {
        // Core services
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
        services.AddScoped<ICacheService, MemoryCacheService>();
        services.AddScoped<ILoggerService, LoggerService>();
        services.AddScoped<IConnectionFactory, ConnectionFactory>();
        services.AddScoped<IConfigurationService, ConfigurationService>();
        services.AddScoped<IFeatureFlagService, FeatureFlagService>();

        // Security services
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IEncryptionService, EncryptionService>();

        // Communication services
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ISmsService, SmsService>();
        services.AddScoped<IHttpClientService, HttpClientService>();

        // CQRS
        services.AddScoped<Application.Handlers.IMediator, Mediator>();

        // Health checks
        services.AddScoped<IHealthCheck, DatabaseHealthCheck>();
        services.AddScoped<IHealthCheck, CacheHealthCheck>();

        // Infrastructure
        services.AddMemoryCache();
        services.AddHttpContextAccessor();

        return services;
    }

    public static IServiceCollection AddMarventaApiVersioning(this IServiceCollection services, ApiVersioningOptions? options = null)
    {
        var versioningOptions = options ?? new ApiVersioningOptions();
        services.AddSingleton(versioningOptions);
        return services;
    }

    public static IServiceCollection AddMarventaRateLimiting(this IServiceCollection services, RateLimitOptions? options = null)
    {
        var rateLimitOptions = options ?? new RateLimitOptions();
        services.AddSingleton(rateLimitOptions);
        return services;
    }
}