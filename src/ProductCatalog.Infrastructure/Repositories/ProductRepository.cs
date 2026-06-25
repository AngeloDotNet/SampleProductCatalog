using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Infrastructure.Persistence;

namespace ProductCatalog.Infrastructure.Repositories;

public class ProductRepository(ApplicationDbContext context) : IProductRepository
{
    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Products.FindAsync([id], cancellationToken);
    }

    public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Products.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await context.Products.AddAsync(product, cancellationToken);
    }

    public Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        context.Products.Update(product);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
    {
        context.Products.Remove(product);
        return Task.CompletedTask;
    }
}