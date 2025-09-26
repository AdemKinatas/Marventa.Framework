using Marventa.Framework.Core.Interfaces;
using System.Collections.Concurrent;

namespace Marventa.Framework.Infrastructure.Security;

public class InMemoryJwtKeyStore : IJwtKeyStore
{
    private readonly ConcurrentDictionary<string, JwtKey> _keys = new();

    public Task<JwtKey?> GetCurrentKeyAsync()
    {
        var currentKey = _keys.Values
            .Where(k => k.IsActive && k.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(k => k.CreatedAt)
            .FirstOrDefault();

        return Task.FromResult(currentKey);
    }

    public Task<IEnumerable<JwtKey>> GetValidKeysAsync()
    {
        var validKeys = _keys.Values
            .Where(k => k.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(k => k.CreatedAt)
            .AsEnumerable();

        return Task.FromResult(validKeys);
    }

    public Task StoreKeyAsync(JwtKey key)
    {
        _keys.TryAdd(key.Id, key);
        return Task.CompletedTask;
    }

    public Task DeactivateKeyAsync(string keyId)
    {
        if (_keys.TryGetValue(keyId, out var key))
        {
            key.IsActive = false;
        }
        return Task.CompletedTask;
    }

    public Task CleanupExpiredKeysAsync()
    {
        var expiredKeys = _keys.Values
            .Where(k => k.ExpiresAt <= DateTime.UtcNow)
            .Select(k => k.Id)
            .ToList();

        foreach (var keyId in expiredKeys)
        {
            _keys.TryRemove(keyId, out _);
        }

        return Task.CompletedTask;
    }
}