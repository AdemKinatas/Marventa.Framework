# Marventa.Framework

A comprehensive .NET 9.0 enterprise e-commerce framework with multi-tenancy, JWT authentication, CQRS, messaging infrastructure, and complete e-commerce domain modules.

## Features

### 🏗️ Core Architecture
- **Multi-Tenancy** - Complete tenant isolation with policy-based authorization
- **CQRS & MediatR** - Command Query Responsibility Segregation with pipeline behaviors
- **Clean Architecture** - Domain-driven design with clear layer separation
- **Dependency Injection** - Built-in DI container integration

### 🔐 Security & Authentication
- **JWT Authentication** - Token-based authentication and authorization
- **Security & Encryption** - Data protection, encryption, and secure storage
- **API Versioning** - Header, query, and URL-based versioning strategies
- **Rate Limiting** - Tenant-aware and endpoint-specific rate limiting

### 💾 Data & Persistence
- **Entity Framework Core** - Repository pattern with tenant scoping
- **Database Seeding** - Multi-tenant seed data management
- **Distributed Locking** - Redis-based distributed locks
- **Caching** - Redis distributed and in-memory caching with tenant isolation

### 📨 Messaging & Communication
- **RabbitMQ + MassTransit** - Reliable message bus with retry policies
- **Kafka Integration** - High-throughput event streaming
- **Outbox/Inbox Patterns** - Guaranteed message delivery
- **Email & SMS Services** - Template-based communication

### 💰 E-Commerce Domain
- **Payment Processing** - Complete payment domain with events and state management
- **Shipping Management** - End-to-end shipping lifecycle with tracking
- **Money/Currency** - Multi-currency value objects with exchange rates
- **Order Management** - Complete order lifecycle with domain events

### 🔄 Workflow & Orchestration
- **Saga Patterns** - Long-running business process orchestration
- **Background Jobs** - Hangfire-based job processing
- **HTTP Idempotency** - Correlation tracking for safe retries

### 🔍 Monitoring & Observability
- **Structured Logging** - Serilog with tenant and correlation context
- **OpenTelemetry** - Distributed tracing and metrics
- **Health Checks** - Database, cache, and service monitoring
- **Feature Flags** - Dynamic feature toggles with tenant support

### 🛠️ Developer Experience
- **Validation** - FluentValidation with RFC 7807 Problem Details
- **Circuit Breaker** - HTTP resilience patterns with fallback
- **Search & Analytics** - Elasticsearch full-text search
- **Cloud Storage** - S3-compatible storage abstraction

## Installation

```bash
dotnet add package Marventa.Framework
```

## Quick Start

```csharp
// Program.cs
using Marventa.Framework;

var builder = WebApplication.CreateBuilder(args);

// Add Marventa Framework services
builder.Services.AddMarventa();

var app = builder.Build();

// Use Marventa Framework middleware
app.UseMarventa();

app.Run();
```

## Complete Feature Guide

### 🏢 Multi-Tenancy

#### Configuration
```json
{
  "MultiTenancy": {
    "ResolutionStrategy": "Header", // Header, Subdomain, Claim
    "HeaderName": "X-Tenant-Id",
    "DefaultTenant": "default"
  }
}
```

#### Implementation
```csharp
// Tenant Entity
public class Tenant : ITenant
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string ConnectionString { get; set; }
    public Dictionary<string, object> Properties { get; set; }
}

// Tenant Context Usage
public class ProductService
{
    private readonly ITenantContext _tenantContext;
    private readonly IRepository<Product> _repository;

    public async Task<Product> CreateProductAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name,
            TenantId = _tenantContext.TenantId // Automatically set
        };

        await _repository.AddAsync(product);
        return product;
    }
}

// Tenant Authorization
[TenantAuthorize("products:read")]
public class ProductController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        // Only products for current tenant will be returned
        return Ok(await _productService.GetProductsAsync());
    }
}
```

### 🔐 JWT Authentication

#### Configuration
```json
{
  "JWT": {
    "SecretKey": "your-super-secret-key-at-least-32-characters-long",
    "Issuer": "your-app-name",
    "Audience": "your-app-users",
    "ExpiryInMinutes": 60,
    "RefreshTokenExpiryInDays": 7
  }
}
```

#### Implementation
```csharp
// Token Service Usage
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly ICurrentUserService _currentUser;

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        // Validate user credentials here
        var user = await ValidateUserAsync(request);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("tenant_id", user.TenantId)
        };

        var accessToken = await _tokenService.GenerateAccessTokenAsync(claims);
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync();

        return Ok(new TokenInfo
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            TokenType = "Bearer"
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
    {
        var principal = await _tokenService.ValidateTokenAsync(request.RefreshToken);
        if (principal == null)
            return Unauthorized();

        var newToken = await _tokenService.GenerateAccessTokenAsync(principal.Claims);
        return Ok(new { Token = newToken });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var token = HttpContext.Request.Headers.Authorization
            .FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
            await _tokenService.RevokeTokenAsync(token);

        return Ok();
    }
}

// Current User Service
public class OrderService
{
    private readonly ICurrentUserService _currentUser;

    public async Task CreateOrderAsync(CreateOrderRequest request)
    {
        var order = new Order
        {
            UserId = _currentUser.UserId, // Automatically from JWT
            UserName = _currentUser.UserName,
            TenantId = _currentUser.TenantId
        };
    }
}
```

### 📋 CQRS & MediatR

#### Commands
```csharp
// Command Definition
public record CreateOrderCommand(
    string CustomerId,
    List<OrderItem> Items,
    string ShippingAddress
) : ICommand<Guid>;

// Command Handler
public class CreateOrderHandler : ICommandHandler<CreateOrderCommand, Guid>
{
    private readonly IRepository<Order> _orderRepository;
    private readonly ITenantContext _tenantContext;
    private readonly IEventBus _eventBus;

    public async Task<Guid> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var order = new Order
        {
            CustomerId = command.CustomerId,
            Items = command.Items,
            ShippingAddress = command.ShippingAddress,
            TenantId = _tenantContext.TenantId,
            Status = OrderStatus.Created
        };

        await _orderRepository.AddAsync(order, cancellationToken);

        // Publish domain event
        await _eventBus.PublishDomainEventAsync(
            new OrderCreatedEvent(order.Id, order.CustomerId),
            cancellationToken);

        return order.Id;
    }
}
```

#### Queries
```csharp
// Query Definition
public record GetOrdersByCustomerQuery(string CustomerId, int Page = 1, int PageSize = 10)
    : IQuery<PagedResult<OrderDto>>;

// Query Handler
public class GetOrdersByCustomerHandler : IQueryHandler<GetOrdersByCustomerQuery, PagedResult<OrderDto>>
{
    private readonly IRepository<Order> _orderRepository;
    private readonly ITenantContext _tenantContext;

    public async Task<PagedResult<OrderDto>> HandleAsync(GetOrdersByCustomerQuery query, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetPagedAsync(
            page: query.Page,
            pageSize: query.PageSize,
            predicate: o => o.CustomerId == query.CustomerId && o.TenantId == _tenantContext.TenantId,
            orderBy: o => o.CreatedDate,
            cancellationToken: cancellationToken
        );

        return orders.Map(o => new OrderDto
        {
            Id = o.Id,
            CustomerId = o.CustomerId,
            Status = o.Status.ToString(),
            CreatedDate = o.CreatedDate
        });
    }
}
```

#### Controller Usage
```csharp
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderCommand command)
    {
        var orderId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetOrder), new { id = orderId }, orderId);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetOrdersByCustomer(string customerId, int page = 1, int pageSize = 10)
    {
        var query = new GetOrdersByCustomerQuery(customerId, page, pageSize);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
```

### 💰 Payment Processing

