using FinWise.Core.Calculations;
using FinWise.Core.Models;

namespace FinWise.Tests;

public class SplitCalculatorTests
{
    private static readonly Guid A = new("00000000-0000-0000-0000-00000000000a");
    private static readonly Guid B = new("00000000-0000-0000-0000-00000000000b");
    private static readonly Guid C = new("00000000-0000-0000-0000-00000000000c");
    private static readonly Guid D = new("00000000-0000-0000-0000-00000000000d");

    [Fact]
    public void Equal_divides_evenly_when_possible()
    {
        var parts = SplitCalculator.Equal(30m, [A, B, C]);

        Assert.Equal([10m, 10m, 10m], parts.Select(p => p.Amount));
        Assert.Equal([A, B, C], parts.Select(p => p.PersonId));
        Assert.All(parts, p => Assert.Null(p.Shares));
    }

    [Fact]
    public void Equal_gives_leftover_cents_to_the_first_participants()
    {
        var parts = SplitCalculator.Equal(10m, [A, B, C]);

        Assert.Equal([3.34m, 3.33m, 3.33m], parts.Select(p => p.Amount));
        Assert.Equal(10m, parts.Sum(p => p.Amount));
    }

    [Fact]
    public void Equal_with_two_leftover_cents()
    {
        var parts = SplitCalculator.Equal(0.02m, [A, B, C]);

        Assert.Equal([0.01m, 0.01m, 0m], parts.Select(p => p.Amount));
    }

    [Fact]
    public void Equal_is_deterministic_and_depends_only_on_input_order()
    {
        var first = SplitCalculator.Equal(100m, [A, B, C]);
        var second = SplitCalculator.Equal(100m, [A, B, C]);
        var reordered = SplitCalculator.Equal(100m, [C, B, A]);

        Assert.Equal(first, second);
        Assert.Equal(33.34m, first.Single(p => p.PersonId == A).Amount);
        Assert.Equal(33.34m, reordered.Single(p => p.PersonId == C).Amount);
    }

    [Fact]
    public void Equal_with_a_single_participant_gets_everything()
    {
        var parts = SplitCalculator.Equal(12.34m, [A]);

        Assert.Equal(12.34m, Assert.Single(parts).Amount);
    }

    [Fact]
    public void Equal_with_zero_decimal_currency()
    {
        var parts = SplitCalculator.Equal(1000m, [A, B, C], decimals: 0);

        Assert.Equal([334m, 333m, 333m], parts.Select(p => p.Amount));
    }

    [Fact]
    public void Equal_with_three_decimal_currency()
    {
        var parts = SplitCalculator.Equal(1m, [A, B, C], decimals: 3);

        Assert.Equal([0.334m, 0.333m, 0.333m], parts.Select(p => p.Amount));
    }

    [Fact]
    public void Equal_with_negative_total_rounds_symmetrically()
    {
        var parts = SplitCalculator.Equal(-10m, [A, B, C]);

        Assert.Equal([-3.34m, -3.33m, -3.33m], parts.Select(p => p.Amount));
    }

    [Fact]
    public void Equal_with_zero_total_gives_zeros()
    {
        var parts = SplitCalculator.Equal(0m, [A, B]);

        Assert.All(parts, p => Assert.Equal(0m, p.Amount));
    }

    [Fact]
    public void Equal_rejects_more_decimals_than_the_currency_has()
    {
        Assert.Throws<ArgumentException>(() => SplitCalculator.Equal(10.005m, [A, B]));
    }

    [Fact]
    public void Equal_rejects_no_participants()
    {
        Assert.Throws<ArgumentException>(() => SplitCalculator.Equal(10m, []));
    }

