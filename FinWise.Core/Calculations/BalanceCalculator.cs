using FinWise.Core.Models;

namespace FinWise.Core.Calculations;

/// <summary>A person's net position in one currency.</summary>
/// <param name="Amount">Positive if others owe this person money, negative if they owe others.</param>
public sealed record Balance(Guid PersonId, string CurrencyCode, decimal Amount);

/// <summary>A payment that would settle part of the balances: <c>From</c> pays <c>To</c>.</summary>
public sealed record Debt(Guid FromPersonId, Guid ToPersonId, string CurrencyCode, decimal Amount);

/// <summary>
/// Works out who owes whom from transactions, their splits and settlements.
/// </summary>
/// <remarks>
/// Currencies are never mixed: every balance and debt is per currency.
/// Deleted transactions, splits and settlements are ignored.
/// </remarks>
public static class BalanceCalculator
{
    /// <summary>
    /// Net balance per person and currency. People whose balance is zero are left out.
    /// </summary>
    /// <remarks>
    /// For each transaction, the payer is credited with what the participants owe and each
    /// participant is debited with their split. The payer's own split cancels out, and a
    /// transaction without splits (a personal expense) has no effect. For each settlement the
    /// sender's balance goes up and the receiver's goes down. All balances in a currency therefore
    /// always add up to zero.
    /// </remarks>
    public static IReadOnlyList<Balance> CalculateBalances(
        IEnumerable<Transaction> transactions,
        IEnumerable<Settlement> settlements)
    {
        var totals = new Dictionary<(Guid PersonId, string Currency), decimal>();

        void Add(Guid personId, string currency, decimal amount)
        {
            var key = (personId, currency);
            totals[key] = totals.GetValueOrDefault(key) + amount;
        }

        foreach (var transaction in transactions.Where(t => !t.IsDeleted))
        {
            var currency = Currency.Normalize(transaction.CurrencyCode);
            foreach (var split in transaction.Splits.Where(s => !s.IsDeleted))
            {
                Add(transaction.PayerId, currency, split.Amount);
                Add(split.PersonId, currency, -split.Amount);
            }
        }

        foreach (var settlement in settlements.Where(s => !s.IsDeleted))
        {
            var currency = Currency.Normalize(settlement.CurrencyCode);
            Add(settlement.FromPersonId, currency, settlement.Amount);
            Add(settlement.ToPersonId, currency, -settlement.Amount);
        }

        return totals
            .Where(kv => kv.Value != 0)
            .Select(kv => new Balance(kv.Key.PersonId, kv.Key.Currency, kv.Value))
            .OrderBy(b => b.CurrencyCode, StringComparer.Ordinal)
            .ThenByDescending(b => b.Amount)
            .ThenBy(b => b.PersonId)
            .ToList();
    }

    /// <summary>
    /// Turns net balances into a short list of payments that would settle everyone up.
    /// </summary>
    /// <remarks>
    /// Per currency, the person who owes the most repeatedly pays the person who is owed the most,
    /// as much as possible. This needs at most one payment fewer than the number of people with a
    /// non-zero balance. Ties are broken by person id, so the result is deterministic.
    /// </remarks>
    /// <exception cref="ArgumentException">The balances of a currency do not add up to zero.</exception>
    public static IReadOnlyList<Debt> SimplifyDebts(IEnumerable<Balance> balances)
    {
        var debts = new List<Debt>();

        foreach (var group in balances.GroupBy(b => Currency.Normalize(b.CurrencyCode)).OrderBy(g => g.Key, StringComparer.Ordinal))
        {
            // Combine duplicate entries for the same person, just in case.
            var net = group
                .GroupBy(b => b.PersonId)
                .Select(g => (PersonId: g.Key, Amount: g.Sum(b => b.Amount)))
                .ToList();

            if (net.Sum(n => n.Amount) != 0)
                throw new ArgumentException($"Balances in {group.Key} do not add up to zero.", nameof(balances));

            var creditors = net.Where(n => n.Amount > 0)
                .OrderByDescending(n => n.Amount).ThenBy(n => n.PersonId)
                .Select(n => new Party(n.PersonId, n.Amount)).ToList();
            var debtors = net.Where(n => n.Amount < 0)
                .OrderByDescending(n => -n.Amount).ThenBy(n => n.PersonId)
                .Select(n => new Party(n.PersonId, -n.Amount)).ToList();

            int c = 0, d = 0;
            while (c < creditors.Count && d < debtors.Count)
            {
                var payment = Math.Min(creditors[c].Remaining, debtors[d].Remaining);
                debts.Add(new Debt(debtors[d].PersonId, creditors[c].PersonId, group.Key, payment));

                creditors[c].Remaining -= payment;
                debtors[d].Remaining -= payment;
                if (creditors[c].Remaining == 0) c++;
                if (debtors[d].Remaining == 0) d++;
            }
        }

        return debts;
    }

    /// <summary>Shortcut for <see cref="CalculateBalances"/> followed by <see cref="SimplifyDebts"/>.</summary>
    public static IReadOnlyList<Debt> CalculateDebts(
        IEnumerable<Transaction> transactions,
        IEnumerable<Settlement> settlements) =>
        SimplifyDebts(CalculateBalances(transactions, settlements));

    private sealed class Party(Guid personId, decimal remaining)
    {
        public Guid PersonId { get; } = personId;
        public decimal Remaining { get; set; } = remaining;
    }
}
