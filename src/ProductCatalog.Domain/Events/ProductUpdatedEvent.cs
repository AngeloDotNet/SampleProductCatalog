using ProductCatalog.Domain.Common;

namespace ProductCatalog.Domain.Events;

public record ProductUpdatedEvent(Guid ProductId, string Name, decimal Price) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}