using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Marventa.Framework.Core.Interfaces.Services;
using Marventa.Framework.Core.Models;
using Marventa.Framework.Infrastructure.Services;

namespace Marventa.Framework.Web.Extensions;

/// <summary>
/// Communication service extensions (Email, SMS, HTTP)
/// </summary>
internal static class ServiceCollectionExtensionsCommunication
{
    internal static IServiceCollection AddCommunicationServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        return services
            .AddEmailServices(configuration, options)
            .AddSmsServices(configuration, options)
            .AddHttpClientServices(configuration, options);
    }

    private static IServiceCollection AddEmailServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableEmail) return services;

        services.AddScoped<IEmailService, EmailService>();
        return services;
    }

    private static IServiceCollection AddSmsServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableSMS) return services;

        services.AddScoped<ISmsService, SmsService>();
        return services;
    }

    private static IServiceCollection AddHttpClientServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableHttpClient) return services;

        services.AddHttpClient();
        return services;
    }
}
