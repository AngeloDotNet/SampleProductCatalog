using ProductCatalog.Application.Common.Validation;

namespace ProductCatalog.Application.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : IValidator<CreateProductCommand>
{
    public Task<ValidationResult> ValidateAsync(CreateProductCommand instance, CancellationToken cancellationToken = default)
    {
        var errors = new List<ValidationFailure>();

        if (string.IsNullOrWhiteSpace(instance.Name))
            errors.Add(new ValidationFailure("Name is required.", nameof(instance.Name)));

        if (instance.Price <= 0)
            errors.Add(new ValidationFailure("Price must be greater than zero.", nameof(instance.Price)));

        if (string.IsNullOrWhiteSpace(instance.Currency) || instance.Currency.Length != 3)
            errors.Add(new ValidationFailure("Currency must be a 3-letter ISO code.", nameof(instance.Currency)));

        if (instance.Stock < 0)
            errors.Add(new ValidationFailure("Stock cannot be negative.", nameof(instance.Stock)));

        if (errors.Count > 0)
            return Task.FromResult(ValidationResult.Failure(errors));

        return Task.FromResult(ValidationResult.Success());
    }
}