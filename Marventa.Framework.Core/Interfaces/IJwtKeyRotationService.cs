namespace Marventa.Framework.Core.Interfaces;

public interface IJwtKeyRotationService
{
    Task<string> GetCurrentSigningKeyAsync();
    Task<IEnumerable<string>> GetValidationKeysAsync();
    Task RotateKeysAsync();
    Task<bool> IsKeyValidAsync(string keyId);
}

public interface IJwtKeyStore
{
    Task<JwtKey?> GetCurrentKeyAsync();
    Task<IEnumerable<JwtKey>> GetValidKeysAsync();
    Task StoreKeyAsync(JwtKey key);
    Task DeactivateKeyAsync(string keyId);
    Task CleanupExpiredKeysAsync();
}

public class JwtKey
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Key { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string Algorithm { get; set; } = "HS256";
}