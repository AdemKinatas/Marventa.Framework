using Amazon.S3;
using Amazon.S3.Model;
using Marventa.Framework.Storage.Abstractions;

namespace Marventa.Framework.Storage.AWS;

public class S3Storage : IStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public S3Storage(IAmazonS3 s3Client, string bucketName)
    {
        _s3Client = s3Client ?? throw new ArgumentNullException(nameof(s3Client));
        _bucketName = bucketName;
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string? contentType = null, CancellationToken cancellationToken = default)
    {
        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName,
            InputStream = fileStream,
            ContentType = contentType ?? "application/octet-stream"
        };

        await _s3Client.PutObjectAsync(request, cancellationToken);
        return $"https://{_bucketName}.s3.amazonaws.com/{fileName}";
    }

    public async Task<Stream> DownloadAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var request = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName
        };

        var response = await _s3Client.GetObjectAsync(request, cancellationToken);
        return response.ResponseStream;
    }

    public async Task<bool> DeleteAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName
        };

        await _s3Client.DeleteObjectAsync(request, cancellationToken);
        return true;
    }

    public async Task<bool> ExistsAsync(string fileName, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = _bucketName,
                Key = fileName
            };

            await _s3Client.GetObjectMetadataAsync(request, cancellationToken);
            return true;
        }
        catch (AmazonS3Exception)
        {
            return false;
        }
    }

    public Task<string> GetUrlAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var url = $"https://{_bucketName}.s3.amazonaws.com/{fileName}";
        return Task.FromResult(url);
    }
}
