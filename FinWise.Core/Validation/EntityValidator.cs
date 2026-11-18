using FinWise.Core.Models;

namespace FinWise.Core.Validation;

/// <summary>
/// Domain rules for entities. Repositories call <c>EnsureValid</c> before saving; the UI can call
/// <c>Validate</c> to show the errors.
/// </summary>
public static class EntityValidator
{
    public static IReadOnlyList<string> Validate(Person person)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(person.Name))
            errors.Add("A person needs a name.");
        return errors;
    }

    public static IReadOnlyList<string> Validate(Category category)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(category.Name))
            errors.Add("A category needs a name.");
        return errors;
    }

    public static IReadOnlyList<string> Validate(Transaction transaction)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(transaction.Title))
            errors.Add("A transaction needs a title.");
        if (transaction.PayerId == Guid.Empty)
            errors.Add("A transaction needs a payer.");

        ValidateAmount(transaction.Amount, transaction.CurrencyCode, errors);

        var splits = transaction.Splits.Where(s => !s.IsDeleted).ToList();
        if (transaction.SplitMethod == SplitMethod.None)
        {
            if (splits.Count > 0)
                errors.Add("A transaction that is not split cannot have splits.");
            return errors;
        }

        if (!Enum.IsDefined(transaction.SplitMethod))
        {
            errors.Add($"Unknown split method {(int)transaction.SplitMethod}.");
            return errors;
        }

        if (splits.Count == 0)
        {
            errors.Add("A split transaction needs at least one participant.");
            return errors;
        }

        if (splits.Any(s => s.PersonId == Guid.Empty))
            errors.Add("Every split needs a person.");
        if (splits.Select(s => s.PersonId).Distinct().Count() != splits.Count)
            errors.Add("A person can only appear once in the splits of a transaction.");
        if (splits.Any(s => s.Amount < 0))
            errors.Add("Split amounts cannot be negative.");
        if (Currency.IsValidCode(transaction.CurrencyCode) &&
            splits.Any(s => !Currency.HasValidPrecision(s.Amount, transaction.CurrencyCode)))
            errors.Add($"Split amounts can have at most {Currency.GetMinorUnitDigits(transaction.CurrencyCode)} decimal places in {transaction.CurrencyCode}.");

        var sum = splits.Sum(s => s.Amount);
        if (sum != transaction.Amount)
            errors.Add($"The splits add up to {sum} but the transaction amount is {transaction.Amount}.");

        if (transaction.SplitMethod == SplitMethod.Shares)
        {
            if (splits.Any(s => s.Shares is null or < 0))
                errors.Add("When splitting by shares, every participant needs a share of zero or more.");
            else if (splits.Sum(s => s.Shares!.Value) <= 0)
                errors.Add("When splitting by shares, at least one share must be positive.");
            else if (splits.Any(s => decimal.Round(s.Shares!.Value, Currency.MaxMinorUnitDigits) != s.Shares.Value))
                errors.Add($"Shares can have at most {Currency.MaxMinorUnitDigits} decimal places.");
        }

        return errors;
    }

    public static IReadOnlyList<string> Validate(Settlement settlement)
    {
        var errors = new List<string>();
        if (settlement.FromPersonId == Guid.Empty || settlement.ToPersonId == Guid.Empty)
            errors.Add("A settlement needs both a sender and a receiver.");
        else if (settlement.FromPersonId == settlement.ToPersonId)
            errors.Add("A person cannot settle with themselves.");
        ValidateAmount(settlement.Amount, settlement.CurrencyCode, errors);
        return errors;
    }

    public static void EnsureValid(Person person) => Throw(Validate(person));

    public static void EnsureValid(Category category) => Throw(Validate(category));

    public static void EnsureValid(Transaction transaction) => Throw(Validate(transaction));

    public static void EnsureValid(Settlement settlement) => Throw(Validate(settlement));

    private static void ValidateAmount(decimal amount, string currencyCode, List<string> errors)
    {
        if (amount <= 0)
            errors.Add("The amount must be greater than zero.");

        if (!Currency.IsValidCode(currencyCode))
            errors.Add($"'{currencyCode}' is not a valid ISO 4217 currency code.");
        else if (!Currency.HasValidPrecision(amount, currencyCode))
            errors.Add($"Amounts in {currencyCode} can have at most {Currency.GetMinorUnitDigits(currencyCode)} decimal places.");
    }

    private static void Throw(IReadOnlyList<string> errors)
    {
        if (errors.Count > 0)
            throw new ValidationException(errors);
    }
}
