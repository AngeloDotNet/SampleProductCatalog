using ProductCatalog.Application.Common.DTOs;
using ProductCatalog.Application.Common.Messaging;
using ProductCatalog.Application.Interfaces;

namespace ProductCatalog.Application.Queries.GetAllProducts;

public class GetAllProductsQueryHandler(IProductRepository repository) : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
{
    public async Task<List<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken = default)
    {
        var products = await repository.GetAllAsync(cancellationToken);

        return products.Select(p => new ProductDto(p.Id, p.Name, p.Description, p.Price.Amount, p.Price.Currency, p.Stock,
            p.IsActive, p.CreatedAt, p.UpdatedAt)).ToList();
    }
}