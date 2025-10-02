using Amazon.S3;
using Marventa.Framework.Configuration;

using Marventa.Framework.Features.Storage.Abstractions;
using Marventa.Framework.Features.Storage.AWS;
using Marventa.Framework.Features.Storage.Azure;
using Marventa.Framework.Features.Storage.Local;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marventa.Framework.Extensions;

/// <summary>
/// Provides extension methods for configuring storage services.
/// Supports Azure Blob Storage, AWS S3, MongoDB, Elasticsearch, and local file storage.
/// </summary>
public static class StorageExtensions
{
    /// <summary>
    /// Adds Azure Blob Storage services.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when Azure Storage connection string is missing.</exception>
    public static IServiceCollection AddMarventaAzureStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (!configuration.HasSection(ConfigurationKeys.AzureStorage))
        {
            return services;
        }

        var connectionString = configuration.GetRequiredValue(ConfigurationKeys.AzureStorageConnectionString);
        var containerName = configuration.GetValueOrDefault(
            ConfigurationKeys.AzureStorageContainerName,
            ConfigurationKeys.Defaults.AzureStorageContainerName);

        services.AddSingleton<IStorageService>(
            _ => new AzureBlobStorage(connectionString, containerName));

        return services;
    }

    /// <summary>
    /// Adds AWS S3 storage services.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when AWS credentials are missing.</exception>
    public static IServiceCollection AddMarventaAwsStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (!configuration.HasSection(ConfigurationKeys.AWS))
        {
            return services;
        }

        var accessKey = configuration.GetRequiredValue(ConfigurationKeys.AWSAccessKey);
        var secretKey = configuration.GetRequiredValue(ConfigurationKeys.AWSSecretKey);

        var region = configuration.GetValueOrDefault(
            ConfigurationKeys.AWSRegion,
            ConfigurationKeys.Defaults.AWSRegion);

        var bucketName = configuration.GetValueOrDefault(
            ConfigurationKeys.AWSBucketName,
            ConfigurationKeys.Defaults.AWSBucketName);

        var s3Client = new AmazonS3Client(
            accessKey,
            secretKey,
            Amazon.RegionEndpoint.GetBySystemName(region));

        services.AddSingleton<IAmazonS3>(s3Client);
        services.AddSingleton<IStorageService>(_ => new S3Storage(s3Client, bucketName));

        return services;
    }

    /// <summary>
    /// Adds local file system storage services.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddMarventaLocalStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (!configuration.HasSection(ConfigurationKeys.LocalStorage))
        {
            return services;
        }

        var basePath = configuration.GetValueOrDefault(
            ConfigurationKeys.LocalStorageBasePath,
            Path.Combine(Directory.GetCurrentDirectory(), "uploads"));

        var baseUrl = configuration[ConfigurationKeys.LocalStorageBaseUrl];

        services.AddSingleton<IStorageService>(_ => new LocalFileStorage(basePath, baseUrl));

        return services;
    }

    /// <summary>
    /// Adds storage services based on available configuration.
    /// Automatically detects and configures Azure, AWS, or local storage.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddMarventaStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Priority: Azure > AWS > Local
        if (configuration.HasSection(ConfigurationKeys.AzureStorage))
        {
            services.AddMarventaAzureStorage(configuration);
        }
        else if (configuration.HasSection(ConfigurationKeys.AWS))
        {
            services.AddMarventaAwsStorage(configuration);
        }
        else if (configuration.HasSection(ConfigurationKeys.LocalStorage))
        {
            services.AddMarventaLocalStorage(configuration);
        }

        return services;
    }
}
