using ProductCatalog.Application.Common.Caching;
using ProductCatalog.Application.Common.Messaging;
using ProductCatalog.Application.Queries.GetAllProducts;
using ProductCatalog.Application.Queries.GetProduct;

namespace ProductCatalog.Application.Products.Commands.UpdateProductStock;

public record UpdateProductStockCommand(Guid Id, int Quantity) : IRequest<Unit>, IInvalidateCacheKeys
{
    public IEnumerable<string> GetCacheKeysToInvalidate()
    {
        var keys = new List<string>();
        var getProductQuery = new GetProductQuery(Id);
        keys.Add(CacheKeyHelper.CreateCacheKey(getProductQuery));

        var getAllQuery = new GetAllProductsQuery();
        keys.Add(CacheKeyHelper.CreateCacheKey(getAllQuery));

        return keys;
    }
}