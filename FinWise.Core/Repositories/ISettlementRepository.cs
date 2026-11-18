using FinWise.Core.Models;

namespace FinWise.Core.Repositories;

/// <summary>Settlements, newest first.</summary>
public interface ISettlementRepository : IRepository<Settlement>
{
    /// <summary>Settlements the person sent or received.</summary>
    Task<IReadOnlyList<Settlement>> GetForPersonAsync(Guid personId);
}
