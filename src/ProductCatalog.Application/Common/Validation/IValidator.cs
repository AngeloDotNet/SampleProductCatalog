using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Application.Common.Validation;

public interface IValidator<T>
{
    /// <summary>
    /// Validates the instance and returns a ValidationResult.
    /// </summary>
    Task<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default);
}