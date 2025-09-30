using MediatR;

namespace Marventa.Framework.Core.Application;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
