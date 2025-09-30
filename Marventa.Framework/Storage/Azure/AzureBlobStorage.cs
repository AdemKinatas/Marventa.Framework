using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Marventa.Framework.Storage.Abstractions;

namespace Marventa.Framework.Storage.Azure;

public class AzureBlobStorage : IStorageService
{
    private readonly BlobContainerClient _containerClient;

    public AzureBlobStorage(string connectionString, string containerName)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);
        _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        _containerClient.CreateIfNotExists();
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string? contentType = null, CancellationToken cancellationToken = default)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);

        var options = new BlobUploadOptions();
        if (!string.IsNullOrEmpty(contentType))
        {
            options.HttpHeaders = new BlobHttpHeaders { ContentType = contentType };
        }

        await blobClient.UploadAsync(fileStream, options, cancellationToken);
        return blobClient.Uri.ToString();
    }

    public async Task<Stream> DownloadAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);
        var response = await blobClient.DownloadAsync(cancellationToken);
        return response.Value.Content;
    }

    public async Task<bool> DeleteAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);
        var response = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        return response.Value;
    }

    public async Task<bool> ExistsAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);
        return await blobClient.ExistsAsync(cancellationToken);
    }

    public Task<string> GetUrlAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);
        return Task.FromResult(blobClient.Uri.ToString());
    }
}
