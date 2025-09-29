namespace Marventa.Framework.Core.Interfaces.Messaging;

public interface ICommandHandler<in T> where T : class
{
    Task Handle(T command, CancellationToken cancellationToken = default);
}