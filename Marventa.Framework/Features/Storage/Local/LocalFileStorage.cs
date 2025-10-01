using Marventa.Framework.Features.Storage.Abstractions;

namespace Marventa.Framework.Features.Storage.Local;

public class LocalFileStorage : IStorageService
{
    private readonly string _basePath;
    private readonly string _baseUrl;

    public LocalFileStorage(string basePath, string? baseUrl = null)
    {
        _basePath = basePath ?? throw new ArgumentNullException(nameof(basePath));
        _baseUrl = baseUrl ?? string.Empty;

        // Create directory if it doesn't exist
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string? contentType = null, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_basePath, fileName);
        var directory = Path.GetDirectoryName(filePath);

        // Create subdirectories if needed
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var fileStreamOutput = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await fileStream.CopyToAsync(fileStreamOutput, cancellationToken);

        return fileName;
    }

    public async Task<Stream> DownloadAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_basePath, fileName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {fileName}", fileName);
        }

        var memoryStream = new MemoryStream();
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        await fileStream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        return memoryStream;
    }

    public Task<bool> DeleteAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_basePath, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public Task<bool> ExistsAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_basePath, fileName);
        return Task.FromResult(File.Exists(filePath));
    }

    public Task<string> GetUrlAsync(string fileName, CancellationToken cancellationToken = default)
    {
        // If base URL is provided, return full URL
        if (!string.IsNullOrEmpty(_baseUrl))
        {
            return Task.FromResult($"{_baseUrl.TrimEnd('/')}/{fileName}");
        }

        // Otherwise return local file path
        var filePath = Path.Combine(_basePath, fileName);
        return Task.FromResult(filePath);
    }
}
