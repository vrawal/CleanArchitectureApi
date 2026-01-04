using System.Collections.Concurrent;
using System.Text.Json;
using CleanArchitectureApi.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureApi.Infrastructure.Services;

public class MessageQueueService : IMessageQueueService, IDisposable
{
    private readonly ILogger<MessageQueueService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ConcurrentDictionary<string, ConcurrentQueue<string>> _queues;
    private readonly ConcurrentDictionary<string, List<Func<object, Task>>> _subscribers;
    private bool _disposed = false;

    public MessageQueueService(IConfiguration configuration, ILogger<MessageQueueService> logger)
    {
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
        _queues = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
        _subscribers = new ConcurrentDictionary<string, List<Func<object, Task>>>();

        _logger.LogInformation("In-memory message queue service initialized");
    }

    public async Task PublishAsync<T>(string queueName, T message, CancellationToken cancellationToken = default)
    {
        try
        {
            await CreateQueueAsync(queueName, true, cancellationToken);

            var messageBody = JsonSerializer.Serialize(message, _jsonOptions);
            var queue = _queues.GetOrAdd(queueName, _ => new ConcurrentQueue<string>());
            queue.Enqueue(messageBody);

            _logger.LogDebug("Message published to queue: {QueueName}", queueName);

            // Process subscribers immediately for in-memory implementation
            if (_subscribers.TryGetValue(queueName, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    try
                    {
                        await handler(message!);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing message in queue: {QueueName}", queueName);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing message to queue: {QueueName}", queueName);
            throw;
        }
    }

    public async Task PublishAsync<T>(string exchangeName, string routingKey, T message, CancellationToken cancellationToken = default)
    {
        // For simplicity, treat exchange + routing key as queue name
        var queueName = $"{exchangeName}:{routingKey}";
        await PublishAsync(queueName, message, cancellationToken);
    }

    public async Task SubscribeAsync<T>(string queueName, Func<T, Task> handler, CancellationToken cancellationToken = default)
    {
        try
        {
            await CreateQueueAsync(queueName, true, cancellationToken);

            var subscribers = _subscribers.GetOrAdd(queueName, _ => new List<Func<object, Task>>());
            subscribers.Add(async (obj) =>
            {
                if (obj is T typedMessage)
                {
                    await handler(typedMessage);
                }
            });

            _logger.LogInformation("Started consuming messages from queue: {QueueName}", queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error subscribing to queue: {QueueName}", queueName);
            throw;
        }
    }

    public Task CreateQueueAsync(string queueName, bool durable = true, CancellationToken cancellationToken = default)
    {
        try
        {
            _queues.GetOrAdd(queueName, _ => new ConcurrentQueue<string>());
            _logger.LogDebug("Queue declared: {QueueName}", queueName);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating queue: {QueueName}", queueName);
            throw;
        }
    }

    public Task CreateExchangeAsync(string exchangeName, string type = "direct", bool durable = true, CancellationToken cancellationToken = default)
    {
        try
        {
            // For in-memory implementation, exchanges are just logical concepts
            _logger.LogDebug("Exchange declared: {ExchangeName} of type: {Type}", exchangeName, type);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating exchange: {ExchangeName}", exchangeName);
            throw;
        }
    }

    public Task BindQueueAsync(string queueName, string exchangeName, string routingKey, CancellationToken cancellationToken = default)
    {
        try
        {
            // For in-memory implementation, binding is just logical
            _logger.LogDebug("Queue bound: {QueueName} to exchange: {ExchangeName} with routing key: {RoutingKey}", 
                queueName, exchangeName, routingKey);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error binding queue: {QueueName} to exchange: {ExchangeName}", queueName, exchangeName);
            throw;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _queues.Clear();
            _subscribers.Clear();
            _disposed = true;
            _logger.LogInformation("In-memory message queue service disposed");
        }
    }
}

