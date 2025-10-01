using Confluent.Kafka;
using System.Text.Json;

namespace Marventa.Framework.Features.EventBus.Kafka;

public class KafkaProducer : IKafkaProducer, IDisposable
{
    private readonly IProducer<string, string> _producer;

    public KafkaProducer(string bootstrapServers)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true,
            MaxInFlight = 5,
            MessageSendMaxRetries = 10
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task ProduceAsync<T>(string topic, T message, CancellationToken cancellationToken = default) where T : class
    {
        var key = Guid.NewGuid().ToString();
        var value = JsonSerializer.Serialize(message);

        var kafkaMessage = new Message<string, string>
        {
            Key = key,
            Value = value,
            Timestamp = new Timestamp(DateTime.UtcNow)
        };

        await _producer.ProduceAsync(topic, kafkaMessage, cancellationToken);
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
}
