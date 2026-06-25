using ProductCatalog.Domain.Common;

namespace ProductCatalog.Domain.Events;

public record ProductDeactivatedEvent(Guid ProductId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}