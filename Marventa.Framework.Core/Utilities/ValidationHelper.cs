using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Marventa.Framework.Core.Utilities;

public static class ValidationHelper
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex PhoneRegex = new(
        @"^\+?[\d\s\-\(\)]{7,15}$",
        RegexOptions.Compiled);

    private static readonly Regex UrlRegex = new(
        @"^https?:\/\/[^\s/$.?#].[^\s]*$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return EmailRegex.IsMatch(email) && new EmailAddressAttribute().IsValid(email);
    }

    public static bool IsValidPhone(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        return PhoneRegex.IsMatch(phoneNumber);
    }

    public static bool IsValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        return UrlRegex.IsMatch(url) && Uri.TryCreate(url, UriKind.Absolute, out _);
    }

    public static bool IsValidGuid(string guid)
    {
        return Guid.TryParse(guid, out _);
    }

    public static bool IsNumeric(string value)
    {
        return double.TryParse(value, out _);
    }

    public static bool IsInteger(string value)
    {
        return int.TryParse(value, out _);
    }

    public static bool IsInRange(int value, int min, int max)
    {
        return value >= min && value <= max;
    }

    public static bool IsInRange(double value, double min, double max)
    {
        return value >= min && value <= max;
    }

    public static bool IsInRange(DateTime value, DateTime min, DateTime max)
    {
        return value >= min && value <= max;
    }

    public static bool HasMinLength(string value, int minLength)
    {
        return !string.IsNullOrEmpty(value) && value.Length >= minLength;
    }

    public static bool HasMaxLength(string value, int maxLength)
    {
        return string.IsNullOrEmpty(value) || value.Length <= maxLength;
    }

    public static bool IsLengthInRange(string value, int minLength, int maxLength)
    {
        var length = value?.Length ?? 0;
        return length >= minLength && length <= maxLength;
    }

    public static bool ContainsOnlyDigits(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return Regex.IsMatch(value, @"^\d+$");
    }

    public static bool ContainsOnlyLetters(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return Regex.IsMatch(value, @"^[a-zA-Z]+$");
    }

    public static bool ContainsOnlyAlphanumeric(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return Regex.IsMatch(value, @"^[a-zA-Z0-9]+$");
    }

    public static bool IsStrongPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        // At least 8 characters, contains uppercase, lowercase, digit, and special character
        return password.Length >= 8 &&
               Regex.IsMatch(password, @"[A-Z]") &&
               Regex.IsMatch(password, @"[a-z]") &&
               Regex.IsMatch(password, @"\d") &&
               Regex.IsMatch(password, @"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
    }

    public static string? ValidateRequired(string value, string fieldName)
    {
        return string.IsNullOrWhiteSpace(value) ? $"{fieldName} is required." : null;
    }

    public static string? ValidateEmail(string email, string fieldName = "Email")
    {
        if (string.IsNullOrWhiteSpace(email))
            return $"{fieldName} is required.";

        return IsValidEmail(email) ? null : $"{fieldName} is not a valid email address.";
    }

    public static string? ValidatePhone(string phoneNumber, string fieldName = "Phone Number")
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return $"{fieldName} is required.";

        return IsValidPhone(phoneNumber) ? null : $"{fieldName} is not a valid phone number.";
    }

    public static string? ValidateLength(string value, int minLength, int maxLength, string fieldName)
    {
        if (string.IsNullOrEmpty(value))
            return $"{fieldName} is required.";

        if (value.Length < minLength)
            return $"{fieldName} must be at least {minLength} characters long.";

        if (value.Length > maxLength)
            return $"{fieldName} must be no more than {maxLength} characters long.";

        return null;
    }
}