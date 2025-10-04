using System.Security.Cryptography;
using System.Text;

namespace Marventa.Framework.Security.Encryption;

/// <summary>
/// Secure AES-GCM encryption service with authenticated encryption.
/// Uses random nonce for each encryption operation.
/// </summary>
public class AesEncryption
{
    private readonly byte[] _key;
    private const int NonceSize = 12; // AES-GCM standard nonce size
    private const int TagSize = 16; // AES-GCM authentication tag size

    public AesEncryption(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Encryption key cannot be null or empty", nameof(key));
        }

        // Use PBKDF2 for key derivation from master key
        using var pbkdf2 = new Rfc2898DeriveBytes(
            key,
            Encoding.UTF8.GetBytes("Marventa-Salt-V1"), // Salt should be from config
            100000,
            HashAlgorithmName.SHA256);

        _key = pbkdf2.GetBytes(32); // 256-bit key for AES-256
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
        {
            throw new ArgumentException("Plain text cannot be null or empty", nameof(plainText));
        }

        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var nonce = new byte[NonceSize];
        var cipherText = new byte[plainBytes.Length];
        var tag = new byte[TagSize];

        // Generate random nonce for each encryption
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(nonce);

        // Encrypt with AES-GCM (provides both confidentiality and authenticity)
        using var aesGcm = new AesGcm(_key, TagSize);
        aesGcm.Encrypt(nonce, plainBytes, cipherText, tag);

        // Format: nonce + tag + ciphertext
        var result = new byte[NonceSize + TagSize + cipherText.Length];
        Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
        Buffer.BlockCopy(tag, 0, result, NonceSize, TagSize);
        Buffer.BlockCopy(cipherText, 0, result, NonceSize + TagSize, cipherText.Length);

        return Convert.ToBase64String(result);
    }

    public string Decrypt(string encryptedText)
    {
        if (string.IsNullOrEmpty(encryptedText))
        {
            throw new ArgumentException("Encrypted text cannot be null or empty", nameof(encryptedText));
        }

        byte[] data;
        try
        {
            data = Convert.FromBase64String(encryptedText);
        }
        catch (FormatException ex)
        {
            throw new CryptographicException("Invalid encrypted text format", ex);
        }

        if (data.Length < NonceSize + TagSize)
        {
            throw new CryptographicException("Encrypted text is too short");
        }

        // Extract nonce, tag, and ciphertext
        var nonce = new byte[NonceSize];
        var tag = new byte[TagSize];
        var cipherText = new byte[data.Length - NonceSize - TagSize];

        Buffer.BlockCopy(data, 0, nonce, 0, NonceSize);
        Buffer.BlockCopy(data, NonceSize, tag, 0, TagSize);
        Buffer.BlockCopy(data, NonceSize + TagSize, cipherText, 0, cipherText.Length);

        var plainText = new byte[cipherText.Length];

        // Decrypt and verify authentication tag
        using var aesGcm = new AesGcm(_key, TagSize);
        try
        {
            aesGcm.Decrypt(nonce, cipherText, tag, plainText);
        }
        catch (CryptographicException ex)
        {
            throw new CryptographicException("Decryption failed. Data may be corrupted or tampered.", ex);
        }

        return Encoding.UTF8.GetString(plainText);
    }
}
