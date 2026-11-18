namespace FinWise.Core.Models;

/// <summary>One person's part of a <see cref="Transaction"/>.</summary>
public class Split : Entity
{
    public Guid TransactionId { get; set; }

    public Guid PersonId { get; set; }

    /// <summary>What this person owes for the transaction, in the transaction's currency.</summary>
    public decimal Amount { get; set; }

    /// <summary>The weight used when the transaction is split by shares; otherwise null.</summary>
    public decimal? Shares { get; set; }
}