#### Payment Domain
```csharp
// Payment Entity with Domain Events
public class Payment : BaseEntity
{
    public string OrderId { get; private set; }
    public Money Amount { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? TransactionId { get; private set; }
    public string? GatewayResponse { get; private set; }

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public Payment(string orderId, Money amount, PaymentMethod method)
    {
        OrderId = orderId;
        Amount = amount;
        Method = method;
        Status = PaymentStatus.Pending;

        _domainEvents.Add(new PaymentInitiatedEvent(Id, OrderId, Amount));
    }

    public void MarkAsProcessing()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException($"Cannot process payment from status {Status}");

        Status = PaymentStatus.Processing;
    }

    public void MarkAsSuccessful(string transactionId, string? gatewayResponse = null)
    {
        if (Status != PaymentStatus.Processing)
            throw new InvalidOperationException($"Cannot complete payment from status {Status}");

        Status = PaymentStatus.Successful;
        TransactionId = transactionId;
        GatewayResponse = gatewayResponse;

        _domainEvents.Add(new PaymentCompletedEvent(Id, OrderId, TransactionId, Amount));
    }

    public void MarkAsFailed(string reason)
    {
        Status = PaymentStatus.Failed;
        GatewayResponse = reason;

        _domainEvents.Add(new PaymentFailedEvent(Id, OrderId, reason));
    }

    public Money ApplyDiscount(decimal percentage)
    {
        return Amount.ApplyDiscount(percentage);
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}

// Payment Service
public class PaymentService
{
    private readonly IRepository<Payment> _paymentRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IEventBus _eventBus;

    public async Task<PaymentResult> ProcessPaymentAsync(ProcessPaymentRequest request)
    {
        var payment = new Payment(
            orderId: request.OrderId,
            amount: new Money(request.Amount, request.Currency),
            method: request.PaymentMethod
        );

        payment.MarkAsProcessing();
        await _paymentRepository.AddAsync(payment);

        try
        {
            var gatewayResult = await _paymentGateway.ProcessAsync(new PaymentGatewayRequest
            {
                Amount = payment.Amount.Amount,
                Currency = payment.Amount.Currency.Code,
                PaymentMethod = payment.Method,
                OrderId = payment.OrderId
            });

            if (gatewayResult.IsSuccess)
            {
                payment.MarkAsSuccessful(gatewayResult.TransactionId, gatewayResult.Response);
            }
            else
            {
                payment.MarkAsFailed(gatewayResult.ErrorMessage);
            }

            await _paymentRepository.UpdateAsync(payment);

            // Publish domain events
            foreach (var domainEvent in payment.DomainEvents)
            {
                await _eventBus.PublishDomainEventAsync(domainEvent);
            }

            payment.ClearDomainEvents();

            return new PaymentResult
            {
                IsSuccess = payment.Status == PaymentStatus.Successful,
                PaymentId = payment.Id,
                TransactionId = payment.TransactionId,
                Status = payment.Status
            };
        }
        catch (Exception ex)
        {
            payment.MarkAsFailed(ex.Message);
            await _paymentRepository.UpdateAsync(payment);
            throw;
        }
    }
}
```

### 📦 Shipping Management

#### Shipping Domain
```csharp
// Shipping Aggregate
public class Shipment : BaseEntity
{
    public string OrderId { get; private set; }
    public string TrackingNumber { get; private set; }
    public ShippingStatus Status { get; private set; }
    public ShippingCarrier Carrier { get; private set; }
    public Address FromAddress { get; private set; }
    public Address ToAddress { get; private set; }
    public Money ShippingCost { get; private set; }
    public decimal Weight { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public DateTime EstimatedDelivery { get; private set; }

    private readonly List<ShipmentItem> _items = new();
    public IReadOnlyCollection<ShipmentItem> Items => _items.AsReadOnly();

    public Shipment(string orderId, ShippingCarrier carrier, Address fromAddress,
                   Address toAddress, Money shippingCost, DateTime estimatedDelivery)
    {
        OrderId = orderId;
        Carrier = carrier;
        FromAddress = fromAddress;
        ToAddress = toAddress;
        ShippingCost = shippingCost;
        EstimatedDelivery = estimatedDelivery;
        Status = ShippingStatus.Pending;
        TrackingNumber = GenerateTrackingNumber();
    }

    public void AddItem(string productId, string productName, int quantity, decimal weight)
    {
        _items.Add(new ShipmentItem(productId, productName, quantity, weight));
        Weight += weight * quantity;
    }

    public void MarkAsShipped()
    {
        if (Status != ShippingStatus.Pending)
            throw new InvalidOperationException($"Cannot ship from status {Status}");

        Status = ShippingStatus.Shipped;
        ShippedAt = DateTime.UtcNow;
    }

    public void MarkAsDelivered(string? signedBy = null)
    {
        Status = ShippingStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
    }

    private static string GenerateTrackingNumber()
    {
        return $"TRK{DateTime.UtcNow:yyyyMMdd}{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }
}

// Shipping Service
public class ShippingService
{
    private readonly IRepository<Shipment> _shipmentRepository;
    private readonly IShippingProvider _shippingProvider;

    public async Task<Shipment> CreateShipmentAsync(CreateShipmentRequest request)
    {
        var shipment = new Shipment(
            orderId: request.OrderId,
            carrier: request.Carrier,
            fromAddress: request.FromAddress,
            toAddress: request.ToAddress,
            shippingCost: new Money(request.ShippingCost, Currency.USD),
            estimatedDelivery: request.EstimatedDelivery
        );

        foreach (var item in request.Items)
        {
            shipment.AddItem(item.ProductId, item.ProductName, item.Quantity, item.Weight);
        }

        await _shipmentRepository.AddAsync(shipment);

        // Create shipping label with carrier
        var label = await _shippingProvider.CreateLabelAsync(new ShippingLabelRequest
        {
            TrackingNumber = shipment.TrackingNumber,
            FromAddress = shipment.FromAddress,
            ToAddress = shipment.ToAddress,
            Weight = shipment.Weight
        });

        return shipment;
    }

    public async Task<TrackingInfo> TrackShipmentAsync(string trackingNumber)
    {
        var shipment = await _shipmentRepository.GetByExpressionAsync(s => s.TrackingNumber == trackingNumber);
        if (shipment == null)
            throw new NotFoundException("Shipment not found");

        var trackingInfo = await _shippingProvider.GetTrackingInfoAsync(trackingNumber);

        // Update shipment status based on tracking info
        if (trackingInfo.Status == "DELIVERED" && shipment.Status != ShippingStatus.Delivered)
        {
            shipment.MarkAsDelivered();
            await _shipmentRepository.UpdateAsync(shipment);
        }

        return trackingInfo;
    }
}
```

### 🔄 Saga Patterns

#### Order Processing Saga
```csharp
// Saga State
public class OrderSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string? CurrentState { get; set; }
    public string? OrderId { get; set; }
    public string? CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public bool PaymentProcessed { get; set; }
    public bool InventoryReserved { get; set; }
    public bool ShippingArranged { get; set; }
    public DateTime OrderDate { get; set; }
}

// Saga State Machine
public class OrderSagaStateMachine : MassTransitStateMachine<OrderSagaState>
{
    public State Processing { get; private set; }
    public State PaymentPending { get; private set; }
    public State InventoryPending { get; private set; }
    public State ShippingPending { get; private set; }
    public State Completed { get; private set; }
    public State Failed { get; private set; }

    public Event<IOrderSubmitted> OrderSubmitted { get; private set; }
    public Event<IPaymentProcessed> PaymentProcessed { get; private set; }
    public Event<IInventoryReserved> InventoryReserved { get; private set; }

    public OrderSagaStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderSubmitted, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => PaymentProcessed, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => InventoryReserved, x => x.CorrelateById(context => context.Message.OrderId));

        Initially(
            When(OrderSubmitted)
                .Then(context =>
                {
                    context.Saga.OrderId = context.Message.OrderId.ToString();
                    context.Saga.CustomerId = context.Message.CustomerId;
                    context.Saga.TotalAmount = context.Message.TotalAmount;
                    context.Saga.OrderDate = DateTime.UtcNow;
                })
                .TransitionTo(Processing)
                .Publish(context => new ProcessPaymentCommand
                {
                    OrderId = context.Message.OrderId,
                    Amount = context.Message.TotalAmount,
                    CustomerId = context.Message.CustomerId
                }));

        During(Processing,
            When(PaymentProcessed)
                .Then(context => context.Saga.PaymentProcessed = true)
                .TransitionTo(PaymentPending)
                .Publish(context => new ReserveInventoryCommand
                {
                    OrderId = Guid.Parse(context.Saga.OrderId!)
                }));

        During(PaymentPending,
            When(InventoryReserved)
                .Then(context => context.Saga.InventoryReserved = true)
                .TransitionTo(InventoryPending)
                .Publish(context => new ArrangeShippingCommand
                {
                    OrderId = Guid.Parse(context.Saga.OrderId!)
                }));
    }
}
```

