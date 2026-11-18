using FinWise.Core.Models;

namespace FinWise.Core.Repositories;

/// <summary>
/// Transactions together with their splits. Saving a transaction saves its splits in the same
/// database transaction, and deleting it deletes its splits. Results are newest first.
/// </summary>
public interface ITransactionRepository : IRepository<Transaction>
{
    /// <summary>Transactions that the person paid for or has a split in.</summary>
    Task<IReadOnlyList<Transaction>> GetForPersonAsync(Guid personId);

    /// <summary>Transactions dated on or after <paramref name="from"/> and before <paramref name="to"/>.</summary>
    Task<IReadOnlyList<Transaction>> GetBetweenAsync(DateTime from, DateTime to);
}
