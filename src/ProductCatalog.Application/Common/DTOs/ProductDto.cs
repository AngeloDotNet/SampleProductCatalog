namespace ProductCatalog.Application.Common.DTOs;

public record ProductDto(Guid Id, string Name, string Description, decimal Price, string Currency, int Stock, bool IsActive,
    DateTime CreatedAt, DateTime? UpdatedAt);