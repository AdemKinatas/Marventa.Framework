namespace Marventa.Framework.Core.Interfaces.Security;

public interface IJwtKeyRotationService
{
    Task<string> GetCurrentSigningKeyAsync();
    Task<IEnumerable<string>> GetValidationKeysAsync();
    Task RotateKeysAsync();
    Task<bool> IsKeyValidAsync(string keyId);
}