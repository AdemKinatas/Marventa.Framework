using Marventa.Framework.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Marventa.Framework.EventBus.RabbitMQ;

public class RabbitMqEventBus : IEventBus, IDisposable
{
    private readonly IRabbitMqConnection _connection;
    private readonly ILogger<RabbitMqEventBus> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _exchangeName;
    private IChannel? _consumerChannel;
    private readonly Dictionary<string, Type> _eventTypes = new();
    private readonly Dictionary<string, Type> _handlerTypes = new();

    public RabbitMqEventBus(
        IRabbitMqConnection connection,
        ILogger<RabbitMqEventBus> logger,
        IServiceProvider serviceProvider,
        string exchangeName = "marventa_event_bus")
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _exchangeName = exchangeName;
        _consumerChannel = CreateConsumerChannel();
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        if (!_connection.IsConnected)
        {
            _connection.TryConnect();
        }

        using var channel = _connection.CreateModel();
        var eventName = @event.GetType().Name;

        await channel.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Direct, cancellationToken: cancellationToken);

        var message = JsonConvert.SerializeObject(@event);
        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(
            exchange: _exchangeName,
            routingKey: eventName,
            body: body,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Published event {EventName} with ID {EventId}", eventName, @event.Id);
    }

    public void Subscribe<TEvent, TEventHandler>()
        where TEvent : IIntegrationEvent
        where TEventHandler : IIntegrationEventHandler<TEvent>
    {
        var eventName = typeof(TEvent).Name;
        var handlerType = typeof(TEventHandler);

        if (!_eventTypes.ContainsKey(eventName))
        {
            _eventTypes.Add(eventName, typeof(TEvent));
            _handlerTypes.Add(eventName, handlerType);

            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            using var channel = _connection.CreateModel();
            channel.QueueBindAsync(
                queue: _exchangeName,
                exchange: _exchangeName,
                routingKey: eventName).GetAwaiter().GetResult();

            _logger.LogInformation("Subscribed to event {EventName} with handler {EventHandler}", eventName, handlerType.Name);
        }
    }

    private IChannel CreateConsumerChannel()
    {
        if (!_connection.IsConnected)
        {
            _connection.TryConnect();
        }

        var channel = _connection.CreateModel();

        channel.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Direct).GetAwaiter().GetResult();
        channel.QueueDeclareAsync(
            queue: _exchangeName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null).GetAwaiter().GetResult();

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var eventName = ea.RoutingKey;
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());

            try
            {
                await ProcessEvent(eventName, message);
                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing event {EventName}", eventName);
                await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        channel.BasicConsumeAsync(
            queue: _exchangeName,
            autoAck: false,
            consumer: consumer).GetAwaiter().GetResult();

        return channel;
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        if (_eventTypes.ContainsKey(eventName))
        {
            using var scope = _serviceProvider.CreateScope();
            var eventType = _eventTypes[eventName];
            var handlerType = _handlerTypes[eventName];
            var @event = JsonConvert.DeserializeObject(message, eventType);
            var handler = scope.ServiceProvider.GetService(handlerType);

            if (handler != null && @event != null)
            {
                var handleMethod = handlerType.GetMethod("HandleAsync");
                await (Task)handleMethod!.Invoke(handler, new[] { @event, CancellationToken.None })!;
                _logger.LogInformation("Processed event {EventName}", eventName);
            }
        }
    }

    public void Dispose()
    {
        _consumerChannel?.Dispose();
    }
}
