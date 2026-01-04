namespace CleanArchitectureApi.Application.Interfaces;

public interface IMessageQueueService
{
    Task PublishAsync<T>(string queueName, T message, CancellationToken cancellationToken = default);
    Task PublishAsync<T>(string exchangeName, string routingKey, T message, CancellationToken cancellationToken = default);
    Task SubscribeAsync<T>(string queueName, Func<T, Task> handler, CancellationToken cancellationToken = default);
    Task CreateQueueAsync(string queueName, bool durable = true, CancellationToken cancellationToken = default);
    Task CreateExchangeAsync(string exchangeName, string type = "direct", bool durable = true, CancellationToken cancellationToken = default);
    Task BindQueueAsync(string queueName, string exchangeName, string routingKey, CancellationToken cancellationToken = default);
}

