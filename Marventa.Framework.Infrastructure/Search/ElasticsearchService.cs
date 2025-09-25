using System.Text.Json;

namespace Marventa.Framework.Infrastructure.Search;

public class ElasticsearchService : ISearchService
{
    private readonly ILogger<ElasticsearchService> _logger;
    private readonly ElasticsearchOptions _options;
    private readonly HttpClient _httpClient;

    public ElasticsearchService(
        ILogger<ElasticsearchService> logger,
        IOptions<ElasticsearchOptions> options,
        HttpClient httpClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        _httpClient.BaseAddress = new Uri(_options.ConnectionString);
        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
    }

    public async Task<SearchResult<T>> SearchAsync<T>(SearchRequest request, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogInformation("Searching with query: {Query}", request.Query);

        try
        {
            var indexName = GetIndexName<T>();

            var searchQuery = new
            {
                query = new
                {
                    multi_match = new
                    {
                        query = request.Query,
                        fields = new[] { "*" }
                    }
                },
                from = (request.Page - 1) * request.PageSize,
                size = request.PageSize
            };

            var jsonContent = JsonSerializer.Serialize(searchQuery);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"/{indexName}/_search", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Elasticsearch search failed. Status: {Status}, Error: {Error}", response.StatusCode, errorContent);
                throw new InvalidOperationException($"Search failed with status {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var searchResponse = JsonSerializer.Deserialize<ElasticsearchSearchResponse<T>>(responseContent);

            return new SearchResult<T>
            {
                Items = searchResponse?.Hits?.Hits?.Select(h => h.Source).ToList() ?? new List<T>(),
                TotalCount = (int)(searchResponse?.Hits?.Total?.Value ?? 0),
                Page = request.Page,
                PageSize = request.PageSize,
                SearchTime = searchResponse?.Took ?? 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing search with query: {Query}", request.Query);
            throw;
        }
    }

    public async Task<bool> IndexAsync<T>(T document, string id, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogInformation("Indexing document with ID: {Id}", id);

        try
        {
            var indexName = GetIndexName<T>();
            var jsonContent = JsonSerializer.Serialize(document);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/{indexName}/_doc/{id}", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Failed to index document {Id}. Status: {Status}, Error: {Error}", id, response.StatusCode, errorContent);
                return false;
            }

            _logger.LogDebug("Successfully indexed document {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing document with ID: {Id}", id);
            return false;
        }
    }

