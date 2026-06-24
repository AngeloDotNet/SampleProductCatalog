using ProductCatalog.Domain.Common;
using ProductCatalog.Domain.Events;
using ProductCatalog.Domain.Exceptions;
using ProductCatalog.Domain.ValueObjects;

namespace ProductCatalog.Domain.Entities;

public class Product : AggregateRoot
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Money Price { get; private set; }
    public int Stock { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Product() { } // EF Core

    public static Product Create(string name, string description, Money price, int stock)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Price = price,
            Stock = stock,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        product.AddDomainEvent(new ProductCreatedEvent(product.Id, product.Name, product.Price.Amount));
        return product;
    }

    public void UpdateDetails(string name, string description, Money price)
    {
        Name = name;
        Description = description;
        Price = price;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ProductUpdatedEvent(Id, Name, Price.Amount));
    }

    public void UpdateStock(int quantity)
    {
        if (Stock + quantity < 0)
            throw new DomainException("Stock cannot be negative");

        Stock += quantity;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ProductStockUpdatedEvent(Id, Stock));
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ProductDeactivatedEvent(Id));
    }
}