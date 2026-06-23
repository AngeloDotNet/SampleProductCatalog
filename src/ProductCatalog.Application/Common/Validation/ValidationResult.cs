namespace ProductCatalog.Application.Common.Validation;

public sealed class ValidationResult
{
    public bool IsValid { get; }
    public IReadOnlyList<ValidationFailure> Errors { get; }

    private ValidationResult(bool isValid, IEnumerable<ValidationFailure>? errors = null)
    {
        IsValid = isValid;
        Errors = (errors ?? []).ToList().AsReadOnly();
    }

    public static ValidationResult Success() => new ValidationResult(true);
    public static ValidationResult Failure(IEnumerable<ValidationFailure> errors) => new ValidationResult(false, errors);
}