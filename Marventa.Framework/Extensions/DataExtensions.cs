using Marventa.Framework.Configuration;

using Marventa.Framework.Features.Search.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Nest;

namespace Marventa.Framework.Extensions;

/// <summary>
/// Provides extension methods for configuring database and search services.
/// Supports MongoDB and Elasticsearch integration.
/// </summary>
public static class DataExtensions
{
    /// <summary>
    /// Adds MongoDB services with database client and database instances.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddMarventaMongoDB(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (!configuration.HasSection(ConfigurationKeys.MongoDB))
        {
            return services;
        }

        var connectionString = configuration.GetValueOrDefault(
            ConfigurationKeys.MongoDBConnectionString,
            ConfigurationKeys.Defaults.MongoDBConnectionString);

        var databaseName = configuration.GetValueOrDefault(
            ConfigurationKeys.MongoDBDatabaseName,
            ConfigurationKeys.Defaults.MongoDBDatabaseName);

        // Register MongoDB client as singleton
        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));

        // Register database instance as scoped service
        services.AddScoped(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            return client.GetDatabase(databaseName);
        });

        return services;
    }

    /// <summary>
    /// Adds Elasticsearch services with client and search service implementations.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddMarventaElasticsearch(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (!configuration.HasSection(ConfigurationKeys.Elasticsearch))
        {
            return services;
        }

        var elasticsearchUri = configuration.GetValueOrDefault(
            ConfigurationKeys.ElasticsearchUri,
            ConfigurationKeys.Defaults.ElasticsearchUri);

        var settings = new ConnectionSettings(new Uri(elasticsearchUri))
            .DefaultIndex(ConfigurationKeys.Defaults.ElasticsearchDefaultIndex)
            .EnableApiVersioningHeader();

        // Register Elasticsearch client as singleton
        services.AddSingleton<IElasticClient>(_ => new ElasticClient(settings));

        // Register Elasticsearch service as scoped
        services.AddScoped<IElasticsearchService, ElasticsearchService>();

        return services;
    }

    /// <summary>
    /// Adds all available data services based on configuration.
    /// Configures MongoDB and Elasticsearch if their sections are present.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddMarventaDataServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMarventaMongoDB(configuration);
        services.AddMarventaElasticsearch(configuration);

        return services;
    }
}
