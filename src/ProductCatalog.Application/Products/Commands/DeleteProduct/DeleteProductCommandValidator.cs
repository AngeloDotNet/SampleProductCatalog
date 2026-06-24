using ProductCatalog.Application.Common.Validation;

namespace ProductCatalog.Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandValidator : IValidator<DeleteProductCommand>
{
    public Task<ValidationResult> ValidateAsync(DeleteProductCommand instance, CancellationToken cancellationToken = default)
    {
        var errors = new List<ValidationFailure>();

        if (instance.Id == Guid.Empty)
        {
            errors.Add(new ValidationFailure("Id is required.", nameof(instance.Id)));
        }

        if (errors.Count > 0)
        {
            return Task.FromResult(ValidationResult.Failure(errors));
        }

        return Task.FromResult(ValidationResult.Success());
    }
}