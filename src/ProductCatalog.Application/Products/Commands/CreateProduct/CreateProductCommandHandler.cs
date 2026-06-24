using ProductCatalog.Application.Common.Caching;
using ProductCatalog.Application.Common.Messaging;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Application.Queries.GetAllProducts;
using ProductCatalog.Application.Queries.GetProduct;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.ValueObjects;

namespace ProductCatalog.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler(IProductRepository repository, IUnitOfWork unitOfWork, ICacheInvalidationService invalidationService) : IRequestHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken = default)
    {
        var money = new Money(request.Price, request.Currency);
        var product = Product.Create(request.Name, request.Description, money, request.Stock);

        await repository.AddAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Fine-grained invalidation: invalidate GetProduct (new id) and GetAllProducts
        try
        {
            var getProductQuery = new GetProductQuery(product.Id);
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
            // Non bloccare l'operazione se l'invalidation fallisce; loggare se necessario.
        }

        return product.Id;
    }
}