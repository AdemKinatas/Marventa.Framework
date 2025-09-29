using Marventa.Framework.Core.Interfaces.Validation;

namespace Marventa.Framework.Application.Queries;

public interface IQuery<out TResponse> : IValidatable
{
}