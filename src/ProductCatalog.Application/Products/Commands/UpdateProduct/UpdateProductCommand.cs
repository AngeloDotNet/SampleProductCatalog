using ProductCatalog.Application.Common.Caching;
using ProductCatalog.Application.Common.Messaging;
using ProductCatalog.Application.Queries.GetAllProducts;
using ProductCatalog.Application.Queries.GetProduct;

namespace ProductCatalog.Application.Products.Commands.UpdateProduct;

public record UpdateProductCommand(Guid Id, string Name, string Description, decimal Price, string Currency) : IRequest<Unit>, IInvalidateCacheKeys
{
    public IEnumerable<string> GetCacheKeysToInvalidate()
    {
        // fine-grained eviction of GetProductQuery for this Id + invalidate GetAllProductsQuery
        var keys = new List<string>();

        // exact product cache key
        var getProductQuery = new GetProductQuery(Id);
        keys.Add(CacheKeyHelper.CreateCacheKey(getProductQuery));

        // clear list cache
        var getAllQuery = new GetAllProductsQuery();
        keys.Add(CacheKeyHelper.CreateCacheKey(getAllQuery));

        return keys;
    }
}