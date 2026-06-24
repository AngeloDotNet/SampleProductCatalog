using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task DeleteAsync(Product product, CancellationToken cancellationToken = default);
}