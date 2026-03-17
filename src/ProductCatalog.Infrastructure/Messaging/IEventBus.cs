using ProductCatalog.Domain.Common;

namespace ProductCatalog.Infrastructure.Messaging;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IDomainEvent;
}