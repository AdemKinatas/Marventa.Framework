using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Marventa.Framework.Features.EventBus.RabbitMQ;

public class RabbitMqConnection : IRabbitMqConnection
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<RabbitMqConnection> _logger;
    private IConnection? _connection;
    private bool _disposed;

    public RabbitMqConnection(IConnectionFactory connectionFactory, ILogger<RabbitMqConnection> logger)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

    public IChannel CreateModel()
    {
        if (!IsConnected)
        {
            throw new InvalidOperationException("No RabbitMQ connections are available");
        }

        return _connection!.CreateChannelAsync().GetAwaiter().GetResult();
    }

    public bool TryConnect()
    {
        _logger.LogInformation("RabbitMQ Client is trying to connect");

        try
        {
            _connection = _connectionFactory.CreateConnectionAsync().GetAwaiter().GetResult();

            if (IsConnected)
            {
                _connection.ConnectionShutdownAsync += OnConnectionShutdownAsync;
                _connection.CallbackExceptionAsync += OnCallbackExceptionAsync;
                _connection.ConnectionBlockedAsync += OnConnectionBlockedAsync;

                _logger.LogInformation("RabbitMQ Client acquired a persistent connection");
                return true;
            }
            else
            {
                _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "FATAL ERROR: RabbitMQ connections could not be created and opened");
            return false;
        }
    }

    private Task OnConnectionBlockedAsync(object? sender, ConnectionBlockedEventArgs e)
    {
        if (_disposed) return Task.CompletedTask;

        _logger.LogWarning("A RabbitMQ connection is blocked. Trying to re-connect...");
        TryConnect();
        return Task.CompletedTask;
    }

    private Task OnCallbackExceptionAsync(object? sender, CallbackExceptionEventArgs e)
    {
        if (_disposed) return Task.CompletedTask;

        _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");
        TryConnect();
        return Task.CompletedTask;
    }

    private Task OnConnectionShutdownAsync(object? sender, ShutdownEventArgs reason)
    {
        if (_disposed) return Task.CompletedTask;

        _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");
        TryConnect();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;

        try
        {
            _connection?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error disposing RabbitMQ connection");
        }
    }
}
