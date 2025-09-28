using Marventa.Framework.Core.Interfaces;
using Marventa.Framework.Core.Models.CDN;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Infrastructure.Services.FileServices;

/// <summary>
/// Mock implementation of CDN service for development and testing
/// </summary>
public class MockCDNService : IMarventaCDN
{
    private readonly ILogger<MockCDNService> _logger;
    private readonly Dictionary<string, MockCDNFile> _cdnFiles = new();

    public MockCDNService(ILogger<MockCDNService> logger)
    {
        _logger = logger;
    }

    public Task<CDNUploadResult> UploadToCDNAsync(string fileId, Stream content, string contentType, CDNUploadOptions? options = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Uploading file {FileId} with content type {ContentType} to CDN", fileId, contentType);
        var data = new byte[content.Length];
        content.ReadExactly(data, 0, (int)content.Length);

        _cdnFiles[fileId] = new MockCDNFile
        {
            FileId = fileId,
            FileName = $"file-{fileId}",
            Data = data,
            UploadedAt = DateTime.UtcNow,
            ContentType = contentType
        };

        var result = new CDNUploadResult
        {
            CDNUrl = $"https://cdn.mock.com/{fileId}",
            RegionalUrls = new Dictionary<string, string>
            {
                ["us-east"] = $"https://us-east.cdn.mock.com/{fileId}",
                ["eu-west"] = $"https://eu-west.cdn.mock.com/{fileId}",
                ["asia-pacific"] = $"https://asia-pacific.cdn.mock.com/{fileId}"
            },
            CDNFileId = fileId,
            UploadedAt = DateTime.UtcNow,
            FileSizeBytes = data.Length,
            ETag = $"etag-{fileId}",
            EstimatedPropagationTime = TimeSpan.FromMinutes(5),
            Success = true
        };

        return Task.FromResult(result);
    }

    public Task<CDNInvalidationResult> InvalidateCacheAsync(string[] urls, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Invalidating cache for {Count} URLs", urls.Length);

        var urlStatuses = urls.ToDictionary(
            url => url,
            _ => InvalidationStatus.Completed);

        var result = new CDNInvalidationResult
        {
            InvalidationId = Guid.NewGuid().ToString(),
            UrlStatuses = urlStatuses,
            EstimatedCompletionTime = DateTime.UtcNow.AddMinutes(2),
            EdgeLocationsCount = 150,
            Success = true
        };

        return Task.FromResult(result);
    }

    public Task<CDNWarmupResult> WarmCacheAsync(string[] urls, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Warming cache for {Count} URLs", urls.Length);

        var urlStatuses = urls.ToDictionary(
            url => url,
            _ => WarmupStatus.Completed);

        var result = new CDNWarmupResult
        {
            WarmupId = Guid.NewGuid().ToString(),
            UrlStatuses = urlStatuses,
            EdgeLocations = new[] { "us-east-1", "eu-west-1", "asia-pacific-1" },
            EstimatedCompletionTime = DateTime.UtcNow.AddMinutes(1),
            Success = true
        };

        return Task.FromResult(result);
    }

    public Task<CDNMetrics> GetMetricsAsync(string fileId, TimeRange timeRange, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Getting CDN metrics for file {FileId} for period {Start} to {End}", fileId, timeRange.StartTime, timeRange.EndTime);

        var result = new CDNMetrics
        {
            TimeRange = timeRange,
            TotalRequests = 1000000,
            TotalBandwidthBytes = 5368709120, // 5GB
            CacheHitRatio = 0.85,
            AverageResponseTimeMs = 45.5,
            RequestsByRegion = new Dictionary<string, long>
            {
                ["us-east"] = 400000,
                ["eu-west"] = 300000,
                ["asia-pacific"] = 300000
            },
            BandwidthByRegion = new Dictionary<string, long>
            {
                ["us-east"] = 2147483648,
                ["eu-west"] = 1610612736,
                ["asia-pacific"] = 1610612736
            },
            StatusCodes = new Dictionary<int, long>
            {
                [200] = 850000,
                [304] = 100000,
                [404] = 45000,
                [500] = 5000
            },
            PeakRequestsPerSecond = 1500,
            ErrorRatePercentage = 5.0
        };

        return Task.FromResult(result);
    }

