namespace Marventa.Framework.Core.Interfaces.Security;

public interface IJwtKeyStore
{
    Task<JwtKey?> GetCurrentKeyAsync();
    Task<IEnumerable<JwtKey>> GetValidKeysAsync();
    Task StoreKeyAsync(JwtKey key);
    Task DeactivateKeyAsync(string keyId);
    Task CleanupExpiredKeysAsync();
}