### 📨 Messaging & Event Streaming

#### RabbitMQ + MassTransit Configuration
```csharp
// Configuration
{
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest",
    "VirtualHost": "/",
    "RetryLimit": 3,
    "RetryInterval": "00:00:30"
  }
}

// Service Registration
services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<OrderSagaStateMachine, OrderSagaState>()
        .InMemoryRepository();

    x.AddConsumer<OrderCreatedEventHandler>();
    x.AddConsumer<PaymentProcessedEventHandler>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(configuration["RabbitMQ:Host"], h =>
        {
            h.Username(configuration["RabbitMQ:Username"]);
            h.Password(configuration["RabbitMQ:Password"]);
        });

        cfg.ConfigureEndpoints(context);

        // Configure retry policy
        cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(30)));
    });
});
```

#### Kafka Configuration
```csharp
// Configuration
{
  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "GroupId": "marventa-consumers",
    "AutoOffsetReset": "Earliest",
    "EnableAutoCommit": false
  }
}

// Kafka Producer
public class KafkaMessageBus : IMessageBus
{
    private readonly IProducer<string, string> _producer;
    private readonly KafkaOptions _options;

    public async Task PublishEventAsync<T>(T eventMessage, string topic = null) where T : IIntegrationEvent
    {
        topic ??= typeof(T).Name.ToLowerInvariant();

        var message = new Message<string, string>
        {
            Key = eventMessage.CorrelationId,
            Value = JsonSerializer.Serialize(eventMessage),
            Headers = new Headers
            {
                { "tenant-id", Encoding.UTF8.GetBytes(_tenantContext.TenantId ?? "global") },
                { "event-type", Encoding.UTF8.GetBytes(typeof(T).Name) },
                { "timestamp", Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("O")) }
            }
        };

        await _producer.ProduceAsync(topic, message);
    }
}

// Kafka Consumer
public abstract class BaseKafkaHandler<T> : IKafkaHandler<T> where T : IIntegrationEvent
{
    private readonly ILogger<BaseKafkaHandler<T>> _logger;
    private readonly ITenantContext _tenantContext;

    public async Task HandleAsync(ConsumeResult<string, string> consumeResult, CancellationToken cancellationToken)
    {
        try
        {
            // Extract tenant context from headers
            if (consumeResult.Message.Headers.TryGetLastBytes("tenant-id", out var tenantIdBytes))
            {
                var tenantId = Encoding.UTF8.GetString(tenantIdBytes);
                _tenantContext.SetTenant(tenantId);
            }

            var eventMessage = JsonSerializer.Deserialize<T>(consumeResult.Message.Value);
            await ProcessEventAsync(eventMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Kafka message: {Topic} {Partition} {Offset}",
                consumeResult.Topic, consumeResult.Partition, consumeResult.Offset);
            throw;
        }
    }

    protected abstract Task ProcessEventAsync(T eventMessage, CancellationToken cancellationToken);
}
```

#### Event Handlers
```csharp
// RabbitMQ Event Handler
public class OrderCreatedEventHandler : IDomainEventHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger;
    private readonly IEventBus _eventBus;

    public async Task HandleAsync(OrderCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing order created event for order {OrderId} in tenant {TenantId}",
            domainEvent.OrderId, _tenantContext.TenantId);

        // Convert to integration event for external systems
        var integrationEvent = new OrderCreatedIntegrationEvent
        {
            OrderId = domainEvent.OrderId,
            CustomerId = domainEvent.CustomerId,
            CorrelationId = Guid.NewGuid().ToString(),
            OccurredOn = DateTime.UtcNow,
            TenantId = _tenantContext.TenantId
        };

        await _eventBus.PublishIntegrationEventAsync(integrationEvent, cancellationToken);
    }
}

// Kafka Event Handler
public class PaymentProcessedKafkaHandler : BaseKafkaHandler<PaymentProcessedIntegrationEvent>
{
    private readonly IMediator _mediator;

    protected override async Task ProcessEventAsync(PaymentProcessedIntegrationEvent eventMessage, CancellationToken cancellationToken)
    {
        var command = new UpdateOrderPaymentStatusCommand(
            eventMessage.OrderId,
            PaymentStatus.Completed,
            eventMessage.TransactionId
        );

        await _mediator.Send(command, cancellationToken);
    }
}
```

### 📤 Outbox/Inbox Patterns

#### Outbox Implementation
```csharp
// Outbox Message
public class OutboxMessage : BaseEntity
{
    public string Type { get; set; }
    public string Data { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    public string? Error { get; set; }
    public int RetryCount { get; set; } = 0;
    public string? IdempotencyKey { get; set; }

    public bool IsProcessed => ProcessedAt.HasValue;
    public bool HasFailed => !string.IsNullOrEmpty(Error);
    public bool ShouldRetry => RetryCount < 3 && !IsProcessed;

    public static OutboxMessage Create<T>(T message, string? idempotencyKey = null) where T : class
    {
        return new OutboxMessage
        {
            Type = typeof(T).Name,
            Data = JsonSerializer.Serialize(message),
            IdempotencyKey = idempotencyKey
        };
    }
}

// Outbox Service
public class OutboxService : IOutboxService
{
    private readonly IRepository<OutboxMessage> _outboxRepository;

    public async Task SaveMessageAsync<T>(T message, string? idempotencyKey = null) where T : class
    {
        var outboxMessage = OutboxMessage.Create(message, idempotencyKey);
        await _outboxRepository.AddAsync(outboxMessage);
    }

    public async Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize = 100)
    {
        return await _outboxRepository.GetListAsync(
            predicate: m => !m.IsProcessed && m.ShouldRetry,
            orderBy: m => m.CreatedAt,
            take: batchSize
        );
    }

    public async Task MarkAsProcessedAsync(Guid messageId)
    {
        var message = await _outboxRepository.GetByIdAsync(messageId);
        if (message != null)
        {
            message.ProcessedAt = DateTime.UtcNow;
            await _outboxRepository.UpdateAsync(message);
        }
    }
}

// Background Outbox Dispatcher
public class OutboxDispatcher : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxDispatcher> _logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var outboxService = scope.ServiceProvider.GetRequiredService<IOutboxService>();
                var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

                var messages = await outboxService.GetUnprocessedMessagesAsync();

                foreach (var message in messages)
                {
                    try
                    {
                        // Deserialize and publish the message
                        var eventType = Type.GetType($"YourApp.Events.{message.Type}");
                        var @event = JsonSerializer.Deserialize(message.Data, eventType!);

                        await eventBus.PublishIntegrationEventAsync(@event as IIntegrationEvent);
                        await outboxService.MarkAsProcessedAsync(message.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);
                        // Implement retry logic
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in outbox dispatcher");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
```

### 💵 Money/Currency Value Objects

