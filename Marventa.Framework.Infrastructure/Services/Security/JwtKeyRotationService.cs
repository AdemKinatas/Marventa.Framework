using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Marventa.Framework.Core.Interfaces.Security;
using System.Security.Cryptography;
using System.Text;

namespace Marventa.Framework.Infrastructure.Services.Security;

public class JwtKeyRotationService : IJwtKeyRotationService
{
    private readonly IJwtKeyStore _keyStore;
    private readonly JwtKeyRotationOptions _options;
    private readonly ILogger<JwtKeyRotationService> _logger;

    public JwtKeyRotationService(
        IJwtKeyStore keyStore,
        IOptions<JwtKeyRotationOptions> options,
        ILogger<JwtKeyRotationService> logger)
    {
        _keyStore = keyStore;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> GetCurrentSigningKeyAsync()
    {
        var currentKey = await _keyStore.GetCurrentKeyAsync();

        if (currentKey == null || currentKey.ExpiresAt <= DateTime.UtcNow)
        {
            _logger.LogWarning("No valid signing key found, rotating keys");
            await RotateKeysAsync();
            currentKey = await _keyStore.GetCurrentKeyAsync();
        }

        return currentKey?.Key ?? throw new InvalidOperationException("No signing key available");
    }

    public async Task<IEnumerable<string>> GetValidationKeysAsync()
    {
        var validKeys = await _keyStore.GetValidKeysAsync();
        return validKeys.Where(k => k.ExpiresAt > DateTime.UtcNow).Select(k => k.Key);
    }

    public async Task RotateKeysAsync()
    {
        _logger.LogInformation("Starting JWT key rotation");

        try
        {
            // Generate new key
            var newKey = GenerateNewKey();
            await _keyStore.StoreKeyAsync(newKey);

            // Deactivate old keys that are past their active period
            var validKeys = await _keyStore.GetValidKeysAsync();
            var oldKeys = validKeys.Where(k => k.CreatedAt < DateTime.UtcNow.Subtract(_options.ActiveKeyLifetime));

            foreach (var oldKey in oldKeys)
            {
                await _keyStore.DeactivateKeyAsync(oldKey.Id);
                _logger.LogInformation("Deactivated key {KeyId}", oldKey.Id);
            }

            // Cleanup expired keys
            await _keyStore.CleanupExpiredKeysAsync();

            _logger.LogInformation("JWT key rotation completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during JWT key rotation");
            throw;
        }
    }

    public async Task<bool> IsKeyValidAsync(string keyId)
    {
        var validKeys = await _keyStore.GetValidKeysAsync();
        return validKeys.Any(k => k.Id == keyId && k.ExpiresAt > DateTime.UtcNow);
    }

    private JwtKey GenerateNewKey()
    {
        var key = new byte[64]; // 512-bit key for HS512
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(key);
        }

        return new JwtKey
        {
            Id = Guid.NewGuid().ToString(),
            Key = Convert.ToBase64String(key),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.Add(_options.KeyValidityPeriod),
            IsActive = true,
            Algorithm = _options.Algorithm
        };
    }
}