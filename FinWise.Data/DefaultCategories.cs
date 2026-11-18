using FinWise.Data.Rows;

namespace FinWise.Data;

/// <summary>
/// The categories seeded into a new database. Their ids are fixed so that the same default
/// category has the same id on every device, which keeps future sync from creating duplicates.
/// </summary>
public static class DefaultCategories
{
    public static readonly IReadOnlyList<(Guid Id, string Name, string Icon, string Color)> All =
    [
        (new Guid("6f0c0b6e-3b1a-4c55-9f3e-000000000001"), "Food & Drink", "food", "#E57373"),
        (new Guid("6f0c0b6e-3b1a-4c55-9f3e-000000000002"), "Groceries", "groceries", "#81C784"),
        (new Guid("6f0c0b6e-3b1a-4c55-9f3e-000000000003"), "Transport", "transport", "#64B5F6"),
        (new Guid("6f0c0b6e-3b1a-4c55-9f3e-000000000004"), "Housing", "housing", "#A1887F"),
        (new Guid("6f0c0b6e-3b1a-4c55-9f3e-000000000005"), "Utilities", "utilities", "#FFD54F"),
        (new Guid("6f0c0b6e-3b1a-4c55-9f3e-000000000006"), "Entertainment", "entertainment", "#BA68C8"),
        (new Guid("6f0c0b6e-3b1a-4c55-9f3e-000000000007"), "Shopping", "shopping", "#F06292"),
        (new Guid("6f0c0b6e-3b1a-4c55-9f3e-000000000008"), "Health", "health", "#4DB6AC"),
        (new Guid("6f0c0b6e-3b1a-4c55-9f3e-000000000009"), "Travel", "travel", "#4FC3F7"),
        (new Guid("6f0c0b6e-3b1a-4c55-9f3e-00000000000a"), "Other", "other", "#90A4AE"),
    ];

    internal static IEnumerable<CategoryRow> Create(DateTimeOffset now) =>
        All.Select((c, i) => new CategoryRow
        {
            Id = c.Id,
            Name = c.Name,
            Icon = c.Icon,
            Color = c.Color,
            SortOrder = i,
            IsDefault = true,
            CreatedAt = now,
            UpdatedAt = now,
        });
}
