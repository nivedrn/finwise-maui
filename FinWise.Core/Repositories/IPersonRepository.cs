using FinWise.Core.Models;

namespace FinWise.Core.Repositories;

public interface IPersonRepository : IRepository<Person>
{
    /// <summary>The person marked as the app's user, or null if none has been set up yet.</summary>
    Task<Person?> GetCurrentUserAsync();
}
