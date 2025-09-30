namespace Marventa.Framework.Storage.Abstractions;

public interface IStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string? contentType = null, CancellationToken cancellationToken = default);
    Task<Stream> DownloadAsync(string fileName, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string fileName, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string fileName, CancellationToken cancellationToken = default);
    Task<string> GetUrlAsync(string fileName, CancellationToken cancellationToken = default);
}
