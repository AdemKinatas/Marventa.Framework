using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Marventa.Framework.Core.Interfaces.Security;
using Microsoft.Extensions.Configuration;

namespace Marventa.Framework.Infrastructure.Services.Security;

public class EncryptionService : IEncryptionService
{
    private readonly string _encryptionKey;

    public EncryptionService(IConfiguration configuration)
    {
        _encryptionKey = configuration["Encryption:Key"] ?? throw new ArgumentNullException("Encryption Key is required");
    }

    public Task<string> EncryptAsync(string plainText, CancellationToken cancellationToken = default)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(_encryptionKey.PadRight(32)[..32]);
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        var encryptedBytes = encryptor.TransformFinalBlock(plainTextBytes, 0, plainTextBytes.Length);

        var result = Convert.ToBase64String(aes.IV.Concat(encryptedBytes).ToArray());
        return Task.FromResult(result);
    }

    public Task<string> DecryptAsync(string encryptedText, CancellationToken cancellationToken = default)
    {
        var encryptedBytes = Convert.FromBase64String(encryptedText);

        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(_encryptionKey.PadRight(32)[..32]);

        var iv = encryptedBytes[..16];
        var encrypted = encryptedBytes[16..];

        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        var decryptedBytes = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);

        var result = Encoding.UTF8.GetString(decryptedBytes);
        return Task.FromResult(result);
    }

    public Task<string> HashAsync(string input, CancellationToken cancellationToken = default)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        var result = Convert.ToBase64String(hashedBytes);
        return Task.FromResult(result);
    }

    public Task<bool> VerifyHashAsync(string input, string hash, CancellationToken cancellationToken = default)
    {
        var inputHash = HashAsync(input, cancellationToken).Result;
        var result = string.Equals(inputHash, hash, StringComparison.Ordinal);
        return Task.FromResult(result);
    }

    public Task<string> GenerateSaltAsync(CancellationToken cancellationToken = default)
    {
        var saltBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);
        var result = Convert.ToBase64String(saltBytes);
        return Task.FromResult(result);
    }
}