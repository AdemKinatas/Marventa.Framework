using RabbitMQ.Client;

namespace Marventa.Framework.Features.EventBus.RabbitMQ;

public interface IRabbitMqConnection : IDisposable
{
    bool IsConnected { get; }
    IChannel CreateModel();
    bool TryConnect();
}
