using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductCatalog.Domain.Common;
using RabbitMQ.Client;

namespace ProductCatalog.Infrastructure.Messaging.RabbitMQ;

public class RabbitMQEventBus : IEventBus, IDisposable
{
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly ILogger<RabbitMQEventBus> logger;
    private const string ExchangeName = "product_catalog_events";

    public RabbitMQEventBus(IOptions<RabbitMQSettings> settings, ILogger<RabbitMQEventBus> logger)
    {
        this.logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = settings.Value.Host,
            Port = settings.Value.Port,
            UserName = settings.Value.UserName,
            Password = settings.Value.Password,
            VirtualHost = settings.Value.VirtualHost
        };

        connection = factory.CreateConnection();
        channel = connection.CreateModel();

        // Declare exchange
        channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, durable: true);
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IDomainEvent
    {
        var eventName = @event.GetType().Name;
        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";
        properties.Headers = new Dictionary<string, object>
        {
            { "event-type", eventName },
            { "event-id", Guid.NewGuid().ToString() },
            { "timestamp", DateTimeOffset.UtcNow. ToUnixTimeSeconds() }
        };

        channel.BasicPublish(
            exchange: ExchangeName,
            routingKey: eventName,
            basicProperties: properties,
            body: body
        );

        logger.LogInformation("Published event {EventName} to RabbitMQ", eventName);

        await Task.CompletedTask;
    }

    public void Dispose()
    {
        channel?.Close();
        channel?.Dispose();
        connection?.Close();
        connection?.Dispose();
    }
}