using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Marventa.Framework.Application.Helpers;

public static class MediatRHelpers
{
    public static async Task<TResponse> SendAsync<TResponse>(this IServiceProvider serviceProvider, IRequest<TResponse> request)
    {
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        return await mediator.Send(request);
    }

    public static async Task SendAsync(this IServiceProvider serviceProvider, IRequest request)
    {
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        await mediator.Send(request);
    }

    public static async Task PublishAsync<TNotification>(this IServiceProvider serviceProvider, TNotification notification)
        where TNotification : INotification
    {
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        await mediator.Publish(notification);
    }
}

public abstract class BaseHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    protected abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);

    async Task<TResponse> IRequestHandler<TRequest, TResponse>.Handle(TRequest request, CancellationToken cancellationToken)
    {
        return await Handle(request, cancellationToken);
    }
}

public abstract class BaseHandler<TRequest> : IRequestHandler<TRequest>
    where TRequest : IRequest
{
    protected abstract Task Handle(TRequest request, CancellationToken cancellationToken);

    async Task IRequestHandler<TRequest>.Handle(TRequest request, CancellationToken cancellationToken)
    {
        await Handle(request, cancellationToken);
    }
}