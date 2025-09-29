using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Marventa.Framework.Core.Interfaces.Caching;
using Marventa.Framework.Core.Interfaces.MultiTenancy;

namespace Marventa.Framework.Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _database;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly RedisCacheOptions _options;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(
        IConnectionMultiplexer connectionMultiplexer,
        ITenantContext tenantContext,
        ILogger<RedisCacheService> logger,
        IOptions<RedisCacheOptions> options)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _database = connectionMultiplexer.GetDatabase();
        _tenantContext = tenantContext;
        _logger = logger;
        _options = options.Value;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var fullKey = BuildKey(key);
            var value = await _database.StringGetAsync(fullKey);

            if (!value.HasValue)
            {
                _logger.LogDebug("Cache miss for key: {Key}", fullKey);
                return default;
            }

            _logger.LogDebug("Cache hit for key: {Key}", fullKey);
            return JsonSerializer.Deserialize<T>(value!, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache value for key: {Key}", key);

            if (_options.ThrowOnError)
                throw;

            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var fullKey = BuildKey(key);
            var serialized = JsonSerializer.Serialize(value, _jsonOptions);
            var finalExpiration = expiration ?? _options.DefaultExpiration;

            await _database.StringSetAsync(fullKey, serialized, finalExpiration);

            _logger.LogDebug("Cache set for key: {Key} with expiration: {Expiration}", fullKey, finalExpiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache value for key: {Key}", key);

            if (_options.ThrowOnError)
                throw;
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var fullKey = BuildKey(key);
            await _database.KeyDeleteAsync(fullKey);

            _logger.LogDebug("Cache removed for key: {Key}", fullKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache value for key: {Key}", key);

            if (_options.ThrowOnError)
                throw;
        }
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        try
        {
            var fullPattern = BuildKey(pattern);
            var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints()[0]);
            var keys = server.Keys(pattern: fullPattern).ToArray();

            if (keys.Length > 0)
            {
                await _database.KeyDeleteAsync(keys);
                _logger.LogDebug("Removed {Count} keys matching pattern: {Pattern}", keys.Length, fullPattern);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache values by pattern: {Pattern}", pattern);

            if (_options.ThrowOnError)
                throw;
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var fullKey = BuildKey(key);
            return await _database.KeyExistsAsync(fullKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking cache existence for key: {Key}", key);

            if (_options.ThrowOnError)
                throw;

            return false;
        }
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_options.AllowFlushDatabase)
            {
                _logger.LogWarning("Clear operation blocked. Set AllowFlushDatabase to true in configuration.");
                return;
            }

            if (_tenantContext.CurrentTenant != null)
            {
                // Clear only tenant-specific keys
                var pattern = $"{_options.KeyPrefix}:{_tenantContext.CurrentTenant.Id}:*";
                await RemoveByPatternAsync(pattern, cancellationToken);
                _logger.LogInformation("Cleared cache for tenant: {TenantId}", _tenantContext.CurrentTenant.Id);
            }
            else
            {
                // Clear all keys with prefix (dangerous in production!)
                var pattern = $"{_options.KeyPrefix}:*";
                await RemoveByPatternAsync(pattern, cancellationToken);
                _logger.LogWarning("Cleared all cache with prefix: {Prefix}", _options.KeyPrefix);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cache");

            if (_options.ThrowOnError)
                throw;
        }
    }

    private string BuildKey(string key)
    {
        var parts = new List<string>();

        // Add prefix
        if (!string.IsNullOrWhiteSpace(_options.KeyPrefix))
            parts.Add(_options.KeyPrefix);

        // Add tenant if multi-tenant mode
        if (_options.EnableMultiTenancy && _tenantContext.CurrentTenant != null)
            parts.Add(_tenantContext.CurrentTenant.Id);

        // Add the actual key
        parts.Add(key);

        return string.Join(":", parts);
    }
}