```csharp
// Currency Value Object
public class Currency : ValueObject
{
    public static readonly Currency USD = new("USD", "$", 2);
    public static readonly Currency EUR = new("EUR", "€", 2);
    public static readonly Currency GBP = new("GBP", "£", 2);
    public static readonly Currency JPY = new("JPY", "¥", 0);
    public static readonly Currency TRY = new("TRY", "₺", 2);

    public string Code { get; }
    public string Symbol { get; }
    public int DecimalPlaces { get; }

    private Currency(string code, string symbol, int decimalPlaces)
    {
        Code = code;
        Symbol = symbol;
        DecimalPlaces = decimalPlaces;
    }

    public static Currency FromCode(string code)
    {
        return code.ToUpper() switch
        {
            "USD" => USD,
            "EUR" => EUR,
            "GBP" => GBP,
            "JPY" => JPY,
            "TRY" => TRY,
            _ => throw new ArgumentException($"Unsupported currency: {code}")
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }
}

// Money Value Object
public class Money : ValueObject
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    public Money(decimal amount, Currency currency)
    {
        Amount = Math.Round(amount, currency.DecimalPlaces);
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));
    }

    // Arithmetic Operations
    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies");

        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot subtract money with different currencies");

        return new Money(left.Amount - right.Amount, left.Currency);
    }

    public static Money operator *(Money money, decimal factor)
    {
        return new Money(money.Amount * factor, money.Currency);
    }

    // Business Operations
    public Money ApplyTax(decimal taxRate)
    {
        return new Money(Amount * (1 + taxRate), Currency);
    }

    public Money ApplyDiscount(decimal discountRate)
    {
        if (discountRate < 0 || discountRate > 1)
            throw new ArgumentOutOfRangeException(nameof(discountRate));

        return new Money(Amount * (1 - discountRate), Currency);
    }

    public Money ConvertTo(Currency targetCurrency, decimal exchangeRate)
    {
        return new Money(Amount * exchangeRate, targetCurrency);
    }

    public string Format()
    {
        return $"{Currency.Symbol}{Amount:N2}";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}

// Usage Examples
public class OrderService
{
    public async Task<Order> CalculateOrderTotalAsync(List<OrderItem> items, decimal taxRate, decimal discountRate)
    {
        var subtotal = Money.Zero(Currency.USD);

        foreach (var item in items)
        {
            var itemTotal = new Money(item.UnitPrice, Currency.USD) * item.Quantity;
            subtotal += itemTotal;
        }

        var discounted = subtotal.ApplyDiscount(discountRate);
        var total = discounted.ApplyTax(taxRate);

        return new Order
        {
            Items = items,
            Subtotal = subtotal,
            DiscountAmount = subtotal - discounted,
            TaxAmount = total - discounted,
            Total = total
        };
    }
}
```

### 🗄️ Caching (Redis & In-Memory)

#### Redis Distributed Caching
```csharp
// Configuration
{
  "Redis": {
    "ConnectionString": "localhost:6379",
    "DatabaseId": 0,
    "KeyPrefix": "MyApp",
    "DefaultExpiration": "00:05:00"
  }
}

// Service Registration
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration.GetConnectionString("Redis");
});
```

#### In-Memory Caching
```csharp
// Configuration
services.AddMemoryCache(options =>
{
    options.SizeLimit = 1024; // 1GB
    options.CompactionPercentage = 0.25;
});
```

#### Tenant-Scoped Caching
```csharp
public class TenantScopedCache : ITenantScopedCache
{
    private readonly IDistributedCache _distributedCache;
    private readonly ITenantContext _tenantContext;

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var tenantKey = GetTenantScopedKey(key);
        var cached = await _distributedCache.GetStringAsync(tenantKey, cancellationToken);

        if (string.IsNullOrEmpty(cached))
            return default;

        return JsonSerializer.Deserialize<T>(cached);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var tenantKey = GetTenantScopedKey(key);
        var json = JsonSerializer.Serialize(value);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
        };

        await _distributedCache.SetStringAsync(tenantKey, json, options, cancellationToken);
    }

    private string GetTenantScopedKey(string key)
    {
        var tenantId = _tenantContext.TenantId ?? "global";
        return $"tenant:{tenantId}:{key}";
    }
}

// Usage
public class ProductService
{
    private readonly ITenantScopedCache _cache;
    private readonly IMemoryCache _memoryCache;
    private readonly IRepository<Product> _productRepository;

    public async Task<Product?> GetProductAsync(Guid productId)
    {
        var cacheKey = $"product:{productId}";

        // Try memory cache first (fastest)
        if (_memoryCache.TryGetValue(cacheKey, out Product? cachedProduct))
            return cachedProduct;

        // Try distributed cache (tenant-scoped)
        cachedProduct = await _cache.GetAsync<Product>(cacheKey);
        if (cachedProduct != null)
        {
            // Store in memory cache for 2 minutes
            _memoryCache.Set(cacheKey, cachedProduct, TimeSpan.FromMinutes(2));
            return cachedProduct;
        }

        // Get from database
        var product = await _productRepository.GetByIdAsync(productId);
        if (product != null)
        {
            // Cache in both levels
            await _cache.SetAsync(cacheKey, product, TimeSpan.FromMinutes(10));
            _memoryCache.Set(cacheKey, product, TimeSpan.FromMinutes(2));
        }

        return product;
    }
}
```

### 🚦 Rate Limiting

```csharp
// Configuration
public class RateLimitOptions
{
    public bool EnableRateLimiting { get; set; } = true;
    public int MaxRequests { get; set; } = 100;
    public TimeSpan WindowSize { get; set; } = TimeSpan.FromMinutes(1);
    public Dictionary<string, EndpointRateLimit> EndpointLimits { get; set; } = new();
}

public class EndpointRateLimit
{
    public int MaxRequests { get; set; }
    public TimeSpan WindowSize { get; set; }
}

// Tenant-Aware Rate Limiter
public class TenantRateLimiter : ITenantRateLimiter
{
    private readonly ICacheService _cache;
    private readonly ITenantContext _tenantContext;

    public async Task<RateLimitResult> CheckRateLimitAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantContext.TenantId ?? "global";
        var key = $"rate_limit:{tenantId}:{endpoint}";

        var currentCount = await _cache.GetAsync<int?>(key) ?? 0;
        var maxRequests = GetMaxRequestsForEndpoint(endpoint);
        var windowSize = GetWindowSizeForEndpoint(endpoint);

        if (currentCount >= maxRequests)
        {
            return new RateLimitResult
            {
                IsAllowed = false,
                Remaining = 0,
                ResetTime = DateTimeOffset.UtcNow.Add(windowSize)
            };
        }

        await _cache.SetAsync(key, currentCount + 1, windowSize);

        return new RateLimitResult
        {
            IsAllowed = true,
            Remaining = maxRequests - currentCount - 1,
            ResetTime = DateTimeOffset.UtcNow.Add(windowSize)
        };
    }
}

// Rate Limit Attribute
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RateLimitAttribute : Attribute
{
    public int MaxRequests { get; set; } = 100;
    public int WindowSizeInSeconds { get; set; } = 60;
}

// Usage
[RateLimit(MaxRequests = 50, WindowSizeInSeconds = 60)]
public class ProductsController : ControllerBase
{
    [HttpGet]
    [RateLimit(MaxRequests = 20, WindowSizeInSeconds = 60)] // Override for specific endpoint
    public async Task<IActionResult> GetProducts()
    {
        // This endpoint is limited to 20 requests per minute per tenant
        return Ok(await _productService.GetProductsAsync());
    }
}
```

### 🏥 Health Checks

