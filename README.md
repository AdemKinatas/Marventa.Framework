# 🚀 Marventa.Framework

A comprehensive .NET 9.0 enterprise e-commerce framework with multi-tenancy, JWT authentication, CQRS, messaging infrastructure, and complete e-commerce domain modules.

[![NuGet](https://img.shields.io/nuget/v/Marventa.Framework.svg)](https://www.nuget.org/packages/Marventa.Framework)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## 📦 Installation

```bash
dotnet add package Marventa.Framework
```

## ⚡ Quick Start

```csharp
// Program.cs
using Marventa.Framework;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMarventa(builder.Configuration);

var app = builder.Build();
app.UseMarventa();
app.Run();
```

```json
// appsettings.json
{
  "Marventa": {
    "MultiTenancy": { "Enabled": true },
    "Jwt": { "SecretKey": "your-secret-key" },
    "Redis": { "ConnectionString": "localhost:6379" }
  }
}
```

## 🏢 Framework Capabilities

### 🔐 **Authentication & Security**
```csharp
// JWT Authentication
[Authorize]
public class ProductsController : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "TenantAdmin")]
    public async Task<IActionResult> GetProducts() => Ok();
}
```

### 🏢 **Multi-Tenancy**
```csharp
// Automatic tenant resolution from headers/host
public class ProductService
{
    private readonly ITenantContext _tenantContext;

    public async Task<Product> GetProductAsync(Guid id)
    {
        // Automatically filtered by current tenant
        return await _repository.GetByIdAsync(id);
    }
}
```

### 💾 **Caching**
```csharp
// Tenant-scoped caching
public class ProductService
{
    private readonly ICacheService _cache;

    public async Task<Product> GetProductAsync(Guid id)
    {
        return await _cache.GetOrSetAsync($"product:{id}",
            () => _repository.GetByIdAsync(id),
            TimeSpan.FromMinutes(30));
    }
}
```

### 📝 **Logging**
```csharp
// Structured logging with tenant context
_logger.LogInformation("Product {ProductId} accessed by user {UserId}",
    productId, userId);

// Automatic tenant ID and correlation ID in logs
```

### 📨 **Messaging & Events**
```csharp
// Domain events
public class Order : BaseAggregateRoot
{
    public void CompleteOrder()
    {
        Status = OrderStatus.Completed;
        AddDomainEvent(new OrderCompletedEvent(Id, CustomerId));
    }
}

// Event handlers
public class OrderCompletedHandler : INotificationHandler<OrderCompletedEvent>
{
    public async Task Handle(OrderCompletedEvent notification, CancellationToken cancellationToken)
    {
        // Send email, update inventory, etc.
    }
}
```

### 📋 **CQRS Pattern**
```csharp
// Commands
public record CreateProductCommand(string Name, decimal Price) : IRequest<Guid>;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Product.Create(request.Name, request.Price);
        await _repository.AddAsync(product);
        return product.Id;
    }
}

// Usage
var productId = await _mediator.Send(new CreateProductCommand("iPhone", 999.99m));
```

### ✅ **Validation**
```csharp
// FluentValidation with automatic pipeline
public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}

// Automatic validation in MediatR pipeline
// Returns RFC 7807 Problem Details on validation failure
```

### 🚦 **Rate Limiting**
```csharp
// Tenant-aware rate limiting
[HttpGet]
[RateLimit(MaxRequests = 100, WindowSizeInSeconds = 60)]
public async Task<IActionResult> GetProducts()
{
    // Limited to 100 requests per minute per tenant
}
```

### 💰 **Money & Currency**
```csharp
// Value objects for financial calculations
var price = new Money(99.99m, "USD");
var taxAmount = price.ApplyTax(0.08m);
var discountedPrice = price.ApplyDiscount(10); // 10% discount

// Multi-currency support
var eurPrice = price.ConvertTo("EUR", 0.85m);
```

### 🔄 **Saga Patterns**
```csharp
// Long-running business processes
public class OrderSaga : BaseSaga
{
    public async Task Handle(OrderCreatedEvent @event)
    {
        // Reserve inventory
        await ExecuteStepAsync("ReserveInventory", () => _inventoryService.ReserveAsync(@event.OrderId));

        // Process payment
        await ExecuteStepAsync("ProcessPayment", () => _paymentService.ProcessAsync(@event.OrderId));

        // Ship order
        await ExecuteStepAsync("ShipOrder", () => _shippingService.ShipAsync(@event.OrderId));
    }
}
```

### 🔍 **Search & Analytics**
```csharp
// Full-text search
var products = await _searchService.SearchAsync<Product>(new SearchRequest
{
    Query = "smartphone",
    Filters = { ["category"] = "electronics" },
    Page = 1,
    PageSize = 20
});

// Analytics tracking
await _analyticsService.TrackEventAsync(new AnalyticsEvent
{
    Name = "product_viewed",
    Properties = { ["product_id"] = productId.ToString() }
});
```

### ☁️ **Cloud Storage**
```csharp
// Unified storage abstraction
var file = await _storageService.UploadAsync(fileStream, "product-image.jpg", "products");
var downloadUrl = await _storageService.GetPresignedUrlAsync(file.Key, TimeSpan.FromHours(1));
```

### 🏥 **Health Checks & Monitoring**
```csharp
// Built-in health checks
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");

// Custom health checks automatically registered
```

### ⚡ **Circuit Breaker & Resilience**
```csharp
// Automatic retry and circuit breaker for HTTP calls
public class ExternalApiService
{
    [CircuitBreaker]
    [Retry(maxRetries: 3)]
    public async Task<ApiResponse> CallExternalApiAsync()
    {
        // Resilient HTTP calls with Polly
    }
}
```

### 📊 **Background Jobs**
```csharp
// Background processing
public class EmailService
{
    public async Task SendWelcomeEmailAsync(string userId)
    {
        // Queue background job
        _backgroundJobService.Enqueue(() => ProcessWelcomeEmailAsync(userId));
    }
}
```

## 🌟 Key Features

| Feature | Description | Status |
|---------|-------------|---------|
| **🏢 Multi-Tenancy** | Complete tenant isolation and context | ✅ |
| **🔐 Authentication** | JWT with refresh tokens | ✅ |
| **💾 Caching** | Redis + Memory with tenant scoping | ✅ |
| **📝 Logging** | Structured logging with Serilog | ✅ |
| **🚦 Rate Limiting** | Tenant-aware API throttling | ✅ |
| **📨 Messaging** | RabbitMQ + Kafka integration | ✅ |
| **📋 CQRS** | MediatR with validation pipeline | ✅ |
| **💰 Payments** | Complete payment domain | ✅ |
| **📦 Shipping** | End-to-end shipping lifecycle | ✅ |
| **🔍 Search** | Elasticsearch abstraction | ✅ |
| **📊 Analytics** | ClickHouse integration | ✅ |
| **☁️ Storage** | S3/Azure/GCS abstraction | ✅ |
| **🔄 Sagas** | Long-running process orchestration | ✅ |
| **✅ Validation** | FluentValidation + RFC 7807 | ✅ |
| **🔍 Observability** | OpenTelemetry tracing | ✅ |
| **⚡ Resilience** | Circuit breaker + retry policies | ✅ |
| **🏥 Health Checks** | Service monitoring | ✅ |
| **🎛️ Feature Flags** | Dynamic feature toggles | ✅ |

## 📚 Documentation

- [📖 Full Documentation](https://docs.marventa.com)
- [🏁 Getting Started Guide](https://docs.marventa.com/getting-started)
- [🏗️ Architecture Guide](https://docs.marventa.com/architecture)
- [🔧 Configuration Reference](https://docs.marventa.com/configuration)
- [📝 API Reference](https://docs.marventa.com/api)

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

Built with ❤️ using .NET 9.0, Entity Framework Core, MediatR, MassTransit, Redis, and many other amazing open-source libraries.

---

**Ready to build enterprise e-commerce applications?** 🚀

```bash
dotnet new webapi -n MyECommerceApp
cd MyECommerceApp
dotnet add package Marventa.Framework
# Start building amazing e-commerce solutions!
```