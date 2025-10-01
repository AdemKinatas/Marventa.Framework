namespace Marventa.Framework.Features.Caching.Distributed;

public class RedisCacheConfiguration
{
    public string ConnectionString { get; set; } = "localhost:6379";
    public string InstanceName { get; set; } = "Marventa:";
    public int DefaultExpirationMinutes { get; set; } = 30;
}
