using FinWise.Core.Models;

namespace FinWise.Data;

/// <summary>Converts decimals to and from the fixed-point integers stored in the database.</summary>
internal static class Money
{
    private const decimal Factor = 10_000m; // 10^Currency.MaxMinorUnitDigits

    public static long ToStorage(decimal value)
    {
        if (decimal.Round(value, Currency.MaxMinorUnitDigits) != value)
            throw new ArgumentException($"{value} has more than {Currency.MaxMinorUnitDigits} decimal places and cannot be stored exactly.", nameof(value));
        return checked((long)(value * Factor));
    }

    public static long? ToStorage(decimal? value) => value is { } v ? ToStorage(v) : null;

    public static decimal FromStorage(long value) => value / Factor;

    public static decimal? FromStorage(long? value) => value is { } v ? FromStorage(v) : null;
}
