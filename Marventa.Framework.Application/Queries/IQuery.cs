using System.Threading;
using System.Threading.Tasks;

namespace Marventa.Framework.Application.Queries;

public interface IQuery<out TResponse>
{
}

public interface IQueryHandler<in TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}