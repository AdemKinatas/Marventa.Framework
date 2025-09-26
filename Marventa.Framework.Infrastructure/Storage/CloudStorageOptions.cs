namespace Marventa.Framework.Infrastructure.Storage;

public class CloudStorageOptions
{
    public string Provider { get; set; } = "S3"; // S3, Azure, GCS
    public string BucketName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty;
    public bool UseSSL { get; set; } = true;
}