# Marventa.Framework v4.1.0

**Enterprise .NET 8.0 & 9.0 framework - Convention over Configuration**

[![NuGet](https://img.shields.io/nuget/v/Marventa.Framework.svg)](https://www.nuget.org/packages/Marventa.Framework/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

---

## 📚 Table of Contents

- [Installation](#-installation)
- [Quick Start](#-quick-start)
- [Features](#-features)
- [Usage Examples](#-usage-examples)
  - [Authentication & Authorization](#authentication--authorization)
  - [CQRS & MediatR](#cqrs--mediatr)
  - [Database & Repository](#database--repository)
  - [Caching](#caching)
  - [Event Bus](#event-bus-rabbitmq--kafka)
  - [Multi-Tenancy](#multi-tenancy)
  - [Rate Limiting](#rate-limiting)
  - [Health Checks](#health-checks)
  - [Logging](#logging)
  - [Storage](#storage-azure--aws)
  - [Search](#search-elasticsearch)
- [Configuration](#-configuration-reference)
- [Migration Guide](#-migration-guide)

---

## 📦 Installation

```bash
dotnet add package Marventa.Framework
```

---

## 🚀 Quick Start

### 1. Basic Setup (Auto-Configuration)

```csharp
var builder = WebApplication.CreateBuilder(args);

// ✨ Auto-detects features from appsettings.json
builder.Services.AddMarventa(builder.Configuration);
builder.Services.AddControllers();

var app = builder.Build();

app.UseMarventa();  // ✨ Automatically configures middleware pipeline
app.MapControllers();
app.Run();
```

**What `UseMarventa()` does automatically:**
```
1. Exception Handling
2. HTTPS Redirection
3. Routing
4. Authentication & Authorization (if JWT configured)
5. Multi-Tenancy (if configured)
6. Rate Limiting (if configured)
```

### 2. Full Setup (with CQRS & Validation)

```csharp
var builder = WebApplication.CreateBuilder(args);

// Infrastructure (auto-configured)
builder.Services.AddMarventa(builder.Configuration);

// CQRS + Validation (manual - requires your assembly)
builder.Services.AddMarventaMediatR(typeof(Program).Assembly);
builder.Services.AddMarventaValidation(typeof(Program).Assembly);

// Database (manual - requires your DbContext)
builder.Services.AddMarventaDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

var app = builder.Build();

app.UseMarventa();
app.MapControllers();
app.Run();
```

### 3. Minimal Configuration

```json
{
  "Jwt": {
    "Secret": "your-super-secret-key-at-least-32-characters-long",
    "Issuer": "MyApp",
    "Audience": "MyApp",
    "ExpirationMinutes": 60
  }
}
```

### 4. Done! 🎉

- ✅ JWT Authentication & Authorization
- ✅ Cache (InMemory)
- ✅ Exception Handling
- ✅ Validation
- ✅ API Responses

---

## 🎯 Features

### ✅ Auto-Configured (Convention over Configuration)

**Security:**
- **JWT Authentication** - Token-based auth with permissions
- **Rate Limiting** - IP/User/ApiKey based throttling
- **Password Hashing** - BCrypt implementation
- **AES Encryption** - Data encryption utilities

**Data & Storage:**
- **Caching** - InMemory, Redis, Hybrid strategies
- **MongoDB** - NoSQL database support
- **Elasticsearch** - Full-text search
- **Local Storage** - File system storage
- **Azure Blob Storage** - Cloud file storage
- **AWS S3** - Cloud file storage

**Event-Driven:**
- **RabbitMQ** - Message bus for events
- **Kafka** - High-throughput event streaming
- **MassTransit** - Advanced messaging with RabbitMQ

**Infrastructure:**
- **Multi-Tenancy** - Header/Subdomain/Claim strategies
- **Exception Handling** - Global error handling
- **Serilog** - Structured logging
- **Health Checks** - Database, Redis, RabbitMQ monitoring
- **API Responses** - Standardized format

### ⚙️ Manual Configuration (Requires User Setup)

**Application Patterns:**
- **CQRS** - Command/Query with MediatR
- **DDD** - Entity, AggregateRoot, ValueObject, DomainEvent
- **Repository & UnitOfWork** - Generic & custom repositories
- **FluentValidation** - Request validation
- **Result Pattern** - Type-safe error handling

**Database:**
- **EF Core** - SQL Server, PostgreSQL support
- **Generic Repository** - CRUD operations
- **Base DbContext** - Multi-tenancy aware

**Observability (Advanced):**
- **OpenTelemetry** - Distributed tracing (manual setup)
- **Resilience** - Polly retry/circuit breaker utilities

---

## 💡 Usage Examples

### Authentication & Authorization

**Setup (auto-configured from appsettings.json):**
```json
{
  "Jwt": {
    "Secret": "your-super-secret-key-at-least-32-characters-long",
    "Issuer": "MyApp",
    "Audience": "MyApp",
    "ExpirationMinutes": 60
  }
}
```

**Login:**
```csharp
[HttpPost("login")]
public async Task<IActionResult> Login(LoginRequest request)
{
    var user = await _userRepository.GetByEmailAsync(request.Email);
    if (user == null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
        return Unauthorized();

    var token = _jwtTokenGenerator.GenerateToken(
        userId: user.Id.ToString(),
        email: user.Email,
        roles: user.Roles
    );

    return Ok(new { token });
}
```

**Protected Endpoint:**
```csharp
[Authorize]
[RequirePermission("products.create")]
[HttpPost("products")]
public async Task<IActionResult> Create(CreateProductCommand command)
{
    var result = await _mediator.Send(command);
    return Ok(result);
}
```

---

### CQRS & MediatR

**Setup:**
```csharp
// In Program.cs
builder.Services.AddMarventa(builder.Configuration);
builder.Services.AddMarventaMediatR(typeof(Program).Assembly);
builder.Services.AddMarventaValidation(typeof(Program).Assembly);
```

**Command:**
```csharp
public record CreateProductCommand : ICommand<Guid>
{
    public string Name { get; init; }
    public decimal Price { get; init; }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IRepository<Product, Guid> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = new Product { Name = request.Name, Price = request.Price };
        await _repository.AddAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success(product.Id);
    }
}
```

**Validation:**
```csharp
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
```

**Controller:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
```

---

### Database & Repository

**Setup:**
```csharp
// In Program.cs
builder.Services.AddMarventaDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Generic Repository
builder.Services.AddMarventaGenericRepository<Product, Guid>();

// Or Custom Repository
builder.Services.AddMarventaRepository<Product, Guid, ProductRepository>();
```

**DbContext:**
```csharp
public class AppDbContext : BaseDbContext
{
    public DbSet<Product> Products { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
```

**Entity (DDD):**
```csharp
public class Product : AuditableEntity<Guid>
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public Guid? TenantId { get; set; }  // For multi-tenancy

    // Domain Events
    public void UpdatePrice(decimal newPrice)
    {
        Price = newPrice;
        AddDomainEvent(new ProductPriceChangedEvent(Id, newPrice));
    }
}
```

---

### Caching

**InMemory (default):**
```csharp
public class ProductService
{
    private readonly ICacheService _cache;

    public async Task<Product> GetAsync(string id)
    {
        var cached = await _cache.GetAsync<Product>($"product:{id}");
        if (cached != null) return cached;

        var product = await _repository.GetByIdAsync(id);
        await _cache.SetAsync($"product:{id}", product,
            CacheOptions.WithAbsoluteExpiration(TimeSpan.FromMinutes(30)));

        return product;
    }
}
```

**Redis:**
```json
{
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "Caching": {
    "Type": "Redis"
  }
}
```

---

### Event Bus (RabbitMQ & Kafka)

**RabbitMQ:**
```json
{
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  }
}
```

```csharp
// Publish
await _eventBus.PublishAsync(new OrderCreatedEvent
{
    OrderId = order.Id,
    Total = order.Total
});

// Subscribe
public class OrderCreatedEventHandler : IIntegrationEventHandler<OrderCreatedEvent>
{
    public async Task HandleAsync(OrderCreatedEvent @event, CancellationToken ct)
    {
        // Send confirmation email, update inventory, etc.
    }
}
```

**Kafka (high-throughput):**
```json
{
  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "GroupId": "myapp-group"
  }
}
```

```csharp
// Producer
await _kafkaProducer.ProduceAsync("orders", new OrderCreatedEvent
{
    OrderId = order.Id,
    Total = order.Total
});

// Consumer
await _kafkaConsumer.SubscribeAsync("orders", async (message) =>
{
    var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(message);
    // Process event
});
```

**MassTransit (advanced messaging):**
```json
{
  "MassTransit": {
    "Enabled": "true"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "VirtualHost": "/",
    "Username": "guest",
    "Password": "guest"
  }
}
```

```csharp
// Consumer
public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var order = context.Message;
        // Process order
        await context.Publish(new OrderProcessedEvent { OrderId = order.OrderId });
    }
}

// Register consumer in Program.cs
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedConsumer>();
    // UseMarventa() will configure RabbitMQ automatically
});
```

---

### Multi-Tenancy

**Config:**
```json
{
  "MultiTenancy": {
    "Strategy": "Header",
    "HeaderName": "X-Tenant-Id",
    "RequireTenant": true
  }
}
```

**Usage:**
```csharp
public class ProductService
{
    private readonly ITenantContext _tenantContext;

    public async Task<List<Product>> GetAllAsync()
    {
        var tenantId = _tenantContext.TenantId;
        return await _repository.GetAllAsync(p => p.TenantId == tenantId);
    }
}
```

### Rate Limiting

**Config:**
```json
{
  "RateLimiting": {
    "RequestLimit": 100,
    "TimeWindowSeconds": 60,
    "Strategy": "IpAddress",
    "ReturnRateLimitHeaders": true
  }
}
```

**Response Headers:**
```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 45
X-RateLimit-Reset: 1234567890
```

---

### Logging

**Setup (auto-configured):**
```csharp
// In Program.cs
builder.Services.AddMarventa(builder.Configuration);
```

**Usage:**
```csharp
public class ProductService
{
    private readonly ILogger<ProductService> _logger;

    public async Task<Product> GetAsync(Guid id)
    {
        _logger.LogInformation("Fetching product {ProductId}", id);

        var product = await _repository.GetByIdAsync(id);

        if (product == null)
            _logger.LogWarning("Product {ProductId} not found", id);

        return product;
    }
}
```

**Configuration (appsettings.json):**
```json
{
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "File", "Args": { "path": "logs/app.log", "rollingInterval": "Day" } }
    ]
  }
}
```

---

### Storage (Local, Azure & AWS)

**Local File System (default):**
```json
{
  "LocalStorage": {
    "BasePath": "D:/uploads",
    "BaseUrl": "https://myapp.com/files"  // Optional - for generating URLs
  }
}
```

```csharp
// Upload
await _storage.UploadAsync(fileStream, "documents/file.pdf", "application/pdf");

// Download
var stream = await _storage.DownloadAsync("documents/file.pdf");

// Delete
await _storage.DeleteAsync("documents/file.pdf");

// Check if exists
var exists = await _storage.ExistsAsync("documents/file.pdf");

// Get URL
var url = await _storage.GetUrlAsync("documents/file.pdf");
// Returns: "https://myapp.com/files/documents/file.pdf" or local path
```

**Azure Blob Storage:**
```json
{
  "Azure": {
    "Storage": {
      "ConnectionString": "your-connection-string",
      "ContainerName": "uploads"
    }
  }
}
```

```csharp
// Upload
await _storage.UploadAsync(fileStream, "documents/file.pdf", "application/pdf");

// Download
var stream = await _storage.DownloadAsync("documents/file.pdf");

// Delete
await _storage.DeleteAsync("documents/file.pdf");

// List
var files = await _storage.ListAsync("documents/");
```

**AWS S3:**
```json
{
  "AWS": {
    "AccessKey": "your-access-key",
    "SecretKey": "your-secret-key",
    "Region": "us-east-1",
    "BucketName": "my-bucket"
  }
}
```

---

### Health Checks

**Setup (auto-configured):**
```json
{
  "HealthChecks": {
    "Enabled": "true"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyApp;..."
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "RabbitMQ": {
    "Host": "localhost"
  }
}
```

**Access:**
```
GET /health

Response:
{
  "status": "Healthy",
  "entries": {
    "database": { "status": "Healthy" },
    "redis": { "status": "Healthy" },
    "rabbitmq": { "status": "Healthy" }
  }
}
```

**Custom Health Check:**
```csharp
public class CustomHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        // Custom logic
        var isHealthy = await CheckSomethingAsync();

        return isHealthy
            ? HealthCheckResult.Healthy("All systems operational")
            : HealthCheckResult.Unhealthy("System is down");
    }
}

// Register
builder.Services.AddHealthChecks()
    .AddCheck<CustomHealthCheck>("custom");
```

---

### Search (Elasticsearch)

**Setup:**
```json
{
  "Elasticsearch": {
    "Uri": "http://localhost:9200"
  }
}
```

**Usage:**
```csharp
// Index a document
await _elasticsearchService.IndexDocumentAsync("products", product, product.Id);

// Search
var results = await _elasticsearchService.SearchAsync<Product>("products", "laptop gaming");

// Advanced search with filters
var searchResults = await _elasticsearchService.SearchAsync<Product>(
    "products",
    searchText: "laptop",
    filters: new Dictionary<string, object>
    {
        { "category", "electronics" },
        { "price", new { gte = 500, lte = 2000 } }
    }
);
```

---

## ⚙️ Configuration

### Auto-Detection Table

Add these sections to `appsettings.json` to enable features:

| Config Section | Feature | Example |
|---------------|---------|---------|
| `Jwt` | Authentication | `"Secret": "your-key"` |
| `Redis` + `Caching` | Redis Cache | `"ConnectionString": "localhost:6379"` |
| `MultiTenancy` | Multi-Tenant | `"Strategy": "Header"` |
| `RateLimiting` | Rate Limiting | `"RequestLimit": 100` |
| `RabbitMQ` | Event Bus | `"Host": "localhost"` |
| `Kafka` | Event Streaming | `"BootstrapServers": "localhost:9092"` |
| `MassTransit` | Advanced Messaging | `"Enabled": "true"` |
| `Elasticsearch` | Search | `"Uri": "http://localhost:9200"` |
| `MongoDB` | NoSQL | `"ConnectionString": "mongodb://..."` |
| `HealthChecks` | Health Monitoring | `"Enabled": "true"` |
| `LocalStorage` | Local Files | `"BasePath": "D:/uploads"` |
| `Azure:Storage` | Azure Blob | `"ConnectionString": "..."` |
| `AWS` | AWS S3 | `"AccessKey": "...", "SecretKey": "..."` |

### Manual Extensions

Features that need your code:

```csharp
// CQRS + Validation
builder.Services.AddMarventaMediatR(typeof(Program).Assembly);
builder.Services.AddMarventaValidation(typeof(Program).Assembly);

// Database
builder.Services.AddMarventaDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repository (choose one)
builder.Services.AddMarventaGenericRepository<Product, Guid>();
builder.Services.AddMarventaRepository<Product, Guid, ProductRepository>();

// OpenTelemetry
builder.Services.AddMarventaOpenTelemetry(builder.Configuration, "MyService");
```

### Custom Middleware

```csharp
var app = builder.Build();

app.UseMarventa();        // Standard pipeline
app.UseCors("MyPolicy");  // Your custom middleware
app.UseStaticFiles();     // Your static files
app.MapControllers();
app.Run();
```

---

## 🔄 Migration Guide

### v3.x → v4.0.2

**Old:**
```csharp
builder.Services.AddMarventaFramework(builder.Configuration);
builder.Services.AddMarventaMediatR(typeof(Program).Assembly);
builder.Services.AddMarventaValidation(typeof(Program).Assembly);
builder.Services.AddMarventaJwtAuthentication(builder.Configuration);
builder.Services.AddMarventaCaching(builder.Configuration, CacheType.InMemory);

app.UseMarventaFramework(app.Environment);
```

**New:**
```csharp
builder.Services.AddMarventa(builder.Configuration);
app.UseMarventa();
```

**That's it!** Everything else is automatic.

---

## 🎨 API Response Format

### Success
```json
{
  "success": true,
  "data": { "id": "123", "name": "Product" },
  "message": null,
  "errorCode": null
}
```

### Error
```json
{
  "success": false,
  "data": null,
  "message": "Product not found",
  "errorCode": "NOT_FOUND"
}
```

### Validation Error
```json
{
  "success": false,
  "message": "Validation failed",
  "errorCode": "VALIDATION_ERROR",
  "errors": {
    "Name": ["Product name is required"],
    "Price": ["Price must be greater than zero"]
  }
}
```

---


## 📝 What's New in v4.0.2

- ✨ **Convention over Configuration** - Zero config by default
- ✨ **Auto-Detection** - Framework scans appsettings.json automatically
- ✨ **One Line Setup** - `AddMarventa()` + `UseMarventa()`
- ✨ **Simplified Documentation** - Single README with everything
- ✨ **Advanced Rate Limiting** - Multiple strategies
- ✨ **Enhanced Multi-Tenancy** - Multiple resolution strategies

---

## 🤝 Contributing

Contributions welcome! Please submit a Pull Request.

---

## 📄 License

MIT License - see [LICENSE](LICENSE)

---

## 📧 Support

[GitHub Issues](https://github.com/AdemKinatas/Marventa.Framework/issues)

---

## 🌟 Show Your Support

⭐️ Star this repo if it helped you!

---

**Built with ❤️ using .NET 8.0 & 9.0**