    public async Task<bool> IndexBulkAsync<T>(IEnumerable<T> documents, CancellationToken cancellationToken = default) where T : class
    {
        var documentList = documents.ToList();
        var count = documentList.Count;

        _logger.LogInformation("Bulk indexing {Count} documents", count);

        try
        {
            var indexName = GetIndexName<T>();
            var bulkBody = new List<string>();

            for (int i = 0; i < documentList.Count; i++)
            {
                var indexAction = JsonSerializer.Serialize(new { index = new { _index = indexName, _id = i.ToString() } });
                var documentJson = JsonSerializer.Serialize(documentList[i]);

                bulkBody.Add(indexAction);
                bulkBody.Add(documentJson);
            }

            var bulkContent = string.Join("\n", bulkBody) + "\n";
            var content = new StringContent(bulkContent, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/_bulk", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Bulk indexing failed. Status: {Status}, Error: {Error}", response.StatusCode, errorContent);
                return false;
            }

            _logger.LogDebug("Successfully bulk indexed {Count} documents", count);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during bulk indexing of {Count} documents", count);
            return false;
        }
    }

    public async Task<bool> UpdateAsync<T>(T document, string id, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogInformation("Updating document with ID: {Id}", id);

        try
        {
            var indexName = GetIndexName<T>();
            var updateDoc = new { doc = document };
            var jsonContent = JsonSerializer.Serialize(updateDoc);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"/{indexName}/_update/{id}", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Failed to update document {Id}. Status: {Status}, Error: {Error}", id, response.StatusCode, errorContent);
                return false;
            }

            _logger.LogDebug("Successfully updated document {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating document with ID: {Id}", id);
            return false;
        }
    }

    public async Task<bool> DeleteAsync<T>(string id, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogInformation("Deleting document with ID: {Id}", id);

        try
        {
            var indexName = GetIndexName<T>();
            var response = await _httpClient.DeleteAsync($"/{indexName}/_doc/{id}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Failed to delete document {Id}. Status: {Status}, Error: {Error}", id, response.StatusCode, errorContent);
                return false;
            }

            _logger.LogDebug("Successfully deleted document {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document with ID: {Id}", id);
            return false;
        }
    }

    public async Task<bool> CreateIndexAsync<T>(string indexName, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogInformation("Creating index: {IndexName}", indexName);

        try
        {
            var indexSettings = new
            {
                settings = new
                {
                    number_of_shards = _options.NumberOfShards,
                    number_of_replicas = _options.NumberOfReplicas
                }
            };

            var jsonContent = JsonSerializer.Serialize(indexSettings);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/{indexName}", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Failed to create index {IndexName}. Status: {Status}, Error: {Error}", indexName, response.StatusCode, errorContent);
                return false;
            }

            _logger.LogDebug("Successfully created index {IndexName}", indexName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating index: {IndexName}", indexName);
            return false;
        }
    }

    public async Task<bool> DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting index: {IndexName}", indexName);

        try
        {
            var response = await _httpClient.DeleteAsync($"/{indexName}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Failed to delete index {IndexName}. Status: {Status}, Error: {Error}", indexName, response.StatusCode, errorContent);
                return false;
            }

            _logger.LogDebug("Successfully deleted index {IndexName}", indexName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting index: {IndexName}", indexName);
            return false;
        }
    }

    public async Task<bool> IndexExistsAsync(string indexName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Checking if index exists: {IndexName}", indexName);

        try
        {
            var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, $"/{indexName}"), cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if index exists: {IndexName}", indexName);
            return false;
        }
    }

    public async Task<T?> GetByIdAsync<T>(string id, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogInformation("Getting document by ID: {Id}", id);

        try
        {
            var indexName = GetIndexName<T>();
            var response = await _httpClient.GetAsync($"/{indexName}/_doc/{id}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Document with ID {Id} not found", id);
                    return null;
                }

                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Failed to get document {Id}. Status: {Status}, Error: {Error}", id, response.StatusCode, errorContent);
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var docResponse = JsonSerializer.Deserialize<ElasticsearchGetResponse<T>>(responseContent);

            return docResponse?.Source;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting document by ID: {Id}", id);
            return null;
        }
    }

    public async Task<AggregationResult> AggregateAsync(string indexName, AggregationRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Running aggregation on index: {IndexName}", indexName);

        try
        {
            // Simple terms aggregation implementation
            var aggQuery = new
            {
                size = 0,
                aggs = new Dictionary<string, object>
                {
                    ["aggregation"] = new
                    {
                        terms = new
                        {
                            field = request.Field + ".keyword", // Assuming keyword field
                            size = 10
                        }
                    }
                }
            };

            var jsonContent = JsonSerializer.Serialize(aggQuery);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"/{indexName}/_search", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Aggregation failed. Status: {Status}, Error: {Error}", response.StatusCode, errorContent);
                throw new InvalidOperationException($"Aggregation failed with status {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            // Simple implementation - just return empty buckets for now
            return new AggregationResult
            {
                Buckets = new Dictionary<string, object>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running aggregation on index: {IndexName}", indexName);
            throw;
        }
    }

    private string GetIndexName<T>()
    {
        var typeName = typeof(T).Name.ToLowerInvariant();
        return $"{_options.IndexPrefix}{typeName}";
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

// Helper classes for JSON deserialization
public class ElasticsearchSearchResponse<T>
{
    public int Took { get; set; }
    public ElasticsearchHits<T> Hits { get; set; } = new();
}

public class ElasticsearchHits<T>
{
    public ElasticsearchTotal Total { get; set; } = new();
    public List<ElasticsearchHit<T>> Hits { get; set; } = new();
}

public class ElasticsearchTotal
{
    public int Value { get; set; }
    public string Relation { get; set; } = "";
}

public class ElasticsearchHit<T>
{
    public string _index { get; set; } = "";
    public string _id { get; set; } = "";
    public T Source { get; set; } = default!;
}

public class ElasticsearchGetResponse<T>
{
    public string _index { get; set; } = "";
    public string _id { get; set; } = "";
    public bool Found { get; set; }
    public T Source { get; set; } = default!;
}