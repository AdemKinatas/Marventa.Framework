# Marventa.Framework v4.0.1

**Enterprise-grade .NET 8.0 & 9.0 framework for building scalable microservices with DDD, CQRS, and Event-Driven Architecture.**

[![NuGet](https://img.shields.io/nuget/v/Marventa.Framework.svg)](https://www.nuget.org/packages/Marventa.Framework/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## 🎯 Version 4.0.1 - Latest Release ⭐ RECOMMENDED

**Complete architectural redesign with single-package approach!**

> 💡 **We strongly recommend using v4.0.1** for all new projects. This version provides a unified, production-ready architecture with complete feature set, simplified dependency management, and superior performance compared to previous versions.

### Breaking Changes
- ⚠️ Complete restructure from multi-project to single-project architecture
- ⚠️ All features now in one unified package: `Marventa.Framework`
- ⚠️ Namespace changes for better organization
- ⚠️ Requires .NET 8.0 or .NET 9.0
- ⚠️ Migration required from v3.x and earlier versions

### What's New in v4.0.1
- ✨ Unified single-package architecture - One package, all features
- ✨ **Multi-targeting support**: .NET 8.0 (LTS) and .NET 9.0
- ✨ Simplified dependency management - No more version conflicts
- ✨ Enhanced performance and efficiency
- ✨ Complete CQRS, Event-Driven, and DDD implementation
- ✨ Enterprise-ready patterns out of the box
- ✨ Full Kafka support for high-throughput event streaming
- ✨ OpenTelemetry integration for distributed tracing
- ✨ MongoDB support for NoSQL scenarios
- ✨ Cloud storage providers (Azure Blob, AWS S3)
- ✨ Built-in rate limiting middleware
- ✨ MassTransit integration for advanced messaging patterns
- ✨ Comprehensive logging with Serilog + Elasticsearch

---

## 🚀 Features

### **Core Patterns**
- ✅ **Domain-Driven Design (DDD)** - Entity, AggregateRoot, ValueObject, DomainEvent
- ✅ **CQRS Pattern** - Command/Query separation with MediatR
- ✅ **Repository & Unit of Work** - Generic repository with EF Core support
- ✅ **Result Pattern** - Type-safe error handling

### **Event-Driven Architecture**
- ✅ **Event Bus** - RabbitMQ and MassTransit support
- ✅ **Domain Events** - In-process event handling
- ✅ **Integration Events** - Cross-service communication
- ✅ **Kafka Support** - High-throughput event streaming

### **Caching**
- ✅ **In-Memory Cache** - Fast local caching
- ✅ **Distributed Cache (Redis)** - Scalable caching
- ✅ **Hybrid Cache** - Multi-level caching strategy

### **Security**
- ✅ **JWT Authentication** - Token-based authentication
- ✅ **Permission-based Authorization** - Fine-grained access control
- ✅ **Password Hashing** - BCrypt integration
- ✅ **AES Encryption** - Data encryption utilities
- ✅ **Rate Limiting** - API throttling

### **Infrastructure**
- ✅ **Entity Framework Core** - SQL Server, PostgreSQL support
- ✅ **MongoDB** - NoSQL database support
- ✅ **Elasticsearch** - Full-text search
- ✅ **Azure Blob Storage** - Cloud file storage
- ✅ **AWS S3** - Cloud file storage

### **Resilience**
- ✅ **Retry Policy** - Automatic retry with exponential backoff
- ✅ **Circuit Breaker** - Fault tolerance
- ✅ **Timeout Policy** - Request timeouts

### **Observability**
- ✅ **Structured Logging** - Serilog with Elasticsearch
- ✅ **Health Checks** - Database, Redis, RabbitMQ
- ✅ **OpenTelemetry** - Distributed tracing

### **API Features**
- ✅ **Standardized API Responses** - Consistent response format
- ✅ **Global Exception Handling** - Centralized error handling
- ✅ **FluentValidation** - Request validation
- ✅ **Swagger/OpenAPI** - API documentation
- ✅ **API Versioning** - Version management

### **Multi-Tenancy**
- ✅ **Tenant Context** - Tenant isolation
- ✅ **Tenant Resolver** - Header/Claim-based resolution
- ✅ **Tenant Middleware** - Automatic tenant detection

---

## 📦 Installation

```bash
dotnet add package Marventa.Framework
```

---

## 🔧 Quick Start

### **1. Basic Setup**

```csharp
using Marventa.Framework.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add Marventa Framework services
builder.Services.AddMarventaFramework(builder.Configuration);
builder.Services.AddMarventaMediatR(typeof(Program).Assembly);
builder.Services.AddMarventaValidation(typeof(Program).Assembly);
builder.Services.AddMarventaLogging(builder.Configuration, "MyApp");

var app = builder.Build();

// Use Marventa middleware
app.UseMarventaFramework(app.Environment);

app.MapControllers();
app.Run();
```

### **2. Authentication & Authorization**

```csharp
// Add JWT authentication
builder.Services.AddMarventaJwtAuthentication(builder.Configuration);

// appsettings.json
{
  "Jwt": {
    "Secret": "your-super-secret-key-min-32-characters",
    "Issuer": "YourApp",
    "Audience": "YourApp",
    "ExpirationMinutes": 60
  }
}
```

### **3. Caching**

```csharp
// In-Memory Cache
builder.Services.AddMarventaCaching(builder.Configuration, CacheType.InMemory);

// Redis Cache
builder.Services.AddMarventaCaching(builder.Configuration, CacheType.Redis);

// Hybrid Cache (Memory + Redis)
builder.Services.AddMarventaCaching(builder.Configuration, CacheType.Hybrid);

// Usage
public class ProductService
{
    private readonly ICacheService _cache;

    public async Task<Product> GetProductAsync(string id)
    {
        var product = await _cache.GetAsync<Product>($"product:{id}");
        if (product == null)
        {
            product = await _repository.GetByIdAsync(id);
            await _cache.SetAsync($"product:{id}", product,
                CacheOptions.WithAbsoluteExpiration(TimeSpan.FromMinutes(30)));
        }
        return product;
    }
}
```

### **4. Event Bus (RabbitMQ)**

```csharp
// Configure RabbitMQ
builder.Services.AddMarventaRabbitMq(builder.Configuration);

// Define an integration event
public class OrderCreatedEvent : IntegrationEvent
{
    public Guid OrderId { get; set; }
    public decimal TotalAmount { get; set; }
}

// Publish event
public class OrderService
{
    private readonly IEventBus _eventBus;

    public async Task CreateOrderAsync(Order order)
    {
        // ... save order
        await _eventBus.PublishAsync(new OrderCreatedEvent
        {
            OrderId = order.Id,
            TotalAmount = order.Total
        });
    }
}

// Subscribe to event
public class OrderCreatedEventHandler : IIntegrationEventHandler<OrderCreatedEvent>
{
    public async Task HandleAsync(OrderCreatedEvent @event, CancellationToken cancellationToken)
    {
        // Handle event
        Console.WriteLine($"Order {@event.OrderId} created!");
    }
}
```

### **5. CQRS with MediatR**

```csharp
// Command
public record CreateProductCommand : ICommand<Guid>
{
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
}

// Command Handler
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IRepository<Product, Guid> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product { Name = request.Name, Price = request.Price };
        await _repository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(product.Id);
    }
}

// Query
public record GetProductQuery : IQuery<Product>
{
    public Guid Id { get; init; }
}

// Query Handler
public class GetProductQueryHandler : IRequestHandler<GetProductQuery, Result<Product>>
{
    private readonly IRepository<Product, Guid> _repository;

    public async Task<Result<Product>> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return product != null
            ? Result.Success(product)
            : Result.Failure<Product>("Product not found", "NOT_FOUND");
    }
}
```

### **6. Database Setup**

```csharp
// Configure DbContext
public class AppDbContext : BaseDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator)
        : base(options, mediator) { }

    public DbSet<Product> Products => Set<Product>();
}

// Register in Program.cs
builder.Services.AddMarventaDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddMarventaGenericRepository<Product, Guid>();
```

### **7. Entity Definition**

```csharp
public class Product : AuditableEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;

    public Product()
    {
        Id = Guid.NewGuid();
    }
}
```

### **8. API Controller**

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductCommand command)
    {
        var result = await _mediator.Send(command);
        var response = ApiResponseFactory.FromResult(result);
        return result.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _mediator.Send(new GetProductQuery { Id = id });
        var response = ApiResponseFactory.FromResult(result);
        return result.IsSuccess ? Ok(response) : NotFound(response);
    }
}
```

---

## 🔍 Advanced Features

### **Structured Logging**

```csharp
// Configure
builder.Services.AddMarventaLogging(builder.Configuration, "MyApp");

