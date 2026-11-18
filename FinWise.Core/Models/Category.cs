namespace FinWise.Core.Models;

/// <summary>A category used to group transactions, such as "Groceries".</summary>
public class Category : Entity
{
    public string Name { get; set; } = string.Empty;

    /// <summary>Key of the icon to show; the UI maps it to a glyph or image.</summary>
    public string? Icon { get; set; }

    /// <summary>Display colour as <c>#RRGGBB</c>.</summary>
    public string? Color { get; set; }

    public int SortOrder { get; set; }

    /// <summary>True for the categories seeded on first run.</summary>
    public bool IsDefault { get; set; }
}
