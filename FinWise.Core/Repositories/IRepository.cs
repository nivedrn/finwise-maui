using FinWise.Core.Models;

namespace FinWise.Core.Repositories;

/// <summary>Basic storage operations shared by all entities.</summary>
public interface IRepository<T> where T : Entity
{
    /// <summary>Returns the entity, or null if it does not exist or has been deleted.</summary>
    Task<T?> GetAsync(Guid id);

    /// <summary>Returns all entities, by default without the deleted ones.</summary>
    Task<IReadOnlyList<T>> GetAllAsync(bool includeDeleted = false);

    /// <summary>
    /// Inserts or updates the entity after validating it. Sets <see cref="Entity.CreatedAt"/> and
    /// <see cref="Entity.UpdatedAt"/> on the instance that is passed in.
    /// </summary>
    /// <exception cref="Validation.ValidationException">The entity breaks a domain rule.</exception>
    Task SaveAsync(T entity);

    /// <summary>Soft-deletes the entity. Returns false if it did not exist or was already deleted.</summary>
    Task<bool> DeleteAsync(Guid id);
}
