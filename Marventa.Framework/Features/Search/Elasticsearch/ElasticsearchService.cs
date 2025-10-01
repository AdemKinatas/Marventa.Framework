using Nest;

namespace Marventa.Framework.Features.Search.Elasticsearch;

public class ElasticsearchService : IElasticsearchService
{
    private readonly IElasticClient _client;

    public ElasticsearchService(IElasticClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<bool> IndexDocumentAsync<T>(string indexName, T document, string id, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = await _client.IndexAsync(document, i => i
            .Index(indexName)
            .Id(id), cancellationToken);

        return response.IsValid;
    }

    public async Task<bool> IndexManyAsync<T>(string indexName, IEnumerable<T> documents, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = await _client.IndexManyAsync(documents, indexName, cancellationToken);
        return response.IsValid;
    }

    public async Task<T?> GetDocumentAsync<T>(string indexName, string id, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = await _client.GetAsync<T>(id, g => g.Index(indexName), cancellationToken);
        return response.IsValid ? response.Source : null;
    }

    public async Task<IEnumerable<T>> SearchAsync<T>(string indexName, string query, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = await _client.SearchAsync<T>(s => s
            .Index(indexName)
            .Query(q => q
                .QueryString(qs => qs
                    .Query(query))), cancellationToken);

        return response.IsValid ? response.Documents : Enumerable.Empty<T>();
    }

    public async Task<bool> DeleteDocumentAsync(string indexName, string id, CancellationToken cancellationToken = default)
    {
        var response = await _client.DeleteAsync<object>(id, d => d.Index(indexName), cancellationToken);
        return response.IsValid;
    }

    public async Task<bool> DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default)
    {
        var response = await _client.Indices.DeleteAsync(indexName, ct: cancellationToken);
        return response.IsValid;
    }

    public async Task<bool> CreateIndexAsync<T>(string indexName, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = await _client.Indices.CreateAsync(indexName, c => c
            .Map<T>(m => m.AutoMap()), cancellationToken);

        return response.IsValid;
    }
}