```csharp
// Database Health Check
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly IConnectionFactory _connectionFactory;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            await connection.OpenAsync(cancellationToken);

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            await command.ExecuteScalarAsync(cancellationToken);

            return HealthCheckResult.Healthy("Database is responsive");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database is not responsive", ex);
        }
    }
}

// Tenant Health Check
public class TenantHealthCheck : IHealthCheck
{
    private readonly ITenantStore _tenantStore;
    private readonly ITenantContext _tenantContext;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenants = await _tenantStore.GetAllAsync();
            var activeTenants = tenants.Count();

            var data = new Dictionary<string, object>
            {
                ["total_tenants"] = activeTenants,
                ["current_tenant"] = _tenantContext.TenantId ?? "none"
            };

            return HealthCheckResult.Healthy($"Multi-tenancy system is healthy with {activeTenants} active tenants", data);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Multi-tenancy system is not healthy", ex);
        }
    }
}

// Registration
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database")
    .AddCheck<TenantHealthCheck>("tenancy")
    .AddCheck<CacheHealthCheck>("cache")
    .AddCheck("external-api", () => HealthCheckResult.Healthy("External API is responsive"));

// Health Check Controller
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var report = await _healthCheckService.CheckHealthAsync();

        var result = new
        {
            Status = report.Status.ToString(),
            TotalDuration = report.TotalDuration,
            Entries = report.Entries.Select(e => new
            {
                Name = e.Key,
                Status = e.Value.Status.ToString(),
                Duration = e.Value.Duration,
                Description = e.Value.Description,
                Data = e.Value.Data
            })
        };

        return report.Status == HealthStatus.Healthy ? Ok(result) : StatusCode(503, result);
    }
}
```

### 📊 API Versioning

```csharp
// Configuration
public class ApiVersioningOptions
{
    public string DefaultVersion { get; set; } = "1.0";
    public bool AssumeDefaultVersionWhenUnspecified { get; set; } = true;
    public ApiVersionReader ApiVersionReader { get; set; } = ApiVersionReader.Header;
    public string HeaderName { get; set; } = "X-Api-Version";
    public string QueryParameterName { get; set; } = "version";
}

// Versioned Controller
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetProductsV1()
    {
        // Version 1.0 implementation
        var products = await _productService.GetProductsAsync();
        return Ok(products.Select(p => new ProductV1Dto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price.Amount
        }));
    }

    [HttpGet]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> GetProductsV2()
    {
        // Version 2.0 implementation with additional fields
        var products = await _productService.GetProductsAsync();
        return Ok(products.Select(p => new ProductV2Dto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Currency = p.Price.Currency.Code,
            CreatedDate = p.CreatedDate,
            Tags = p.Tags
        }));
    }
}

// Version-specific DTOs
public class ProductV1Dto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class ProductV2Dto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Money Price { get; set; }
    public string Currency { get; set; }
    public DateTime CreatedDate { get; set; }
    public List<string> Tags { get; set; }
}
```

### 🔒 Security & Encryption

```csharp
// Encryption Service
public class EncryptionService : IEncryptionService
{
    private readonly IDataProtector _dataProtector;

    public EncryptionService(IDataProtectionProvider dataProtectionProvider)
    {
        _dataProtector = dataProtectionProvider.CreateProtector("Marventa.Framework.Encryption");
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;

        return _dataProtector.Protect(plainText);
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return cipherText;

        try
        {
            return _dataProtector.Unprotect(cipherText);
        }
        catch
        {
            throw new InvalidOperationException("Unable to decrypt data");
        }
    }

    public string Hash(string input, string salt = null)
    {
        salt ??= GenerateSalt();
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input + salt));
        return Convert.ToBase64String(hashedBytes);
    }

    private static string GenerateSalt()
    {
        var salt = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        return Convert.ToBase64String(salt);
    }
}

// Sensitive Data Attribute
[AttributeUsage(AttributeTargets.Property)]
public class SensitiveDataAttribute : Attribute
{
    public bool ShouldEncrypt { get; set; } = true;
    public bool ShouldHash { get; set; } = false;
}

// Entity with Sensitive Data
public class Customer : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    [SensitiveData(ShouldEncrypt = true)]
    public string Email { get; set; }

    [SensitiveData(ShouldEncrypt = true)]
    public string PhoneNumber { get; set; }

    [SensitiveData(ShouldHash = true)]
    public string SocialSecurityNumber { get; set; }
}

// Automatic Encryption Interceptor
public class EncryptionInterceptor : IEntityInterceptor
{
    private readonly IEncryptionService _encryptionService;

    public void BeforeSaving(object entity)
    {
        var properties = entity.GetType().GetProperties()
            .Where(p => p.GetCustomAttribute<SensitiveDataAttribute>() != null);

        foreach (var property in properties)
        {
            var attribute = property.GetCustomAttribute<SensitiveDataAttribute>();
            var value = property.GetValue(entity) as string;

            if (!string.IsNullOrEmpty(value))
            {
                if (attribute.ShouldEncrypt)
                {
                    property.SetValue(entity, _encryptionService.Encrypt(value));
                }
                else if (attribute.ShouldHash)
                {
                    property.SetValue(entity, _encryptionService.Hash(value));
                }
            }
        }
    }

    public void AfterLoading(object entity)
    {
        var properties = entity.GetType().GetProperties()
            .Where(p => p.GetCustomAttribute<SensitiveDataAttribute>()?.ShouldEncrypt == true);

        foreach (var property in properties)
        {
            var value = property.GetValue(entity) as string;
            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    property.SetValue(entity, _encryptionService.Decrypt(value));
                }
                catch
                {
                    // Handle decryption failure
                }
            }
        }
    }
}
```

### 📧 Communication Services

```csharp
// Email Service
public class EmailService : IEmailService
{
    private readonly ISmtpClient _smtpClient;
    private readonly EmailOptions _options;
    private readonly ITenantContext _tenantContext;

    public async Task SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_options.FromEmail, _options.FromName),
            Subject = message.Subject,
            Body = message.Body,
            IsBodyHtml = message.IsHtml
        };

        foreach (var recipient in message.To)
        {
            mailMessage.To.Add(recipient);
        }

        // Add tenant-specific headers
        if (!string.IsNullOrEmpty(_tenantContext.TenantId))
        {
            mailMessage.Headers.Add("X-Tenant-Id", _tenantContext.TenantId);
        }

        try
        {
            await _smtpClient.SendAsync(mailMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new EmailDeliveryException($"Failed to send email: {ex.Message}", ex);
        }
    }

    public async Task SendTemplatedEmailAsync<T>(string templateName, T model, string[] recipients, CancellationToken cancellationToken = default) where T : class
    {
        var template = await LoadTemplateAsync(templateName);
        var body = await RenderTemplateAsync(template, model);

        var message = new EmailMessage
        {
            To = recipients,
            Subject = template.Subject,
            Body = body,
            IsHtml = true
        };

        await SendEmailAsync(message, cancellationToken);
    }
}

// SMS Service
public class SmsService : ISmsService
{
    private readonly ISmsProvider _smsProvider;
    private readonly SmsOptions _options;

    public async Task SendSmsAsync(SmsMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            await _smsProvider.SendAsync(new SmsRequest
            {
                To = message.PhoneNumber,
                Message = message.Text,
                From = _options.FromNumber
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new SmsDeliveryException($"Failed to send SMS: {ex.Message}", ex);
        }
    }

    public async Task SendBulkSmsAsync(BulkSmsMessage message, CancellationToken cancellationToken = default)
    {
        var tasks = message.PhoneNumbers.Select(phoneNumber =>
            SendSmsAsync(new SmsMessage { PhoneNumber = phoneNumber, Text = message.Text }, cancellationToken)
        );

        await Task.WhenAll(tasks);
    }
}

// Usage Example
public class OrderNotificationService
{
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;

    public async Task NotifyOrderCreatedAsync(Order order)
    {
        // Send email notification
        await _emailService.SendTemplatedEmailAsync("order-created", new
        {
            OrderId = order.Id,
            CustomerName = order.CustomerName,
            Total = order.Total.Format(),
            Items = order.Items
        }, new[] { order.CustomerEmail });

        // Send SMS notification if phone number is provided
        if (!string.IsNullOrEmpty(order.CustomerPhone))
        {
            await _smsService.SendSmsAsync(new SmsMessage
            {
                PhoneNumber = order.CustomerPhone,
                Text = $"Your order #{order.Id} has been created successfully. Total: {order.Total.Format()}"
            });
        }
    }
}
```

