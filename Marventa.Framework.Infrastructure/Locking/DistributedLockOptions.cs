using System;

namespace Marventa.Framework.Infrastructure.Locking;

public class DistributedLockOptions
{
    public const string SectionName = "DistributedLock";
    public TimeSpan DefaultExpiry { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan WaitTime { get; set; } = TimeSpan.FromSeconds(10);
    public TimeSpan RetryTime { get; set; } = TimeSpan.FromMilliseconds(200);
    public string RedisConnectionString { get; set; } = "localhost:6379";
}