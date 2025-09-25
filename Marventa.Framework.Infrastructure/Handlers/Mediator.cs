using System;
using System.Threading;
using System.Threading.Tasks;
using Marventa.Framework.Application.Commands;
using Marventa.Framework.Application.Handlers;
using Marventa.Framework.Application.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Infrastructure.Handlers;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Mediator> _logger;

    public Mediator(IServiceProvider serviceProvider, ILogger<Mediator> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        var queryType = query.GetType();
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResponse));

        _logger.LogDebug("Handling query: {QueryType}", queryType.Name);

        var handler = _serviceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod("HandleAsync");

        if (method == null)
            throw new InvalidOperationException($"HandleAsync method not found on {handlerType.Name}");

        var result = await (Task<TResponse>)method.Invoke(handler, new object[] { query, cancellationToken })!;

        _logger.LogDebug("Query handled successfully: {QueryType}", queryType.Name);
        return result;
    }

    public async Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        var commandType = command.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(commandType, typeof(TResponse));

        _logger.LogDebug("Handling command with response: {CommandType}", commandType.Name);

        var handler = _serviceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod("HandleAsync");

        if (method == null)
            throw new InvalidOperationException($"HandleAsync method not found on {handlerType.Name}");

        var result = await (Task<TResponse>)method.Invoke(handler, new object[] { command, cancellationToken })!;

        _logger.LogDebug("Command handled successfully: {CommandType}", commandType.Name);
        return result;
    }

    public async Task SendAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        var commandType = command.GetType();
        var handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);

        _logger.LogDebug("Handling command: {CommandType}", commandType.Name);

        var handler = _serviceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod("HandleAsync");

        if (method == null)
            throw new InvalidOperationException($"HandleAsync method not found on {handlerType.Name}");

        await (Task)method.Invoke(handler, new object[] { command, cancellationToken })!;

        _logger.LogDebug("Command handled successfully: {CommandType}", commandType.Name);
    }
}