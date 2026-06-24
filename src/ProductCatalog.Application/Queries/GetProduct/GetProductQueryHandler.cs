using ProductCatalog.Application.Common.DTOs;
using ProductCatalog.Application.Common.Messaging;
using ProductCatalog.Application.Interfaces;

namespace ProductCatalog.Application.Queries.GetProduct;

public class GetProductQueryHandler(IProductRepository repository) : IRequestHandler<GetProductQuery, ProductDto?>
{
    public async Task<ProductDto?> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (product == null)
        {
            return null;
        }

        return new ProductDto(product.Id, product.Name, product.Description, product.Price.Amount, product.Price.Currency,
            product.Stock, product.IsActive, product.CreatedAt, product.UpdatedAt);
    }
}