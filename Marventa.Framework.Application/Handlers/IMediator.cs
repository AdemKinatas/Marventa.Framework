using System.Threading;
using System.Threading.Tasks;
using Marventa.Framework.Application.Commands;
using Marventa.Framework.Application.Queries;

namespace Marventa.Framework.Application.Handlers;

public interface IMediator
{
    Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
    Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
    Task SendAsync(ICommand command, CancellationToken cancellationToken = default);
}