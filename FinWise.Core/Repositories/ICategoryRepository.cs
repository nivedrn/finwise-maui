using FinWise.Core.Models;

namespace FinWise.Core.Repositories;

/// <summary>Categories, ordered by <see cref="Category.SortOrder"/> and then by name.</summary>
public interface ICategoryRepository : IRepository<Category>
{
}
