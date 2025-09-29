namespace Marventa.Framework.Core.Interfaces.Messaging;

public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
}