namespace ProductCatalog.Application.Common.Validation;

public sealed class ValidationFailure(string message, string? field = null)
{
    /// <summary>
    /// Optional field/property name related to the error.
    /// </summary>
    public string? Field { get; } = field;

    /// <summary>
    /// Error message.
    /// </summary>
    public string Message { get; } = message;
}