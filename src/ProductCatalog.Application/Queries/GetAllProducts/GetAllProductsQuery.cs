using ProductCatalog.Application.Common.Caching;
using ProductCatalog.Application.Common.DTOs;
using ProductCatalog.Application.Common.Messaging;

namespace ProductCatalog.Application.Queries.GetAllProducts;

[Cacheable(30)] // cache for 30 seconds
public record GetAllProductsQuery() : IQuery<List<ProductDto>>;