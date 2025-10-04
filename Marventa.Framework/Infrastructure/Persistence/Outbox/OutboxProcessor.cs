using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Marventa.Framework.Infrastructure.Persistence.Outbox;

/// <summary>
/// Background service that processes outbox messages and publishes them as domain events.
/// Implements the polling publisher pattern with retry logic and exponential backoff.
/// </summary>
public class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(30);
    private readonly int _batchSize = 100;
    private readonly TimeSpan _retentionPeriod = TimeSpan.FromDays(7);

    /// <summary>
    /// Initializes a new instance of the <see cref="OutboxProcessor"/> class.
    /// </summary>
    /// <param name="serviceScopeFactory">Factory for creating service scopes.</param>
    /// <param name="logger">Logger instance.</param>
    public OutboxProcessor(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<OutboxProcessor> logger)
    {
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Processor started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxMessagesAsync(stoppingToken);
                await CleanupProcessedMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing outbox messages");
            }

            await Task.Delay(_pollingInterval, stoppingToken);
        }

        _logger.LogInformation("Outbox Processor stopped");
    }

    /// <summary>
    /// Processes unprocessed outbox messages.
    /// </summary>
    private async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IOutboxMessageRepository>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var messages = await repository.GetUnprocessedMessagesAsync(_batchSize, cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                await ProcessMessageAsync(message, mediator, repository, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox message {MessageId}", message.Id);
                await HandleMessageFailureAsync(message, repository, ex.Message, cancellationToken);
            }
        }

        // Save changes if using UnitOfWork pattern
        var context = scope.ServiceProvider.GetService<Microsoft.EntityFrameworkCore.DbContext>();
        if (context != null)
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Processes a single outbox message.
    /// </summary>
    private async Task ProcessMessageAsync(
        OutboxMessage message,
        IMediator mediator,
        IOutboxMessageRepository repository,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Processing outbox message {MessageId} of type {EventType}", message.Id, message.EventType);

        // Deserialize the event
        var eventType = Type.GetType(message.EventType);
        if (eventType == null)
        {
            throw new InvalidOperationException($"Event type {message.EventType} could not be resolved");
        }

        var @event = JsonSerializer.Deserialize(message.Payload, eventType);
        if (@event == null)
        {
            throw new InvalidOperationException($"Failed to deserialize event of type {message.EventType}");
        }

        // Publish the event
        await mediator.Publish(@event, cancellationToken);

        // Mark as processed
        await repository.MarkAsProcessedAsync(message.Id, cancellationToken);

        _logger.LogInformation("Successfully processed outbox message {MessageId}", message.Id);
    }

    /// <summary>
    /// Handles message processing failure with exponential backoff.
    /// </summary>
    private async Task HandleMessageFailureAsync(
        OutboxMessage message,
        IOutboxMessageRepository repository,
        string error,
        CancellationToken cancellationToken)
    {
        await repository.MarkAsFailedAsync(message.Id, error, cancellationToken);

        if (message.RetryCount + 1 >= message.MaxRetries)
        {
            _logger.LogWarning(
                "Outbox message {MessageId} has exceeded max retries ({MaxRetries}). Error: {Error}",
                message.Id,
                message.MaxRetries,
                error);
        }
        else
        {
            _logger.LogWarning(
                "Outbox message {MessageId} failed (retry {RetryCount}/{MaxRetries}). Error: {Error}",
                message.Id,
                message.RetryCount + 1,
                message.MaxRetries,
                error);
        }
    }

    /// <summary>
    /// Cleans up old processed messages based on retention policy.
    /// </summary>
    private async Task CleanupProcessedMessagesAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IOutboxMessageRepository>();

            await repository.DeleteProcessedMessagesAsync(_retentionPeriod, cancellationToken);

            var context = scope.ServiceProvider.GetService<Microsoft.EntityFrameworkCore.DbContext>();
            if (context != null)
            {
                await context.SaveChangesAsync(cancellationToken);
            }

            _logger.LogDebug("Cleaned up processed outbox messages older than {RetentionPeriod}", _retentionPeriod);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while cleaning up processed outbox messages");
        }
    }
}
