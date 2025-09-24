using Confluent.Kafka;
using Marventa.Framework.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Marventa.Framework.Infrastructure.Messaging.Kafka;

public abstract class BaseKafkaHandler<T> : BackgroundService, IMessageHandler<T> where T : class
{
    protected readonly ILogger Logger;
    private readonly KafkaOptions _options;
    private readonly IConsumer<string, string> _consumer;
    private readonly string _topicName;

    protected BaseKafkaHandler(IOptions<KafkaOptions> options, ILogger logger)
    {
        Logger = logger;
        _options = options.Value;
        _topicName = GetTopicName();

        var config = new ConsumerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            GroupId = _options.GroupId,
            AutoOffsetReset = Enum.Parse<AutoOffsetReset>(_options.AutoOffsetReset),
            EnableAutoCommit = _options.EnableAutoCommit,
            SessionTimeoutMs = _options.SessionTimeoutMs,
            HeartbeatIntervalMs = _options.HeartbeatIntervalMs,
            MaxPollIntervalMs = _options.MaxPollIntervalMs
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

        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _consumer.Subscribe(_topicName);
            Logger.LogInformation("Started consuming messages from topic: {TopicName}", _topicName);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);

                    if (consumeResult?.Message != null)
                    {
                        await ProcessMessage(consumeResult, stoppingToken);

                        if (!_options.EnableAutoCommit)
                        {
                            _consumer.Commit(consumeResult);
                        }
                    }
                }
                catch (ConsumeException ex)
                {
                    Logger.LogError(ex, "Error consuming message from topic: {TopicName}", _topicName);
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation is requested
                    break;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Unexpected error in Kafka consumer for topic: {TopicName}", _topicName);
                }
            }
        }
        finally
        {
            _consumer.Close();
            _consumer.Dispose();
        }
    }

    private async Task ProcessMessage(ConsumeResult<string, string> consumeResult, CancellationToken cancellationToken)
    {
        try
        {
            Logger.LogDebug("Processing message from topic {TopicName}, Partition: {Partition}, Offset: {Offset}",
                _topicName, consumeResult.Partition.Value, consumeResult.Offset.Value);

            var message = JsonSerializer.Deserialize<T>(consumeResult.Message.Value);
            if (message != null)
            {
                await Handle(message, cancellationToken);

                Logger.LogInformation("Successfully processed message {MessageType} from topic {TopicName}, Offset: {Offset}",
                    typeof(T).Name, _topicName, consumeResult.Offset.Value);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to process message {MessageType} from topic {TopicName}, Offset: {Offset}",
                typeof(T).Name, _topicName, consumeResult.Offset.Value);

            // You might want to implement dead letter queue logic here
            throw;
        }
    }

    private string GetTopicName()
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

    public abstract Task Handle(T message, CancellationToken cancellationToken = default);

    public override void Dispose()
    {
        _consumer?.Close();
        _consumer?.Dispose();
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}