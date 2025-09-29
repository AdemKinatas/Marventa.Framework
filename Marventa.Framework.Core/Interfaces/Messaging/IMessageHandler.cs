namespace Marventa.Framework.Core.Interfaces.Messaging;

public interface IMessageHandler<in T> where T : class
{
    Task Handle(T message, CancellationToken cancellationToken = default);
}