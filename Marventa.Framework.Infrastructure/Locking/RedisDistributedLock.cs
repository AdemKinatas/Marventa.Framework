using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using Marventa.Framework.Core.Interfaces;

namespace Marventa.Framework.Infrastructure.Locking;

public class RedisDistributedLock : IDistributedLock, IDisposable
{
    private readonly RedLockFactory _redLockFactory;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<RedisDistributedLock> _logger;
    private readonly DistributedLockOptions _options;

    public RedisDistributedLock(
        IConnectionMultiplexer redis,
        ITenantContext tenantContext,
        ILogger<RedisDistributedLock> logger,
        IOptions<DistributedLockOptions> options)
    {
        _tenantContext = tenantContext;
        _logger = logger;
        _options = options.Value;

        var redLockConfig = new List<RedLockMultiplexer>
        {
            new RedLockMultiplexer(redis)
        };

        _redLockFactory = RedLockFactory.Create(redLockConfig);
    }

    public async Task<ILockHandle?> AcquireAsync(string resource, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        var tenantResource = GetTenantScopedResource(resource);
        var lockExpiry = expiry ?? _options.DefaultExpiry;

        try
        {
            var redLock = await _redLockFactory.CreateLockAsync(
                tenantResource,
                lockExpiry,
                _options.WaitTime,
                _options.RetryTime,
                cancellationToken);

            if (redLock.IsAcquired)
            {
                _logger.LogDebug("Acquired lock for resource {Resource} with ID {LockId}", tenantResource, redLock.LockId);
                return new RedisLockHandle(redLock, tenantResource, _logger);
            }

            _logger.LogWarning("Failed to acquire lock for resource {Resource}", tenantResource);
            redLock.Dispose();
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error acquiring lock for resource {Resource}", tenantResource);
            throw;
        }
    }

    public async Task<T> ExecuteWithLockAsync<T>(string resource, Func<Task<T>> action, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        await using var lockHandle = await AcquireAsync(resource, expiry, cancellationToken);

        if (lockHandle == null || !lockHandle.IsAcquired)
        {
            throw new InvalidOperationException($"Could not acquire lock for resource: {resource}");
        }

        try
        {
            return await action();
        }
        finally
        {
            await lockHandle.ReleaseAsync();
        }
    }

    public async Task ExecuteWithLockAsync(string resource, Func<Task> action, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        await ExecuteWithLockAsync(resource, async () =>
        {
            await action();
            return Task.CompletedTask;
        }, expiry, cancellationToken);
    }

    private string GetTenantScopedResource(string resource)
    {
        var tenantId = _tenantContext.TenantId ?? "global";
        return $"lock:{tenantId}:{resource}";
    }

    public void Dispose()
    {
        _redLockFactory?.Dispose();
    }
}

public class RedisLockHandle : ILockHandle
{
    private readonly IRedLock _redLock;
    private readonly ILogger _logger;
    private bool _disposed;

    public RedisLockHandle(IRedLock redLock, string resource, ILogger logger)
    {
        _redLock = redLock;
        Resource = resource;
        _logger = logger;
    }

    public string Resource { get; }
    public string LockId => _redLock.LockId;
    public bool IsAcquired => _redLock.IsAcquired;

    public Task ReleaseAsync()
    {
        if (!_disposed)
        {
            _logger.LogDebug("Releasing lock for resource {Resource} with ID {LockId}", Resource, LockId);
            _redLock.Dispose();
            _disposed = true;
        }

        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await ReleaseAsync();
    }
}