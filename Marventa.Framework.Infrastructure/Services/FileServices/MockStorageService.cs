using Marventa.Framework.Core.Interfaces.Storage;
using Marventa.Framework.Core.Models.Storage;
using Marventa.Framework.Core.Models.CDN;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Infrastructure.Services.FileServices;

/// <summary>
/// Mock implementation of storage service for development and testing
/// </summary>
public class MockStorageService : IMarventaStorage
{
    private readonly ILogger<MockStorageService> _logger;
    private readonly Dictionary<string, MockFileData> _files = new();

    public MockStorageService(ILogger<MockStorageService> logger)
    {
        _logger = logger;
    }

    public Task<StorageUploadResult> UploadFileAsync(Stream stream, string fileName, string contentType, StorageUploadOptions? options = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Uploading file {FileName}", fileName);

        var fileId = Guid.NewGuid().ToString();
        var data = new byte[stream.Length];
        stream.ReadExactly(data, 0, (int)stream.Length);

        _files[fileId] = new MockFileData
        {
            FileId = fileId,
            FileName = fileName,
            ContentType = contentType,
            Data = data,
            UploadedAt = DateTime.UtcNow
        };

        var result = new StorageUploadResult
        {
            FileId = fileId,
            FilePath = $"mock://files/{fileId}",
            PublicUrl = $"https://mock-storage.com/files/{fileId}",
            FileSizeBytes = data.Length,
            MD5Hash = Convert.ToHexString(System.Security.Cryptography.MD5.HashData(data)),
            ETag = $"etag-{fileId}",
            UploadedAt = DateTime.UtcNow,
            Success = true
        };

        return Task.FromResult(result);
    }

    public Task<StorageDownloadResult> DownloadFileAsync(string fileId, StorageDownloadOptions? options = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Downloading file {FileId}", fileId);

        if (!_files.TryGetValue(fileId, out var fileData))
        {
            return Task.FromResult(new StorageDownloadResult
            {
                Success = false,
                ErrorMessage = "File not found"
            });
        }

        var result = new StorageDownloadResult
        {
            Content = new MemoryStream(fileData.Data),
            FileInfo = new StorageFileInfo
            {
                FileId = fileId,
                FileName = fileData.FileName,
                FilePath = $"mock://files/{fileId}",
                FileSizeBytes = fileData.Data.Length,
                ContentType = fileData.ContentType,
                CreatedAt = fileData.UploadedAt,
                LastModified = fileData.UploadedAt
            },
            ContentType = fileData.ContentType,
            ContentLength = fileData.Data.Length,
            LastModified = fileData.UploadedAt,
            Success = true
        };

        return Task.FromResult(result);
    }

    public Task<StorageDeletionResult> DeleteFileAsync(string fileId, StorageDeletionOptions? options = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Deleting file {FileId}", fileId);

        var success = _files.Remove(fileId);

        var result = new StorageDeletionResult
        {
            FileId = fileId,
            Success = success,
            DeletedAt = DateTime.UtcNow,
            PermanentlyDeleted = options?.PermanentDelete ?? false,
            ErrorMessage = success ? null : "File not found"
        };

        return Task.FromResult(result);
    }

    public Task<bool> FileExistsAsync(string fileId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Checking if file exists {FileId}", fileId);
        return Task.FromResult(_files.ContainsKey(fileId));
    }

    public Task<StorageFileInfo?> GetFileInfoAsync(string fileId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Getting file info {FileId}", fileId);

        if (!_files.TryGetValue(fileId, out var fileData))
        {
            return Task.FromResult<StorageFileInfo?>(null);
        }

        var result = new StorageFileInfo
        {
            FileId = fileId,
            FileName = fileData.FileName,
            FilePath = $"mock://files/{fileId}",
            FileSizeBytes = fileData.Data.Length,
            ContentType = fileData.ContentType,
            CreatedAt = fileData.UploadedAt,
            LastModified = fileData.UploadedAt
        };

        return Task.FromResult<StorageFileInfo?>(result);
    }