// Usage in services/controllers
public class OrderService
{
    private readonly ILogger<OrderService> _logger;

    public OrderService(ILogger<OrderService> logger)
    {
        _logger = logger;
    }

    public async Task CreateOrderAsync(Order order)
    {
        _logger.LogInformationStructured("Creating order for customer {CustomerId} with total {Total}",
            order.CustomerId, order.Total);

        try
        {
            // ... save order
            _logger.LogInformation("Order {OrderId} created successfully", order.Id);
        }
        catch (Exception ex)
        {
            _logger.LogErrorStructured(ex, "Failed to create order for customer {CustomerId}",
                order.CustomerId);
            throw;
        }
    }
}
```

### **Multi-Tenancy**

```csharp
// Configure
builder.Services.AddMarventaMultiTenancy();
app.UseMarventaMultiTenancy();

// Usage
public class ProductService
{
    private readonly ITenantContext _tenantContext;

    public async Task<List<Product>> GetProductsAsync()
    {
        var tenantId = _tenantContext.TenantId;
        // Filter by tenant
    }
}
```

### **Elasticsearch Integration**

```csharp
builder.Services.AddMarventaElasticsearch(builder.Configuration);

public class ProductSearchService
{
    private readonly IElasticsearchService _search;

    public async Task IndexProductAsync(Product product)
    {
        await _search.IndexDocumentAsync("products", product, product.Id.ToString());
    }