### ⚡ Circuit Breaker Pattern

```csharp
// Circuit Breaker Implementation
public class CircuitBreakerHandler : DelegatingHandler
{
    private readonly CircuitBreakerOptions _options;
    private readonly ILogger<CircuitBreakerHandler> _logger;
    private readonly object _lock = new();
    private CircuitState _state = CircuitState.Closed;
    private int _failureCount = 0;
    private DateTime _lastFailureTime = DateTime.MinValue;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_state == CircuitState.Open)
        {
            if (DateTime.UtcNow - _lastFailureTime < _options.OpenTimeout)
            {
                throw new CircuitBreakerOpenException("Circuit breaker is open");
            }

            _state = CircuitState.HalfOpen;
        }

        try
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (IsSuccessStatusCode(response.StatusCode))
            {
                OnSuccess();
                return response;
            }

            OnFailure();
            return response;
        }
        catch (Exception)
        {
            OnFailure();
            throw;
        }
    }

    private void OnSuccess()
    {
        lock (_lock)
        {
            _failureCount = 0;
            _state = CircuitState.Closed;
        }
    }

    private void OnFailure()
    {
        lock (_lock)
        {
            _failureCount++;
            _lastFailureTime = DateTime.UtcNow;

            if (_failureCount >= _options.FailureThreshold)
            {
                _state = CircuitState.Open;
                _logger.LogWarning("Circuit breaker opened after {FailureCount} failures", _failureCount);
            }
        }
    }

    private static bool IsSuccessStatusCode(HttpStatusCode statusCode)
    {
        return (int)statusCode >= 200 && (int)statusCode <= 299;
    }
}

// Usage with HttpClient
public class ExternalApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExternalApiService> _logger;

    public ExternalApiService(HttpClient httpClient, ILogger<ExternalApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<T>(content);
                return ApiResponse<T>.SuccessResult(data);
            }

            return ApiResponse<T>.FailureResult($"API call failed with status: {response.StatusCode}");
        }
        catch (CircuitBreakerOpenException ex)
        {
            _logger.LogWarning("Circuit breaker is open: {Message}", ex.Message);
            return ApiResponse<T>.FailureResult("Service temporarily unavailable");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling external API");
            return ApiResponse<T>.FailureResult(ex.Message);
        }
    }
}

// Registration
builder.Services.AddHttpClient<ExternalApiService>(client =>
{
    client.BaseAddress = new Uri("https://api.example.com/");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<CircuitBreakerHandler>();

builder.Services.Configure<CircuitBreakerOptions>(options =>
{
    options.FailureThreshold = 5;
    options.OpenTimeout = TimeSpan.FromMinutes(1);
});
```

### 🎛️ Feature Flags

```csharp
// Feature Flag Service
public class FeatureFlagService : IFeatureFlagService
{
    private readonly IConfiguration _configuration;
    private readonly ITenantContext _tenantContext;
    private readonly ICacheService _cache;

    public async Task<bool> IsEnabledAsync(string featureName, CancellationToken cancellationToken = default)
    {
        // Check tenant-specific feature flags first
        if (_tenantContext.HasTenant)
        {
            var tenantFlag = await GetTenantFeatureFlagAsync(featureName, _tenantContext.TenantId!, cancellationToken);
            if (tenantFlag.HasValue)
                return tenantFlag.Value;
        }

        // Fallback to global feature flags
        var globalFlag = await GetGlobalFeatureFlagAsync(featureName, cancellationToken);
        return globalFlag;
    }

    public async Task<T> GetFeatureValueAsync<T>(string featureName, T defaultValue, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = $"feature_value:{_tenantContext.TenantId}:{featureName}";
            var cachedValue = await _cache.GetAsync<T>(cacheKey, cancellationToken);

            if (cachedValue != null)
                return cachedValue;

            var configValue = _configuration[$"FeatureFlags:{featureName}:Value"];
            if (!string.IsNullOrEmpty(configValue))
            {
                var convertedValue = (T)Convert.ChangeType(configValue, typeof(T));
                await _cache.SetAsync(cacheKey, convertedValue, TimeSpan.FromMinutes(5), cancellationToken);
                return convertedValue;
            }

            return defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }

    private async Task<bool?> GetTenantFeatureFlagAsync(string featureName, string tenantId, CancellationToken cancellationToken)
    {
        var cacheKey = $"tenant_feature:{tenantId}:{featureName}";
        return await _cache.GetAsync<bool?>(cacheKey, cancellationToken);
    }

    private async Task<bool> GetGlobalFeatureFlagAsync(string featureName, CancellationToken cancellationToken)
    {
        var cacheKey = $"global_feature:{featureName}";
        var cachedValue = await _cache.GetAsync<bool?>(cacheKey, cancellationToken);

        if (cachedValue.HasValue)
            return cachedValue.Value;

        var configValue = _configuration.GetValue<bool>($"FeatureFlags:{featureName}:Enabled");
        await _cache.SetAsync(cacheKey, configValue, TimeSpan.FromMinutes(10), cancellationToken);

        return configValue;
    }
}

// Feature Flag Attribute
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class FeatureFlagAttribute : Attribute, IAsyncActionFilter
{
    private readonly string _featureName;

    public FeatureFlagAttribute(string featureName)
    {
        _featureName = featureName;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var featureFlagService = context.HttpContext.RequestServices.GetRequiredService<IFeatureFlagService>();

        if (await featureFlagService.IsEnabledAsync(_featureName))
        {
            await next();
        }
        else
        {
            context.Result = new NotFoundResult();
        }
    }
}

// Usage
[FeatureFlag("NewCheckoutProcess")]
public class CheckoutController : ControllerBase
{
    private readonly IFeatureFlagService _featureFlags;

    [HttpPost("process")]
    public async Task<IActionResult> ProcessCheckout(CheckoutRequest request)
    {
        // Feature is automatically checked by attribute

        var discountEnabled = await _featureFlags.IsEnabledAsync("DiscountCalculation");
        var maxDiscount = await _featureFlags.GetFeatureValueAsync("MaxDiscountPercentage", 0.1m);

        if (discountEnabled)
        {
            // Apply discount logic with feature-controlled max discount
            request.DiscountPercentage = Math.Min(request.DiscountPercentage, maxDiscount);
        }

        return Ok();
    }
}

// Configuration
{
  "FeatureFlags": {
    "NewCheckoutProcess": {
      "Enabled": true,
      "Description": "New checkout process with enhanced validation"
    },
    "DiscountCalculation": {
      "Enabled": false,
      "Description": "Advanced discount calculation"
    },
    "MaxDiscountPercentage": {
      "Value": "0.15",
      "Description": "Maximum discount percentage allowed"
    }
  }
}
```

### 📝 Validation with FluentValidation