    public Task<StorageCopyResult> CopyFileAsync(string sourceFileId, string destinationPath, StorageCopyOptions? options = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Copying file {SourceFileId} to {DestinationPath}", sourceFileId, destinationPath);

        if (!_files.TryGetValue(sourceFileId, out var sourceFile))
        {
            return Task.FromResult(new StorageCopyResult
            {
                Success = false,
                ErrorMessage = "Source file not found"
            });
        }

        var newFileId = Guid.NewGuid().ToString();
        _files[newFileId] = new MockFileData
        {
            FileId = newFileId,
            FileName = Path.GetFileName(destinationPath),
            ContentType = sourceFile.ContentType,
            Data = (byte[])sourceFile.Data.Clone(),
            UploadedAt = DateTime.UtcNow
        };

        var result = new StorageCopyResult
        {
            NewFileId = newFileId,
            DestinationPath = destinationPath,
            Success = true,
            CopiedAt = DateTime.UtcNow
        };

        return Task.FromResult(result);
    }

    public Task<StorageMoveResult> MoveFileAsync(string sourceFileId, string destinationPath, StorageMoveOptions? options = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Moving file {SourceFileId} to {DestinationPath}", sourceFileId, destinationPath);

        if (!_files.TryGetValue(sourceFileId, out var sourceFile))
        {
            return Task.FromResult(new StorageMoveResult
            {
                Success = false,
                ErrorMessage = "Source file not found"
            });
        }

        sourceFile.FileName = Path.GetFileName(destinationPath);

        var result = new StorageMoveResult
        {
            FileId = sourceFileId,
            NewPath = destinationPath,
            Success = true,
            MovedAt = DateTime.UtcNow
        };

        return Task.FromResult(result);
    }

    public Task<StorageListResult> ListFilesAsync(string? directoryPath = null, StorageListOptions? options = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Listing files in directory {DirectoryPath}", directoryPath);

        var files = _files.Values
            .Select(f => new StorageFileInfo
            {
                FileId = f.FileId,
                FileName = f.FileName,
                FilePath = $"mock://files/{f.FileId}",
                FileSizeBytes = f.Data.Length,
                ContentType = f.ContentType,
                CreatedAt = f.UploadedAt,
                LastModified = f.UploadedAt
            })
            .Take(options?.PageSize ?? 100)
            .ToArray();

        var result = new StorageListResult
        {
            Files = files,
            TotalCount = files.Length,
            HasMoreResults = false,
            DirectoryPath = directoryPath ?? "/"
        };

        return Task.FromResult(result);
    }

    public Task<StorageDirectoryResult> CreateDirectoryAsync(string directoryPath, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Creating directory {DirectoryPath}", directoryPath);

        var result = new StorageDirectoryResult
        {
            DirectoryPath = directoryPath,
            Success = true,
            Timestamp = DateTime.UtcNow
        };

        return Task.FromResult(result);
    }

    public Task<StorageDirectoryResult> DeleteDirectoryAsync(string directoryPath, bool recursive = false, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Deleting directory {DirectoryPath}", directoryPath);

        var result = new StorageDirectoryResult
        {
            DirectoryPath = directoryPath,
            Success = true,
            Timestamp = DateTime.UtcNow
        };

        return Task.FromResult(result);
    }

    public Task<StorageBulkUploadResult> BulkUploadAsync(Dictionary<string, Stream> files, StorageBulkOptions? options = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Bulk uploading {Count} files", files.Count);

        var results = new Dictionary<string, StorageUploadResult>();
        var totalBytes = 0L;

        foreach (var (fileName, stream) in files)
        {
            var uploadResult = UploadFileAsync(stream, fileName, "application/octet-stream", null, cancellationToken).Result;
            results[fileName] = uploadResult;
            totalBytes += uploadResult.FileSizeBytes;
        }

        var result = new StorageBulkUploadResult
        {
            TotalFiles = files.Count,
            SuccessfulUploads = files.Count,
            FailedUploads = 0,
            Results = results,
            TotalBytesUploaded = totalBytes,
            Duration = TimeSpan.FromMilliseconds(files.Count * 10)
        };

        return Task.FromResult(result);
    }

