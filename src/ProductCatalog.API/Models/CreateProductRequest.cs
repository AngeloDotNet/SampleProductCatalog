namespace ProductCatalog.API.Models;

public record CreateProductRequest(string Name, string Description, decimal Price, string Currency, int Stock);