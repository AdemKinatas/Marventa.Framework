namespace Marventa.Framework.Core.Interfaces;

public interface IMessageBus
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
    Task SendAsync<T>(T command, CancellationToken cancellationToken = default) where T : class;
    Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class
        where TResponse : class;
}

public interface IMessageHandler<in T> where T : class
{
    Task Handle(T message, CancellationToken cancellationToken = default);
}

public interface ICommandHandler<in T> where T : class
{
    Task Handle(T command, CancellationToken cancellationToken = default);
}

public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
}