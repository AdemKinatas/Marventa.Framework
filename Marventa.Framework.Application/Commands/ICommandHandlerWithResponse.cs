using System.Threading;
using System.Threading.Tasks;

namespace Marventa.Framework.Application.Commands;

public interface ICommandHandler<in TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}