    public async Task<IEnumerable<Product>> SearchAsync(string query)
    {
        return await _search.SearchAsync<Product>("products", query);
    }
}
```

### **Health Checks**

```csharp
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database")
    .AddCheck<RedisHealthCheck>("redis")
    .AddCheck<RabbitMqHealthCheck>("rabbitmq");

app.MapHealthChecks("/health");
```

### **Resilience Policies**

```csharp
var retryPolicy = RetryPolicy.CreateRetryPolicy(3, logger);
var circuitBreaker = CircuitBreakerPolicy.CreateCircuitBreakerPolicy(5, 30, logger);
var timeout = TimeoutPolicy.CreateTimeoutPolicy(30, logger);

var result = await retryPolicy.WrapAsync(circuitBreaker).WrapAsync(timeout)
    .ExecuteAsync(async () => await _httpClient.GetAsync("https://api.example.com"));
```

### **Cloud Storage**

```csharp
// Azure Blob Storage
builder.Services.AddMarventaAzureStorage(builder.Configuration);

// AWS S3
builder.Services.AddMarventaAwsStorage(builder.Configuration);

// Usage
public class FileService
{
    private readonly IStorageService _storage;

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {
        return await _storage.UploadAsync(fileStream, fileName, "image/png");
    }

    public async Task<Stream> DownloadFileAsync(string fileName)
    {
        return await _storage.DownloadAsync(fileName);
    }

