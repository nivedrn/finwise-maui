namespace FinWise.Core.Models;

/// <summary>An expense paid by one person, optionally shared with others.</summary>
public class Transaction : Entity
{
    public string Title { get; set; } = string.Empty;

    public string? Note { get; set; }

    /// <summary>Total amount paid. Always positive.</summary>
    public decimal Amount { get; set; }

    /// <summary>ISO 4217 currency code, such as "EUR".</summary>
    public string CurrencyCode { get; set; } = string.Empty;

    /// <summary>The local date (and optionally time) of the transaction.</summary>
    public DateTime Date { get; set; }

    public Guid? CategoryId { get; set; }

    /// <summary>The person who paid.</summary>
    public Guid PayerId { get; set; }

    public SplitMethod SplitMethod { get; set; }

    /// <summary>
    /// Who shares this transaction and what each of them owes. Empty when
    /// <see cref="SplitMethod"/> is <see cref="SplitMethod.None"/>. The payer usually has a split
    /// as well, for their own part.
    /// </summary>
    public List<Split> Splits { get; set; } = [];

    /// <summary>
    /// Replaces the splits with the result of a split calculation.
    /// </summary>
    public void SetSplits(SplitMethod method, IEnumerable<Calculations.SplitPart> parts)
    {
        SplitMethod = method;
        Splits = parts
            .Select(p => new Split
            {
                TransactionId = Id,
                PersonId = p.PersonId,
                Amount = p.Amount,
                Shares = method == SplitMethod.Shares ? p.Shares : null,
            })
            .ToList();
    }

    /// <summary>Removes all splits and marks the transaction as a personal expense.</summary>
    public void ClearSplits()
    {
        SplitMethod = SplitMethod.None;
        Splits = [];
    }
}
