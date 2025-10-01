namespace Marventa.Framework.Features.Search.Elasticsearch;

public interface IElasticsearchService
{
    Task<bool> IndexDocumentAsync<T>(string indexName, T document, string id, CancellationToken cancellationToken = default) where T : class;
    Task<bool> IndexManyAsync<T>(string indexName, IEnumerable<T> documents, CancellationToken cancellationToken = default) where T : class;
    Task<T?> GetDocumentAsync<T>(string indexName, string id, CancellationToken cancellationToken = default) where T : class;
    Task<IEnumerable<T>> SearchAsync<T>(string indexName, string query, CancellationToken cancellationToken = default) where T : class;
    Task<bool> DeleteDocumentAsync(string indexName, string id, CancellationToken cancellationToken = default);
    Task<bool> DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default);
    Task<bool> CreateIndexAsync<T>(string indexName, CancellationToken cancellationToken = default) where T : class;
}
