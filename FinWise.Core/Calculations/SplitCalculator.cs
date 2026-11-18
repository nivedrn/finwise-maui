using System.Numerics;
using FinWise.Core.Models;

namespace FinWise.Core.Calculations;

/// <summary>
/// Divides an amount between people.
/// </summary>
/// <remarks>
/// All work is done in whole minor units (cents) with exact integer arithmetic, so the parts always
/// add up to the total. Amounts that cannot be divided evenly are rounded with the largest
/// remainder method: everyone first gets the rounded-down value of their exact share, and the
/// cents left over go one at a time to the people whose exact share lost the most in rounding.
/// Ties go to whoever comes first in the input, so the same input always gives the same result.
/// A participant with zero shares never receives a leftover cent.
/// </remarks>
public static class SplitCalculator
{
    /// <summary>
    /// Splits <paramref name="total"/> using <paramref name="method"/>.
    /// </summary>
    /// <param name="inputs">
    /// The participants. The value is ignored for <see cref="SplitMethod.Equal"/>, is the weight
    /// for <see cref="SplitMethod.Shares"/> and the amount owed for <see cref="SplitMethod.Exact"/>.
    /// </param>
    public static IReadOnlyList<SplitPart> Calculate(
        SplitMethod method,
        decimal total,
        IEnumerable<(Guid PersonId, decimal Value)> inputs,
        int decimals = 2) => method switch
    {
        SplitMethod.Equal => Equal(total, inputs.Select(i => i.PersonId), decimals),
        SplitMethod.Shares => ByShares(total, inputs, decimals),
        SplitMethod.Exact => Exact(total, inputs, decimals),
        _ => throw new ArgumentOutOfRangeException(nameof(method), method, "This split method does not divide an amount."),
    };

    /// <summary>Splits <paramref name="total"/> equally between <paramref name="participants"/>.</summary>
    public static IReadOnlyList<SplitPart> Equal(decimal total, IEnumerable<Guid> participants, int decimals = 2)
    {
        var parts = ByShares(total, participants.Select(id => (id, 1m)), decimals);
        return parts.Select(p => p with { Shares = null }).ToList();
    }

    /// <summary>
    /// Splits <paramref name="total"/> in proportion to each participant's shares.
    /// Shares can be any non-negative numbers (1, 2, 0.5 ...) but at least one must be positive.
    /// </summary>
    public static IReadOnlyList<SplitPart> ByShares(
        decimal total,
        IEnumerable<(Guid PersonId, decimal Shares)> shares,
        int decimals = 2)
    {
        var list = shares.ToList();
        ValidateParticipants(list.Select(s => s.PersonId).ToList());
        ValidateDecimals(decimals);

        if (list.Any(s => s.Shares < 0))
            throw new ArgumentException("Shares cannot be negative.", nameof(shares));

        // Turn the shares into integer weights by scaling them all by the same power of ten.
        var scale = list.Max(s => (int)s.Shares.Scale);
        var weights = list.Select(s => ToScaledInteger(s.Shares, scale)).ToList();
        var totalWeight = weights.Aggregate(BigInteger.Zero, (a, w) => a + w);
        if (totalWeight.IsZero)
            throw new ArgumentException("At least one participant must have a positive share.", nameof(shares));

        var totalMinor = ToMinorUnits(total, decimals, nameof(total));
        var sign = totalMinor.Sign;
        var remaining = BigInteger.Abs(totalMinor);

        var allocated = new BigInteger[list.Count];
        var remainders = new BigInteger[list.Count];
        for (var i = 0; i < list.Count; i++)
        {
            allocated[i] = BigInteger.DivRem(remaining * weights[i], totalWeight, out remainders[i]);
        }

        var leftover = remaining - allocated.Aggregate(BigInteger.Zero, (a, v) => a + v);
        var order = Enumerable.Range(0, list.Count)
            .OrderByDescending(i => remainders[i])
            .ThenBy(i => i)
            .ToList();
        for (var k = 0; k < leftover; k++)
        {
            allocated[order[k]] += 1;
        }

        return list
            .Select((s, i) => new SplitPart(s.PersonId, FromMinorUnits(sign * allocated[i], decimals), s.Shares))
            .ToList();
    }

    /// <summary>
    /// Validates amounts that were entered per person. They must add up to <paramref name="total"/>
    /// exactly and use no more than <paramref name="decimals"/> decimal places.
    /// </summary>
    public static IReadOnlyList<SplitPart> Exact(
        decimal total,
        IEnumerable<(Guid PersonId, decimal Amount)> amounts,
        int decimals = 2)
    {
        var list = amounts.ToList();
        ValidateParticipants(list.Select(a => a.PersonId).ToList());
        ValidateDecimals(decimals);
        ToMinorUnits(total, decimals, nameof(total));

        foreach (var (_, amount) in list)
        {
            ToMinorUnits(amount, decimals, nameof(amounts));
            if (amount != 0 && Math.Sign(amount) != Math.Sign(total))
                throw new ArgumentException("Each amount must have the same sign as the total.", nameof(amounts));
        }

        var sum = list.Sum(a => a.Amount);
        if (sum != total)
            throw new ArgumentException($"The amounts add up to {sum}, not {total}.", nameof(amounts));

        return list.Select(a => new SplitPart(a.PersonId, a.Amount)).ToList();
    }

    private static void ValidateParticipants(IReadOnlyList<Guid> ids)
    {
        if (ids.Count == 0)
            throw new ArgumentException("A split needs at least one participant.");
        if (ids.Distinct().Count() != ids.Count)
            throw new ArgumentException("Each participant can only appear once in a split.");
    }

    private static void ValidateDecimals(int decimals)
    {
        if (decimals is < 0 or > Currency.MaxMinorUnitDigits)
            throw new ArgumentOutOfRangeException(nameof(decimals), decimals, $"Must be between 0 and {Currency.MaxMinorUnitDigits}.");
    }

    private static BigInteger ToMinorUnits(decimal value, int decimals, string paramName)
    {
        if (decimal.Round(value, decimals) != value)
            throw new ArgumentException($"{value} has more than {decimals} decimal places.", paramName);
        return ToScaledInteger(value, decimals);
    }

    /// <summary>
    /// Returns <paramref name="value"/> × 10^<paramref name="scale"/> as an exact integer. Callers
    /// make sure that any digits beyond <paramref name="scale"/> are trailing zeros.
    /// </summary>
    private static BigInteger ToScaledInteger(decimal value, int scale)
    {
        // A decimal is a 96-bit integer mantissa divided by 10^valueScale.
        var bits = decimal.GetBits(value);
        var mantissa = new BigInteger((uint)bits[0])
                     | (new BigInteger((uint)bits[1]) << 32)
                     | (new BigInteger((uint)bits[2]) << 64);
        var valueScale = (bits[3] >> 16) & 0xFF;

        var result = valueScale <= scale
            ? mantissa * BigInteger.Pow(10, scale - valueScale)
            : mantissa / BigInteger.Pow(10, valueScale - scale);
        return bits[3] < 0 ? -result : result;
    }

    private static decimal FromMinorUnits(BigInteger minor, int decimals) =>
        (decimal)minor / Pow10(decimals);

    private static decimal Pow10(int n)
    {
        var result = 1m;
        for (var i = 0; i < n; i++) result *= 10m;
        return result;
    }
}
