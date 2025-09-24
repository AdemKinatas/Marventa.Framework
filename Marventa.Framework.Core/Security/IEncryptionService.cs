using System.Threading;
using System.Threading.Tasks;

namespace Marventa.Framework.Core.Security;

public interface IEncryptionService
{
    Task<string> EncryptAsync(string plainText, CancellationToken cancellationToken = default);
    Task<string> DecryptAsync(string encryptedText, CancellationToken cancellationToken = default);
    Task<string> HashAsync(string input, CancellationToken cancellationToken = default);
    Task<bool> VerifyHashAsync(string input, string hash, CancellationToken cancellationToken = default);
    Task<string> GenerateSaltAsync(CancellationToken cancellationToken = default);
}