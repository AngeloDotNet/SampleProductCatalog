using ProductCatalog.Domain.Common;

namespace ProductCatalog.Domain.Events;

public record ProductStockUpdatedEvent(Guid ProductId, int NewStock) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}