using System.Globalization;

namespace Marventa.Framework.Domain.ValueObjects;

public class Money : IEquatable<Money>, IComparable<Money>
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    public Money(decimal amount, Currency currency)
    {
        Amount = Math.Round(amount, currency.DecimalPlaces);
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));
    }

    public Money(decimal amount, string currencyCode)
        : this(amount, Currency.FromCode(currencyCode))
    {
    }

    public static Money Zero(Currency currency) => new(0, currency);
    public static Money Zero(string currencyCode) => new(0, currencyCode);

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal factor)
    {
        return new Money(Amount * factor, Currency);
    }

    public Money Divide(decimal divisor)
    {
        if (divisor == 0)
            throw new DivideByZeroException();
        return new Money(Amount / divisor, Currency);
    }

    public Money ApplyTax(decimal taxRate)
    {
        return Multiply(1 + taxRate);
    }

    public Money ApplyDiscount(decimal discountRate)
    {
        if (discountRate < 0 || discountRate > 1)
            throw new ArgumentException("Discount rate must be between 0 and 1");
        return Multiply(1 - discountRate);
    }

    public Money ConvertTo(Currency targetCurrency, decimal exchangeRate)
    {
        return new Money(Amount * exchangeRate, targetCurrency);
    }

    public Money Round(int decimalPlaces = 2)
    {
        return new Money(Math.Round(Amount, decimalPlaces), Currency);
    }

    private void EnsureSameCurrency(Money other)
    {
        if (!Currency.Equals(other.Currency))
            throw new InvalidOperationException($"Cannot operate on different currencies: {Currency.Code} and {other.Currency.Code}");
    }

    public override string ToString()
    {
        return Currency.Format(Amount);
    }

    public string ToString(CultureInfo? culture = null)
    {
        culture ??= Currency.GetCulture();
        return Amount.ToString("C", culture);
    }

    public bool Equals(Money? other)
    {
        if (other is null) return false;
        return Amount == other.Amount && Currency.Equals(other.Currency);
    }

    public override bool Equals(object? obj)
    {
        return obj is Money other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Amount, Currency);
    }

    public int CompareTo(Money? other)
    {
        if (other is null) return 1;
        EnsureSameCurrency(other);
        return Amount.CompareTo(other.Amount);
    }

    public static bool operator ==(Money? left, Money? right) => Equals(left, right);
    public static bool operator !=(Money? left, Money? right) => !Equals(left, right);
    public static bool operator <(Money left, Money right) => left.CompareTo(right) < 0;
    public static bool operator >(Money left, Money right) => left.CompareTo(right) > 0;
    public static bool operator <=(Money left, Money right) => left.CompareTo(right) <= 0;
    public static bool operator >=(Money left, Money right) => left.CompareTo(right) >= 0;
    public static Money operator +(Money left, Money right) => left.Add(right);
    public static Money operator -(Money left, Money right) => left.Subtract(right);
    public static Money operator *(Money money, decimal factor) => money.Multiply(factor);
    public static Money operator /(Money money, decimal divisor) => money.Divide(divisor);
}