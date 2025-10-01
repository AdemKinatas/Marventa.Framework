namespace Marventa.Framework.EventBus.Kafka;

public interface IKafkaConsumer
{
    Task ConsumeAsync<T>(string topic, Func<T, Task> handler, CancellationToken cancellationToken = default) where T : class;
}
