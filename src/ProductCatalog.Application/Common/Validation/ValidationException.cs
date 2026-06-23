using System.Text.Json;

namespace ProductCatalog.Application.Common.Validation;

public class ValidationException(IEnumerable<ValidationFailure> errors) : Exception("Validation failed for one or more commands/requests.")
{
    public IReadOnlyList<ValidationFailure> Errors { get; } = new List<ValidationFailure>(errors).AsReadOnly();

    public override string ToString() => $"{Message} Errors: {JsonSerializer.Serialize(Errors)}";
}