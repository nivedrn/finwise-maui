namespace FinWise.Core.Validation;

/// <summary>Thrown when an entity that breaks the domain rules is about to be saved.</summary>
public class ValidationException(IReadOnlyList<string> errors)
    : Exception(string.Join(" ", errors))
{
    public IReadOnlyList<string> Errors { get; } = errors;
}
