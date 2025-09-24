using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Marventa.Framework.Core.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string? value)
        => string.IsNullOrEmpty(value);

    public static bool IsNullOrWhiteSpace(this string? value)
        => string.IsNullOrWhiteSpace(value);

    public static bool HasValue(this string? value)
        => !string.IsNullOrWhiteSpace(value);

    public static string ToTitleCase(this string value)
    {
        if (IsNullOrWhiteSpace(value))
            return value;

        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
    }

    public static string ToCamelCase(this string value)
    {
        if (IsNullOrWhiteSpace(value))
            return value;

        return char.ToLowerInvariant(value[0]) + value[1..];
    }

    public static string ToPascalCase(this string value)
    {
        if (IsNullOrWhiteSpace(value))
            return value;

        return char.ToUpperInvariant(value[0]) + value[1..];
    }

    public static string ToSlug(this string value)
    {
        if (IsNullOrWhiteSpace(value))
            return value;

        value = value.ToLowerInvariant();
        value = Regex.Replace(value, @"[^a-z0-9\s-]", "");
        value = Regex.Replace(value, @"\s+", " ").Trim();
        value = Regex.Replace(value, @"\s", "-");

        return value;
    }

    public static string ToMd5(this string value)
    {
        if (IsNullOrWhiteSpace(value))
            return value;

        using var md5 = MD5.Create();
        var inputBytes = Encoding.UTF8.GetBytes(value);
        var hashBytes = md5.ComputeHash(inputBytes);

        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    public static bool IsValidEmail(this string email)
    {
        if (IsNullOrWhiteSpace(email))
            return false;

        const string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    public static string Truncate(this string value, int maxLength, string suffix = "...")
    {
        if (IsNullOrWhiteSpace(value) || value.Length <= maxLength)
            return value;

        return value[..(maxLength - suffix.Length)] + suffix;
    }
}