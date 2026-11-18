using FinWise.Core.Calculations;
using FinWise.Core.Models;

namespace FinWise.Tests;

public class BalanceCalculatorTests
{
    private static readonly Guid Alice = new("00000000-0000-0000-0000-00000000000a");
    private static readonly Guid Bob = new("00000000-0000-0000-0000-00000000000b");
    private static readonly Guid Carol = new("00000000-0000-0000-0000-00000000000c");
    private static readonly Guid Dave = new("00000000-0000-0000-0000-00000000000d");

    private static Transaction Expense(Guid payer, decimal amount, params Guid[] participants) =>
        Expense(payer, amount, "EUR", participants);

    private static Transaction Expense(Guid payer, decimal amount, string currency, params Guid[] participants)
    {
        var transaction = new Transaction { PayerId = payer, Amount = amount, CurrencyCode = currency, Title = "x" };
        transaction.SetSplits(SplitMethod.Equal, SplitCalculator.Equal(amount, participants));
        return transaction;
    }

    private static Settlement Payment(Guid from, Guid to, decimal amount, string currency = "EUR") =>
        new() { FromPersonId = from, ToPersonId = to, Amount = amount, CurrencyCode = currency };

    private static decimal BalanceOf(IEnumerable<Balance> balances, Guid person, string currency = "EUR") =>
        balances.SingleOrDefault(b => b.PersonId == person && b.CurrencyCode == currency)?.Amount ?? 0m;

    [Fact]
    public void Nothing_in_gives_nothing_out()
    {
        Assert.Empty(BalanceCalculator.CalculateBalances([], []));
        Assert.Empty(BalanceCalculator.CalculateDebts([], []));
    }

    [Fact]
    public void Payer_is_owed_the_other_participants_parts()
    {
        var balances = BalanceCalculator.CalculateBalances([Expense(Alice, 30m, Alice, Bob, Carol)], []);

        Assert.Equal(20m, BalanceOf(balances, Alice));
        Assert.Equal(-10m, BalanceOf(balances, Bob));
        Assert.Equal(-10m, BalanceOf(balances, Carol));
        Assert.Equal(0m, balances.Sum(b => b.Amount));
    }

    [Fact]
    public void Payer_who_is_not_a_participant_is_owed_everything()
    {
        var balances = BalanceCalculator.CalculateBalances([Expense(Alice, 30m, Bob, Carol)], []);

        Assert.Equal(30m, BalanceOf(balances, Alice));
        Assert.Equal(-15m, BalanceOf(balances, Bob));
    }

    [Fact]
    public void Personal_expenses_do_not_affect_balances()
    {
        var personal = new Transaction { PayerId = Alice, Amount = 99m, CurrencyCode = "EUR", Title = "Lunch" };
        var onlyPayer = Expense(Alice, 40m, Alice);

        Assert.Empty(BalanceCalculator.CalculateBalances([personal, onlyPayer], []));
    }

    [Fact]
    public void Deleted_transactions_splits_and_settlements_are_ignored()
    {
        var deleted = Expense(Alice, 30m, Alice, Bob);
        deleted.IsDeleted = true;

        var withDeletedSplit = Expense(Alice, 30m, Alice, Bob);
        withDeletedSplit.Splits[1].IsDeleted = true; // Bob's split

        var deletedSettlement = Payment(Bob, Alice, 10m);
        deletedSettlement.IsDeleted = true;

        Assert.Empty(BalanceCalculator.CalculateBalances([deleted, withDeletedSplit], [deletedSettlement]));
    }

    [Fact]
    public void Settlements_reduce_what_is_owed()
    {
        var balances = BalanceCalculator.CalculateBalances(
            [Expense(Alice, 30m, Alice, Bob, Carol)],
            [Payment(Bob, Alice, 4m)]);

        Assert.Equal(16m, BalanceOf(balances, Alice));
        Assert.Equal(-6m, BalanceOf(balances, Bob));
        Assert.Equal(-10m, BalanceOf(balances, Carol));
    }

    [Fact]
    public void Full_settlement_clears_the_balance()
    {
        var balances = BalanceCalculator.CalculateBalances(
            [Expense(Alice, 30m, Alice, Bob, Carol)],
            [Payment(Bob, Alice, 10m), Payment(Carol, Alice, 10m)]);

        Assert.Empty(balances);
        Assert.Empty(BalanceCalculator.SimplifyDebts(balances));
    }

    [Fact]
    public void Overpaying_reverses_the_debt()
    {
        var debts = BalanceCalculator.CalculateDebts(
            [Expense(Alice, 20m, Alice, Bob)],
            [Payment(Bob, Alice, 15m)]);

        Assert.Equal([new Debt(Alice, Bob, "EUR", 5m)], debts);
    }

