namespace FinWise.Core.Models;

/// <summary>How a transaction is divided between people. Values are persisted; do not renumber.</summary>
public enum SplitMethod
{
    /// <summary>Not shared: a personal expense of the payer. The transaction has no splits.</summary>
    None = 0,

    /// <summary>Divided equally between the participants.</summary>
    Equal = 1,

    /// <summary>Divided in proportion to each participant's <see cref="Split.Shares"/>.</summary>
    Shares = 2,

    /// <summary>Each participant owes an amount that was entered explicitly.</summary>
    Exact = 3,
}
