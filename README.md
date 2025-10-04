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
   - 4.4. [Persistence - Data Seeding](#44-persistence---data-seeding)
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
16. [Infrastructure - API Versioning](#16-infrastructure---api-versioning)
17. [Infrastructure - Swagger/OpenAPI](#17-infrastructure---swaggeropenapi)
18. [Middleware - Exception Handling](#18-middleware---exception-handling)
19. [Enterprise Patterns](#19-enterprise-patterns)
    - 19.1. [Transactional Outbox Pattern](#191-transactional-outbox-pattern)
    - 19.2. [Repository Specification Pattern](#192-repository-specification-pattern)
    - 19.3. [Idempotency Pattern](#193-idempotency-pattern)
    - 19.4. [Circuit Breaker & Resilience](#194-circuit-breaker--resilience)
20. [Advanced Security](#20-advanced-security)
    - 20.1. [API Key Authentication](#201-api-key-authentication)
    - 20.2. [Request/Response Logging](#202-requestresponse-logging)
21. [Configuration - appsettings.json](#21-configuration---appsettingsjson)

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

// ✨ ONE LINE - All services registered automatically
// Includes: Controllers, MediatR, FluentValidation, Mapster, CORS, and all configured features
builder.Services.AddMarventa(builder.Configuration);

var app = builder.Build();

// ✨ ONE LINE - All middleware configured automatically
// Includes: Exception handling, CORS, Authentication, Authorization, Rate Limiting, and Endpoints
app.UseMarventa(builder.Configuration);

app.Run();
```

**That's it!** The framework automatically:
- ✅ Registers controllers and JSON serialization
- ✅ Scans assemblies for MediatR handlers, FluentValidation validators, and Mapster mappings
- ✅ Configures middleware pipeline in correct order
- ✅ Maps controller endpoints and health checks
- ✅ Activates features based on `appsettings.json`

### Advanced: Specify Assemblies to Scan

```csharp
// Automatically scans calling assembly (recommended)
builder.Services.AddMarventa(builder.Configuration);

// Or explicitly specify assemblies to scan
builder.Services.AddMarventa(builder.Configuration, typeof(Program).Assembly);

// Or scan multiple assemblies
builder.Services.AddMarventa(
    builder.Configuration,
    typeof(Program).Assembly,
    typeof(SomeOtherClass).Assembly
);
```

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

builder.Services.AddScoped<IUnitOfWork>(sp =>
    new UnitOfWork(sp.GetRequiredService<ApplicationDbContext>()));
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

### 4.4. Persistence - Data Seeding

**Purpose:** Seed initial data into database with helper infrastructure.

```csharp
// Create a seeder
public class UserSeeder : DataSeederBase<ApplicationDbContext>
{
    public UserSeeder(ApplicationDbContext context) : base(context)
    {
    }

    public override int Order => 1; // Execution order

    public override async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await AnyAsync<User>(cancellationToken))
            return;

        var users = new List<User>
        {
            User.Create("admin@example.com", "Admin User"),
            User.Create("user@example.com", "Regular User")
        };

        await AddRangeAsync(users, cancellationToken);
    }
}
```

**Register Seeders:**
```csharp
builder.Services.AddScoped<IDataSeeder, UserSeeder>();
builder.Services.AddScoped<IDataSeeder, ProductSeeder>();
```

**Run Seeders:**
```csharp
// In Program.cs after app.Build()
using (var scope = app.Services.CreateScope())
{
    var seederRunner = scope.ServiceProvider.GetRequiredService<DataSeederRunner>();
    await seederRunner.RunAsync();
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

**Purpose:** Framework supports three caching strategies: InMemory, Redis, and Hybrid (two-level cache).

### 6.1. InMemory Cache

**Configuration (appsettings.json):**
```json
{
  "MemoryCache": {
    "SizeLimit": 1024,
    "CompactionPercentage": 0.25,
    "ExpirationScanFrequency": "00:01:00"
  }
}
```

**Usage:**
```csharp
using Marventa.Framework.Features.Caching.Abstractions;

public class ProductService
{
    private readonly ICacheService _cache;

    public async Task<Product?> GetProductAsync(Guid id)
    {
        var cacheKey = $"product:{id}";

        // Try get from cache
        var cached = await _cache.GetAsync<Product>(cacheKey);
        if (cached != null) return cached;

        // Get from database
        var product = await _repository.GetByIdAsync(id);

        // Set cache with expiration
        await _cache.SetAsync(cacheKey, product, TimeSpan.FromHours(1));

        return product;
    }

    public async Task RemoveProductCacheAsync(Guid id)
    {
        await _cache.RemoveAsync($"product:{id}");
    }
}
```

### 6.2. Output Cache

**Purpose:** ASP.NET Core 7+ output caching for HTTP responses.

**Configuration:**
```json
{
  "OutputCache": {
    "Enabled": true,
    "DefaultExpirationSeconds": 60,
    "VaryByQuery": true,
    "VaryByHeader": false,
    "VaryByHeaderNames": []
  }
}
```

**Add to Program.cs:**
```csharp
builder.Services.AddMarventaOutputCache(builder.Configuration);
```

**Usage in Controllers:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    // Cache response for 60 seconds (from configuration)
    [OutputCache]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _mediator.Send(new GetAllProductsQuery());
        return Ok(products);
    }

    // Custom cache duration
    [OutputCache(Duration = 300)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id));
        return Ok(product);
    }
}
```

### 6.3. Redis Cache

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

**Usage:** Same interface as InMemory (`ICacheService`). Framework automatically switches based on configuration.

### 6.4. Hybrid Cache

**Purpose:** Two-level caching - reads from InMemory (L1) first, then Redis (L2). Best of both worlds!

**Configuration:**
```json
{
  "Caching": { "Type": "Hybrid" },
  "MemoryCache": {
    "SizeLimit": 1024,
    "CompactionPercentage": 0.25
  },
  "Redis": {
    "ConnectionString": "localhost:6379",
    "InstanceName": "MyApp:"
  }
}
```

**How it works:**
- **Get**: Checks InMemory first, then Redis if not found
- **Set**: Writes to both InMemory and Redis
- **Remove**: Removes from both caches

**Usage:** Same `ICacheService` interface - completely transparent!

### 6.5. Modular Caching Setup

**Add specific cache type:**
```csharp
// Add InMemory cache only
builder.Services.AddInMemoryCaching(builder.Configuration);

// Add Redis cache only
builder.Services.AddRedisCaching(builder.Configuration);

// Add Hybrid cache
builder.Services.AddHybridCaching(builder.Configuration);

// Auto-detect from configuration (used by AddMarventa)
builder.Services.AddMarventaCaching(builder.Configuration);
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

### 11.1. JWT Authentication Service

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

**Generate Access Token:**
```csharp
using Marventa.Framework.Security.Authentication.Abstractions;

public class AuthService
{
    private readonly IJwtService _jwtService;

    // Simple usage
    public string Login(User user)
    {
        var token = _jwtService.GenerateAccessToken(
            userId: user.Id.ToString(),
            email: user.Email,
            roles: new[] { "Admin" },
            additionalClaims: new Dictionary<string, string>
            {
                ["department"] = "IT",
                ["permission"] = "products.write"
            }
        );

        return token;
    }
}
```

**Validate and Extract Claims:**
```csharp
// Validate token
var principal = _jwtService.ValidateAccessToken(token);
if (principal == null)
{
    // Token invalid or expired
}

// Get user ID from token
var userId = _jwtService.GetUserIdFromToken(token);

// Get all claims
var claims = _jwtService.GetClaimsFromToken(token);

// Check if token expired
var isExpired = _jwtService.IsTokenExpired(token);

// Get remaining lifetime
var remainingTime = _jwtService.GetTokenRemainingLifetime(token);
```

### 11.2. Refresh Token Generation

**Purpose:** Generate cryptographically secure refresh token strings.

**Important:** The framework only provides token generation. You must implement your own:
- RefreshToken entity in your domain model
- Repository for database storage
- Validation, rotation, and revocation logic

**Generate Refresh Token:**
```csharp
using Marventa.Framework.Security.Authentication.Abstractions;

public class AuthService
{
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<LoginResponse> LoginAsync(string email, string password)
    {
        // Validate user credentials...

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user.Id.ToString(), user.Email);
        var refreshTokenString = _jwtService.GenerateRefreshToken();

        // Create your own domain entity
        var refreshToken = new Domain.Entities.RefreshToken
        {
            Token = refreshTokenString,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedByIp = HttpContext.Connection.RemoteIpAddress?.ToString()
        };

        // Save to your database
        await _refreshTokenRepository.AddAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenString,
            ExpiresAt = refreshToken.ExpiresAt
        };
    }

    public async Task<LoginResponse> RefreshTokenAsync(string refreshToken)
    {
        // Validate from your database
        var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
        if (token == null || token.IsExpired || token.IsRevoked)
        {
            throw new UnauthorizedException("Invalid refresh token");
        }

        // Optional: Implement token rotation
        token.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepository.UpdateAsync(token);

        // Generate new tokens
        var accessToken = _jwtService.GenerateAccessToken(token.UserId.ToString(), user.Email);
        var newRefreshTokenString = _jwtService.GenerateRefreshToken();

        var newRefreshToken = new Domain.Entities.RefreshToken
        {
            Token = newRefreshTokenString,
            UserId = token.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            ReplacedByToken = token.Token
        };

        await _refreshTokenRepository.AddAsync(newRefreshToken);
        await _unitOfWork.SaveChangesAsync();

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshTokenString,
            ExpiresAt = newRefreshToken.ExpiresAt
        };
    }
}
```

**Your Domain Entity Example:**
```csharp
public class RefreshToken : Entity<Guid>
{
    public string Token { get; set; }
    public Guid UserId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? CreatedByIp { get; set; }
    public string? ReplacedByToken { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsExpired && !IsRevoked;
}
```

### 11.3. Password Service

**Purpose:** Secure password hashing with **Argon2id** (winner of Password Hashing Competition 2015) and strength validation.

**Why Argon2id?**
- ✅ 2-6x FASTER than BCrypt
- ✅ More secure against GPU/ASIC attacks (high memory requirement)
- ✅ OWASP recommended (2024)
- ✅ Automatic migration from BCrypt hashes

```csharp
using Marventa.Framework.Security.Encryption.Abstractions;

public class UserService
{
    private readonly IPasswordService _passwordService;

    // Hash password with Argon2id
    public async Task RegisterAsync(string email, string password)
    {
        // Validate password strength
        var (isValid, errorMessage) = _passwordService.ValidatePasswordStrength(
            password: password,
            minLength: 8,
            requireUppercase: true,
            requireLowercase: true,
            requireDigit: true,
            requireSpecialChar: true
        );

        if (!isValid)
        {
            throw new BusinessException($"Weak password: {errorMessage}");
        }

        // Hash with Argon2id (OWASP settings: m=19456, t=2, p=1)
        var hashedPassword = _passwordService.HashPassword(password);

        // Save user with hashed password...
    }

    // Verify password (supports both Argon2id and legacy BCrypt)
    public async Task<bool> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            return false;

        var isValid = _passwordService.VerifyPassword(password, user.PasswordHash);

        // Automatic migration from BCrypt to Argon2id
        if (isValid && _passwordService.NeedsRehash(user.PasswordHash))
        {
            user.PasswordHash = _passwordService.HashPassword(password);
            await _userRepository.UpdateAsync(user);
        }

        return isValid;
    }

    // Generate secure random password
    public string GenerateTemporaryPassword()
    {
        return _passwordService.GenerateSecurePassword(
            length: 16,
            includeSpecialCharacters: true
        );
    }
}
```

**Hash Format:**
```
Argon2id: $argon2id$v=19$m=19456,t=2,p=1$<salt>$<hash>
BCrypt (legacy): $2a$12$<salt+hash>
```

**Performance Comparison:**
| Algorithm | Hash Time | Security |
|-----------|-----------|----------|
| Argon2id (current) | 30-50ms | ✅ GPU/ASIC resistant |
| BCrypt (legacy) | 100-300ms | ⚠️ Vulnerable to GPU attacks |

**Migration is automatic!** Existing BCrypt hashes are verified normally, then upgraded to Argon2id on next successful login.

### 11.4. AES Encryption

**Purpose:** Symmetric encryption for sensitive data.

```csharp
using Marventa.Framework.Security.Encryption;

var encryption = new AesEncryption(
    key: "your-32-character-secret-key!",
    iv: "your-16-char-iv"
);

// Encrypt sensitive data
var encrypted = encryption.Encrypt("sensitive data");

// Decrypt
var decrypted = encryption.Decrypt(encrypted);
```

**Use Cases:**
- Encrypting database connection strings
- Storing sensitive configuration values
- Protecting PII (Personal Identifiable Information)

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
    "RequestLimit": 100,
    "TimeWindowSeconds": 60
  }
}
```

**Purpose:** Automatically limits to 100 requests per 60 seconds per IP.

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

## 16. Infrastructure - API Versioning

**Purpose:** Provides flexible API versioning strategies.

**Configuration:**
```json
{
  "ApiVersioning": {
    "Enabled": true,
    "DefaultVersion": "1.0",
    "ReportApiVersions": true,
    "AssumeDefaultVersionWhenUnspecified": true,
    "VersioningType": "UrlSegment",
    "HeaderName": "X-API-Version",
    "QueryStringParameterName": "api-version"
  }
}
```

**Versioning Types:**
- `UrlSegment` - `/api/v1/products` (default)
- `QueryString` - `/api/products?api-version=1.0`
- `Header` - Header: `X-API-Version: 1.0`
- `MediaType` - Accept: `application/json;v=1.0`

**Usage in Controllers:**
```csharp
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class ProductsV1Controller : ControllerBase
{
    [HttpGet]
    public IActionResult GetProducts()
    {
        return Ok(new[] { "Product 1", "Product 2" });
    }
}

[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class ProductsV2Controller : ControllerBase
{
    [HttpGet]
    public IActionResult GetProducts()
    {
        return Ok(new { products = new[] { "Product 1", "Product 2" }, version = "2.0" });
    }
}
```

**Response Headers:**
```
api-supported-versions: 1.0, 2.0
api-deprecated-versions: (none)
```

---

## 17. Infrastructure - Swagger/OpenAPI

**Purpose:** Auto-configured OpenAPI documentation with JWT support and environment restrictions.

**Configuration:**
```json
{
  "Swagger": {
    "Enabled": true,
    "Title": "My API",
    "Description": "My API Documentation",
    "Version": "v1",
    "RequireAuthorization": true,
    "EnvironmentRestriction": ["Development", "Staging"],
    "Contact": {
      "Name": "API Support",
      "Email": "support@example.com",
      "Url": "https://example.com/support"
    },
    "License": {
      "Name": "MIT",
      "Url": "https://opensource.org/licenses/MIT"
    }
  }
}
```

**Features:**
- ✅ Automatic JWT Bearer integration
- ✅ Multi-version support (when API Versioning enabled)
- ✅ XML comments auto-included
- ✅ Environment-based restrictions
- ✅ Swagger UI auto-configured

**Access:**
```bash
# Development/Staging only (based on EnvironmentRestriction)
https://localhost:5001/swagger
```

**Usage in Program.cs:**
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMarventa(builder.Configuration);

var app = builder.Build();

// Pass IWebHostEnvironment for environment-based Swagger
app.UseMarventa(builder.Configuration, app.Environment);

app.Run();
```

**Controller XML Comments:**
```csharp
/// <summary>
/// Creates a new product
/// </summary>
/// <param name="command">Product creation data</param>
/// <returns>The created product ID</returns>
/// <response code="200">Product created successfully</response>
/// <response code="400">Invalid request</response>
[HttpPost]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
{
    var result = await _mediator.Send(command);
    return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
}
```

---

## 18. Middleware - Exception Handling

**Purpose:** Catches all exceptions and returns standard format.
**Auto-active!**

**Custom Exceptions:**
```csharp
throw new NotFoundException("Product not found");
throw new BusinessException("Insufficient stock");
throw new UnauthorizedException("Invalid credentials");
```

---

## 19. Enterprise Patterns

### 19.1. Transactional Outbox Pattern

The Outbox Pattern ensures reliable event publishing by storing events in the database in the same transaction as business data.

**Setup:**

```csharp
// Program.cs
builder.Services.AddDbContext<YourDbContext>();
builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<YourDbContext>());
builder.Services.AddOutbox(); // Registers outbox services and background processor
```

**Usage:**

```csharp
// Domain Entity
public class Product : Entity<Guid>, IHasDomainEvents
{
    public void Create()
    {
        AddDomainEvent(new ProductCreatedEvent(Id, Name, Price));
    }
}

// Event Handler
public class ProductCreatedEventHandler : INotificationHandler<ProductCreatedEvent>
{
    public async Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Event will be published reliably via outbox
        await _eventBus.PublishAsync(notification);
    }
}
```

**How it works:**
1. Domain events saved to `OutboxMessages` table in same transaction
2. Background `OutboxProcessor` polls every 30 seconds
3. Processes pending messages and publishes events
4. Marks messages as processed
5. Cleans up old messages (7-day retention)

---

### 19.2. Repository Specification Pattern

Build reusable, composable query specifications for complex queries.

**Create Specification:**

```csharp
public class ActiveProductsSpecification : BaseSpecification<Product>
{
    public ActiveProductsSpecification()
    {
        // Filtering
        Criteria = p => p.IsActive && !p.IsDeleted;

        // Eager Loading
        AddInclude(p => p.Category);
        AddInclude(p => p.Supplier);

        // Ordering
        AddOrderBy(p => p.Name);

        // Pagination
        ApplyPaging(0, 20);
    }
}

public class ProductsByPriceRangeSpec : BaseSpecification<Product>
{
    public ProductsByPriceRangeSpec(decimal minPrice, decimal maxPrice)
    {
        Criteria = p => p.Price >= minPrice && p.Price <= maxPrice;
        AddOrderByDescending(p => p.Price);
    }
}
```

**Use Specification:**

```csharp
// In Repository or Command Handler
var spec = new ActiveProductsSpecification();
var products = await _repository.FindAsync(spec);

var priceSpec = new ProductsByPriceRangeSpec(50m, 200m);
var filteredProducts = await _repository.FindAsync(priceSpec);
```

**Available Methods:**
- `Criteria` - WHERE clause filter
- `AddInclude()` / `AddInclude(string)` - Eager loading
- `AddOrderBy()` / `AddOrderByDescending()` - Sorting
- `ApplyPaging(skip, take)` - Pagination

---

### 19.3. Idempotency Pattern

Prevent duplicate request processing with distributed caching.

**Setup:**

```csharp
// Program.cs
builder.Services.AddDistributedMemoryCache(); // or AddStackExchangeRedisCache
builder.Services.AddIdempotency();

var app = builder.Build();
app.UseRouting();
app.UseIdempotency(); // Must be after UseRouting()
```

**Usage:**

```csharp
// Client sends Idempotency-Key header
POST /api/orders
Headers:
  Idempotency-Key: 550e8400-e29b-41d4-a716-446655440000

Body: { "productId": "123", "quantity": 2 }
```

**How it works:**
1. Middleware detects `Idempotency-Key` header
2. Checks cache for existing response
3. If found, returns cached response (duplicate detected)
4. If not found, processes request and caches response
5. Default 24-hour expiration
6. Only caches successful responses (2xx status codes)

---

### 19.4. Circuit Breaker & Resilience

Add resilience to HTTP clients with Polly integration.

**Setup:**

```csharp
// Program.cs
builder.Services.AddResilientHttpClient("PaymentService", "https://api.payment.com")
    .AddHttpMessageHandler(() => new AuthHeaderHandler());

// Or configure manually
builder.Services.AddHttpClient("OrderService")
    .AddPolicyHandler(ResilienceExtensions.GetRetryPolicy(3))
    .AddPolicyHandler(ResilienceExtensions.GetCircuitBreakerPolicy(5, TimeSpan.FromSeconds(30)));
```

**Usage:**

```csharp
public class PaymentService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public async Task<bool> ProcessPayment(PaymentRequest request)
    {
        var client = _httpClientFactory.CreateClient("PaymentService");
        var response = await client.PostAsJsonAsync("/payments", request);
        return response.IsSuccessStatusCode;
    }
}
```

**Policies:**
- **Retry Policy:** 3 retries with exponential backoff (2s, 4s, 8s)
- **Circuit Breaker:** Opens after 5 consecutive failures, stays open for 30s
- **Timeout:** Configurable per request

---

## 20. Advanced Security

### 20.1. API Key Authentication

Header-based authentication with role support.

**Setup:**

```csharp
// Program.cs
builder.Services.AddApiKeyAuthentication();

// appsettings.json
{
  "Authentication": {
    "ApiKeys": [
      {
        "Key": "your-api-key-12345",
        "Owner": "MobileApp",
        "Roles": "User,Customer"
      },
      {
        "Key": "admin-key-67890",
        "Owner": "BackofficeAdmin",
        "Roles": "Admin,User"
      }
    ]
  }
}
```

**Usage:**

```csharp
[Authorize(AuthenticationSchemes = "ApiKey")]
[ApiController]
public class ProductsController : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "User")] // Requires User role
    public async Task<IActionResult> GetProducts() { }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")] // Requires Admin role
    public async Task<IActionResult> DeleteProduct(Guid id) { }
}

// Client Request
GET /api/products
Headers:
  X-API-Key: your-api-key-12345
```

**Features:**
- Header name: `X-API-Key`
- Claims-based authentication
- Role-based authorization
- API key masking in logs (shows first 4 chars)

---

### 20.2. Request/Response Logging

Comprehensive logging with automatic sensitive data masking.

**Setup:**

```csharp
// Program.cs
builder.Services.Configure<LoggingOptions>(builder.Configuration.GetSection("LoggingOptions"));

var app = builder.Build();
app.UseMiddleware<RequestResponseLoggingMiddleware>(); // Must be early in pipeline

// appsettings.json
{
  "LoggingOptions": {
    "EnableRequestResponseLogging": true,
    "MaxBodyLogSize": 4096,
    "LogRequestHeaders": true,
    "LogRequestBody": true,
    "LogResponseHeaders": true,
    "LogResponseBody": true,
    "SensitiveHeaders": ["Authorization", "X-API-Key", "Cookie"],
    "SensitiveBodyFields": ["password", "token", "secret", "apikey"]
  }
}
```

**Features:**
- Automatic header masking (Authorization, X-API-Key, Cookie)
- Automatic body field masking (password, token, secret, etc.)
- Configurable max body size
- JSON field detection and masking
- Response time tracking
- Content-type aware (only logs text-based content)

**Example Log Output:**

```
[INFO] HTTP GET /api/users/login
Request Headers: {"User-Agent": "PostmanRuntime/7.32.0", "Authorization": "***MASKED***"}
Request Body: {"username":"john","password":"***MASKED***","apiKey":"***MASKED***"}
Response Status: 200 OK
Response Body: {"success":true,"token":"***MASKED***","user":{...}}
Response Time: 45ms
```

---

## 21. Configuration - appsettings.json

**Complete configuration example:**

```json
{
  "ApplicationName": "MyApp",

  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyDb;Trusted_Connection=true;"
  },

  "Cors": {
    "AllowedOrigins": ["http://localhost:3000", "https://myapp.com"]
  },

  "ApiVersioning": {
    "Enabled": true,
    "DefaultVersion": "1.0",
    "ReportApiVersions": true,
    "AssumeDefaultVersionWhenUnspecified": true,
    "VersioningType": "UrlSegment"
  },

  "Swagger": {
    "Enabled": true,
    "Title": "My API",
    "Description": "My API Documentation",
    "Version": "v1",
    "RequireAuthorization": true,
    "EnvironmentRestriction": ["Development", "Staging"]
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

  "MemoryCache": {
    "SizeLimit": 1024,
    "CompactionPercentage": 0.25,
    "ExpirationScanFrequency": "00:01:00"
  },

  "OutputCache": {
    "Enabled": true,
    "DefaultExpirationSeconds": 60,
    "VaryByQuery": true,
    "VaryByHeader": false,
    "VaryByHeaderNames": []
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
    "RequestLimit": 100,
    "TimeWindowSeconds": 60
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
✅ **Behaviors:** CQRS (MediatR + FluentValidation + Mapster + Logging + Performance)
✅ **Infrastructure:** Repository Pattern, Unit of Work, Multi-Tenancy, Health Checks, Data Seeding
✅ **API:** Swagger/OpenAPI, API Versioning (URL/Query/Header), XML Documentation
✅ **Features:** Caching, Event Bus (RabbitMQ/Kafka/MassTransit), Storage, Search, Logging
✅ **Security:** JWT Auth, CORS, Permission Authorization, Rate Limiting, Password Hashing
✅ **Middleware:** Global Exception Handling with correct pipeline order

**With just 2 lines of setup!** 🚀

### 🆕 What's New in v4.6.0

- **Password Hashing Upgrade - Argon2id:**
  - Migrated from BCrypt to Argon2id (Password Hashing Competition winner 2015)
  - **5x FASTER** performance (115ms vs 634ms average)
  - **More secure** against GPU/ASIC attacks (high memory requirement)
  - OWASP recommended settings (m=19456, t=2, p=1)
  - Automatic backward compatibility with BCrypt hashes
  - Seamless migration on user login

### 🆕 What's New in v5.0.0 - MAJOR SECURITY AND ARCHITECTURE UPDATE

**🔒 Critical Security Fixes:**
- **AES-GCM Encryption with Random Nonce:** Fixed encryption vulnerability by using AES-GCM with cryptographically random nonce for each operation (previously used static IV with CBC mode)
- **Entity Equality Implementation:** Corrected GetHashCode() for transient entities to prevent hash collisions
- **JWT Secret Validation:** Added minimum 32-character validation for JWT secrets
- **Domain Events Transaction Safety:** Events now dispatched AFTER SaveChanges for transaction consistency

**🏗️ Repository & Database Enhancements:**
- **Pagination Support:** Added `GetPagedAsync()` with eager loading support to prevent memory issues
- **N+1 Query Prevention:** Include/ThenInclude support in all query methods
- **Soft Delete Global Filters:** Automatic filtering of soft-deleted entities via `ISoftDeletable`
- **ICurrentUserService:** Claims-based current user service for audit trails

**📦 New Enterprise Patterns:**
- **Transactional Outbox Pattern:** Ensures reliable event publishing with background processor
- **Repository Specification Pattern:** Reusable, composable query specifications with filtering, ordering, and pagination
- **Idempotency Pattern:** Prevents duplicate request processing with distributed caching
- **Circuit Breaker with Polly:** Resilient HTTP client with retry and circuit breaker policies

**🔐 Advanced Security Features:**
- **API Key Authentication:** Header-based authentication with role support
- **Request/Response Logging:** Comprehensive logging with automatic sensitive data masking (passwords, tokens, API keys)
- **Configuration Validation:** Startup validation for all configuration options

**🚀 Infrastructure Improvements:**
- **Redis Prefix Removal:** Efficient `RemoveByPrefixAsync()` implementation
- **Performance Middleware:** Automatic response time tracking with slow request logging
- **Code Quality:** Eliminated all magic strings, improved error handling, enhanced XML documentation

### 🆕 What's New in v4.6.0

- **Password Hashing Migration:** Migrated from BCrypt to Argon2id for enhanced security
  - More secure against GPU/ASIC attacks (high memory requirement)
  - OWASP recommended settings (m=19456, t=2, p=1)
  - Automatic backward compatibility with BCrypt hashes
  - Seamless migration on user login

---

## 📄 License

MIT License - See [LICENSE](LICENSE) for details.

---

## 📧 Support

For questions, please open an issue on [GitHub Issues](https://github.com/AdemKinatas/Marventa.Framework/issues).
