using System.Linq.Expressions;

namespace Marventa.Framework.Core.Interfaces;

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

public class SearchRequest
{
    public string Query { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public Dictionary<string, object> Filters { get; set; } = new();
    public List<SortField> SortFields { get; set; } = new();
    public List<string> Fields { get; set; } = new();
    public bool IncludeHighlights { get; set; }
    public string? IndexName { get; set; }
}

public class SearchResult<T>
{
    public List<T> Items { get; set; } = new();
    public long TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public double SearchTime { get; set; }
    public Dictionary<string, List<string>>? Highlights { get; set; }
    public Dictionary<string, FacetResult>? Facets { get; set; }
}

public class SortField
{
    public string Field { get; set; } = string.Empty;
    public SortDirection Direction { get; set; } = SortDirection.Ascending;
}

public enum SortDirection
{
    Ascending,
    Descending
}

public class FacetResult
{
    public string Field { get; set; } = string.Empty;
    public List<FacetValue> Values { get; set; } = new();
}

public class FacetValue
{
    public string Value { get; set; } = string.Empty;
    public long Count { get; set; }
}

public class AggregationRequest
{
    public string Field { get; set; } = string.Empty;
    public AggregationType Type { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
}

public enum AggregationType
{
    Sum,
    Average,
    Min,
    Max,
    Count,
    Terms,
    DateHistogram,
    Range
}

public class AggregationResult
{
    public object? Value { get; set; }
    public Dictionary<string, object> Buckets { get; set; } = new();
}