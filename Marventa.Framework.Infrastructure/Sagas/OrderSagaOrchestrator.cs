using Microsoft.Extensions.Logging;
using Marventa.Framework.Core.Interfaces.Sagas;
using Marventa.Framework.Domain.Entities;

namespace Marventa.Framework.Infrastructure.Sagas;

/// <summary>
/// Example order processing saga orchestrator
/// </summary>
public class OrderSagaOrchestrator : ISagaOrchestrator<OrderSaga>
{
    private readonly ILogger<OrderSagaOrchestrator> _logger;
    private readonly IServiceProvider _serviceProvider;

    public OrderSagaOrchestrator(
        ILogger<OrderSagaOrchestrator> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task HandleAsync(OrderSaga saga, object @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling event {EventType} for order saga {CorrelationId}", @event.GetType().Name, saga.CorrelationId);

        try
        {
            switch (@event)
            {
                case OrderCreatedEvent orderCreated:
                    await HandleOrderCreatedAsync(saga, orderCreated, cancellationToken);
                    break;

                case PaymentProcessedEvent paymentProcessed:
                    await HandlePaymentProcessedAsync(saga, paymentProcessed, cancellationToken);
                    break;

                case InventoryReservedEvent inventoryReserved:
                    await HandleInventoryReservedAsync(saga, inventoryReserved, cancellationToken);
                    break;

                case ShipmentCreatedEvent shipmentCreated:
                    await HandleShipmentCreatedAsync(saga, shipmentCreated, cancellationToken);
                    break;

                case OrderCompletedEvent orderCompleted:
                    await HandleOrderCompletedAsync(saga, orderCompleted, cancellationToken);
                    break;

                default:
                    _logger.LogWarning("Unknown event type {EventType} for order saga {CorrelationId}", @event.GetType().Name, saga.CorrelationId);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling event {EventType} for order saga {CorrelationId}", @event.GetType().Name, saga.CorrelationId);
            saga.Status = SagaStatus.Failed;
            saga.ErrorMessage = ex.Message;
            throw;
        }
    }

    public async Task CompensateAsync(OrderSaga saga, string reason, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Compensating order saga {CorrelationId}, reason: {Reason}", saga.CorrelationId, reason);

        try
        {
            saga.Status = SagaStatus.Compensating;

            // Compensate in reverse order of completed steps
            var completedSteps = saga.CompletedSteps.OrderByDescending(x => x).ToList();

            foreach (var step in completedSteps)
            {
                switch (step)
                {
                    case "ShipmentCreated":
                        await CancelShipmentAsync(saga, cancellationToken);
                        break;

                    case "InventoryReserved":
                        await ReleaseInventoryAsync(saga, cancellationToken);
                        break;

                    case "PaymentProcessed":
                        await RefundPaymentAsync(saga, cancellationToken);
                        break;
                }
            }

            saga.Status = SagaStatus.Compensated;
            _logger.LogInformation("Successfully compensated order saga {CorrelationId}", saga.CorrelationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error compensating order saga {CorrelationId}", saga.CorrelationId);
            saga.Status = SagaStatus.Failed;
            saga.ErrorMessage = $"Compensation failed: {ex.Message}";
            throw;
        }
    }

    private async Task HandleOrderCreatedAsync(OrderSaga saga, OrderCreatedEvent orderCreated, CancellationToken cancellationToken)
    {
        saga.OrderId = orderCreated.OrderId;
        saga.CustomerId = orderCreated.CustomerId;
        saga.TotalAmount = orderCreated.TotalAmount;
        saga.Status = SagaStatus.InProgress;
        saga.CurrentStep = "ProcessPayment";

        // Start payment processing
        await ProcessPaymentAsync(saga, cancellationToken);
    }

    private async Task HandlePaymentProcessedAsync(OrderSaga saga, PaymentProcessedEvent paymentProcessed, CancellationToken cancellationToken)
    {
        if (paymentProcessed.Success)
        {
            saga.PaymentId = paymentProcessed.PaymentId;
            saga.CompletedSteps.Add("PaymentProcessed");
            saga.CurrentStep = "ReserveInventory";

            // Reserve inventory
            await ReserveInventoryAsync(saga, cancellationToken);
        }
        else
        {
            saga.Status = SagaStatus.Failed;
            saga.ErrorMessage = "Payment processing failed";
        }
    }

    private async Task HandleInventoryReservedAsync(OrderSaga saga, InventoryReservedEvent inventoryReserved, CancellationToken cancellationToken)
    {
        if (inventoryReserved.Success)
        {
            saga.CompletedSteps.Add("InventoryReserved");
            saga.CurrentStep = "CreateShipment";

            // Create shipment
            await CreateShipmentAsync(saga, cancellationToken);
        }
        else
        {
            saga.Status = SagaStatus.Failed;
            saga.ErrorMessage = "Inventory reservation failed";
            await CompensateAsync(saga, "Inventory not available", cancellationToken);
        }
    }

    private async Task HandleShipmentCreatedAsync(OrderSaga saga, ShipmentCreatedEvent shipmentCreated, CancellationToken cancellationToken)
    {
        saga.ShipmentId = shipmentCreated.ShipmentId;
        saga.CompletedSteps.Add("ShipmentCreated");
        saga.CurrentStep = "CompleteOrder";

        // Complete the order
        await CompleteOrderAsync(saga, cancellationToken);
    }

    private async Task HandleOrderCompletedAsync(OrderSaga saga, OrderCompletedEvent orderCompleted, CancellationToken cancellationToken)
    {
        saga.Status = SagaStatus.Completed;
        saga.CompletedAt = DateTime.UtcNow;
        saga.CurrentStep = "Completed";

        _logger.LogInformation("Order saga {CorrelationId} completed successfully", saga.CorrelationId);

        // Complete the async operation
        await Task.CompletedTask;
    }

    // Saga step implementations
    private async Task ProcessPaymentAsync(OrderSaga saga, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Processing payment for order saga {CorrelationId}", saga.CorrelationId);
        // Implementation would call payment service
        await Task.Delay(100, cancellationToken); // Simulate processing
    }

    private async Task ReserveInventoryAsync(OrderSaga saga, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Reserving inventory for order saga {CorrelationId}", saga.CorrelationId);
        // Implementation would call inventory service
        await Task.Delay(100, cancellationToken); // Simulate processing
    }

    private async Task CreateShipmentAsync(OrderSaga saga, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Creating shipment for order saga {CorrelationId}", saga.CorrelationId);
        // Implementation would call shipping service
        await Task.Delay(100, cancellationToken); // Simulate processing
    }

    private async Task CompleteOrderAsync(OrderSaga saga, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Completing order for saga {CorrelationId}", saga.CorrelationId);
        // Implementation would update order status
        await Task.Delay(100, cancellationToken); // Simulate processing
    }

    // Compensation methods
    private async Task CancelShipmentAsync(OrderSaga saga, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Cancelling shipment for order saga {CorrelationId}", saga.CorrelationId);
        // Implementation would cancel shipment
        await Task.Delay(100, cancellationToken); // Simulate processing
    }

    private async Task ReleaseInventoryAsync(OrderSaga saga, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Releasing inventory for order saga {CorrelationId}", saga.CorrelationId);
        // Implementation would release reserved inventory
        await Task.Delay(100, cancellationToken); // Simulate processing
    }

    private async Task RefundPaymentAsync(OrderSaga saga, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Refunding payment for order saga {CorrelationId}", saga.CorrelationId);
        // Implementation would process refund
        await Task.Delay(100, cancellationToken); // Simulate processing
    }
}

// Example saga implementation
public class OrderSaga : ISaga
{
    public Guid CorrelationId { get; set; } = Guid.NewGuid();
    public SagaStatus Status { get; set; } = SagaStatus.Started;
    public string CurrentStep { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public List<string> CompletedSteps { get; set; } = new();
    public string? ErrorMessage { get; set; }

    // Order-specific properties
    public string OrderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string? PaymentId { get; set; }
    public string? ShipmentId { get; set; }
}

// Example events
public record OrderCreatedEvent(string OrderId, string CustomerId, decimal TotalAmount);
public record PaymentProcessedEvent(string PaymentId, bool Success, string? ErrorMessage = null);
public record InventoryReservedEvent(bool Success, string? ErrorMessage = null);
public record ShipmentCreatedEvent(string ShipmentId);
public record OrderCompletedEvent(string OrderId);