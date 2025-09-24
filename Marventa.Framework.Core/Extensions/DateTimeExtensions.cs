using System;

namespace Marventa.Framework.Core.Extensions;

public static class DateTimeExtensions
{
    public static DateTime StartOfDay(this DateTime dateTime)
        => new(dateTime.Year, dateTime.Month, dateTime.Day);

    public static DateTime EndOfDay(this DateTime dateTime)
        => new(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, 999);

    public static DateTime StartOfWeek(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Monday)
    {
        var diff = (7 + (dateTime.DayOfWeek - startOfWeek)) % 7;
        return dateTime.AddDays(-1 * diff).StartOfDay();
    }

    public static DateTime EndOfWeek(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Monday)
        => dateTime.StartOfWeek(startOfWeek).AddDays(6).EndOfDay();

    public static DateTime StartOfMonth(this DateTime dateTime)
        => new(dateTime.Year, dateTime.Month, 1);

    public static DateTime EndOfMonth(this DateTime dateTime)
        => dateTime.StartOfMonth().AddMonths(1).AddDays(-1).EndOfDay();

    public static DateTime StartOfYear(this DateTime dateTime)
        => new(dateTime.Year, 1, 1);

    public static DateTime EndOfYear(this DateTime dateTime)
        => new(dateTime.Year, 12, 31, 23, 59, 59, 999);

    public static bool IsWeekend(this DateTime dateTime)
        => dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday;

    public static bool IsWeekday(this DateTime dateTime)
        => !dateTime.IsWeekend();

    public static int Age(this DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;

        if (birthDate.Date > today.AddYears(-age))
            age--;

        return age;
    }

    public static string ToIso8601String(this DateTime dateTime)
        => dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

    public static long ToUnixTimeSeconds(this DateTime dateTime)
        => ((DateTimeOffset)dateTime).ToUnixTimeSeconds();

    public static long ToUnixTimeMilliseconds(this DateTime dateTime)
        => ((DateTimeOffset)dateTime).ToUnixTimeMilliseconds();
}