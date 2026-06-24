using ProductCatalog.Application.Common.Validation;

namespace ProductCatalog.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandValidator : IValidator<UpdateProductCommand>
{
    public Task<ValidationResult> ValidateAsync(UpdateProductCommand instance, CancellationToken cancellationToken = default)
    {
        var errors = new List<ValidationFailure>();

        if (instance.Id == Guid.Empty)
            errors.Add(new ValidationFailure("Id is required.", nameof(instance.Id)));

        if (string.IsNullOrWhiteSpace(instance.Name))
            errors.Add(new ValidationFailure("Name is required.", nameof(instance.Name)));

        if (instance.Price <= 0)
            errors.Add(new ValidationFailure("Price must be greater than zero.", nameof(instance.Price)));

        if (string.IsNullOrWhiteSpace(instance.Currency) || instance.Currency.Length != 3)
            errors.Add(new ValidationFailure("Currency must be a 3-letter ISO code.", nameof(instance.Currency)));

        if (errors.Count > 0)
            return Task.FromResult(ValidationResult.Failure(errors));

        return Task.FromResult(ValidationResult.Success());
    }
}