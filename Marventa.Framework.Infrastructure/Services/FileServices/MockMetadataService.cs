using Marventa.Framework.Core.Interfaces.Storage;
using Marventa.Framework.Core.Models.FileMetadata;
using Marventa.Framework.Core.Models.CDN;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Infrastructure.Services.FileServices;

/// <summary>
/// Mock implementation of metadata service for development and testing
/// </summary>
public class MockMetadataService : IMarventaFileMetadata
{
    private readonly ILogger<MockMetadataService> _logger;
    private readonly Dictionary<string, FileMetadata> _metadata = new();
    private readonly Random _random = new();

    public MockMetadataService(ILogger<MockMetadataService> logger)
    {
        _logger = logger;
    }

    public Task<MetadataStorageResult> StoreMetadataAsync(string fileId, FileMetadata metadata, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Storing metadata for file {FileId}", fileId);

        metadata.FileId = fileId;
        metadata.CreatedAt = DateTime.UtcNow;
        metadata.UpdatedAt = DateTime.UtcNow;

        _metadata[fileId] = metadata;

        var result = new MetadataStorageResult
        {
            FileId = fileId,
            Success = true,
            StoredAt = DateTime.UtcNow,
            MetadataId = Guid.NewGuid().ToString()
        };

        return Task.FromResult(result);
    }

    public Task<FileMetadata?> GetMetadataAsync(string fileId, bool includeAnalytics = false, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Getting metadata for file {FileId}", fileId);

        if (!_metadata.TryGetValue(fileId, out var metadata))
        {
            return Task.FromResult<FileMetadata?>(null);
        }

        if (includeAnalytics)
        {
            metadata.Analytics = new FileAnalytics
            {
                TotalViews = _random.Next(10, 1000),
                TotalDownloads = _random.Next(5, 100),
                LastAccessedAt = DateTime.UtcNow.AddDays(-_random.Next(1, 30)),
                AverageRating = Math.Round(_random.NextDouble() * 2 + 3, 1), // 3.0-5.0
                UniqueViewers = _random.Next(5, 50)
            };
        }

        return Task.FromResult<FileMetadata?>(metadata);
    }

    public Task<MetadataUpdateResult> UpdateMetadataAsync(string fileId, FileMetadata metadata, MetadataMergeMode mergeMode = MetadataMergeMode.Merge, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Updating metadata for file {FileId} with mode {MergeMode}", fileId, mergeMode);

        if (!_metadata.ContainsKey(fileId))
        {
            return Task.FromResult(new MetadataUpdateResult
            {
                FileId = fileId,
                Success = false,
                ErrorMessage = "Metadata not found"
            });
        }

        var existing = _metadata[fileId];

        switch (mergeMode)
        {
            case MetadataMergeMode.Replace:
                metadata.FileId = fileId;
                metadata.CreatedAt = existing.CreatedAt;
                metadata.UpdatedAt = DateTime.UtcNow;
                _metadata[fileId] = metadata;
                break;

            case MetadataMergeMode.Merge:
                existing.Title = metadata.Title ?? existing.Title;
                existing.Description = metadata.Description ?? existing.Description;
                existing.ContentType = metadata.ContentType ?? existing.ContentType;
                if (metadata.Tags?.Any() == true)
                {
                    existing.Tags = existing.Tags.Union(metadata.Tags).ToList();
                }
                existing.UpdatedAt = DateTime.UtcNow;
                break;
        }

        var result = new MetadataUpdateResult
        {
            FileId = fileId,
            Success = true,
            UpdatedAt = DateTime.UtcNow,
            PreviousVersion = "v1.0",
            NewVersion = "v1.1"
        };

        return Task.FromResult(result);
    }

    public Task<MetadataDeletionResult> DeleteMetadataAsync(string fileId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Deleting metadata for file {FileId}", fileId);

        var success = _metadata.Remove(fileId);

        var result = new MetadataDeletionResult
        {
            FileId = fileId,
            Success = success,
            DeletedAt = DateTime.UtcNow,
            ErrorMessage = success ? null : "Metadata not found"
        };

        return Task.FromResult(result);
    }

    public Task<MetadataSearchResult> SearchByTagsAsync(MetadataSearchCriteria searchCriteria, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Searching by tags: {Tags}", string.Join(", ", searchCriteria.Tags));

        var matchingFiles = _metadata.Values
            .Where(m => searchCriteria.Tags.Any(tag => m.Tags.Any(t => t.Name.Contains(tag, StringComparison.OrdinalIgnoreCase))))
            .Take(searchCriteria.MaxResults)
            .Select(m => new MetadataSearchResultItem
            {
                FileId = m.FileId,
                Title = m.Title,
                Description = m.Description,
                Tags = m.Tags.ToArray(),
                Relevance = Math.Round(_random.NextDouble(), 3),
                CreatedAt = m.CreatedAt
            })
            .ToArray();

        var result = new MetadataSearchResult
        {
            Items = matchingFiles,
            TotalCount = matchingFiles.Length,
            SearchCriteria = searchCriteria,
            SearchTime = TimeSpan.FromMilliseconds(_random.Next(50, 200)),
            Success = true
        };

        return Task.FromResult(result);
    }

