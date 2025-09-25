using System.Threading;
using System.Threading.Tasks;

namespace Marventa.Framework.Application.Commands;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}