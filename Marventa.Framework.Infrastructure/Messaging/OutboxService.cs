using Marventa.Framework.Core.Interfaces;
using Marventa.Framework.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Infrastructure.Messaging;

public class OutboxService : IOutboxService
{
    private readonly DbContext _context;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<OutboxService> _logger;

    public OutboxService(
        DbContext context,
        ITenantContext tenantContext,
        ILogger<OutboxService> logger)
    {
        _context = context;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task PublishAsync<T>(T message, string? idempotencyKey = null, CancellationToken cancellationToken = default)
        where T : class
    {
        var tenantId = _tenantContext.TenantId;

        // Check for duplicate idempotency key
        if (!string.IsNullOrEmpty(idempotencyKey))
        {
            var existing = await _context.Set<OutboxMessage>()
                .AnyAsync(m => m.IdempotencyKey == idempotencyKey && m.TenantId == tenantId, cancellationToken);

            if (existing)
            {
                _logger.LogDebug("Duplicate idempotency key detected: {IdempotencyKey}", idempotencyKey);
                return;
            }
        }

        var outboxMessage = OutboxMessage.Create(message, idempotencyKey, tenantId);
        _context.Set<OutboxMessage>().Add(outboxMessage);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogDebug("Outbox message created: {MessageId} for tenant: {TenantId}",
            outboxMessage.Id, tenantId);
    }

    public async Task PublishManyAsync<T>(IEnumerable<T> messages, CancellationToken cancellationToken = default)
        where T : class
    {
        var tenantId = _tenantContext.TenantId;
        var outboxMessages = messages.Select(m => OutboxMessage.Create(m, null, tenantId)).ToList();

        _context.Set<OutboxMessage>().AddRange(outboxMessages);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogDebug("Batch outbox messages created: {Count} for tenant: {TenantId}",
            outboxMessages.Count, tenantId);
    }

    public async Task<IEnumerable<IOutboxMessage>> GetPendingMessagesAsync(int batchSize = 100, CancellationToken cancellationToken = default)
    {
        return await _context.Set<OutboxMessage>()
            .Where(m => m.ProcessedAt == null && m.RetryCount < 3)
            .OrderBy(m => m.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await _context.Set<OutboxMessage>()
            .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);

        if (message != null)
        {
            message.ProcessedAt = DateTime.UtcNow;
            message.Error = null;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("Outbox message marked as processed: {MessageId}", messageId);
        }
    }

    public async Task MarkAsFailedAsync(Guid messageId, string error, CancellationToken cancellationToken = default)
    {
        var message = await _context.Set<OutboxMessage>()
            .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);

        if (message != null)
        {
            message.Error = error;
            message.RetryCount++;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogWarning("Outbox message marked as failed: {MessageId}, Retry: {RetryCount}, Error: {Error}",
                messageId, message.RetryCount, error);
        }
    }
}

public class InboxService : IInboxService
{
    private readonly DbContext _context;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<InboxService> _logger;

    public InboxService(
        DbContext context,
        ITenantContext tenantContext,
        ILogger<InboxService> logger)
    {
        _context = context;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<bool> IsProcessedAsync(string messageId, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantContext.TenantId;
        return await _context.Set<InboxMessage>()
            .AnyAsync(m => m.MessageId == messageId && m.TenantId == tenantId && m.IsProcessed, cancellationToken);
    }

    public async Task MarkAsProcessedAsync(string messageId, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantContext.TenantId;
        var message = await _context.Set<InboxMessage>()
            .FirstOrDefaultAsync(m => m.MessageId == messageId && m.TenantId == tenantId, cancellationToken);

        if (message != null)
        {
            message.IsProcessed = true;
            message.ProcessedAt = DateTime.UtcNow;
            message.Error = null;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("Inbox message marked as processed: {MessageId}", messageId);
        }
    }

    public async Task<IInboxMessage> StoreMessageAsync<T>(string messageId, T message, Dictionary<string, object>? headers = null, CancellationToken cancellationToken = default)
        where T : class
    {
        var tenantId = _tenantContext.TenantId;

        // Check if message already exists
        var existing = await _context.Set<InboxMessage>()
            .FirstOrDefaultAsync(m => m.MessageId == messageId && m.TenantId == tenantId, cancellationToken);

        if (existing != null)
        {
            _logger.LogDebug("Inbox message already exists: {MessageId}", messageId);
            return existing;
        }

        var inboxMessage = InboxMessage.Create(messageId, message, tenantId, headers);
        _context.Set<InboxMessage>().Add(inboxMessage);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogDebug("Inbox message stored: {MessageId} for tenant: {TenantId}",
            messageId, tenantId);

        return inboxMessage;
    }

    public async Task<IEnumerable<IInboxMessage>> GetFailedMessagesAsync(int batchSize = 100, CancellationToken cancellationToken = default)
    {
        return await _context.Set<InboxMessage>()
            .Where(m => !m.IsProcessed && m.RetryCount < 3 && !string.IsNullOrEmpty(m.Error))
            .OrderBy(m => m.ReceivedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }
}

public class TransactionalMessageService : ITransactionalMessageService
{
    private readonly IOutboxService _outboxService;
    private readonly IInboxService _inboxService;
    private readonly ILogger<TransactionalMessageService> _logger;

    public TransactionalMessageService(
        IOutboxService outboxService,
        IInboxService inboxService,
        ILogger<TransactionalMessageService> logger)
    {
        _outboxService = outboxService;
        _inboxService = inboxService;
        _logger = logger;
    }

    public async Task PublishAsync<T>(T message, string? idempotencyKey = null, CancellationToken cancellationToken = default)
        where T : class
    {
        await _outboxService.PublishAsync(message, idempotencyKey, cancellationToken);
    }

    public async Task<bool> HandleInboxMessageAsync<T>(string messageId, T message, Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default)
        where T : class
    {
        // Check if message already processed
        if (await _inboxService.IsProcessedAsync(messageId, cancellationToken))
        {
            _logger.LogDebug("Message already processed: {MessageId}", messageId);
            return false;
        }

        try
        {
            // Store the message first
            await _inboxService.StoreMessageAsync(messageId, message, cancellationToken: cancellationToken);

            // Process the message
            await handler(message, cancellationToken);

            // Mark as processed
            await _inboxService.MarkAsProcessedAsync(messageId, cancellationToken);

            _logger.LogDebug("Inbox message processed successfully: {MessageId}", messageId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process inbox message: {MessageId}", messageId);
            throw;
        }
    }
}