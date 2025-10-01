namespace Marventa.Framework.EventBus.Kafka;

public interface IKafkaProducer
{
    Task ProduceAsync<T>(string topic, T message, CancellationToken cancellationToken = default) where T : class;
}
