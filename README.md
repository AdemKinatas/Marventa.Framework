# 🚀 Marventa.Framework

**Enterprise .NET Framework - Convention over Configuration**

[![NuGet](https://img.shields.io/nuget/v/Marventa.Framework.svg)](https://www.nuget.org/packages/Marventa.Framework/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

---

## 📖 Table of Contents

1. [Installation](#1-installation)
2. [Basic Setup (2 Lines!)](#2-basic-setup)
3. [Core - Domain Driven Design (DDD)](#3-core---domain-driven-design)
   - 3.1. [Domain - Entity](#31-domain---entity)
   - 3.2. [Domain - Aggregate Root](#32-domain---aggregate-root)
   - 3.3. [Domain - Value Object](#33-domain---value-object)
   - 3.4. [Domain - Domain Events](#34-domain---domain-events)
   - 3.5. [Domain - Auditable Entity](#35-domain---auditable-entity)
   - 3.6. [Application - Result Pattern](#36-application---result-pattern)
4. [Infrastructure - Database & Repository](#4-infrastructure---database--repository)
   - 4.1. [Persistence - Create DbContext](#41-persistence---create-dbcontext)
   - 4.2. [Persistence - Repository Pattern](#42-persistence---repository-pattern)
   - 4.3. [Persistence - Unit of Work](#43-persistence---unit-of-work)
5. [Behaviors - CQRS with MediatR](#5-behaviors---cqrs-with-mediatr)
   - 5.1. [Create Command](#51-create-command)
   - 5.2. [Create Query](#52-create-query)
   - 5.3. [Validation Behavior](#53-validation-behavior)
   - 5.4. [Logging Behavior](#54-logging-behavior)
   - 5.5. [Performance Behavior](#55-performance-behavior)
6. [Features - Caching](#6-features---caching)
   - 6.1. [InMemory Cache](#61-inmemory-cache)
   - 6.2. [Redis Cache](#62-redis-cache)
   - 6.3. [Hybrid Cache](#63-hybrid-cache)
7. [Features - Event Bus](#7-features---event-bus)
   - 7.1. [RabbitMQ Event Bus](#71-rabbitmq-event-bus)
   - 7.2. [Kafka Producer/Consumer](#72-kafka-producerconsumer)
   - 7.3. [MassTransit Integration](#73-masstransit-integration)
8. [Features - Storage](#8-features---storage)
   - 8.1. [Local File Storage](#81-local-file-storage)
   - 8.2. [Azure Blob Storage](#82-azure-blob-storage)
   - 8.3. [AWS S3 Storage](#83-aws-s3-storage)
9. [Features - Search](#9-features---search)
   - 9.1. [Elasticsearch](#91-elasticsearch)
10. [Features - Logging](#10-features---logging)
    - 10.1. [Serilog](#101-serilog)
    - 10.2. [OpenTelemetry Tracing](#102-opentelemetry-tracing)
11. [Security - Authentication](#11-security---authentication)
    - 11.1. [JWT Token Generator](#111-jwt-token-generator)
    - 11.2. [Password Hasher](#112-password-hasher)
    - 11.3. [AES Encryption](#113-aes-encryption)
12. [Security - Authorization](#12-security---authorization)
    - 12.1. [Permission Based Authorization](#121-permission-based-authorization)
13. [Security - Rate Limiting](#13-security---rate-limiting)
14. [Infrastructure - Multi-Tenancy](#14-infrastructure---multi-tenancy)
15. [Infrastructure - Health Checks](#15-infrastructure---health-checks)
16. [Middleware - Exception Handling](#16-middleware---exception-handling)
17. [Configuration - appsettings.json](#17-configuration---appsettingsjson)

---

## 1. Installation

```bash
dotnet add package Marventa.Framework
```

---

## 2. Basic Setup

### Program.cs - Just 2 Lines!

```csharp
var builder = WebApplication.CreateBuilder(args);

// ✨ ONE LINE - All services registered
builder.Services.AddMarventa(builder.Configuration);

builder.Services.AddControllers();

var app = builder.Build();

// ✨ ONE LINE - All middleware configured
app.UseMarventa(builder.Configuration);

app.MapControllers();
app.Run();
```

That's it! Now activate features via `appsettings.json`.

---

## 3. Core - Domain Driven Design

### 3.1. Domain - Entity

**Purpose:** Represents domain objects with identity.

```csharp
using Marventa.Framework.Core.Domain;

public class Product : Entity<Guid>
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }

    private Product() { }

    public static Product Create(string name, decimal price, int stock)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Price = price,
            Stock = stock
        };
    }

    public void UpdateStock(int quantity)
    {
        Stock += quantity;
    }
}
```

### 3.2. Domain - Aggregate Root

**Purpose:** Root entity that manages business rules and dispatches domain events.

```csharp
public class Order : AggregateRoot<Guid>
{
    private readonly List<OrderItem> _items = new();

    public string OrderNumber { get; private set; }
    public OrderStatus Status { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public static Order Create(string orderNumber)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = orderNumber,
            Status = OrderStatus.Pending
        };

        order.AddDomainEvent(new OrderCreatedEvent(order.Id));
        return order;
    }

    public void Confirm()
    {
        Status = OrderStatus.Confirmed;
        AddDomainEvent(new OrderConfirmedEvent(Id));
    }
}
```

### 3.3. Domain - Value Object

**Purpose:** Objects without identity, compared by their values.

```csharp
public class Address : ValueObject
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string ZipCode { get; private set; }

    public Address(string street, string city, string zipCode)
    {
        Street = street;
        City = city;
        ZipCode = zipCode;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return ZipCode;
    }
}
```

### 3.4. Domain - Domain Events

**Purpose:** Represents events that occur within the domain.

```csharp
public record ProductCreatedEvent(Guid ProductId, string Name) : DomainEvent;
public record OrderCreatedEvent(Guid OrderId) : DomainEvent;
public record OrderConfirmedEvent(Guid OrderId) : DomainEvent;
```

### 3.5. Domain - Auditable Entity

**Purpose:** Automatically tracks creation and update information.

```csharp
public class Customer : AuditableEntity<Guid>
{
    public string Name { get; set; }
    public string Email { get; set; }

    // CreatedAt, UpdatedAt, CreatedBy, UpdatedBy tracked automatically!
}
```

### 3.6. Application - Result Pattern

**Purpose:** Type-safe way to return success/failure states.

```csharp
public async Task<Result<Guid>> CreateProduct(string name, decimal price)
{
    if (price <= 0)
        return Result<Guid>.Failure("Price must be positive");

    var product = Product.Create(name, price, 0);
    await _repository.AddAsync(product);

    return Result<Guid>.Success(product.Id);
}
```

---

## 4. Infrastructure - Database & Repository

### 4.1. Persistence - Create DbContext

**Purpose:** Database connection with Entity Framework Core.

```csharp
using Marventa.Framework.Infrastructure.Persistence;

public class ApplicationDbContext : BaseDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IHttpContextAccessor httpContextAccessor)
        : base(options, httpContextAccessor)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
}
```

**Add to Program.cs:**
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork<ApplicationDbContext>>();
```

**appsettings.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyDb;Trusted_Connection=true;"
  }
}
```

### 4.2. Persistence - Repository Pattern

**Purpose:** Abstracts database operations.

```csharp
// Interface
public interface IProductRepository : IRepository<Product, Guid>
{
    Task<Product?> GetByNameAsync(string name);
    Task<List<Product>> SearchAsync(string searchTerm);
}

// Implementation
public class ProductRepository : GenericRepository<Product, Guid>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Product?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Name == name);
    }

    public async Task<List<Product>> SearchAsync(string searchTerm)
    {
        return await _dbSet.Where(p => p.Name.Contains(searchTerm)).ToListAsync();
    }
}
```

**Add to Program.cs:**
```csharp
builder.Services.AddScoped<IProductRepository, ProductRepository>();
```

### 4.3. Persistence - Unit of Work

**Purpose:** Manages transactions and dispatches domain events.

```csharp
public class ProductService
{
    private readonly IProductRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Result<Guid>> CreateProductAsync(string name, decimal price)
    {
        var product = Product.Create(name, price, 0);
        await _repository.AddAsync(product);

        // Transaction + Domain Events
        await _unitOfWork.SaveChangesAsync();

        return Result<Guid>.Success(product.Id);
    }
}
```

---

## 5. Behaviors - CQRS with MediatR

**Purpose:** MediatR is auto-registered with validation/logging/performance behaviors active.

### 5.1. Create Command

```csharp
// Command
public record CreateProductCommand(string Name, decimal Price) : IRequest<Result<Guid>>;

// Handler
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IProductRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = Product.Create(request.Name, request.Price, 0);
        await _repository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(product.Id);
    }
}

// Validator
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
```

### 5.2. Create Query

```csharp
// Query
public record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;

// Handler
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IProductRepository _repository;

    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(request.Id);
        if (product == null)
            return Result<ProductDto>.Failure("Product not found");

        return Result<ProductDto>.Success(new ProductDto(product.Id, product.Name, product.Price));
    }
}

public record ProductDto(Guid Id, string Name, decimal Price);
```

**Usage in Controller:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id));
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.ErrorMessage);
    }
}
```

### 5.3. Validation Behavior

**Purpose:** Automatically validates all requests with FluentValidation.
**Auto-active!** Just write validator classes.

### 5.4. Logging Behavior

**Purpose:** Logs all requests/responses.
**Auto-active!**

### 5.5. Performance Behavior

**Purpose:** Warns about requests taking longer than 500ms.
**Auto-active!**

---

## 6. Features - Caching

### 6.1. InMemory Cache

**Purpose:** Default cache, no configuration required.

```csharp
using Marventa.Framework.Features.Caching.Abstractions;

public class ProductService
{
    private readonly ICacheService _cache;

    public async Task<Product?> GetProductAsync(Guid id)
    {
        var cacheKey = $"product:{id}";

        var cached = await _cache.GetAsync<Product>(cacheKey);
        if (cached != null) return cached;

        var product = await _repository.GetByIdAsync(id);
        await _cache.SetAsync(cacheKey, product, TimeSpan.FromHours(1));

        return product;
    }
}
```

### 6.2. Redis Cache

**Configuration (appsettings.json):**
```json
{
  "Caching": { "Type": "Redis" },
  "Redis": {
    "ConnectionString": "localhost:6379",
    "InstanceName": "MyApp:"
  }
}
```

**Usage:** Same interface as InMemory (`ICacheService`).

### 6.3. Hybrid Cache

**Purpose:** Reads from InMemory first, then Redis.

**Configuration:**
```json
{
  "Caching": { "Type": "Hybrid" },
  "Redis": {
    "ConnectionString": "localhost:6379",
    "InstanceName": "MyApp:"
  }
}
```

---

## 7. Features - Event Bus

### 7.1. RabbitMQ Event Bus

**Configuration:**
```json
{
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  }
}
```

**Publish:**
```csharp
public class OrderCreatedEvent : IntegrationEvent
{
    public Guid OrderId { get; }
    public string OrderNumber { get; }

    public OrderCreatedEvent(Guid orderId, string orderNumber)
    {
        OrderId = orderId;
        OrderNumber = orderNumber;
    }
}

await _eventBus.PublishAsync(new OrderCreatedEvent(orderId, orderNumber));
```

**Subscribe:**
```csharp
public class OrderCreatedEventHandler : IIntegrationEventHandler<OrderCreatedEvent>
{
    public async Task HandleAsync(OrderCreatedEvent @event)
    {
        Console.WriteLine($"Order created: {@event.OrderNumber}");
    }
}

// Add to Program.cs
builder.Services.AddScoped<IIntegrationEventHandler<OrderCreatedEvent>, OrderCreatedEventHandler>();
```

### 7.2. Kafka Producer/Consumer

**Configuration:**
```json
{
  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "GroupId": "myapp-group"
  }
}
```

**Usage:**
```csharp
// Produce
await _kafkaProducer.ProduceAsync("my-topic", new { UserId = 123 });

// Consume
await _kafkaConsumer.ConsumeAsync("my-topic", async message =>
{
    Console.WriteLine($"Received: {message}");
});
```

### 7.3. MassTransit Integration

**Configuration:**
```json
{
  "MassTransit": { "Enabled": "true" },
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  }
}
```

**Usage:**
```csharp
// Consumer
public class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        Console.WriteLine($"Order {context.Message.OrderId}");
    }
}

// Publish
await _publishEndpoint.Publish(new OrderCreated { OrderId = 123 });
```

---

## 8. Features - Storage

### 8.1. Local File Storage

**Configuration:**
```json
{
  "LocalStorage": {
    "BasePath": "D:/uploads",
    "BaseUrl": "https://myapp.com/files"
  }
}
```

**Usage:**
```csharp
// Upload
await _storage.UploadAsync(fileStream, "documents/file.pdf");

// Download
var stream = await _storage.DownloadAsync("documents/file.pdf");

// Delete
await _storage.DeleteAsync("documents/file.pdf");

// Get URL
var url = await _storage.GetUrlAsync("documents/file.pdf");
```

### 8.2. Azure Blob Storage

**Configuration:**
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

**Usage:** Same as Local Storage.

### 8.3. AWS S3 Storage

**Configuration:**
```json
{
  "AWS": {
    "AccessKey": "your-key",
    "SecretKey": "your-secret",
    "Region": "us-east-1",
    "BucketName": "my-bucket"
  }
}
```

**Usage:** Same as Local Storage.

---

## 9. Features - Search

### 9.1. Elasticsearch

**Configuration:**
```json
{
  "Elasticsearch": {
    "Uri": "http://localhost:9200"
  }
}
```

**Usage:**
```csharp
// Index
await _elasticsearchService.IndexAsync("products", product);

// Search
var results = await _elasticsearchService.SearchAsync<Product>("products", "laptop");
```

---

## 10. Features - Logging

### 10.1. Serilog

**Configuration:**
```json
{
  "ApplicationName": "MyApp",
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "File", "Args": { "path": "logs/log-.txt", "rollingInterval": "Day" } }
    ]
  }
}
```

**Usage:**
```csharp
_logger.LogInformation("Product {ProductId} created", productId);
_logger.LogError(ex, "Failed to create product");
```

### 10.2. OpenTelemetry Tracing

**Configuration:**
```json
{
  "OpenTelemetry": {
    "ServiceName": "MyApp",
    "OtlpEndpoint": "http://localhost:4317"
  }
}
```

**Purpose:** Automatically traces HTTP, Database, and External API calls.

---

## 11. Security - Authentication

### 11.1. JWT Token Generator

**Configuration:**
```json
{
  "Jwt": {
    "Secret": "your-super-secret-key-at-least-32-characters",
    "Issuer": "MyApp",
    "Audience": "MyApp",
    "ExpirationMinutes": 60
  }
}
```

**Usage:**
```csharp
var token = _jwtTokenGenerator.GenerateToken(
    userId: user.Id.ToString(),
    email: user.Email,
    roles: new[] { "Admin" },
    permissions: new[] { "products.read", "products.write" }
);
```

### 11.2. Password Hasher

```csharp
// Hash
var hashedPassword = _passwordHasher.HashPassword("password123");

// Verify
bool isValid = _passwordHasher.VerifyPassword("password123", hashedPassword);
```

### 11.3. AES Encryption

```csharp
var encryption = new AesEncryption(
    key: "your-32-character-secret-key!",
    iv: "your-16-char-iv"
);

var encrypted = encryption.Encrypt("sensitive data");
var decrypted = encryption.Decrypt(encrypted);
```

---

## 12. Security - Authorization

### 12.1. Permission Based Authorization

```csharp
[Authorize]
[RequirePermission("products.write")]
public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
{
    var result = await _mediator.Send(command);
    return Ok(result);
}
```

---

## 13. Security - Rate Limiting

**Configuration:**
```json
{
  "RateLimiting": {
    "Strategy": "IpAddress",
    "RequestsPerMinute": 100,
    "Enabled": true
  }
}
```

**Purpose:** Automatically limits to 100 requests/minute per IP.

**Response Headers:**
```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1633024800
```

---

## 14. Infrastructure - Multi-Tenancy

**Configuration:**
```json
{
  "MultiTenancy": {
    "Strategy": "Header",
    "HeaderName": "X-Tenant-Id"
  }
}
```

**Usage:**
```csharp
var tenantId = _tenantContext.TenantId;
var tenantName = _tenantContext.TenantName;

var data = _repository.GetAll()
    .Where(x => x.TenantId == tenantId)
    .ToList();
```

**Client Request:**
```bash
curl -H "X-Tenant-Id: tenant-123" https://api.myapp.com/products
```

---

## 15. Infrastructure - Health Checks

**Configuration:**
```json
{
  "HealthChecks": {
    "Enabled": "true"
  }
}
```

**Purpose:** Creates `/health` endpoint, automatically monitors Database/Redis/RabbitMQ.

**Check:**
```bash
curl http://localhost:5000/health
```

---

## 16. Middleware - Exception Handling

**Purpose:** Catches all exceptions and returns standard format.
**Auto-active!**

**Custom Exceptions:**
```csharp
throw new NotFoundException("Product not found");
throw new BusinessException("Insufficient stock");
throw new UnauthorizedException("Invalid credentials");
```

---

## 17. Configuration - appsettings.json

**Complete configuration example:**

```json
{
  "ApplicationName": "MyApp",

  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyDb;Trusted_Connection=true;"
  },

  "Jwt": {
    "Secret": "your-super-secret-key-at-least-32-characters-long",
    "Issuer": "MyApp",
    "Audience": "MyApp",
    "ExpirationMinutes": 60
  },

  "Caching": {
    "Type": "Hybrid"
  },

  "Redis": {
    "ConnectionString": "localhost:6379",
    "InstanceName": "MyApp:"
  },

  "MultiTenancy": {
    "Strategy": "Header",
    "HeaderName": "X-Tenant-Id"
  },

  "RateLimiting": {
    "Strategy": "IpAddress",
    "RequestsPerMinute": 100,
    "Enabled": true
  },

  "RabbitMQ": {
    "Host": "localhost",
    "VirtualHost": "/",
    "Username": "guest",
    "Password": "guest"
  },

  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "GroupId": "myapp-group"
  },

  "MassTransit": {
    "Enabled": "true"
  },

  "LocalStorage": {
    "BasePath": "D:/uploads",
    "BaseUrl": "https://myapp.com/files"
  },

  "Azure": {
    "Storage": {
      "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=...",
      "ContainerName": "uploads"
    }
  },

  "AWS": {
    "AccessKey": "your-access-key",
    "SecretKey": "your-secret-key",
    "Region": "us-east-1",
    "BucketName": "my-bucket"
  },

  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "MyDatabase"
  },

  "Elasticsearch": {
    "Uri": "http://localhost:9200"
  },

  "HealthChecks": {
    "Enabled": "true"
  },

  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "logs/log-.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 7
        }
      }
    ]
  },

  "OpenTelemetry": {
    "ServiceName": "MyApp",
    "OtlpEndpoint": "http://localhost:4317"
  }
}
```

---

## 🎉 That's It!

Your application now has:

✅ **Core:** Domain Driven Design (Entity, Aggregate, ValueObject, DomainEvent)
✅ **Behaviors:** CQRS (MediatR + Validation + Logging + Performance)
✅ **Infrastructure:** Repository Pattern, Unit of Work, Multi-Tenancy, Health Checks
✅ **Features:** Caching, Event Bus, Storage, Search, Logging
✅ **Security:** JWT Auth, Permission Authorization, Rate Limiting, Password Hashing
✅ **Middleware:** Global Exception Handling

**With just 2 lines of setup!** 🚀

---

## 📄 License

MIT License - See [LICENSE](LICENSE) for details.

---

## 📧 Support

For questions, please open an issue on [GitHub Issues](https://github.com/AdemKinatas/Marventa.Framework/issues).
