# Marventa Framework

Modern .NET 9.0 enterprise framework with clean architecture, comprehensive features, and production-ready components.

## Installation

```bash
dotnet add package Marventa.Framework --version 2.0.1
```

## Features

### 1. Multi-Tenancy Support
Isolate data and configuration per tenant with flexible resolution strategies.

```csharp
// Startup.cs
services.AddMarventaMultitenancy(configuration);
// or with custom configuration
services.AddMarventaMultitenancy(options =>
{
    options.ResolutionStrategy = TenantResolutionStrategy.Header;
    options.DefaultTenantId = "default";
});

// Usage in services
public class ProductService
{
    private readonly ITenantContext _tenantContext;

    public async Task<Product> GetProductAsync(int id)
    {
        var tenantId = _tenantContext.CurrentTenant?.Id;
        // Query with tenant isolation
    }
}
```

### 2. Transactional Messaging (Outbox/Inbox Pattern)
Ensure message delivery reliability with transactional outbox pattern.

```csharp
// Configure
services.AddMarventaTransactionalMessaging();

// Usage
public class OrderService
{
    private readonly IOutboxService _outboxService;

    public async Task CreateOrderAsync(Order order)
    {
        using var transaction = await _context.BeginTransactionAsync();

        // Save order
        await _context.Orders.AddAsync(order);

        // Queue message transactionally
        await _outboxService.AddMessageAsync(new OrderCreatedEvent
        {
            OrderId = order.Id
        });

        await transaction.CommitAsync();
    }
}
```

### 3. HTTP Idempotency
Prevent duplicate processing of HTTP requests.

```csharp
// Configure
services.AddMarventaIdempotency(options =>
{
    options.CacheDuration = TimeSpan.FromMinutes(30);
    options.HeaderName = "X-Idempotency-Key";
});

// Usage in controller
[HttpPost("orders")]
[Idempotent]
public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
{
    // This will only execute once per idempotency key
    var order = await _orderService.CreateOrderAsync(request);
    return Ok(order);
}
```

### 4. Observability with OpenTelemetry
Comprehensive tracing, metrics, and logging with correlation context.

```csharp
// Configure
services.AddMarventaObservability(configuration, "MyService", "1.0.0");

// Automatic correlation ID propagation
[HttpGet("products")]
public async Task<IActionResult> GetProducts()
{
    // Correlation ID is automatically attached to all logs and traces
    _logger.LogInformation("Fetching products");
    return Ok(await _productService.GetAllAsync());
}
```

### 5. Entity Framework Multi-Tenant Support
Automatic tenant isolation at the database level.

```csharp
// Configure
services.AddMarventaEntityFramework<ApplicationDbContext>(configuration);

// DbContext automatically filters by tenant
public class ApplicationDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Automatic tenant filters applied
        modelBuilder.Entity<Product>()
            .HasQueryFilter(p => p.TenantId == _tenantContext.CurrentTenant.Id);
    }
}
```

### 6. Saga/Process Manager
Manage complex distributed transactions with saga pattern.

```csharp
// Configure
services.AddMarventaSagas();

// Define saga
public class OrderSaga : ISaga
{
    public async Task<SagaStepResult> ProcessPayment(OrderContext context)
    {
        var result = await _paymentService.ProcessAsync(context.PaymentInfo);
        return result.Success
            ? SagaStepResult.Success()
            : SagaStepResult.Failure("Payment failed");
    }

    public async Task<SagaStepResult> CompensatePayment(OrderContext context)
    {
        await _paymentService.RefundAsync(context.PaymentInfo);
        return SagaStepResult.Success();
    }
}
```

### 7. Read Model Projections
CQRS with MongoDB projections for optimized read models.

```csharp
// Configure
services.AddMarventaProjections(options =>
{
    options.ConnectionString = "mongodb://localhost:27017";
    options.DatabaseName = "projections";
});

// Usage
public class ProductProjection : IProjection
{
    public string Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int ViewCount { get; set; }
}

// Query projection
var popularProducts = await _projectionRepository
    .QueryAsync<ProductProjection>(p => p.ViewCount > 100);
```

### 8. FluentValidation Integration
Automatic request validation with detailed error responses.

```csharp
// Configure
services.AddMarventaValidation(typeof(Program).Assembly);

// Define validator
public class CreateProductValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .LessThanOrEqualTo(10000);
    }
}

// Automatic validation in MediatR pipeline
public class CreateProductHandler : IRequestHandler<CreateProductRequest, Product>
{
    // Validation happens automatically before this executes
}
```

### 9. Elasticsearch Integration
Full-text search capabilities with simplified HTTP client.

```csharp
// Configure
services.AddElasticsearch(options =>
{
    options.ConnectionString = "http://localhost:9200";
    options.IndexPrefix = "myapp-";
    options.TimeoutSeconds = 60;
});

// Usage
public class SearchController : BaseController
{
    private readonly ISearchService _searchService;

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        var result = await _searchService.SearchAsync<Product>(new SearchRequest
        {
            Query = query,
            Page = 1,
            PageSize = 20,
            Filters = new Dictionary<string, object>
            {
                ["category"] = "electronics",
                ["minPrice"] = 100
            }
        });

        return Ok(result);
    }
}
```

### 10. Distributed Caching
Multi-level caching with tenant isolation.

```csharp
// Usage
public class ProductService
{
    private readonly ITenantScopedCache _cache;

    public async Task<Product> GetProductAsync(int id)
    {
        return await _cache.GetOrSetAsync(
            $"product:{id}",
            async () => await _repository.GetByIdAsync(id),
            TimeSpan.FromMinutes(10)
        );
    }
}
```

