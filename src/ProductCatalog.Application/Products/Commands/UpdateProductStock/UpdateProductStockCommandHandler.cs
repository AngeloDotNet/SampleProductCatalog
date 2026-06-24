using ProductCatalog.Application.Common.Caching;
using ProductCatalog.Application.Common.Messaging;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Application.Queries.GetAllProducts;
using ProductCatalog.Application.Queries.GetProduct;

namespace ProductCatalog.Application.Products.Commands.UpdateProductStock;

public class UpdateProductStockCommandHandler(
    IProductRepository repository,
    IUnitOfWork unitOfWork,
    ICacheInvalidationService invalidationService) : IRequestHandler<UpdateProductStockCommand, Unit>
{
    public async Task<Unit> Handle(UpdateProductStockCommand request, CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdAsync(request.Id, cancellationToken) ?? throw new KeyNotFoundException($"Product with ID {request.Id} not found");

        product.UpdateStock(request.Quantity);

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
            // swallow; consider logging
        }

        return Unit.Value;
    }
}