    public Task<string> GetCDNUrlAsync(string fileId, URLTransformations? transformations = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Getting CDN URL for file {FileId}", fileId);

        var baseUrl = $"https://cdn.mock.com/{fileId}";

        if (transformations == null)
            return Task.FromResult(baseUrl);

        var queryParams = new List<string>();

        if (transformations.Width.HasValue)
            queryParams.Add($"w={transformations.Width}");
        if (transformations.Height.HasValue)
            queryParams.Add($"h={transformations.Height}");
        if (transformations.Quality.HasValue)
            queryParams.Add($"q={transformations.Quality}");
        if (!string.IsNullOrEmpty(transformations.Format))
            queryParams.Add($"f={transformations.Format}");
        if (transformations.SmartCrop.HasValue)
            queryParams.Add($"c={transformations.SmartCrop}");

        var optimizedUrl = queryParams.Count > 0
            ? $"{baseUrl}?{string.Join("&", queryParams)}"
            : baseUrl;

        return Task.FromResult(optimizedUrl);
    }

    public Task<string> GenerateOptimizedUrlAsync(string baseUrl, URLTransformations transformations, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Generating optimized URL for {BaseUrl}", baseUrl);

        var queryParams = new List<string>();

        if (transformations.Width.HasValue)
            queryParams.Add($"w={transformations.Width}");
        if (transformations.Height.HasValue)
            queryParams.Add($"h={transformations.Height}");
        if (transformations.Quality.HasValue)
            queryParams.Add($"q={transformations.Quality}");
        if (!string.IsNullOrEmpty(transformations.Format))
            queryParams.Add($"f={transformations.Format}");
        if (transformations.SmartCrop.HasValue)
            queryParams.Add($"c={transformations.SmartCrop}");

        var optimizedUrl = queryParams.Count > 0
            ? $"{baseUrl}?{string.Join("&", queryParams)}"
            : baseUrl;

        return Task.FromResult(optimizedUrl);
    }

    public Task<CDNConfigurationResult> ConfigureCachingRulesAsync(CachingRule[] rules, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Configuring {Count} caching rules", rules.Length);

        var result = new CDNConfigurationResult
        {
            ConfigurationId = Guid.NewGuid().ToString(),
            AppliedRules = rules,
            FailedRules = new Dictionary<CachingRule, string>(),
            DeployedAt = DateTime.UtcNow,
            EstimatedPropagationTime = TimeSpan.FromMinutes(10),
            Success = true
        };

        return Task.FromResult(result);
    }

    public Task<CDNDeletionResult> DeleteFromCDNAsync(string fileId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Deleting file {FileId} from CDN", fileId);

        var deletedUrls = _cdnFiles.ContainsKey(fileId)
            ? new[] { $"https://cdn.mock.com/{fileId}" }
            : Array.Empty<string>();

        _cdnFiles.Remove(fileId);

        var result = new CDNDeletionResult
        {
            DeletionId = Guid.NewGuid().ToString(),
            DeletedUrls = deletedUrls,
            EdgeLocationsClearedCount = 150,
            EstimatedCompletionTime = DateTime.UtcNow.AddMinutes(3),
            Success = true
        };

        return Task.FromResult(result);
    }

    public Task<CDNDistributionStatus> GetDistributionStatusAsync(string fileId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Getting distribution status for file {FileId}", fileId);

        var edgeStatuses = new Dictionary<string, EdgeLocationStatus>
        {
            ["us-east-1"] = new() { LocationId = "us-east-1", Region = "us-east", Status = EdgeStatus.Available, CacheHitRatio = 0.9 },
            ["eu-west-1"] = new() { LocationId = "eu-west-1", Region = "eu-west", Status = EdgeStatus.Available, CacheHitRatio = 0.85 },
            ["asia-pacific-1"] = new() { LocationId = "asia-pacific-1", Region = "asia-pacific", Status = EdgeStatus.Available, CacheHitRatio = 0.8 }
        };

        var result = new CDNDistributionStatus
        {
            FileId = fileId,
            Status = DistributionState.Distributed,
            EdgeStatuses = edgeStatuses,
            TotalEdgeLocations = 150,
            DistributedEdgeLocations = 150,
            LastUpdated = DateTime.UtcNow
        };

        return Task.FromResult(result);
    }

    private class MockCDNFile
    {
        public string FileId { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public byte[] Data { get; set; } = Array.Empty<byte>();
        public DateTime UploadedAt { get; set; }
        public string ContentType { get; set; } = string.Empty;
    }
}