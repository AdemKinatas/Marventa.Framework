using Konscious.Security.Cryptography;
using Marventa.Framework.Security.Encryption.Abstractions;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Marventa.Framework.Security.Encryption.Services;

/// <summary>
/// Provides password hashing, verification, and validation services using Argon2id.
/// Falls back to BCrypt for verifying legacy hashes.
/// </summary>
public class PasswordService : IPasswordService
{
    // Argon2 configuration (OWASP recommendations)
    private const int DegreeOfParallelism = 1;
    private const int Iterations = 2;
    private const int MemorySize = 19456; // 19 MB
    private const int HashSize = 32; // 256 bits
    private const int SaltSize = 16; // 128 bits

    private const string SpecialCharacters = "!@#$%^&*()_+-=[]{}|;:,.<>?/~`'\"";

    /// <inheritdoc/>
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));
        }

        var salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = DegreeOfParallelism,
            Iterations = Iterations,
            MemorySize = MemorySize
        };

        var hash = argon2.GetBytes(HashSize);

        // Format: $argon2id$v=19$m=19456,t=2,p=1$salt$hash
        return $"$argon2id$v=19$m={MemorySize},t={Iterations},p={DegreeOfParallelism}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    /// <inheritdoc/>
    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(hashedPassword))
        {
            return false;
        }

        try
        {
            // Check if it's an Argon2 hash
            if (hashedPassword.StartsWith("$argon2"))
            {
                return VerifyArgon2Password(password, hashedPassword);
            }
            // Fall back to BCrypt for legacy hashes
            else if (hashedPassword.StartsWith("$2"))
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    private bool VerifyArgon2Password(string password, string hashedPassword)
    {
        try
        {
            var parts = hashedPassword.Split('$');
            if (parts.Length != 6)
            {
                return false;
            }

            // Parse parameters
            var parameters = parts[3].Split(',');
            var memorySize = int.Parse(parameters[0].Split('=')[1]);
            var iterations = int.Parse(parameters[1].Split('=')[1]);
            var degreeOfParallelism = int.Parse(parameters[2].Split('=')[1]);

            var salt = Convert.FromBase64String(parts[4]);
            var storedHash = Convert.FromBase64String(parts[5]);

            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = degreeOfParallelism,
                Iterations = iterations,
                MemorySize = memorySize
            };

            var computedHash = argon2.GetBytes(storedHash.Length);

            return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public bool NeedsRehash(string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword))
        {
            return true;
        }

        try
        {
            // If it's a BCrypt hash, it needs rehashing to Argon2
            if (hashedPassword.StartsWith("$2"))
            {
                return true;
            }

            // If it's an Argon2 hash, check if parameters match current settings
            if (hashedPassword.StartsWith("$argon2"))
            {
                var parts = hashedPassword.Split('$');
                if (parts.Length != 6)
                {
                    return true;
                }

                var parameters = parts[3].Split(',');
                var memorySize = int.Parse(parameters[0].Split('=')[1]);
                var iterations = int.Parse(parameters[1].Split('=')[1]);
                var degreeOfParallelism = int.Parse(parameters[2].Split('=')[1]);

                return memorySize != MemorySize ||
                       iterations != Iterations ||
                       degreeOfParallelism != DegreeOfParallelism;
            }

            return true;
        }
        catch
        {
            return true;
        }
    }

    /// <inheritdoc/>
    public string GenerateSecurePassword(int length = 16, bool includeSpecialCharacters = true)
    {
        if (length < 8)
        {
            throw new ArgumentException("Password length must be at least 8 characters.", nameof(length));
        }

        const string lowercase = "abcdefghijklmnopqrstuvwxyz";
        const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string digits = "0123456789";

        var characterSet = lowercase + uppercase + digits;
        if (includeSpecialCharacters)
        {
            characterSet += SpecialCharacters;
        }

        var password = new StringBuilder(length);
        using var rng = RandomNumberGenerator.Create();

        // Ensure at least one of each required character type
        password.Append(GetRandomCharacter(lowercase, rng));
        password.Append(GetRandomCharacter(uppercase, rng));
        password.Append(GetRandomCharacter(digits, rng));

        if (includeSpecialCharacters)
        {
            password.Append(GetRandomCharacter(SpecialCharacters, rng));
        }

        // Fill the rest with random characters
        for (int i = password.Length; i < length; i++)
        {
            password.Append(GetRandomCharacter(characterSet, rng));
        }

        // Shuffle the password to avoid predictable patterns
        return ShuffleString(password.ToString(), rng);
    }

    /// <inheritdoc/>
    public (bool IsValid, string? ErrorMessage) ValidatePasswordStrength(
        string password,
        int minLength = 8,
        bool requireUppercase = true,
        bool requireLowercase = true,
        bool requireDigit = true,
        bool requireSpecialChar = true)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return (false, "Password cannot be empty.");
        }

        if (password.Length < minLength)
        {
            return (false, $"Password must be at least {minLength} characters long.");
        }

        if (requireUppercase && !Regex.IsMatch(password, @"[A-Z]"))
        {
            return (false, "Password must contain at least one uppercase letter.");
        }

        if (requireLowercase && !Regex.IsMatch(password, @"[a-z]"))
        {
            return (false, "Password must contain at least one lowercase letter.");
        }

        if (requireDigit && !Regex.IsMatch(password, @"[0-9]"))
        {
            return (false, "Password must contain at least one digit.");
        }

        if (requireSpecialChar)
        {
            var hasSpecialChar = password.IndexOfAny(SpecialCharacters.ToCharArray()) >= 0;
            if (!hasSpecialChar)
            {
                return (false, "Password must contain at least one special character.");
            }
        }

        return (true, null);
    }

    private static char GetRandomCharacter(string characterSet, RandomNumberGenerator rng)
    {
        var randomBytes = new byte[4];
        rng.GetBytes(randomBytes);
        var randomNumber = BitConverter.ToUInt32(randomBytes, 0);
        var index = randomNumber % characterSet.Length;
        return characterSet[(int)index];
    }

    private static string ShuffleString(string input, RandomNumberGenerator rng)
    {
        var chars = input.ToCharArray();
        var n = chars.Length;

        for (int i = n - 1; i > 0; i--)
        {
            var randomBytes = new byte[4];
            rng.GetBytes(randomBytes);
            var randomNumber = BitConverter.ToUInt32(randomBytes, 0);
            var j = (int)(randomNumber % (i + 1));

            // Swap
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }

        return new string(chars);
    }
}
