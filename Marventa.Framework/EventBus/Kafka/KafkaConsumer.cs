using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Marventa.Framework.EventBus.Kafka;

public class KafkaConsumer : IKafkaConsumer, IDisposable
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<KafkaConsumer> _logger;

    public KafkaConsumer(string bootstrapServers, string groupId, ILogger<KafkaConsumer> logger)
    {
        _logger = logger;

        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            EnableAutoOffsetStore = false
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    public async Task ConsumeAsync<T>(string topic, Func<T, Task> handler, CancellationToken cancellationToken = default) where T : class
    {
        _consumer.Subscribe(topic);

        await Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(cancellationToken);

                    if (consumeResult?.Message?.Value != null)
                    {
                        var message = JsonSerializer.Deserialize<T>(consumeResult.Message.Value);
                        if (message != null)
                        {
                            await handler(message);
                            _consumer.Commit(consumeResult);
                            _consumer.StoreOffset(consumeResult);
                        }
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming Kafka message from topic {Topic}", topic);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing Kafka message from topic {Topic}", topic);
                }
            }
        }, cancellationToken);
    }

    public void Dispose()
    {
        _consumer?.Close();
        _consumer?.Dispose();
    }
}
