namespace FinWise.Core.Models;

/// <summary>A payment from one person to another that pays back what is owed.</summary>
public class Settlement : Entity
{
    /// <summary>The person who paid (the one who owed money).</summary>
    public Guid FromPersonId { get; set; }

    /// <summary>The person who received the money.</summary>
    public Guid ToPersonId { get; set; }

    /// <summary>Amount paid. Always positive.</summary>
    public decimal Amount { get; set; }

    /// <summary>ISO 4217 currency code, such as "EUR".</summary>
    public string CurrencyCode { get; set; } = string.Empty;

    /// <summary>The local date (and optionally time) of the payment.</summary>
    public DateTime Date { get; set; }

    public string? Note { get; set; }
}
