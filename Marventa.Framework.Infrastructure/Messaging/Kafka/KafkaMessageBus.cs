using Confluent.Kafka;
using Marventa.Framework.Core.Interfaces.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Marventa.Framework.Infrastructure.Messaging.Kafka;

public class KafkaMessageBus : IMessageBus, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaMessageBus> _logger;
    private readonly KafkaOptions _options;

    public KafkaMessageBus(IOptions<KafkaOptions> options, ILogger<KafkaMessageBus> logger)
    {
        _options = options.Value;
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            Acks = _options.Acks,
            EnableIdempotence = _options.EnableIdempotence,
            MaxInFlight = _options.MaxInFlight,
            MessageTimeoutMs = _options.MessageTimeoutMs,
            RequestTimeoutMs = _options.RequestTimeoutMs,
            RetryBackoffMs = _options.RetryBackoffMs
        };

        // Add security configurations if provided
        if (!string.IsNullOrEmpty(_options.SecurityProtocol))
        {
            config.SecurityProtocol = Enum.Parse<SecurityProtocol>(_options.SecurityProtocol);
        }

        if (!string.IsNullOrEmpty(_options.SaslMechanism))
        {
            config.SaslMechanism = Enum.Parse<SaslMechanism>(_options.SaslMechanism);
            config.SaslUsername = _options.SaslUsername;
            config.SaslPassword = _options.SaslPassword;
        }

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var topicName = GetTopicName<T>();
            var messageKey = GetMessageKey(message);
            var messageJson = JsonSerializer.Serialize(message);

            _logger.LogDebug("Publishing message {MessageType} to topic {TopicName}: {@Message}",
                typeof(T).Name, topicName, message);

            var kafkaMessage = new Message<string, string>
            {
                Key = messageKey,
                Value = messageJson,
                Headers = new Headers
                {
                    { "MessageType", System.Text.Encoding.UTF8.GetBytes(typeof(T).Name) },
                    { "Timestamp", System.Text.Encoding.UTF8.GetBytes(DateTimeOffset.UtcNow.ToString()) }
                }
            };

            var result = await _producer.ProduceAsync(topicName, kafkaMessage, cancellationToken);

            _logger.LogInformation("Successfully published message {MessageType} to topic {TopicName}, Partition: {Partition}, Offset: {Offset}",
                typeof(T).Name, topicName, result.Partition.Value, result.Offset.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message {MessageType}", typeof(T).Name);
            throw;
        }
    }

    public async Task SendAsync<T>(T command, CancellationToken cancellationToken = default) where T : class
    {
        // In Kafka, sending is similar to publishing but typically uses command-specific topics
        await PublishAsync(command, cancellationToken);
    }

    public Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class
        where TResponse : class
    {
        // Request-Response pattern in Kafka requires correlation IDs and response topics
        // This is a simplified implementation - in production, you'd want a more sophisticated correlation mechanism
        throw new NotImplementedException("Request-Response pattern requires additional correlation infrastructure in Kafka. Consider using a separate correlation service or use RabbitMQ for request-response patterns.");
    }

    private string GetTopicName<T>()
    {
        var messageType = typeof(T);

        // Check if custom topic mapping exists
        if (_options.TopicMappings.TryGetValue(messageType.Name, out var customTopic))
        {
            return customTopic;
        }

        // Default topic naming convention
        return $"{_options.TopicPrefix}{messageType.Name.ToLowerInvariant()}";
    }

    private string GetMessageKey<T>(T message)
    {
        // Try to get key from message properties (Id, Key, etc.)
        var messageType = typeof(T);
        var keyProperty = messageType.GetProperty("Id") ??
                         messageType.GetProperty("Key") ??
                         messageType.GetProperty("PartitionKey");

        if (keyProperty != null)
        {
            var keyValue = keyProperty.GetValue(message);
            return keyValue?.ToString() ?? Guid.NewGuid().ToString();
        }

        return Guid.NewGuid().ToString();
    }

    public void Dispose()
    {
        _producer?.Flush(TimeSpan.FromSeconds(10));
        _producer?.Dispose();
        GC.SuppressFinalize(this);
    }
}