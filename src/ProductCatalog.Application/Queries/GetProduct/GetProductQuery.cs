using ProductCatalog.Application.Common.Caching;
using ProductCatalog.Application.Common.DTOs;
using ProductCatalog.Application.Common.Messaging;

namespace ProductCatalog.Application.Queries.GetProduct;

[Cacheable(60)] // cache for 60 seconds
public record GetProductQuery(Guid Id) : IQuery<ProductDto?>;