```csharp
// Base Validator
public abstract class BaseValidator<T> : AbstractValidator<T> where T : class
{
    protected void RuleForMoney(Expression<Func<T, Money>> expression)
    {
        RuleFor(expression)
            .NotNull().WithMessage("Money amount is required")
            .Must(money => money.Amount >= 0).WithMessage("Money amount must be non-negative");
    }

    protected void RuleForEmail(Expression<Func<T, string>> expression)
    {
        RuleFor(expression)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email format is invalid")
            .MaximumLength(100).WithMessage("Email is too long");
    }

    protected void RuleForPhoneNumber(Expression<Func<T, string>> expression)
    {
        RuleFor(expression)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Phone number format is invalid");
    }

    protected void RuleForTenantId(Expression<Func<T, string>> expression)
    {
        RuleFor(expression)
            .NotEmpty().WithMessage("Tenant ID is required")
            .Length(1, 50).WithMessage("Tenant ID must be between 1 and 50 characters");
    }
}

// Command Validator
public class CreateOrderValidator : BaseValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Order must contain at least one item")
            .Must(items => items.Count <= 100).WithMessage("Order cannot contain more than 100 items");

        RuleForEach(x => x.Items).SetValidator(new OrderItemValidator());

        RuleFor(x => x.ShippingAddress)
            .NotEmpty().WithMessage("Shipping address is required")
            .Length(10, 500).WithMessage("Shipping address must be between 10 and 500 characters");

        RuleFor(x => x.TotalAmount)
            .Must(amount => amount > 0).WithMessage("Order total must be greater than zero");

        RuleForMoney(x => x.TotalAmount);
    }
}

public class OrderItemValidator : BaseValidator<OrderItem>
{
    public OrderItemValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero")
            .LessThanOrEqualTo(1000).WithMessage("Quantity cannot exceed 1000");

        RuleForMoney(x => x.UnitPrice);

        RuleFor(x => x.UnitPrice)
            .Must(money => money.Amount > 0).WithMessage("Unit price must be greater than zero");
    }
}

// Validation Behavior
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken))
        );

        var failures = validationResults
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Any())
        {
            var problemDetails = new ValidationProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "One or more validation errors occurred",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Please refer to the errors property for additional details.",
                Instance = $"urn:validation:{Guid.NewGuid()}"
            };

            foreach (var error in failures)
            {
                if (problemDetails.Errors.ContainsKey(error.PropertyName))
                {
                    problemDetails.Errors[error.PropertyName] =
                        problemDetails.Errors[error.PropertyName].Concat(new[] { error.ErrorMessage }).ToArray();
                }
                else
                {
                    problemDetails.Errors.Add(error.PropertyName, new[] { error.ErrorMessage });
                }
            }

            throw new ValidationException(problemDetails);
        }

        return await next();
    }
}

// Global Exception Handler for Validation
public class ValidationExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ValidationExceptionMiddleware> _logger;

    public ValidationExceptionMiddleware(RequestDelegate next, ILogger<ValidationExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("Validation failed: {@ValidationErrors}", ex.ProblemDetails.Errors);

            context.Response.StatusCode = ex.ProblemDetails.Status ?? StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/problem+json";

            var json = JsonSerializer.Serialize(ex.ProblemDetails, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}
```

### 📊 Structured Logging & Observability

#### Serilog Configuration
```csharp
// Configuration
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {CorrelationId} {TenantId} {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/app-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30,
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {CorrelationId} {TenantId} {SourceContext} {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "Elasticsearch",
        "Args": {
          "nodeUris": "http://localhost:9200",
          "indexFormat": "marventa-logs-{0:yyyy.MM}",
          "autoRegisterTemplate": true
        }
      }
    ],
    "Enrich": ["FromLogContext", "WithMachineName", "WithThreadId"],
    "Properties": {
      "Application": "Marventa.Framework"
    }
  }
}

// Service Registration
services.AddSerilog((serviceProvider, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(configuration)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "Marventa.Framework")
        .Enrich.WithProperty("Environment", environment.EnvironmentName);
});
```

#### OpenTelemetry Integration
```csharp
// Activity Service with Tenant Context
public class ActivityService : IActivityService
{
    private static readonly ActivitySource ActivitySource = new("Marventa.Framework");
    private readonly ITenantContext _tenantContext;
    private readonly ICorrelationContext _correlationContext;

    public Activity? StartActivity(string name, Dictionary<string, string>? tags = null)
    {
        var activity = ActivitySource.StartActivity(name);

        if (activity != null)
        {
            // Add tenant information
            if (_tenantContext.HasTenant)
            {
                activity.SetTag("tenant.id", _tenantContext.TenantId);
                activity.SetTag("tenant.name", _tenantContext.CurrentTenant?.Name);
            }

            // Add correlation context
            activity.SetTag("correlation.id", _correlationContext.CorrelationId);
            activity.SetTag("user.id", _correlationContext.UserId);

            // Add custom tags
            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    activity.SetTag(tag.Key, tag.Value);
                }
            }
        }

        return activity;
    }

    public void RecordException(Activity activity, Exception exception)
    {
        activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
        activity?.SetTag("error.type", exception.GetType().Name);
        activity?.SetTag("error.message", exception.Message);
        activity?.SetTag("error.stack", exception.StackTrace);
    }
}

// Structured Logging in Services
public class OrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly ITenantContext _tenantContext;
    private readonly ICorrelationContext _correlationContext;

    public async Task<Order> CreateOrderAsync(CreateOrderCommand command)
    {
        using var activity = _activityService.StartActivity("order.create");

        // Structured logging with context
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["TenantId"] = _tenantContext.TenantId ?? "global",
            ["CorrelationId"] = _correlationContext.CorrelationId,
            ["UserId"] = _correlationContext.UserId ?? "anonymous",
            ["OperationType"] = "CreateOrder"
        });

        try
        {
            _logger.LogInformation("Creating order for customer {CustomerId} with {ItemCount} items",
                command.CustomerId, command.Items.Count);

            var order = new Order(command.CustomerId, command.Items);

            _logger.LogInformation("Order {OrderId} created successfully with total {Total}",
                order.Id, order.Total.Format());

            activity?.SetTag("order.id", order.Id.ToString());
            activity?.SetStatus(ActivityStatusCode.Ok);

            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create order for customer {CustomerId}", command.CustomerId);
            _activityService.RecordException(activity!, ex);
            throw;
        }
    }
}

// OpenTelemetry Configuration
services.AddOpenTelemetry()
    .WithTracing(builder =>
    {
        builder
            .AddSource("Marventa.Framework")
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
                options.EnrichWithHttpRequest = (activity, request) =>
                {
                    activity.SetTag("http.request.size", request.ContentLength);
                    activity.SetTag("http.client.ip", request.HttpContext.Connection.RemoteIpAddress?.ToString());
                };
                options.EnrichWithHttpResponse = (activity, response) =>
                {
                    activity.SetTag("http.response.size", response.ContentLength);
                };
            })
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddRedisInstrumentation()
            .AddJaegerExporter()
            .AddConsoleExporter();
    })
    .WithMetrics(builder =>
    {
        builder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddPrometheusExporter();
    });
```

### 🔍 Search with Elasticsearch