    public Task<SimilaritySearchResult> SearchBySimilarityAsync(string referenceFileId, double similarityThreshold = 0.7, int maxResults = 20, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Searching for files similar to {ReferenceFileId}", referenceFileId);

        if (!_metadata.ContainsKey(referenceFileId))
        {
            return Task.FromResult(new SimilaritySearchResult
            {
                Success = false,
                ErrorMessage = "Reference file not found"
            });
        }

        var similarFiles = _metadata.Values
            .Where(m => m.FileId != referenceFileId)
            .Take(maxResults)
            .Select(m => new SimilarityMatch
            {
                FileMetadata = m,
                SimilarityScore = Math.Round(_random.NextDouble() * (1.0 - similarityThreshold) + similarityThreshold, 3)
            })
            .OrderByDescending(item => item.SimilarityScore)
            .ToList();

        var result = new SimilaritySearchResult
        {
            ReferenceFileId = referenceFileId,
            SimilarFiles = similarFiles,
            SearchTime = TimeSpan.FromMilliseconds(_random.Next(100, 500)),
            Success = true
        };

        return Task.FromResult(result);
    }

    public Task<TagOperationResult> AddTagsAsync(string fileId, string[] tags, TagSource source = TagSource.User, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Adding {Count} tags to file {FileId}", tags.Length, fileId);

        if (!_metadata.TryGetValue(fileId, out var metadata))
        {
            return Task.FromResult(new TagOperationResult
            {
                FileId = fileId,
                Success = false,
                ErrorMessage = "File metadata not found"
            });
        }

        var newTags = tags.Select(tag => new FileTag
        {
            Name = tag,
            Source = source,
            Confidence = source == TagSource.AI ? _random.NextDouble() * 0.4 + 0.6 : 1.0,
            CreatedAt = DateTime.UtcNow
        }).ToArray();

        metadata.Tags = metadata.Tags.Union(newTags).ToList();
        metadata.UpdatedAt = DateTime.UtcNow;

        var result = new TagOperationResult
        {
            FileId = fileId,
            Success = true,
            ProcessedTags = newTags,
            TotalTagCount = metadata.Tags.Count,
            UpdatedAt = DateTime.UtcNow
        };

        return Task.FromResult(result);
    }

    public Task<TagOperationResult> RemoveTagsAsync(string fileId, string[] tags, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Removing {Count} tags from file {FileId}", tags.Length, fileId);

        if (!_metadata.TryGetValue(fileId, out var metadata))
        {
            return Task.FromResult(new TagOperationResult
            {
                FileId = fileId,
                Success = false,
                ErrorMessage = "File metadata not found"
            });
        }

        var removedTags = metadata.Tags.Where(t => tags.Contains(t.Name)).ToArray();
        metadata.Tags = metadata.Tags.Where(t => !tags.Contains(t.Name)).ToList();
        metadata.UpdatedAt = DateTime.UtcNow;

        var result = new TagOperationResult
        {
            FileId = fileId,
            Success = true,
            ProcessedTags = removedTags,
            TotalTagCount = metadata.Tags.Count,
            UpdatedAt = DateTime.UtcNow
        };

        return Task.FromResult(result);
    }

    public Task<TagPopularityResult> GetPopularTagsAsync(int limit = 50, TimeRange? timeRange = null, TagFilters? filters = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Getting popular tags (limit: {Limit})", limit);

        var allTags = _metadata.Values
            .SelectMany(m => m.Tags)
            .GroupBy(t => t.Name)
            .Select(g => new TagStatistics
            {
                Tag = g.Key,
                UsageCount = g.Count(),
                AverageConfidence = g.Average(t => t.Confidence),
                UsagePercentage = Math.Round((double)g.Count() / Math.Max(_metadata.Count, 1) * 100, 2),
                Trend = TagTrend.Stable,
                SourceBreakdown = g.GroupBy(t => t.Source).ToDictionary(sg => sg.Key, sg => (long)sg.Count())
            })
            .OrderByDescending(t => t.UsageCount)
            .Take(limit)
            .ToArray();

        var result = new TagPopularityResult
        {
            PopularTags = allTags.ToList(),
            TimeRange = timeRange ?? new TimeRange { StartTime = DateTime.UtcNow.AddDays(-30), EndTime = DateTime.UtcNow },
            TotalUniqueTagCount = allTags.Length,
            AnalysisTime = TimeSpan.FromMilliseconds(_random.Next(100, 300)),
            Success = true
        };

        return Task.FromResult(result);
    }