    [Fact]
    public void Settlement_without_any_expense_creates_a_debt()
    {
        var debts = BalanceCalculator.CalculateDebts([], [Payment(Alice, Bob, 12.5m)]);

        Assert.Equal([new Debt(Bob, Alice, "EUR", 12.5m)], debts);
    }

    [Fact]
    public void Mutual_expenses_cancel_out()
    {
        var debts = BalanceCalculator.CalculateDebts(
            [Expense(Alice, 20m, Alice, Bob), Expense(Bob, 30m, Alice, Bob)],
            []);

        // Bob owes Alice 10, Alice owes Bob 15: net Alice owes Bob 5.
        Assert.Equal([new Debt(Alice, Bob, "EUR", 5m)], debts);
    }

    [Fact]
    public void Chains_are_simplified()
    {
        // Alice owes Bob 10 and Bob owes Carol 10, so Alice can pay Carol directly.
        var transactions = new[]
        {
            Expense(Bob, 10m, Alice),
            Expense(Carol, 10m, Bob),
        };

        var debts = BalanceCalculator.CalculateDebts(transactions, []);

        Assert.Equal([new Debt(Alice, Carol, "EUR", 10m)], debts);
    }

    [Fact]
    public void Simplified_debts_settle_every_balance_with_at_most_n_minus_one_payments()
    {
        var transactions = new[]
        {
            Expense(Alice, 100m, Alice, Bob, Carol, Dave),
            Expense(Bob, 45.5m, Alice, Bob, Carol),
            Expense(Carol, 10m, Dave),
            Expense(Dave, 7.77m, Alice, Bob, Carol, Dave),
        };
        var settlements = new[] { Payment(Carol, Alice, 5m) };

        var balances = BalanceCalculator.CalculateBalances(transactions, settlements);
        var debts = BalanceCalculator.SimplifyDebts(balances);

        Assert.True(debts.Count <= balances.Count - 1);
        Assert.All(debts, d => Assert.True(d.Amount > 0));

        // Applying the suggested payments as settlements leaves nobody owing anything.
        var payments = debts.Select(d => Payment(d.FromPersonId, d.ToPersonId, d.Amount));
        Assert.Empty(BalanceCalculator.CalculateBalances(transactions, settlements.Concat(payments)));
    }

    [Fact]
    public void Simplification_is_deterministic()
    {
        // Equal balances everywhere force tie-breaking.
        var transactions = new[]
        {
            Expense(Alice, 20m, Alice, Carol),
            Expense(Bob, 20m, Bob, Dave),
        };

        var first = BalanceCalculator.CalculateDebts(transactions, []);
        var second = BalanceCalculator.CalculateDebts(transactions.Reverse().ToArray(), []);

        Assert.Equal(first, second);
        Assert.Equal(
            [new Debt(Carol, Alice, "EUR", 10m), new Debt(Dave, Bob, "EUR", 10m)],
            first);
    }

    [Fact]
    public void Currencies_are_kept_separate()
    {
        var debts = BalanceCalculator.CalculateDebts(
            [Expense(Alice, 20m, "EUR", Alice, Bob), Expense(Bob, 3000m, "JPY", Alice, Bob)],
            [Payment(Bob, Alice, 4m, "EUR")]);

        Assert.Equal(
            [new Debt(Bob, Alice, "EUR", 6m), new Debt(Alice, Bob, "JPY", 1500m)],
            debts);
    }

    [Fact]
    public void Currency_codes_are_normalised()
    {
        var balances = BalanceCalculator.CalculateBalances(
            [Expense(Alice, 20m, "eur", Alice, Bob)],
            [Payment(Bob, Alice, 10m, " EUR ")]);

        Assert.Empty(balances);
    }

    [Fact]
    public void Rounded_splits_keep_balances_exact()
    {
        var balances = BalanceCalculator.CalculateBalances([Expense(Alice, 10m, Alice, Bob, Carol)], []);

        Assert.Equal(6.66m, BalanceOf(balances, Alice)); // Alice's own part is the 3.34 one
        Assert.Equal(-3.33m, BalanceOf(balances, Bob));
        Assert.Equal(-3.33m, BalanceOf(balances, Carol));
    }

    [Fact]
    public void SimplifyDebts_rejects_balances_that_do_not_add_up()
    {
        Assert.Throws<ArgumentException>(() =>
            BalanceCalculator.SimplifyDebts([new Balance(Alice, "EUR", 10m), new Balance(Bob, "EUR", -9m)]));
    }
}
