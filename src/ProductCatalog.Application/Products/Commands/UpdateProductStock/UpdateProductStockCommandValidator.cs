using ProductCatalog.Application.Common.Validation;

namespace ProductCatalog.Application.Products.Commands.UpdateProductStock;

public class UpdateProductStockCommandValidator : IValidator<UpdateProductStockCommand>
{
    public Task<ValidationResult> ValidateAsync(UpdateProductStockCommand instance, CancellationToken cancellationToken = default)
    {
        var errors = new List<ValidationFailure>();

        if (instance.Id == Guid.Empty)
            errors.Add(new ValidationFailure("Id is required.", nameof(instance.Id)));

        if (instance.Quantity == 0)
            errors.Add(new ValidationFailure("Quantity must be non-zero.", nameof(instance.Quantity)));

        if (errors.Count > 0)
            return Task.FromResult(ValidationResult.Failure(errors));

        return Task.FromResult(ValidationResult.Success());
    }
}