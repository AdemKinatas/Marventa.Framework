namespace Marventa.Framework.Core.Interfaces;

public interface IStorageService
{
    Task<StorageFile> UploadAsync(Stream fileStream, string fileName, string? folder = null, CancellationToken cancellationToken = default);
    Task<StorageFile> UploadAsync(byte[] fileContent, string fileName, string? folder = null, CancellationToken cancellationToken = default);
    Task<Stream> DownloadAsync(string fileKey, CancellationToken cancellationToken = default);
    Task<byte[]> DownloadBytesAsync(string fileKey, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string fileKey, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string fileKey, CancellationToken cancellationToken = default);
    Task<StorageFile> GetFileInfoAsync(string fileKey, CancellationToken cancellationToken = default);
    Task<IEnumerable<StorageFile>> ListFilesAsync(string? prefix = null, int maxResults = 100, CancellationToken cancellationToken = default);
    Task<string> GetPresignedUrlAsync(string fileKey, TimeSpan expiration, CancellationToken cancellationToken = default);
    Task<bool> CopyAsync(string sourceKey, string destinationKey, CancellationToken cancellationToken = default);
    Task<bool> MoveAsync(string sourceKey, string destinationKey, CancellationToken cancellationToken = default);
}

public class StorageFile
{
    public string Key { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ETag { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
    public string Url { get; set; } = string.Empty;
}