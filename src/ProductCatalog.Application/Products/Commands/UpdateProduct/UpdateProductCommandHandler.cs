using ProductCatalog.Application.Common.Caching;
using ProductCatalog.Application.Common.Messaging;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Application.Queries.GetAllProducts;
using ProductCatalog.Application.Queries.GetProduct;
using ProductCatalog.Domain.ValueObjects;

namespace ProductCatalog.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler(IProductRepository repository, IUnitOfWork unitOfWork, ICacheInvalidationService invalidationService) : IRequestHandler<UpdateProductCommand, Unit>
{
    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdAsync(request.Id, cancellationToken) ?? throw new KeyNotFoundException($"Product with ID {request.Id} not found");
        var money = new Money(request.Price, request.Currency);

        product.UpdateDetails(request.Name, request.Description, money);

        await repository.UpdateAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Fine-grained invalidation: specific product and list
        try
        {
            var getProductQuery = new GetProductQuery(request.Id);
            var getAllQuery = new GetAllProductsQuery();

            var keys = new[]
            {
                    CacheKeyHelper.CreateCacheKey(getProductQuery),
                    CacheKeyHelper.CreateCacheKey(getAllQuery)
                };

            invalidationService?.InvalidateKeys(keys);
        }
        catch
        {
            // swallow to avoid breaking business flow; consider logging
        }

        return Unit.Value;
    }
}