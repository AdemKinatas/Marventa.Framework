using MediatR;

namespace Marventa.Framework.Core.Application;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
