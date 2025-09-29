using System.Linq.Expressions;

namespace Marventa.Framework.Core.Interfaces.Services;

public interface ISearchService
{
    Task<SearchResult<T>> SearchAsync<T>(SearchRequest request, CancellationToken cancellationToken = default) where T : class;
    Task<bool> IndexAsync<T>(T document, string id, CancellationToken cancellationToken = default) where T : class;
    Task<bool> IndexBulkAsync<T>(IEnumerable<T> documents, CancellationToken cancellationToken = default) where T : class;
    Task<bool> UpdateAsync<T>(T document, string id, CancellationToken cancellationToken = default) where T : class;
    Task<bool> DeleteAsync<T>(string id, CancellationToken cancellationToken = default) where T : class;
    Task<bool> CreateIndexAsync<T>(string indexName, CancellationToken cancellationToken = default) where T : class;
    Task<bool> DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default);
    Task<bool> IndexExistsAsync(string indexName, CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync<T>(string id, CancellationToken cancellationToken = default) where T : class;
    Task<AggregationResult> AggregateAsync(string indexName, AggregationRequest request, CancellationToken cancellationToken = default);
}