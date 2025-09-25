using Marventa.Framework.Core.Interfaces;

namespace Marventa.Framework.Application.Queries;

public interface IQuery<out TResponse> : IValidatable
{
}