    public Task<StorageBulkDeletionResult> BulkDeleteAsync(string[] fileIds, StorageBulkOptions? options = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Bulk deleting {Count} files", fileIds.Length);

        var results = new Dictionary<string, StorageDeletionResult>();

        foreach (var fileId in fileIds)
        {
            var deleteResult = DeleteFileAsync(fileId, null, cancellationToken).Result;
            results[fileId] = deleteResult;
        }

        var result = new StorageBulkDeletionResult
        {
            TotalFiles = fileIds.Length,
            SuccessfulDeletions = results.Values.Count(r => r.Success),
            FailedDeletions = results.Values.Count(r => !r.Success),
            Results = results,
            Duration = TimeSpan.FromMilliseconds(fileIds.Length * 5)
        };

        return Task.FromResult(result);
    }

    public Task<StorageSignedUrlResult> GenerateSignedUrlAsync(string fileId, TimeSpan expiration, StoragePermissions permissions = StoragePermissions.Read, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Generating signed URL for file {FileId}", fileId);

        var result = new StorageSignedUrlResult
        {
            SignedUrl = $"https://mock-storage.com/signed/{fileId}?expires={DateTimeOffset.UtcNow.Add(expiration).ToUnixTimeSeconds()}",
            ExpiresAt = DateTime.UtcNow.Add(expiration),
            Permissions = permissions,
            Success = _files.ContainsKey(fileId),
            ErrorMessage = _files.ContainsKey(fileId) ? null : "File not found"
        };

        return Task.FromResult(result);
    }

    public Task<StoragePermissionResult> SetPermissionsAsync(string fileId, StorageAccessControl permissions, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Setting permissions for file {FileId}", fileId);

        var result = new StoragePermissionResult
        {
            FileId = fileId,
            AccessControl = permissions,
            Success = _files.ContainsKey(fileId),
            UpdatedAt = DateTime.UtcNow,
            ErrorMessage = _files.ContainsKey(fileId) ? null : "File not found"
        };

        return Task.FromResult(result);
    }

    public Task<StorageAnalyticsResult> GetStorageAnalyticsAsync(TimeRange? timeRange = null, StorageGroupBy groupBy = StorageGroupBy.Day, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Getting storage analytics");

        var result = new StorageAnalyticsResult
        {
            TimeRange = timeRange ?? new TimeRange { StartTime = DateTime.UtcNow.AddDays(-30), EndTime = DateTime.UtcNow },
            TotalStorageBytes = _files.Values.Sum(f => f.Data.Length),
            TotalFileCount = _files.Count,
            BandwidthStats = new BandwidthStatistics
            {
                TotalUploadBytes = _files.Values.Sum(f => f.Data.Length),
                TotalDownloadBytes = _files.Values.Sum(f => f.Data.Length) * 3 // Mock download activity
            }
        };

        return Task.FromResult(result);
    }

    public Task<StorageHealthResult> GetStorageHealthAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Getting storage health");

        var result = new StorageHealthResult
        {
            Status = Core.Models.Storage.HealthStatus.Healthy,
            ResponseTime = TimeSpan.FromMilliseconds(10),
            UsedCapacityBytes = _files.Values.Sum(f => f.Data.Length),
            CheckedAt = DateTime.UtcNow
        };

        return Task.FromResult(result);
    }

    public StorageProviderInfo GetProviderInfo()
    {
        return new StorageProviderInfo
        {
            Name = "MockStorage",
            Version = "1.0.0",
            SupportedFeatures = new StorageFeatures
            {
                SupportsVersioning = false,
                SupportsEncryption = false,
                SupportsACL = true,
                SupportsSignedUrls = true,
                SupportsBulkOperations = true,
                SupportsRangeRequests = false,
                SupportsAnalytics = true
            },
            MaxFileSizeBytes = long.MaxValue
        };
    }

    public Task<StorageConnectivityResult> TestConnectivityAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Testing connectivity");

        var result = new StorageConnectivityResult
        {
            Success = true,
            ResponseTime = TimeSpan.FromMilliseconds(5),
            TestedAt = DateTime.UtcNow,
            Endpoint = "mock://storage"
        };

        return Task.FromResult(result);
    }

    private class MockFileData
    {
        public string FileId { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public byte[] Data { get; set; } = Array.Empty<byte>();
        public DateTime UploadedAt { get; set; }
    }
}