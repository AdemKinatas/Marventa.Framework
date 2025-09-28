using Marventa.Framework.Core.Interfaces;
using Marventa.Framework.Core.Models.Configuration;
using Marventa.Framework.Infrastructure.Services.FileServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Marventa.Framework.Web.Extensions;

/// <summary>
/// Extension methods for configuring file services
/// </summary>
public static class FileServiceExtensions
{
    /// <summary>
    /// Adds all file services to the dependency injection container
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configureOptions">Configuration options</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddMarventaFileServices(this IServiceCollection services, Action<FileServiceOptions>? configureOptions = null)
    {
        var options = new FileServiceOptions();
        configureOptions?.Invoke(options);

        services.AddSingleton(Microsoft.Extensions.Options.Options.Create(options));

        // Register file processing service
        if (options.EnableFileProcessor)
        {
            services.AddFileProcessor(options.FileProcessorOptions);
        }

        // Register storage service
        if (options.EnableStorage)
        {
            services.AddStorageService(options.StorageOptions);
        }

        // Register CDN service
        if (options.EnableCDN)
        {
            services.AddCDNService(options.CDNOptions);
        }

        // Register ML service
        if (options.EnableML)
        {
            services.AddMLService(options.MLOptions);
        }

        // Register metadata service
        if (options.EnableMetadata)
        {
            services.AddMetadataService(options.MetadataOptions);
        }

        return services;
    }

    /// <summary>
    /// Adds file processor service to DI container
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="options">File processor options</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddFileProcessor(this IServiceCollection services, FileProcessorOptions? options = null)
    {
        var processorOptions = options ?? new FileProcessorOptions();
        services.AddSingleton(Microsoft.Extensions.Options.Options.Create(processorOptions));

        switch (processorOptions.Provider)
        {
            case FileProcessorProvider.ImageSharp:
                // ImageSharp implementation would go here when available
                services.AddScoped<IMarventaFileProcessor, MockFileProcessor>();
                break;
            case FileProcessorProvider.Mock:
                services.AddScoped<IMarventaFileProcessor, MockFileProcessor>();
                break;
            default:
                services.AddScoped<IMarventaFileProcessor, MockFileProcessor>();
                break;
        }

        return services;
    }

    /// <summary>
    /// Adds storage service to DI container
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="options">Storage options</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddStorageService(this IServiceCollection services, StorageServiceOptions? options = null)
    {
        var storageOptions = options ?? new StorageServiceOptions();
        services.AddSingleton(Microsoft.Extensions.Options.Options.Create(storageOptions));

        switch (storageOptions.Provider)
        {
            case StorageProvider.AzureBlob:
                // Azure Blob implementation would go here when available
                services.AddScoped<IMarventaStorage, MockStorageService>();
                break;
            case StorageProvider.MinIO:
                // MinIO implementation would go here when available
                services.AddScoped<IMarventaStorage, MockStorageService>();
                break;
            case StorageProvider.LocalFile:
                // LocalFile implementation would go here when available
                services.AddScoped<IMarventaStorage, MockStorageService>();
                break;
            case StorageProvider.Mock:
                services.AddScoped<IMarventaStorage, MockStorageService>();
                break;
            default:
                services.AddScoped<IMarventaStorage, MockStorageService>();
                break;
        }

        return services;
    }

    /// <summary>
    /// Adds CDN service to DI container
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="options">CDN options</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddCDNService(this IServiceCollection services, CDNServiceOptions? options = null)
    {
        var cdnOptions = options ?? new CDNServiceOptions();
        services.AddSingleton(Microsoft.Extensions.Options.Options.Create(cdnOptions));

        switch (cdnOptions.Provider)
        {
            case CDNProvider.CloudFlare:
                // CloudFlare implementation would go here when available
                services.AddScoped<IMarventaCDN, MockCDNService>();
                break;
            case CDNProvider.AzureCDN:
                // Azure CDN implementation would go here when available
                services.AddScoped<IMarventaCDN, MockCDNService>();
                break;
            case CDNProvider.Mock:
                services.AddScoped<IMarventaCDN, MockCDNService>();
                break;
            default:
                services.AddScoped<IMarventaCDN, MockCDNService>();
                break;
        }

        return services;
    }

    /// <summary>
    /// Adds ML service to DI container
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="options">ML options</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddMLService(this IServiceCollection services, MLServiceOptions? options = null)
    {
        var mlOptions = options ?? new MLServiceOptions();
        services.AddSingleton(Microsoft.Extensions.Options.Options.Create(mlOptions));

        switch (mlOptions.Provider)
        {
            case MLProvider.AzureCognitive:
                // Azure Cognitive implementation would go here when available
                services.AddScoped<IMarventaML, MockMLService>();
                break;
            case MLProvider.OpenAI:
                // OpenAI implementation would go here when available
                services.AddScoped<IMarventaML, MockMLService>();
                break;
            case MLProvider.Local:
                // Local ML implementation would go here when available
                services.AddScoped<IMarventaML, MockMLService>();
                break;
            case MLProvider.Mock:
                services.AddScoped<IMarventaML, MockMLService>();
                break;
            default:
                services.AddScoped<IMarventaML, MockMLService>();
                break;
        }

        return services;
    }

    /// <summary>
    /// Adds metadata service to DI container
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="options">Metadata options</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddMetadataService(this IServiceCollection services, MetadataServiceOptions? options = null)
    {
        var metadataOptions = options ?? new MetadataServiceOptions();
        services.AddSingleton(Microsoft.Extensions.Options.Options.Create(metadataOptions));

        switch (metadataOptions.Provider)
        {
            case MetadataProvider.EntityFramework:
                // EntityFramework implementation would go here when available
                services.AddScoped<IMarventaFileMetadata, MockMetadataService>();
                break;
            case MetadataProvider.MongoDB:
                // MongoDB implementation would go here when available
                services.AddScoped<IMarventaFileMetadata, MockMetadataService>();
                break;
            case MetadataProvider.Elasticsearch:
                // Elasticsearch implementation would go here when available
                services.AddScoped<IMarventaFileMetadata, MockMetadataService>();
                break;
            case MetadataProvider.Mock:
                services.AddScoped<IMarventaFileMetadata, MockMetadataService>();
                break;
            default:
                services.AddScoped<IMarventaFileMetadata, MockMetadataService>();
                break;
        }

        return services;
    }
}