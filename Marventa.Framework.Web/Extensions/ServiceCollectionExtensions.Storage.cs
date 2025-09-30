using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Marventa.Framework.Core.Interfaces.Storage;
using Marventa.Framework.Core.Models;
using Marventa.Framework.Infrastructure.Storage;
using Marventa.Framework.Infrastructure.Services.FileServices;

namespace Marventa.Framework.Web.Extensions;

/// <summary>
/// Data storage and file service extensions
/// </summary>
internal static class ServiceCollectionExtensionsStorage
{
    internal static IServiceCollection AddDataStorageServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        return services
            .AddStorageServices(configuration, options)
            .AddFileProcessorServices(configuration, options)
            .AddMetadataServices(configuration, options)
            .AddCdnServices(configuration, options);
    }

    private static IServiceCollection AddStorageServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableStorage) return services;

        var storageProvider = ConfigurationHelper.GetStorageProvider(configuration, options);
        var storageConnectionString = ConfigurationHelper.GetStorageConnectionString(configuration, options);

        if (storageProvider == "Cloud")
        {
            var storageOptions = ConfigurationHelper.CreateStorageOptions(configuration, options, storageProvider, storageConnectionString);
            services.AddSingleton(storageOptions);
            services.AddScoped<IStorageService, CloudStorageService>();
            services.AddScoped<IMarventaStorage, MockStorageService>();
        }
        else
        {
            var basePath = ConfigurationHelper.GetStorageBasePath(configuration, options);
            services.AddScoped<IStorageService>(serviceProvider =>
                new LocalFileStorageService(serviceProvider.GetRequiredService<ILogger<LocalFileStorageService>>(), basePath));
            services.AddScoped<IMarventaStorage, MockStorageService>();
        }

        return services;
    }

    private static IServiceCollection AddFileProcessorServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableFileProcessor) return services;

        // File processor services would be implemented here
        // services.AddScoped<IMarventaFileProcessor, FileProcessorService>();
        return services;
    }

    private static IServiceCollection AddMetadataServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableMetadata) return services;

        services.AddScoped<IMarventaFileMetadata, MockMetadataService>();
        return services;
    }

    private static IServiceCollection AddCdnServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        if (!options.EnableCDN) return services;

        // Register HttpClient for CDN services
        services.AddHttpClient<AzureCDNService>();
        services.AddHttpClient<AwsCDNService>();
        services.AddHttpClient<CloudFlareCDNService>();

        // Register CDN service based on configuration
        var cdnProvider = configuration.GetValue<string>("Marventa:CDN:Provider") ?? "Mock";

        services.AddScoped<IMarventaCDN>(provider => cdnProvider.ToLowerInvariant() switch
        {
            "azure" => provider.GetRequiredService<AzureCDNService>(),
            "aws" => provider.GetRequiredService<AwsCDNService>(),
            "cloudflare" => provider.GetRequiredService<CloudFlareCDNService>(),
            _ => provider.GetRequiredService<MockCDNService>()
        });

        // Register all CDN services for factory pattern usage
        services.AddScoped<MockCDNService>();
        services.AddScoped<AzureCDNService>();
        services.AddScoped<AwsCDNService>();
        services.AddScoped<CloudFlareCDNService>();

        return services;
    }
}
