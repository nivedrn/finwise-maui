using FinWise.Core.Calculations;
using FinWise.Core.Models;
using FinWise.Core.Validation;

namespace FinWise.Tests;

public class ValidationTests
{
    private static readonly Guid A = Guid.NewGuid();
    private static readonly Guid B = Guid.NewGuid();

    private static Transaction ValidTransaction()
    {
        var t = new Transaction { Title = "Dinner", Amount = 30m, CurrencyCode = "EUR", PayerId = A, Date = new DateTime(2026, 11, 1) };
        t.SetSplits(SplitMethod.Equal, SplitCalculator.Equal(30m, [A, B]));
        return t;
    }

    [Fact]
    public void A_valid_transaction_has_no_errors()
    {
        Assert.Empty(EntityValidator.Validate(ValidTransaction()));
    }

    [Fact]
    public void A_personal_transaction_without_splits_is_valid()
    {
        var t = ValidTransaction();
        t.ClearSplits();

        Assert.Empty(EntityValidator.Validate(t));
    }

    [Theory]
    [InlineData("")]
    [InlineData("EU")]
    [InlineData("eur")]
    [InlineData("EURO")]
    [InlineData("E1R")]
    public void Invalid_currency_codes_are_rejected(string code)
    {
        var t = ValidTransaction();
        t.CurrencyCode = code;

        Assert.NotEmpty(EntityValidator.Validate(t));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Amount_must_be_positive(int amount)
    {
        var t = ValidTransaction();
        t.Amount = amount;
        t.ClearSplits();

        Assert.NotEmpty(EntityValidator.Validate(t));
    }

    [Fact]
    public void Amount_precision_follows_the_currency()
    {
        var t = ValidTransaction();
        t.ClearSplits();

        t.Amount = 10.5m;
        t.CurrencyCode = "JPY";
        Assert.NotEmpty(EntityValidator.Validate(t));

        t.Amount = 1.234m;
        t.CurrencyCode = "KWD";
        Assert.Empty(EntityValidator.Validate(t));

        t.CurrencyCode = "EUR";
        Assert.NotEmpty(EntityValidator.Validate(t));
    }

    [Fact]
    public void Splits_must_add_up_to_the_amount()
    {
        var t = ValidTransaction();
        t.Splits[0].Amount += 0.01m;

        Assert.Contains(EntityValidator.Validate(t), e => e.Contains("add up"));
    }

    [Fact]
    public void Deleted_splits_are_not_counted()
    {
        var t = ValidTransaction();
        t.Splits.Add(new Split { PersonId = Guid.NewGuid(), Amount = 5m, IsDeleted = true });

        Assert.Empty(EntityValidator.Validate(t));
    }

    [Fact]
    public void A_person_can_only_have_one_split()
    {
        var t = ValidTransaction();
        t.Splits[1].PersonId = A;

        Assert.NotEmpty(EntityValidator.Validate(t));
    }

    [Fact]
    public void A_split_transaction_needs_splits_and_an_unsplit_one_must_not_have_any()
    {
        var t = ValidTransaction();
        t.Splits.Clear();
        Assert.NotEmpty(EntityValidator.Validate(t));

        t = ValidTransaction();
        t.SplitMethod = SplitMethod.None;
        Assert.NotEmpty(EntityValidator.Validate(t));
    }

    [Fact]
    public void Shares_are_required_when_splitting_by_shares()
    {
        var t = ValidTransaction();
        t.SplitMethod = SplitMethod.Shares;
        Assert.NotEmpty(EntityValidator.Validate(t));

        t.SetSplits(SplitMethod.Shares, SplitCalculator.ByShares(30m, [(A, 1m), (B, 2m)]));
        Assert.Empty(EntityValidator.Validate(t));

        t.SetSplits(SplitMethod.Shares, SplitCalculator.ByShares(30m, [(A, 0.33333m), (B, 0.66667m)]));
        Assert.Contains(EntityValidator.Validate(t), e => e.Contains("Shares can have at most"));
    }

    [Fact]
    public void Transaction_needs_title_and_payer()
    {
        var t = ValidTransaction();
        t.Title = " ";
        t.PayerId = Guid.Empty;

        Assert.Equal(2, EntityValidator.Validate(t).Count);
    }

    [Fact]
    public void Settlement_rules()
    {
        var s = new Settlement { FromPersonId = A, ToPersonId = B, Amount = 5m, CurrencyCode = "EUR" };
        Assert.Empty(EntityValidator.Validate(s));

        s.ToPersonId = A;
        Assert.NotEmpty(EntityValidator.Validate(s));

        s.ToPersonId = Guid.Empty;
        Assert.NotEmpty(EntityValidator.Validate(s));
    }

    [Fact]
    public void EnsureValid_throws_with_all_errors()
    {
        var ex = Assert.Throws<ValidationException>(() => EntityValidator.EnsureValid(new Person { Name = "" }));
        Assert.Single(ex.Errors);
    }

    [Theory]
    [InlineData("JPY", 0)]
    [InlineData("eur", 2)]
    [InlineData("USD", 2)]
    [InlineData("KWD", 3)]
    [InlineData("CLF", 4)]
    [InlineData("XYZ", 2)]
    public void Minor_unit_digits(string code, int expected)
    {
        Assert.Equal(expected, Currency.GetMinorUnitDigits(code));
    }
}
