using ProductCatalog.Domain.Common;

namespace ProductCatalog.Domain.Events;

public record ProductCreatedEvent(Guid ProductId, string Name, decimal Price) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}