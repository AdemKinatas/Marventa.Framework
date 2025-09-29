using Marventa.Framework.Core.Interfaces.Validation;

namespace Marventa.Framework.Application.Commands;

public interface ICommand : IValidatable
{
}

public interface ICommand<out TResponse> : ICommand
{
}