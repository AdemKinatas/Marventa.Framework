using Marventa.Framework.Core.Interfaces;

namespace Marventa.Framework.Application.Commands;

public interface ICommand : IValidatable
{
}

public interface ICommand<out TResponse> : ICommand
{
}