using Marventa.Framework.Security.Encryption.Abstractions;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Marventa.Framework.Security.Encryption.Services;

/// <summary>
/// Provides password hashing, verification, and validation services using BCrypt.
/// </summary>
public class PasswordService : IPasswordService
{
    private const int DefaultWorkFactor = 12;
    private const string SpecialCharacters = "!@#$%^&*()_+-=[]{}|;:,.<>?";

    /// <inheritdoc/>
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));
        }

        return BCrypt.Net.BCrypt.HashPassword(password, DefaultWorkFactor);
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
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
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
            // BCrypt hashes start with "$2a$" or similar, followed by work factor
            // Check if the work factor is less than the current default
            return !BCrypt.Net.BCrypt.PasswordNeedsRehash(hashedPassword, DefaultWorkFactor);
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

        if (requireSpecialChar && !Regex.IsMatch(password, $@"[{Regex.Escape(SpecialCharacters)}]"))
        {
            return (false, "Password must contain at least one special character.");
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
