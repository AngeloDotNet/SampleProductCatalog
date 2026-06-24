using ProductCatalog.Application.Common.Caching;
using ProductCatalog.Application.Common.Messaging;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Application.Queries.GetAllProducts;
using ProductCatalog.Application.Queries.GetProduct;

namespace ProductCatalog.Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler(
    IProductRepository repository,
    IUnitOfWork unitOfWork,
    ICacheInvalidationService invalidationService) : IRequestHandler<DeleteProductCommand, Unit>
{
    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdAsync(request.Id, cancellationToken) ?? throw new KeyNotFoundException($"Product with ID {request.Id} not found");

        await repository.DeleteAsync(product, cancellationToken);
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
            // swallow to avoid breaking flow; consider logging
        }

        return Unit.Value;
    }
}