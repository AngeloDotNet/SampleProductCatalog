namespace ProductCatalog.API.Models;

public record UpdateProductRequest(string Name, string Description, decimal Price, string Currency);