```csharp
// Configuration
{
  "Elasticsearch": {
    "Uri": "http://localhost:9200",
    "DefaultIndex": "marventa-search",
    "Username": "elastic",
    "Password": "changeme",
    "EnableDebugMode": false
  }
}

// Search Service Implementation
public class ElasticsearchService : ISearchService
{
    private readonly HttpClient _httpClient;
    private readonly ElasticsearchOptions _options;
    private readonly ITenantContext _tenantContext;

    public async Task<SearchResult<T>> SearchAsync<T>(SearchRequest request, CancellationToken cancellationToken = default) where T : class
    {
        var indexName = GetTenantScopedIndex(request.IndexName ?? _options.DefaultIndex);

        var searchQuery = new
        {
            query = new
            {
                bool = new
                {
                    must = new[]
                    {
                        new { multi_match = new { query = request.Query, fields = new[] { "*" } } }
                    },
                    filter = new[]
                    {
                        new { term = new { tenant_id = _tenantContext.TenantId ?? "global" } }
                    }
                }
            },
            from = (request.Page - 1) * request.PageSize,
            size = request.PageSize,
            highlight = new
            {
                fields = new { @"*" = new { } }
            }
        };

        var response = await _httpClient.PostAsync($"{indexName}/_search",
            new StringContent(JsonSerializer.Serialize(searchQuery), Encoding.UTF8, "application/json"),
            cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var elasticResponse = JsonSerializer.Deserialize<ElasticsearchResponse<T>>(content);

        return new SearchResult<T>
        {
            Items = elasticResponse.Hits.Hits.Select(h => h.Source).ToList(),
            TotalCount = elasticResponse.Hits.Total.Value,
            Page = request.Page,
            PageSize = request.PageSize,
            ExecutionTime = elasticResponse.Took
        };
    }

    public async Task IndexDocumentAsync<T>(T document, string? id = null, string? indexName = null, CancellationToken cancellationToken = default) where T : class
    {
        indexName = GetTenantScopedIndex(indexName ?? _options.DefaultIndex);
        id ??= Guid.NewGuid().ToString();

        // Add tenant context to document
        var documentWithContext = new
        {
            tenant_id = _tenantContext.TenantId ?? "global",
            indexed_at = DateTime.UtcNow,
            document = document
        };

        var response = await _httpClient.PutAsync($"{indexName}/_doc/{id}",
            new StringContent(JsonSerializer.Serialize(documentWithContext), Encoding.UTF8, "application/json"),
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    private string GetTenantScopedIndex(string baseIndex)
    {
        var tenantId = _tenantContext.TenantId ?? "global";
        return $"{baseIndex}-{tenantId}";
    }
}

// Usage Example
public class ProductSearchService
{
    private readonly ISearchService _searchService;

    public async Task<SearchResult<ProductSearchDto>> SearchProductsAsync(string query, int page = 1, int pageSize = 20)
    {
        var searchRequest = new SearchRequest
        {
            Query = query,
            Page = page,
            PageSize = pageSize,
            IndexName = "products"
        };

        return await _searchService.SearchAsync<ProductSearchDto>(searchRequest);
    }

    public async Task IndexProductAsync(Product product)
    {
        var searchDto = new ProductSearchDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price.Amount,
            Currency = product.Price.Currency.Code,
            CategoryName = product.Category?.Name,
            Tags = product.Tags,
            CreatedAt = product.CreatedDate
        };

        await _searchService.IndexDocumentAsync(searchDto, product.Id.ToString(), "products");
    }
}
```

### ☁️ Cloud Storage Abstraction

```csharp
// Storage Service Usage
public class DocumentService
{
    private readonly IStorageService _storage;

    public async Task<StorageFile> UploadDocumentAsync(IFormFile file, string folder = "documents")
    {
        using var stream = file.OpenReadStream();
        return await _storage.UploadAsync(stream, file.FileName, folder);
    }

    public async Task<byte[]> DownloadDocumentAsync(string fileKey)
    {
        return await _storage.DownloadBytesAsync(fileKey);
    }

    public async Task<string> GeneratePreSignedUrlAsync(string fileKey, TimeSpan expiration)
    {
        return await _storage.GetPresignedUrlAsync(fileKey, expiration);
    }
}

// Configuration for Different Providers
public class StorageConfiguration
{
    // AWS S3
    services.Configure<CloudStorageOptions>(options =>
    {
        options.Provider = StorageProvider.S3;
        options.ConnectionString = "DefaultEndpointsProtocol=https;AccountName=myaccount;AccountKey=mykey";
        options.ContainerName = "documents";
        options.BaseUrl = "https://mybucket.s3.amazonaws.com";
    });

    // Azure Blob Storage
    services.Configure<CloudStorageOptions>(options =>
    {
        options.Provider = StorageProvider.AzureBlob;
        options.ConnectionString = "DefaultEndpointsProtocol=https;AccountName=myaccount;AccountKey=mykey";
        options.ContainerName = "documents";
    });

    // Google Cloud Storage
    services.Configure<CloudStorageOptions>(options =>
    {
        options.Provider = StorageProvider.GoogleCloud;
        options.ConnectionString = "/path/to/service-account.json";
        options.ContainerName = "my-bucket";
    });
}

// Multi-tenant Storage with Tenant Isolation
public class TenantStorageService
{
    private readonly IStorageService _storage;
    private readonly ITenantContext _tenantContext;

    public async Task<StorageFile> UploadTenantFileAsync(IFormFile file, string category)
    {
        var tenantFolder = $"tenant_{_tenantContext.TenantId}/{category}";
        using var stream = file.OpenReadStream();

        return await _storage.UploadAsync(stream, file.FileName, tenantFolder);
    }

    public async Task<IEnumerable<StorageFile>> ListTenantFilesAsync(string category)
    {
        var tenantFolder = $"tenant_{_tenantContext.TenantId}/{category}";
        return await _storage.ListFilesAsync(tenantFolder);
    }
}
```

### 🌱 Database Seed Data

```csharp
// Program.cs - Seed database on startup
public class Program
{
    public static async Task Main(string[] args)
    {
        var app = CreateApp(args);

        // Seed database with initial data
        await app.SeedDatabaseAsync<ApplicationDbContext>();

        await app.RunAsync();
    }

    private static WebApplication CreateApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add Marventa services
        builder.Services.AddMarventaV13(builder.Configuration, "MyEcommerce", "1.0.0");

        // Add database seeding
        builder.Services.AddDatabaseSeeding();

        var app = builder.Build();
        app.UseMarventaMiddleware();

        return app;
    }
}

// Custom seeder for specific entities
public class CustomDatabaseSeeder : IDatabaseSeeder
{
    private readonly ILogger<CustomDatabaseSeeder> _logger;

    public async Task SeedAsync<TContext>(TContext context) where TContext : DbContext
    {
        _logger.LogInformation("Seeding custom data");

        // Seed categories
        var categorySet = context.Set<Category>();
        if (!await categorySet.AnyAsync())
        {
            var categories = new[]
            {
                new Category { Name = "Electronics", Description = "Electronic devices" },
                new Category { Name = "Clothing", Description = "Fashion items" }
            };

            categorySet.AddRange(categories);
            await context.SaveChangesAsync();
        }

        // Seed products
        var productSet = context.Set<Product>();
        if (!await productSet.AnyAsync())
        {
            var electronics = await categorySet.FirstAsync(c => c.Name == "Electronics");

            var products = new[]
            {
                new Product
                {
                    Name = "Smartphone",
                    Description = "Latest smartphone",
                    Price = new Money(699.99m, Currency.USD),
                    CategoryId = electronics.Id,
                    Stock = 50
                }
            };

            productSet.AddRange(products);
            await context.SaveChangesAsync();
        }
    }
}

// Multi-tenant seeding with tenant-specific data
public class TenantAwareSeeder
{
    private readonly ITenantContext _tenantContext;

    public async Task SeedTenantDataAsync(string tenantId)
    {
        // Set tenant context
        _tenantContext.SetTenant(tenantId);

        // Tenant-specific seeding
        var tenantProducts = GetTenantSpecificProducts(tenantId);
        // ... seed logic
    }

    private Product[] GetTenantSpecificProducts(string tenantId)
    {
        return tenantId switch
        {
            "demo" => new[] { new Product { Name = "Demo Product" } },
            "enterprise" => new[] { new Product { Name = "Enterprise Product" } },
            _ => Array.Empty<Product>()
        };
    }
}

// Configuration for seed data
{
  "SeedData": {
    "Enabled": true,
    "DefaultTenants": [
      {
        "Id": "demo",
        "Name": "Demo Company"
      }
    ]
  }
}
```

## Architecture

The framework follows Clean Architecture principles:

- **Core** - Domain entities, interfaces, and shared abstractions
- **Domain** - Business logic, aggregates, and domain events
- **Application** - CQRS handlers, validators, and application services
- **Infrastructure** - External service implementations (database, messaging, caching)
- **Web** - Controllers, middleware, and API concerns

## Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/my-feature`
3. Commit your changes: `git commit -am 'Add my feature'`
4. Push to the branch: `git push origin feature/my-feature`
5. Submit a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

- Documentation: [GitHub Wiki](https://github.com/AdemKinatas/Marventa.Framework/wiki)
- Issues: [GitHub Issues](https://github.com/AdemKinatas/Marventa.Framework/issues)
- Discussions: [GitHub Discussions](https://github.com/AdemKinatas/Marventa.Framework/discussions)

---

Built with for .NET developers by [Adem Kınataş](https://github.com/AdemKinatas)