using ProductCatalog.Application.Common.Messaging;
using ProductCatalog.Application.Queries.GetAllProducts;
using ProductCatalog.Application.Queries.GetProduct;

namespace ProductCatalog.Application.Products.Commands.CreateProduct;

// example: when product is created we want to invalidate GetAllProductsQuery and GetProductQuery caches
[Common.Caching.InvalidateCache(typeof(GetAllProductsQuery), typeof(GetProductQuery))]
public record CreateProductCommand(string Name, string Description, decimal Price, string Currency, int Stock) : IRequest<Guid>;