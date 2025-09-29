using MassTransit;
using Marventa.Framework.Core.Interfaces.Messaging;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Infrastructure.Messaging.RabbitMQ;

public abstract class BaseRabbitMqHandler<T> : IConsumer<T>, IMessageHandler<T> where T : class
{
    protected readonly ILogger Logger;

    protected BaseRabbitMqHandler(ILogger logger)
    {
        Logger = logger;
    }

    public async Task Consume(ConsumeContext<T> context)
    {
        try
        {
            Logger.LogDebug("Processing message {MessageType}: {@Message}", typeof(T).Name, context.Message);

            await Handle(context.Message, context.CancellationToken);

            Logger.LogInformation("Successfully processed message {MessageType} with MessageId: {MessageId}",
                typeof(T).Name, context.MessageId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to process message {MessageType} with MessageId: {MessageId}",
                typeof(T).Name, context.MessageId);

            // Re-throw to let MassTransit handle retries and error queues
            throw;
        }
    }

    public abstract Task Handle(T message, CancellationToken cancellationToken = default);
}

public abstract class BaseRabbitMqCommandHandler<T> : IConsumer<T>, ICommandHandler<T> where T : class
{
    protected readonly ILogger Logger;

    protected BaseRabbitMqCommandHandler(ILogger logger)
    {
        Logger = logger;
    }

    public async Task Consume(ConsumeContext<T> context)
    {
        try
        {
            Logger.LogDebug("Processing command {CommandType}: {@Command}", typeof(T).Name, context.Message);

            await Handle(context.Message, context.CancellationToken);

            Logger.LogInformation("Successfully processed command {CommandType} with MessageId: {MessageId}",
                typeof(T).Name, context.MessageId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to process command {CommandType} with MessageId: {MessageId}",
                typeof(T).Name, context.MessageId);
            throw;
        }
    }

    public abstract Task Handle(T command, CancellationToken cancellationToken = default);
}

public abstract class BaseRabbitMqRequestHandler<TRequest, TResponse> : IConsumer<TRequest>, IRequestHandler<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    protected readonly ILogger Logger;

    protected BaseRabbitMqRequestHandler(ILogger logger)
    {
        Logger = logger;
    }

    public async Task Consume(ConsumeContext<TRequest> context)
    {
        try
        {
            Logger.LogDebug("Processing request {RequestType}: {@Request}", typeof(TRequest).Name, context.Message);

            var response = await Handle(context.Message, context.CancellationToken);

            await context.RespondAsync(response);

            Logger.LogInformation("Successfully processed request {RequestType} with MessageId: {MessageId}",
                typeof(TRequest).Name, context.MessageId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to process request {RequestType} with MessageId: {MessageId}",
                typeof(TRequest).Name, context.MessageId);
            throw;
        }
    }

    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
}