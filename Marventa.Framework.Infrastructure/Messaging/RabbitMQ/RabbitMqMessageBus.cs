using MassTransit;
using Marventa.Framework.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Infrastructure.Messaging.RabbitMQ;

public class RabbitMqMessageBus : IMessageBus
{
    private readonly IBus _bus;
    private readonly ILogger<RabbitMqMessageBus> _logger;

    public RabbitMqMessageBus(IBus bus, ILogger<RabbitMqMessageBus> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            _logger.LogDebug("Publishing message {MessageType}: {@Message}", typeof(T).Name, message);
            await _bus.Publish(message, cancellationToken);
            _logger.LogInformation("Successfully published message {MessageType}", typeof(T).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message {MessageType}", typeof(T).Name);
            throw;
        }
    }

    public async Task SendAsync<T>(T command, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            _logger.LogDebug("Sending command {CommandType}: {@Command}", typeof(T).Name, command);
            await _bus.Send(command, cancellationToken);
            _logger.LogInformation("Successfully sent command {CommandType}", typeof(T).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send command {CommandType}", typeof(T).Name);
            throw;
        }
    }

    public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class
        where TResponse : class
    {
        try
        {
            _logger.LogDebug("Sending request {RequestType}: {@Request}", typeof(TRequest).Name, request);

            var client = _bus.CreateRequestClient<TRequest>();
            var response = await client.GetResponse<TResponse>(request, cancellationToken);

            _logger.LogInformation("Successfully received response for request {RequestType}", typeof(TRequest).Name);
            return response.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get response for request {RequestType}", typeof(TRequest).Name);
            throw;
        }
    }
}