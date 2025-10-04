using Marventa.Framework.ApiResponse;
using Marventa.Framework.Infrastructure.Persistence.Outbox;
using Marventa.Framework.TestApi.Data;
using Marventa.Framework.TestApi.Events;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Marventa.Framework.TestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OutboxTestController : ControllerBase
{
    private readonly TestDbContext _context;
    private readonly IOutboxMessageRepository _outboxRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<OutboxTestController> _logger;

    public OutboxTestController(
        TestDbContext context,
        IOutboxMessageRepository outboxRepository,
        IMediator mediator,
        ILogger<OutboxTestController> logger)
    {
        _context = context;
        _outboxRepository = outboxRepository;
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a product and stores the event in the outbox table
    /// </summary>
    [HttpPost("create-product")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price,
            Description = request.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);

        // Create domain event
        var domainEvent = new ProductCreatedEvent
        {
            ProductId = product.Id,
            ProductName = product.Name,
            Price = product.Price,
            CreatedAt = product.CreatedAt
        };

        // Store in outbox within the same transaction
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            EventType = domainEvent.GetType().AssemblyQualifiedName!,
            Payload = JsonSerializer.Serialize(domainEvent),
            OccurredOn = DateTime.UtcNow
        };

        await _outboxRepository.AddAsync(outboxMessage);

        // Save everything in one transaction
        await _context.SaveChangesAsync();

        _logger.LogInformation("Product created with outbox message: {ProductId}", product.Id);

        var response = ApiResponse<object>.SuccessResponse(
            new { productId = product.Id, outboxMessageId = outboxMessage.Id },
            "Product created successfully. Event stored in outbox for processing."
        );

        return Ok(response);
    }

    /// <summary>
    /// Gets all unprocessed outbox messages
    /// </summary>
    [HttpGet("outbox/unprocessed")]
    public async Task<IActionResult> GetUnprocessedMessages([FromQuery] int batchSize = 10)
    {
        var messages = await _outboxRepository.GetUnprocessedMessagesAsync(batchSize);

        var response = ApiResponse<object>.SuccessResponse(
            messages.Select(m => new
            {
                m.Id,
                m.EventType,
                m.OccurredOn,
                m.RetryCount,
                m.Error
            }),
            $"Retrieved {messages.Count()} unprocessed outbox messages"
        );

        return Ok(response);
    }

    /// <summary>
    /// Gets all outbox messages (for testing)
    /// </summary>
    [HttpGet("outbox/all")]
    public async Task<IActionResult> GetAllMessages()
    {
        var messages = await _context.Set<OutboxMessage>().ToListAsync();

        var response = ApiResponse<object>.SuccessResponse(
            messages.Select(m => new
            {
                m.Id,
                m.EventType,
                m.OccurredOn,
                m.ProcessedOn,
                m.RetryCount,
                m.Error,
                Status = m.ProcessedOn.HasValue ? "Processed" : (m.RetryCount >= m.MaxRetries ? "Failed" : "Pending")
            }),
            $"Retrieved {messages.Count} total outbox messages"
        );

        return Ok(response);
    }

    /// <summary>
    /// Manually triggers processing of a specific outbox message (for testing)
    /// </summary>
    [HttpPost("outbox/process/{messageId}")]
    public async Task<IActionResult> ProcessMessage(Guid messageId)
    {
        var message = await _context.Set<OutboxMessage>().FindAsync(messageId);
        if (message == null)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("Outbox message not found"));
        }

        try
        {
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

            await _mediator.Publish(@event);
            await _outboxRepository.MarkAsProcessedAsync(message.Id);
            await _context.SaveChangesAsync();

            var response = ApiResponse<object>.SuccessResponse(
                new { messageId, processedAt = DateTime.UtcNow },
                "Outbox message processed successfully"
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            await _outboxRepository.MarkAsFailedAsync(message.Id, ex.Message);
            await _context.SaveChangesAsync();

            var response = ApiResponse<object>.ErrorResponse($"Failed to process outbox message: {ex.Message}");
            return StatusCode(500, response);
        }
    }
}

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Description { get; set; }
}