    public async Task<bool> DeleteFileAsync(string fileName)
    {
        return await _storage.DeleteAsync(fileName);
    }
}
```

### **MongoDB Integration**

```csharp
builder.Services.AddMarventaMongoDB(builder.Configuration);

public class ProductRepository
{
    private readonly IMongoDatabase _database;

    public ProductRepository(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<Product> GetByIdAsync(string id)
    {
        var collection = _database.GetCollection<Product>("products");
        return await collection.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task InsertAsync(Product product)
    {
        var collection = _database.GetCollection<Product>("products");
        await collection.InsertOneAsync(product);
    }
}
```

### **Kafka Event Streaming**

```csharp
builder.Services.AddMarventaKafka(builder.Configuration);

// Producer
public class OrderEventProducer
{
    private readonly IKafkaProducer _producer;

    public async Task PublishOrderCreatedAsync(Order order)
    {
        await _producer.ProduceAsync("orders-topic", new
        {
            OrderId = order.Id,
            Total = order.Total,
            CreatedAt = DateTime.UtcNow
        });
    }
}

// Consumer
public class OrderEventConsumer : BackgroundService
{
    private readonly IKafkaConsumer _consumer;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consumer.ConsumeAsync<OrderCreatedEvent>("orders-topic", async message =>
        {
            // Process message
            Console.WriteLine($"Order {message.OrderId} received");
        }, stoppingToken);
    }
}
```

### **MassTransit Event Bus**

```csharp
builder.Services.AddMarventaMassTransit(builder.Configuration, cfg =>
{
    cfg.AddConsumer<OrderCreatedEventConsumer>();
});

// Consumer
public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var order = context.Message;
        Console.WriteLine($"Processing order {order.OrderId}");
    }
}
```

### **OpenTelemetry Tracing**

```csharp
builder.Services.AddMarventaOpenTelemetry(builder.Configuration, "MyApp");

// Traces are automatically collected for:
// - ASP.NET Core requests
// - HTTP client calls
// - Entity Framework queries
// - Redis operations
```

### **Rate Limiting**

```csharp
// Configure (100 requests per 60 seconds)
app.UseMarventaRateLimiting(requestLimit: 100, timeWindowSeconds: 60);

// Requests exceeding the limit receive HTTP 429 (Too Many Requests)
```

---

## 📚 Configuration Examples

### **appsettings.json**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyApp;User Id=sa;Password=***;"
  },
  "Jwt": {
    "Secret": "your-super-secret-key-min-32-characters",
    "Issuer": "MyApp",
    "Audience": "MyApp",
    "ExpirationMinutes": 60
  },
  "Redis": {
    "ConnectionString": "localhost:6379",
    "InstanceName": "MyApp:"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  },
  "Elasticsearch": {
    "Uri": "http://localhost:9200"
  },
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "myapp"
  },
  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "GroupId": "myapp-consumer-group"
  },
  "Azure": {
    "Storage": {
      "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=***;AccountKey=***",
      "ContainerName": "myapp-files"
    }
  },
  "AWS": {
    "AccessKey": "your-access-key",
    "SecretKey": "your-secret-key",
    "Region": "us-east-1",
    "BucketName": "myapp-files"
  },
  "OpenTelemetry": {
    "OtlpEndpoint": "http://localhost:4317"
  },
  "Environment": "Development"
}
```

**Notes:**
- `Environment`: Used by Serilog for log enrichment
- Logs are sent to Console + Elasticsearch (`myapp-logs-yyyy-MM`)
- OpenTelemetry exports traces to OTLP endpoint (Jaeger, Zipkin, etc.)

---

## 🧪 Testing

```bash
dotnet test
```

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

---

## 📧 Support

For issues and questions, please use [GitHub Issues](https://github.com/AdemKinatas/Marventa.Framework/issues).

---

## 🌟 Show Your Support

Give a ⭐️ if this project helped you!
