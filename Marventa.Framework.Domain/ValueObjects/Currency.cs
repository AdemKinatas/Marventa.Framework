using System.Globalization;

namespace Marventa.Framework.Domain.ValueObjects;

public class Currency : IEquatable<Currency>
{
    private static readonly Dictionary<string, Currency> _currencies = new()
    {
        ["USD"] = new("USD", "$", 2, "en-US", "US Dollar"),
        ["EUR"] = new("EUR", "€", 2, "de-DE", "Euro"),
        ["GBP"] = new("GBP", "£", 2, "en-GB", "British Pound"),
        ["TRY"] = new("TRY", "₺", 2, "tr-TR", "Turkish Lira"),
        ["JPY"] = new("JPY", "¥", 0, "ja-JP", "Japanese Yen"),
        ["CNY"] = new("CNY", "¥", 2, "zh-CN", "Chinese Yuan"),
        ["AED"] = new("AED", "د.إ", 2, "ar-AE", "UAE Dirham"),
        ["SAR"] = new("SAR", "﷼", 2, "ar-SA", "Saudi Riyal"),
    };

    public string Code { get; }
    public string Symbol { get; }
    public int DecimalPlaces { get; }
    public string CultureName { get; }
    public string Name { get; }

    private Currency(string code, string symbol, int decimalPlaces, string cultureName, string name)
    {
        Code = code;
        Symbol = symbol;
        DecimalPlaces = decimalPlaces;
        CultureName = cultureName;
        Name = name;
    }

    public static Currency USD => _currencies["USD"];
    public static Currency EUR => _currencies["EUR"];
    public static Currency GBP => _currencies["GBP"];
    public static Currency TRY => _currencies["TRY"];
    public static Currency JPY => _currencies["JPY"];

    public static Currency FromCode(string code)
    {
        if (_currencies.TryGetValue(code.ToUpperInvariant(), out var currency))
            return currency;

        throw new ArgumentException($"Unknown currency code: {code}");
    }

    public static bool IsValidCode(string code)
    {
        return _currencies.ContainsKey(code.ToUpperInvariant());
    }

    public static IEnumerable<Currency> GetAll()
    {
        return _currencies.Values;
    }

    public CultureInfo GetCulture()
    {
        return new CultureInfo(CultureName);
    }

    public string Format(decimal amount)
    {
        var format = DecimalPlaces > 0
            ? $"{{0:N{DecimalPlaces}}}"
            : "{0:N0}";
        return $"{Symbol}{string.Format(format, amount)}";
    }

    public bool Equals(Currency? other)
    {
        if (other is null) return false;
        return Code == other.Code;
    }

    public override bool Equals(object? obj)
    {
        return obj is Currency other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Code.GetHashCode();
    }

    public override string ToString()
    {
        return Code;
    }
}