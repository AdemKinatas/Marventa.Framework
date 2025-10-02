namespace Marventa.Framework.Security.Encryption.Abstractions;

/// <summary>
/// Provides password hashing and verification services using secure cryptographic algorithms.
/// </summary>
public interface IPasswordService
{
    /// <summary>
    /// Hashes a plain text password using a secure hashing algorithm (BCrypt).
    /// </summary>
    /// <param name="password">The plain text password to hash.</param>
    /// <returns>The hashed password.</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifies that a plain text password matches a hashed password.
    /// </summary>
    /// <param name="password">The plain text password to verify.</param>
    /// <param name="hashedPassword">The hashed password to compare against.</param>
    /// <returns>True if the password matches; otherwise, false.</returns>
    bool VerifyPassword(string password, string hashedPassword);

    /// <summary>
    /// Determines whether a password needs to be rehashed (e.g., due to algorithm changes or work factor updates).
    /// </summary>
    /// <param name="hashedPassword">The hashed password to check.</param>
    /// <returns>True if the password should be rehashed; otherwise, false.</returns>
    bool NeedsRehash(string hashedPassword);

    /// <summary>
    /// Generates a cryptographically secure random password.
    /// </summary>
    /// <param name="length">The desired password length. Default is 16 characters.</param>
    /// <param name="includeSpecialCharacters">Whether to include special characters. Default is true.</param>
    /// <returns>A randomly generated secure password.</returns>
    string GenerateSecurePassword(int length = 16, bool includeSpecialCharacters = true);

    /// <summary>
    /// Validates password strength based on common security requirements.
    /// </summary>
    /// <param name="password">The password to validate.</param>
    /// <param name="minLength">Minimum required length. Default is 8.</param>
    /// <param name="requireUppercase">Require at least one uppercase letter. Default is true.</param>
    /// <param name="requireLowercase">Require at least one lowercase letter. Default is true.</param>
    /// <param name="requireDigit">Require at least one digit. Default is true.</param>
    /// <param name="requireSpecialChar">Require at least one special character. Default is true.</param>
    /// <returns>A tuple indicating validity and an error message if invalid.</returns>
    (bool IsValid, string? ErrorMessage) ValidatePasswordStrength(
        string password,
        int minLength = 8,
        bool requireUppercase = true,
        bool requireLowercase = true,
        bool requireDigit = true,
        bool requireSpecialChar = true);
}
