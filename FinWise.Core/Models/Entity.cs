namespace FinWise.Core.Models;

/// <summary>
/// Base class for everything FinWise stores.
/// </summary>
/// <remarks>
/// Ids are generated on the client (time-ordered GUID v7) and timestamps are kept in UTC, so that
/// records created offline on several devices can later be merged by a sync service.
/// Rows are never physically deleted: <see cref="IsDeleted"/> acts as a tombstone that sync can
/// propagate. That applies to every entity, including splits that are removed when a transaction
/// is edited.
/// </remarks>
public abstract class Entity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    /// <summary>When the record was first saved (UTC). Set by the repository.</summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>When the record was last saved or deleted (UTC). Set by the repository.</summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>Soft-delete flag. Deleted records are hidden from normal queries.</summary>
    public bool IsDeleted { get; set; }
}