### 11. Distributed Locking
Prevent concurrent access to critical resources.

```csharp
public class InventoryService
{
    private readonly IDistributedLock _distributedLock;

    public async Task UpdateInventoryAsync(int productId, int quantity)
    {
        await using var lockHandle = await _distributedLock.AcquireAsync(
            $"inventory:{productId}",
            TimeSpan.FromSeconds(30)
        );

        if (!lockHandle.IsAcquired)
            throw new InvalidOperationException("Could not acquire lock");

        // Critical section - only one instance can execute this
        var inventory = await _repository.GetInventoryAsync(productId);
        inventory.Quantity -= quantity;
        await _repository.SaveAsync(inventory);
    }
}
```

### 12. Rate Limiting
Protect APIs from abuse with flexible rate limiting.

```csharp
// Configure globally
services.AddRateLimiting(options =>
{
    options.GlobalLimit = 1000;
    options.Window = TimeSpan.FromMinutes(1);
});

// Or per endpoint
[HttpGet("expensive-operation")]
[RateLimit(MaxRequests = 10, Window = "1m")]
public async Task<IActionResult> ExpensiveOperation()
{
    // This endpoint allows only 10 requests per minute
}
```

### 13. API Versioning
Support multiple API versions simultaneously.

```csharp
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/products")]
public class ProductsController : VersionedControllerBase
{
    [HttpGet]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetV1()
    {
        // Version 1.0 response format
    }

    [HttpGet]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> GetV2()
    {
        // Version 2.0 response format with additional fields
    }
}
```

### 14. Health Checks
Monitor application and dependency health.

```csharp
// Configure
services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>()
    .AddElasticsearch()
    .AddRedis()
    .AddMongoDb();

// Access at /health
{
    "status": "Healthy",
    "checks": {
        "database": "Healthy",
        "elasticsearch": "Healthy",
        "redis": "Healthy",
        "mongodb": "Healthy"
    }
}
```

### 15. Security Features
JWT authentication, API key validation, and encryption services.

```csharp
// JWT Authentication
services.AddJwtAuthentication(configuration);

// API Key middleware
app.UseApiKeyAuthentication();

// Encryption service
public class SecureService
{
    private readonly IEncryptionService _encryption;

    public async Task StoreSecretAsync(string secret)
    {
        var encrypted = await _encryption.EncryptAsync(secret);
        await _repository.SaveAsync(encrypted);
    }
}
```

### 16. Circuit Breaker Pattern
Prevent cascading failures with circuit breaker.

```csharp
public class ExternalApiService
{
    private readonly IHttpClientService _httpClient;

    public async Task<ApiResponse> CallExternalApiAsync()
    {
        // Automatic circuit breaker protection
        return await _httpClient.GetAsync<ApiResponse>(
            "https://api.external.com/data",
            new CircuitBreakerOptions
            {
                FailureThreshold = 5,
                ResetTimeout = TimeSpan.FromSeconds(30)
            }
        );
    }
}
```

### 17. Event Store
Event sourcing support for audit trails and event replay.

```csharp
public class OrderAggregate
{
    private readonly IEventStore _eventStore;

    public async Task CreateOrderAsync(Order order)
    {
        var events = new List<IDomainEvent>
        {
            new OrderCreatedEvent(order.Id, order.CustomerId),
            new OrderItemsAddedEvent(order.Id, order.Items)
        };

        await _eventStore.AppendEventsAsync(order.Id, events);
    }

    public async Task<Order> RehydrateOrderAsync(Guid orderId)
    {
        var events = await _eventStore.GetEventsAsync(orderId);
        return Order.FromEvents(events);
    }
}
```

### 18. Complete v1.3 Setup
Configure all features with a single call.

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add all Marventa v1.3 features
        builder.Services.AddMarventaV13(
            builder.Configuration,
            "MyEcommerceService",
            "1.3.0"
        );

        // Add validation for all assemblies
        builder.Services.AddMarventaValidation(
            typeof(Program).Assembly,
            typeof(ApplicationLayer).Assembly
        );

        var app = builder.Build();

        // Use all middleware in correct order
        app.UseMarventaMiddleware();

        app.MapControllers();
        app.Run();
    }
}
```

## Architecture

```
Marventa.Framework/
├── Core/                 # Interfaces and abstractions
├── Domain/              # Domain entities and value objects
├── Application/         # Business logic and use cases
├── Infrastructure/      # External service implementations
└── Web/                # API controllers and middleware
```

## Configuration

```json
{
  "Elasticsearch": {
    "ConnectionString": "http://localhost:9200",
    "IndexPrefix": "myapp-",
    "TimeoutSeconds": 60
  },
  "Tenancy": {
    "ResolutionStrategy": "Header",
    "DefaultTenantId": "default"
  },
  "Idempotency": {
    "CacheDuration": "00:30:00",
    "HeaderName": "X-Idempotency-Key"
  },
  "Correlation": {
    "HeaderName": "X-Correlation-ID",
    "IncludeInResponse": true
  },
  "Projections": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "projections"
  }
}
```

## Requirements

- .NET 9.0 or later
- SQL Server or PostgreSQL for primary database
- MongoDB for projections (optional)
- Redis for distributed caching/locking (optional)
- Elasticsearch for search functionality (optional)

## License

MIT License - See LICENSE file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Support

For issues and feature requests, please use the GitHub issues page.