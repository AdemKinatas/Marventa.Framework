using Marventa.Framework.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Infrastructure.Storage;

public class LocalFileStorageService : IStorageService
{
    private readonly ILogger<LocalFileStorageService> _logger;
    private readonly string _basePath;

    public LocalFileStorageService(ILogger<LocalFileStorageService> logger, string basePath = "uploads")
    {
        _logger = logger;
        _basePath = Path.GetFullPath(basePath);

        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    public async Task<StorageFile> UploadAsync(Stream fileStream, string fileName, string? folder = null, CancellationToken cancellationToken = default)
    {
        var key = GenerateFileKey(fileName, folder);
        var fullPath = Path.Combine(_basePath, key);
        var directory = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        _logger.LogInformation("Uploading file: {FileName} to {Path}", fileName, fullPath);

        using var fileStream2 = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        await fileStream.CopyToAsync(fileStream2, cancellationToken);

        var fileInfo = new FileInfo(fullPath);

        return new StorageFile
        {
            Key = key,
            FileName = fileName,
            ContentType = GetContentType(fileName),
            Size = fileInfo.Length,
            CreatedAt = fileInfo.CreationTimeUtc,
            ModifiedAt = fileInfo.LastWriteTimeUtc,
            Url = Path.Combine(_basePath, key).Replace('\\', '/')
        };
    }

    public async Task<StorageFile> UploadAsync(byte[] fileContent, string fileName, string? folder = null, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(fileContent);
        return await UploadAsync(stream, fileName, folder, cancellationToken);
    }

    public async Task<Stream> DownloadAsync(string fileKey, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, fileKey);
        _logger.LogInformation("Downloading file: {FileKey} from {Path}", fileKey, fullPath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"File not found: {fileKey}");
        }

        var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        return await Task.FromResult(fileStream);
    }

    public async Task<byte[]> DownloadBytesAsync(string fileKey, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, fileKey);
        _logger.LogInformation("Downloading file bytes: {FileKey} from {Path}", fileKey, fullPath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"File not found: {fileKey}");
        }

        return await File.ReadAllBytesAsync(fullPath, cancellationToken);
    }

    public async Task<bool> DeleteAsync(string fileKey, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, fileKey);
        _logger.LogInformation("Deleting file: {FileKey} from {Path}", fileKey, fullPath);

        if (!File.Exists(fullPath))
        {
            return false;
        }

        try
        {
            File.Delete(fullPath);
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file: {FileKey}", fileKey);
            return false;
        }
    }

    public async Task<bool> ExistsAsync(string fileKey, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, fileKey);
        return await Task.FromResult(File.Exists(fullPath));
    }

    public async Task<StorageFile> GetFileInfoAsync(string fileKey, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, fileKey);
        _logger.LogInformation("Getting info for file: {FileKey} from {Path}", fileKey, fullPath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"File not found: {fileKey}");
        }

        var fileInfo = new FileInfo(fullPath);

        return await Task.FromResult(new StorageFile
        {
            Key = fileKey,
            FileName = Path.GetFileName(fileKey),
            ContentType = GetContentType(fileKey),
            Size = fileInfo.Length,
            CreatedAt = fileInfo.CreationTimeUtc,
            ModifiedAt = fileInfo.LastWriteTimeUtc,
            Url = fullPath.Replace('\\', '/')
        });
    }

    public async Task<IEnumerable<StorageFile>> ListFilesAsync(string? prefix = null, int maxResults = 100, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Listing files with prefix: {Prefix}", prefix);

        var searchPath = string.IsNullOrEmpty(prefix) ? _basePath : Path.Combine(_basePath, prefix);
        var searchPattern = "*";

        if (!Directory.Exists(searchPath))
        {
            return await Task.FromResult(Enumerable.Empty<StorageFile>());
        }

        var files = Directory.GetFiles(searchPath, searchPattern, SearchOption.AllDirectories)
            .Take(maxResults)
            .Select(filePath =>
            {
                var fileInfo = new FileInfo(filePath);
                var relativeKey = Path.GetRelativePath(_basePath, filePath).Replace('\\', '/');

                return new StorageFile
                {
                    Key = relativeKey,
                    FileName = fileInfo.Name,
                    ContentType = GetContentType(fileInfo.Name),
                    Size = fileInfo.Length,
                    CreatedAt = fileInfo.CreationTimeUtc,
                    ModifiedAt = fileInfo.LastWriteTimeUtc,
                    Url = filePath.Replace('\\', '/')
                };
            });

        return await Task.FromResult(files);
    }

    public async Task<string> GetPresignedUrlAsync(string fileKey, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating presigned URL for: {FileKey}", fileKey);
        // For local files, we just return the file path with an expiration token
        var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{fileKey}:{DateTimeOffset.UtcNow.Add(expiration).ToUnixTimeSeconds()}"));
        return await Task.FromResult($"file:///{Path.Combine(_basePath, fileKey).Replace('\\', '/')}?token={token}");
    }

    public async Task<bool> CopyAsync(string sourceKey, string destinationKey, CancellationToken cancellationToken = default)
    {
        var sourcePath = Path.Combine(_basePath, sourceKey);
        var destPath = Path.Combine(_basePath, destinationKey);

        _logger.LogInformation("Copying file from {Source} to {Destination}", sourcePath, destPath);

        if (!File.Exists(sourcePath))
        {
            return false;
        }

        var destDir = Path.GetDirectoryName(destPath);
        if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
        {
            Directory.CreateDirectory(destDir);
        }

        try
        {
            File.Copy(sourcePath, destPath, true);
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to copy file from {Source} to {Destination}", sourceKey, destinationKey);
            return false;
        }
    }

    public async Task<bool> MoveAsync(string sourceKey, string destinationKey, CancellationToken cancellationToken = default)
    {
        var sourcePath = Path.Combine(_basePath, sourceKey);
        var destPath = Path.Combine(_basePath, destinationKey);

        _logger.LogInformation("Moving file from {Source} to {Destination}", sourcePath, destPath);

        if (!File.Exists(sourcePath))
        {
            return false;
        }

        var destDir = Path.GetDirectoryName(destPath);
        if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
        {
            Directory.CreateDirectory(destDir);
        }

        try
        {
            File.Move(sourcePath, destPath, true);
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to move file from {Source} to {Destination}", sourceKey, destinationKey);
            return false;
        }
    }

    private string GenerateFileKey(string fileName, string? folder)
    {
        var key = string.IsNullOrEmpty(folder) ? fileName : Path.Combine(folder, fileName);
        return key.Replace('\\', '/');
    }

    private string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".pdf" => "application/pdf",
            ".doc" or ".docx" => "application/msword",
            ".xls" or ".xlsx" => "application/vnd.ms-excel",
            ".txt" => "text/plain",
            ".json" => "application/json",
            ".xml" => "application/xml",
            ".zip" => "application/zip",
            _ => "application/octet-stream"
        };
    }
}