    public Task<FileAnalyticsResult> GetFileAnalyticsAsync(string? fileId = null, TimeRange? timeRange = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Getting file analytics for {FileId}", fileId ?? "all files");

        var files = fileId != null
            ? _metadata.Where(kvp => kvp.Key == fileId).ToArray()
            : _metadata.ToArray();

        var analytics = new FileAnalyticsResult
        {
            TimeRange = timeRange ?? new TimeRange { StartTime = DateTime.UtcNow.AddDays(-30), EndTime = DateTime.UtcNow },
            TotalFiles = files.Length,
            TotalViews = files.Sum(_ => _random.Next(10, 1000)),
            TotalDownloads = files.Sum(_ => _random.Next(5, 100)),
            AverageRating = Math.Round(_random.NextDouble() * 2 + 3, 1),
            TopTags = files.SelectMany(f => f.Value.Tags)
                .GroupBy(t => t.Name)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => g.Key)
                .ToList(),
            FileTypeDistribution = new Dictionary<string, long>
            {
                ["image/jpeg"] = _random.Next(10, 100),
                ["image/png"] = _random.Next(5, 50),
                ["video/mp4"] = _random.Next(1, 20),
                ["application/pdf"] = _random.Next(1, 30)
            },
            Success = true
        };

        return Task.FromResult(analytics);
    }

    public Task<AccessRecordingResult> RecordAccessAsync(string fileId, FileAccessType accessType, string? userId = null, Dictionary<string, object>? metadata = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Recording {AccessType} access for file {FileId}", accessType, fileId);

        var result = new AccessRecordingResult
        {
            FileId = fileId,
            AccessType = accessType,
            UserId = userId,
            RecordedAt = DateTime.UtcNow,
            Success = true,
            AccessId = Guid.NewGuid().ToString()
        };

        return Task.FromResult(result);
    }

    public Task<BulkImportResult> BulkImportMetadataAsync(Dictionary<string, FileMetadata> metadataEntries, BulkImportOptions? importOptions = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Bulk importing {Count} metadata entries", metadataEntries.Count);

        var results = new Dictionary<string, MetadataStorageResult>();

        foreach (var (fileId, metadata) in metadataEntries)
        {
            var result = StoreMetadataAsync(fileId, metadata, cancellationToken).Result;
            results[fileId] = result;
        }

        var importResult = new BulkImportResult
        {
            TotalEntries = metadataEntries.Count,
            SuccessfulImports = results.Values.Count(r => r.Success),
            FailedImports = results.Values.Count(r => !r.Success),
            Results = results.Values.ToList(),
            ImportTime = DateTime.UtcNow,
            Success = true
        };

        return Task.FromResult(importResult);
    }

    public Task<MetadataExportResult> ExportMetadataAsync(MetadataExportCriteria exportCriteria, string format = "json", CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Exporting metadata in {Format} format", format);

        var filesToExport = _metadata.Values.Take(exportCriteria.MaxFiles ?? 1000).ToArray();

        var exportData = format.ToLower() switch
        {
            "json" => System.Text.Json.JsonSerializer.Serialize(filesToExport),
            "csv" => "FileId,Title,Description,Tags\n" + string.Join("\n", filesToExport.Select(f => $"{f.FileId},{f.Title},{f.Description},{string.Join(";", f.Tags.Select(t => t.Name))}")),
            "xml" => "<metadata>" + string.Join("", filesToExport.Select(f => $"<file id=\"{f.FileId}\"><title>{f.Title}</title></file>")) + "</metadata>",
            _ => System.Text.Json.JsonSerializer.Serialize(filesToExport)
        };

        var result = new MetadataExportResult
        {
            Format = format,
            TotalExported = filesToExport.Length,
            DataStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(exportData)),
            ExportTime = TimeSpan.FromMilliseconds(_random.Next(100, 500)),
            Success = true
        };

        return Task.FromResult(result);
    }

    public Task<TagSuggestionResult> SuggestTagsAsync(string fileId, int maxSuggestions = 10, double confidence = 0.5, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Suggesting tags for file {FileId}", fileId);

        if (!_metadata.ContainsKey(fileId))
        {
            return Task.FromResult(new TagSuggestionResult
            {
                FileId = fileId,
                Success = false,
                ErrorMessage = "File not found"
            });
        }

        var suggestedTags = new[]
        {
            "nature", "landscape", "portrait", "urban", "vintage", "modern", "colorful",
            "black-white", "outdoor", "indoor", "professional", "casual", "artistic"
        };

        var suggestions = suggestedTags
            .OrderBy(_ => _random.Next())
            .Take(maxSuggestions)
            .Select(tag => new TagSuggestion
            {
                Name = tag,
                Confidence = Math.Round(_random.NextDouble() * (1.0 - confidence) + confidence, 3),
                Source = TagSuggestionSource.ContentAnalysis,
                Reason = "Based on visual content analysis"
            })
            .ToArray();

        var result = new TagSuggestionResult
        {
            FileId = fileId,
            Suggestions = suggestions.ToList(),
            ProcessingTime = TimeSpan.FromMilliseconds(_random.Next(200, 600)),
            Success = true
        };

        return Task.FromResult(result);
    }
}