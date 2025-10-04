using MediatR;

namespace Marventa.Framework.TestApi.Events;

public class ProductCreatedEvent : INotification
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ProductCreatedEventHandler : INotificationHandler<ProductCreatedEvent>
{
    private readonly ILogger<ProductCreatedEventHandler> _logger;

    public ProductCreatedEventHandler(ILogger<ProductCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Product created event handled: {ProductId} - {ProductName} - ${Price}",
            notification.ProductId,
            notification.ProductName,
            notification.Price);

        // Here you would typically send notifications, update search indexes, etc.
        return Task.CompletedTask;
    }
}

public class OrderCreatedEvent : INotification
{
    public Guid OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int ItemCount { get; set; }
    public DateTime OrderDate { get; set; }
}

public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Order created event handled: {OrderId} - {CustomerName} - ${TotalAmount} - {ItemCount} items",
            notification.OrderId,
            notification.CustomerName,
            notification.TotalAmount,
            notification.ItemCount);

        return Task.CompletedTask;
    }
}
