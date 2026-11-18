using FinWise.Core.Models;
using FinWise.Core.Repositories;
using FinWise.Data.Rows;
using SQLite;

namespace FinWise.Data.Repositories;

/// <summary>
/// Shared implementation of <see cref="IRepository{T}"/>: mapping between entity and row,
/// timestamps and soft delete. Subclasses add validation, ordering and child rows.
/// </summary>
public abstract class SqliteRepository<TEntity, TRow> : IRepository<TEntity>
    where TEntity : Entity
    where TRow : RowBase, new()
{
    protected SqliteRepository(FinWiseDatabase database)
    {
        Database = database;
    }

    protected FinWiseDatabase Database { get; }

    public async Task<TEntity?> GetAsync(Guid id)
    {
        var connection = await Database.GetConnectionAsync().ConfigureAwait(false);
        var row = await connection.FindAsync<TRow>(id).ConfigureAwait(false);
        if (row is null || row.IsDeleted)
            return null;

        var entities = await MaterializeAsync(connection, [row]).ConfigureAwait(false);
        return entities[0];
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(bool includeDeleted = false)
    {
        var connection = await Database.GetConnectionAsync().ConfigureAwait(false);
        var query = connection.Table<TRow>();
        if (!includeDeleted)
            query = query.Where(r => r.IsDeleted == false);

        var rows = await query.ToListAsync().ConfigureAwait(false);
        return Sort(await MaterializeAsync(connection, rows).ConfigureAwait(false)).ToList();
    }

    public async Task SaveAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        if (entity.Id == Guid.Empty)
            entity.Id = Guid.CreateVersion7();

        Normalize(entity);
        Validate(entity);

        var connection = await Database.GetConnectionAsync().ConfigureAwait(false);
        var now = Database.Clock.GetUtcNow();
        await connection.RunInTransactionAsync(c =>
        {
            var existing = c.Find<TRow>(entity.Id);
            entity.CreatedAt = existing?.CreatedAt ?? now;
            entity.UpdatedAt = now;
            c.InsertOrReplace(ToRow(entity));
            AfterSave(c, entity, now);
        }).ConfigureAwait(false);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var connection = await Database.GetConnectionAsync().ConfigureAwait(false);
        var now = Database.Clock.GetUtcNow();
        var deleted = false;
        await connection.RunInTransactionAsync(c =>
        {
            var row = c.Find<TRow>(id);
            if (row is null || row.IsDeleted)
                return;

            row.IsDeleted = true;
            row.UpdatedAt = now;
            c.Update(row);
            AfterDelete(c, id, now);
            deleted = true;
        }).ConfigureAwait(false);
        return deleted;
    }

    protected abstract TEntity ToEntity(TRow row);

    protected abstract TRow ToRow(TEntity entity);

    /// <summary>Throws a <see cref="Core.Validation.ValidationException"/> for invalid entities.</summary>
    protected abstract void Validate(TEntity entity);

    /// <summary>Cleans up input (trimming, upper-casing codes) before validation.</summary>
    protected virtual void Normalize(TEntity entity)
    {
    }

    /// <summary>The order in which <see cref="GetAllAsync"/> returns entities.</summary>
    protected abstract IEnumerable<TEntity> Sort(IEnumerable<TEntity> entities);

    /// <summary>Converts rows to entities, loading any child rows.</summary>
    protected virtual Task<List<TEntity>> MaterializeAsync(SQLiteAsyncConnection connection, IReadOnlyList<TRow> rows) =>
        Task.FromResult(rows.Select(ToEntity).ToList());

    /// <summary>Runs inside the save transaction after the row has been written.</summary>
    protected virtual void AfterSave(SQLiteConnection connection, TEntity entity, DateTimeOffset now)
    {
    }

    /// <summary>Runs inside the delete transaction after the row has been soft-deleted.</summary>
    protected virtual void AfterDelete(SQLiteConnection connection, Guid id, DateTimeOffset now)
    {
    }

    protected static void CopyCommon(Entity source, RowBase target)
    {
        target.Id = source.Id;
        target.CreatedAt = source.CreatedAt;
        target.UpdatedAt = source.UpdatedAt;
        target.IsDeleted = source.IsDeleted;
    }

    protected static void CopyCommon(RowBase source, Entity target)
    {
        target.Id = source.Id;
        target.CreatedAt = source.CreatedAt;
        target.UpdatedAt = source.UpdatedAt;
        target.IsDeleted = source.IsDeleted;
    }
}
