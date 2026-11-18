namespace FinWise.Core.Models;

/// <summary>Helpers for ISO 4217 currency codes.</summary>
public static class Currency
{
    /// <summary>The most decimal places any ISO 4217 currency uses (CLF, UYW).</summary>
    public const int MaxMinorUnitDigits = 4;

    private static readonly HashSet<string> ZeroDigits = new(StringComparer.Ordinal)
    {
        "BIF", "CLP", "DJF", "GNF", "ISK", "JPY", "KMF", "KRW", "PYG",
        "RWF", "UGX", "UYI", "VND", "VUV", "XAF", "XOF", "XPF",
    };

    private static readonly HashSet<string> ThreeDigits = new(StringComparer.Ordinal)
    {
        "BHD", "IQD", "JOD", "KWD", "LYD", "OMR", "TND",
    };

    private static readonly HashSet<string> FourDigits = new(StringComparer.Ordinal)
    {
        "CLF", "UYW",
    };

    /// <summary>Trims and upper-cases a currency code.</summary>
    public static string Normalize(string? code) => (code ?? string.Empty).Trim().ToUpperInvariant();

    /// <summary>True if <paramref name="code"/> is three upper-case ASCII letters.</summary>
    public static bool IsValidCode(string? code) =>
        code is { Length: 3 } && code.All(c => c is >= 'A' and <= 'Z');

    /// <summary>
    /// Number of decimal places used by the currency: 0 for JPY, 3 for KWD, 2 for most others
    /// (including unknown codes).
    /// </summary>
    public static int GetMinorUnitDigits(string code)
    {
        code = Normalize(code);
        if (ZeroDigits.Contains(code)) return 0;
        if (ThreeDigits.Contains(code)) return 3;
        if (FourDigits.Contains(code)) return 4;
        return 2;
    }

    /// <summary>True if <paramref name="amount"/> has no more decimal places than the currency allows.</summary>
    public static bool HasValidPrecision(decimal amount, string code) =>
        decimal.Round(amount, GetMinorUnitDigits(code)) == amount;
}
