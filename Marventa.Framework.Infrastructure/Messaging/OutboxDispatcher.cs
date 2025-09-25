using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MassTransit;
using Marventa.Framework.Core.Interfaces;
using Marventa.Framework.Domain.Entities;
using System.Text.Json;

namespace Marventa.Framework.Infrastructure.Messaging;

public class OutboxDispatcherService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxDispatcherService> _logger;
    private readonly OutboxDispatcherOptions _options;

    public OutboxDispatcherService(
        IServiceProvider serviceProvider,
        ILogger<OutboxDispatcherService> logger,
        IOptions<OutboxDispatcherOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Dispatcher Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingMessagesAsync(stoppingToken);
                await Task.Delay(_options.PollingInterval, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // Expected when cancellation is requested
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Outbox Dispatcher Service");
                await Task.Delay(_options.ErrorRetryInterval, stoppingToken);
            }
        }

        _logger.LogInformation("Outbox Dispatcher Service stopped");
    }

    private async Task ProcessPendingMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        var pendingMessages = await dbContext.Set<OutboxMessage>()
            .Where(m => m.ProcessedAt == null && m.RetryCount < _options.MaxRetries)
            .OrderBy(m => m.CreatedAt)
            .Take(_options.BatchSize)
            .ToListAsync(cancellationToken);

        if (!pendingMessages.Any())
            return;

        _logger.LogDebug("Processing {Count} pending outbox messages", pendingMessages.Count);

        var tasks = pendingMessages.Select(message => ProcessMessageAsync(message, publishEndpoint, dbContext, cancellationToken));
        await Task.WhenAll(tasks);

        await dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Processed {Count} outbox messages", pendingMessages.Count);
    }

    private async Task ProcessMessageAsync(
        OutboxMessage message,
        IPublishEndpoint publishEndpoint,
        DbContext dbContext,
        CancellationToken cancellationToken)
    {
        try
        {
            var messageType = Type.GetType(message.MessageType);
            if (messageType == null)
            {
                _logger.LogWarning("Cannot resolve message type {MessageType} for message {MessageId}",
                    message.MessageType, message.Id);

                message.RetryCount = _options.MaxRetries;
                message.Error = $"Cannot resolve message type: {message.MessageType}";
                return;
            }

            var messageData = JsonSerializer.Deserialize(message.MessageData, messageType);
            if (messageData == null)
            {
                _logger.LogWarning("Cannot deserialize message {MessageId}", message.Id);
                message.RetryCount = _options.MaxRetries;
                message.Error = "Cannot deserialize message data";
                return;
            }

            await publishEndpoint.Publish(messageData, messageType, cancellationToken);

            message.ProcessedAt = DateTime.UtcNow;
            message.Error = null;

            _logger.LogDebug("Successfully dispatched message {MessageId} of type {MessageType}",
                message.Id, message.MessageType);
        }
        catch (Exception ex)
        {
            message.RetryCount++;
            message.Error = ex.Message;

            _logger.LogError(ex, "Error dispatching message {MessageId}, retry count: {RetryCount}",
                message.Id, message.RetryCount);

            if (message.RetryCount >= _options.MaxRetries)
            {
                _logger.LogError("Message {MessageId} exceeded max retries ({MaxRetries})",
                    message.Id, _options.MaxRetries);
            }
        }
    }
}

public class OutboxDispatcherOptions
{
    public const string SectionName = "OutboxDispatcher";
    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(5);
    public TimeSpan ErrorRetryInterval { get; set; } = TimeSpan.FromSeconds(30);
    public int BatchSize { get; set; } = 100;
    public int MaxRetries { get; set; } = 3;
    public bool EnableDetailedLogging { get; set; } = false;
}