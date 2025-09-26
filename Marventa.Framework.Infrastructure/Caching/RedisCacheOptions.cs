namespace Marventa.Framework.Infrastructure.Caching;

public class RedisCacheOptions
{
    public string KeyPrefix { get; set; } = "cache";
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(5);
    public bool EnableMultiTenancy { get; set; } = true;
    public bool ThrowOnError { get; set; } = false;
    public bool AllowFlushDatabase { get; set; } = false;
    public string ConnectionString { get; set; } = "localhost:6379";
    public int Database { get; set; } = 0;
}