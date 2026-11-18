namespace FinWise.Core.Calculations;

/// <summary>One person's part of a split, as produced by <see cref="SplitCalculator"/>.</summary>
/// <param name="PersonId">The participant.</param>
/// <param name="Amount">What the participant owes.</param>
/// <param name="Shares">The weight that was used, for splits by shares; otherwise null.</param>
public sealed record SplitPart(Guid PersonId, decimal Amount, decimal? Shares = null);
