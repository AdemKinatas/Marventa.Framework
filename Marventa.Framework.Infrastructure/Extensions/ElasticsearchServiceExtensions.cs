using Marventa.Framework.Infrastructure.Search;

namespace Marventa.Framework.Infrastructure.Extensions;

public static class ElasticsearchServiceExtensions
{
    public static IServiceCollection AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ElasticsearchOptions>(configuration.GetSection("Elasticsearch"));
        services.AddSingleton<IValidateOptions<ElasticsearchOptions>, ElasticsearchOptions>();
        services.AddHttpClient<ElasticsearchService>();
        services.AddScoped<ISearchService, ElasticsearchService>();
        return services;
    }

    public static IServiceCollection AddElasticsearch(this IServiceCollection services, Action<ElasticsearchOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddSingleton<IValidateOptions<ElasticsearchOptions>, ElasticsearchOptions>();
        services.AddHttpClient<ElasticsearchService>();
        services.AddScoped<ISearchService, ElasticsearchService>();
        return services;
    }
}