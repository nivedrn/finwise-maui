using SQLite;

namespace FinWise.Data.Rows;

// Storage shapes of the domain entities. They are kept separate from FinWise.Core so the domain
// model has no storage attributes, and so money can be stored exactly: sqlite-net would store a
// decimal as a REAL (double). Amounts are stored as integers in units of 1/10,000 (see Money),
// which covers every ISO 4217 currency and still allows SUM/ORDER BY in SQL.
//
// The tables themselves are created by explicit SQL in SchemaMigrations, not by
// CreateTable<T>(), so each schema version is fixed. Column names must match the property names.

public abstract class RowBase
{
    [PrimaryKey]
    public Guid Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }
}

[Table("people")]
public sealed class PersonRow : RowBase
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsCurrentUser { get; set; }
}

[Table("categories")]
public sealed class CategoryRow : RowBase
{
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public int SortOrder { get; set; }
    public bool IsDefault { get; set; }
}

[Table("transactions")]
public sealed class TransactionRow : RowBase
{
    public string Title { get; set; } = string.Empty;
    public string? Note { get; set; }
    public long AmountE4 { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid PayerId { get; set; }
    public int SplitMethod { get; set; }
}

[Table("splits")]
public sealed class SplitRow : RowBase
{
    public Guid TransactionId { get; set; }
    public Guid PersonId { get; set; }
    public long AmountE4 { get; set; }
    public long? SharesE4 { get; set; }
}

[Table("settlements")]
public sealed class SettlementRow : RowBase
{
    public Guid FromPersonId { get; set; }
    public Guid ToPersonId { get; set; }
    public long AmountE4 { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? Note { get; set; }
}