    [Fact]
    public void Equal_rejects_duplicate_participants()
    {
        Assert.Throws<ArgumentException>(() => SplitCalculator.Equal(10m, [A, B, A]));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(5)]
    public void Rejects_unsupported_decimal_places(int decimals)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => SplitCalculator.Equal(10m, [A], decimals));
    }

    [Fact]
    public void Equal_handles_very_large_totals_without_overflow()
    {
        var total = 79_228_162_514_264_337_593_543_950.33m; // close to decimal.MaxValue with 2 decimals
        var parts = SplitCalculator.Equal(total, [A, B, C]);

        Assert.Equal(total, parts.Sum(p => p.Amount));
    }

    [Fact]
    public void ByShares_divides_proportionally()
    {
        var parts = SplitCalculator.ByShares(100m, [(A, 1m), (B, 3m)]);

        Assert.Equal([25m, 75m], parts.Select(p => p.Amount));
        Assert.Equal([1m, 3m], parts.Select(p => p.Shares));
    }

    [Fact]
    public void ByShares_gives_leftover_cents_to_the_largest_remainders()
    {
        // Exact shares: 1.428571.., 2.857142.., 5.714285.. -> 1.42 / 2.85 / 5.71 = 9.98,
        // the remainders are .857, .714, .428, so the two leftover cents go to A and B.
        var parts = SplitCalculator.ByShares(10m, [(A, 1m), (B, 2m), (C, 4m)]);

        Assert.Equal([1.43m, 2.86m, 5.71m], parts.Select(p => p.Amount));
        Assert.Equal(10m, parts.Sum(p => p.Amount));
    }

    [Fact]
    public void ByShares_accepts_fractional_shares()
    {
        var parts = SplitCalculator.ByShares(10m, [(A, 0.5m), (B, 1.5m)]);

        Assert.Equal([2.5m, 7.5m], parts.Select(p => p.Amount));
    }

    [Fact]
    public void ByShares_never_gives_a_cent_to_zero_shares()
    {
        var parts = SplitCalculator.ByShares(0.05m, [(A, 0m), (B, 1m), (C, 1m), (D, 1m)]);

        Assert.Equal(0m, parts[0].Amount);
        Assert.Equal(0.05m, parts.Sum(p => p.Amount));
        Assert.Equal([0m, 0.02m, 0.02m, 0.01m], parts.Select(p => p.Amount));
    }

    [Fact]
    public void ByShares_rejects_negative_shares()
    {
        Assert.Throws<ArgumentException>(() => SplitCalculator.ByShares(10m, [(A, -1m), (B, 2m)]));
    }

    [Fact]
    public void ByShares_rejects_all_zero_shares()
    {
        Assert.Throws<ArgumentException>(() => SplitCalculator.ByShares(10m, [(A, 0m), (B, 0m)]));
    }

    [Theory]
    [InlineData(0.01, 7)]
    [InlineData(1, 3)]
    [InlineData(99.99, 6)]
    [InlineData(1234567.89, 11)]
    [InlineData(0.07, 9)]
    public void Equal_parts_always_add_up_and_differ_by_at_most_one_cent(double totalValue, int people)
    {
        var total = (decimal)totalValue;
        var ids = Enumerable.Range(1, people).Select(_ => Guid.NewGuid()).ToList();

        var parts = SplitCalculator.Equal(total, ids);

        Assert.Equal(total, parts.Sum(p => p.Amount));
        Assert.True(parts.Max(p => p.Amount) - parts.Min(p => p.Amount) <= 0.01m);
        Assert.All(parts, p => Assert.Equal(decimal.Round(p.Amount, 2), p.Amount));
    }

    [Fact]
    public void ByShares_parts_always_add_up_for_random_inputs()
    {
        var random = new Random(1234);
        for (var run = 0; run < 500; run++)
        {
            var total = random.Next(0, 10_000_000) / 100m;
            var shares = Enumerable.Range(0, random.Next(1, 8))
                .Select(_ => (Guid.NewGuid(), random.Next(0, 5) + random.Next(0, 4) * 0.25m))
                .ToList();
            if (shares.All(s => s.Item2 == 0))
                shares[0] = (shares[0].Item1, 1m);

            var parts = SplitCalculator.ByShares(total, shares);

            Assert.Equal(total, parts.Sum(p => p.Amount));
            Assert.All(parts, p => Assert.True(p.Amount >= 0));
            Assert.All(parts.Zip(shares), x =>
            {
                // Each part is within one cent of the exact proportional value.
                var exact = total * x.Second.Item2 / shares.Sum(s => s.Item2);
                Assert.True(Math.Abs(x.First.Amount - exact) < 0.01m, $"{x.First.Amount} vs {exact}");
            });
        }
    }

    [Fact]
    public void Exact_returns_the_amounts_when_they_add_up()
    {
        var parts = SplitCalculator.Exact(50m, [(A, 20m), (B, 30m)]);

        Assert.Equal([20m, 30m], parts.Select(p => p.Amount));
    }

    [Fact]
    public void Exact_allows_zero_amounts()
    {
        var parts = SplitCalculator.Exact(50m, [(A, 0m), (B, 50m)]);

        Assert.Equal([0m, 50m], parts.Select(p => p.Amount));
    }

    [Theory]
    [InlineData(20, 29.99)]
    [InlineData(20, 30.01)]
    public void Exact_rejects_amounts_that_do_not_add_up(double a, double b)
    {
        Assert.Throws<ArgumentException>(() =>
            SplitCalculator.Exact(50m, [(A, (decimal)a), (B, (decimal)b)]));
    }

    [Fact]
    public void Exact_rejects_too_many_decimals()
    {
        Assert.Throws<ArgumentException>(() =>
            SplitCalculator.Exact(50m, [(A, 20.005m), (B, 29.995m)]));
    }

    [Fact]
    public void Exact_rejects_amounts_with_the_wrong_sign()
    {
        Assert.Throws<ArgumentException>(() =>
            SplitCalculator.Exact(50m, [(A, -10m), (B, 60m)]));
    }

    [Fact]
    public void Calculate_dispatches_on_method()
    {
        (Guid, decimal)[] inputs = [(A, 1m), (B, 2m)];

        Assert.Equal([1.5m, 1.5m], SplitCalculator.Calculate(SplitMethod.Equal, 3m, inputs).Select(p => p.Amount));
        Assert.Equal([1m, 2m], SplitCalculator.Calculate(SplitMethod.Shares, 3m, inputs).Select(p => p.Amount));
        Assert.Equal([1m, 2m], SplitCalculator.Calculate(SplitMethod.Exact, 3m, inputs).Select(p => p.Amount));
        Assert.Throws<ArgumentOutOfRangeException>(() => SplitCalculator.Calculate(SplitMethod.None, 3m, inputs));
    }

    [Fact]
    public void Transaction_SetSplits_builds_splits_from_parts()
    {
        var transaction = new Transaction { Amount = 10m, CurrencyCode = "EUR" };

        transaction.SetSplits(SplitMethod.Shares, SplitCalculator.ByShares(10m, [(A, 1m), (B, 1m)]));

        Assert.Equal(SplitMethod.Shares, transaction.SplitMethod);
        Assert.All(transaction.Splits, s => Assert.Equal(transaction.Id, s.TransactionId));
        Assert.Equal([5m, 5m], transaction.Splits.Select(s => s.Amount));
        Assert.Equal([1m, 1m], transaction.Splits.Select(s => s.Shares));

        transaction.SetSplits(SplitMethod.Equal, SplitCalculator.Equal(10m, [A, B]));
        Assert.All(transaction.Splits, s => Assert.Null(s.Shares));
    }
}
