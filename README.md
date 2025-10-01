# Marventa.Framework v4.0.2

**Enterprise-grade .NET 8.0 & 9.0 framework for building scalable microservices with DDD, CQRS, and Event-Driven Architecture.**

[![NuGet](https://img.shields.io/nuget/v/Marventa.Framework.svg)](https://www.nuget.org/packages/Marventa.Framework/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## 🎯 Version 4.0.2 - Latest Release ⭐ RECOMMENDED

**Complete architectural redesign with single-package approach!**

> 💡 **We strongly recommend using v4.0.2** for all new projects. This version provides a unified, production-ready architecture with complete feature set, simplified dependency management, and superior performance compared to previous versions.

### Breaking Changes
- ⚠️ Complete restructure from multi-project to single-project architecture
- ⚠️ All features now in one unified package: `Marventa.Framework`
- ⚠️ Namespace changes for better organization
- ⚠️ Requires .NET 8.0 or .NET 9.0
- ⚠️ Migration required from v3.x and earlier versions

### What's New in v4.0.2
- 📚 **Enhanced Documentation** - Comprehensive usage examples for all features
- 📚 **API Features Examples** - Detailed examples for validation, error handling, Swagger, versioning, CORS
- 📚 **Security Examples** - JWT authentication, permission-based authorization, password hashing, AES encryption
- 📚 **Result Pattern Examples** - Type-safe error handling patterns
- 📚 **User-Friendly Guide** - Real-world code examples for better developer experience

### Features from v4.0.x
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
- ✅ **CORS** - Cross-Origin Resource Sharing

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
// Program.cs - Add JWT authentication
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

// Login endpoint - Generate JWT token
[HttpPost("login")]
public async Task<IActionResult> Login(LoginRequest request)
{
    var user = await _userRepository.GetByEmailAsync(request.Email);
    if (user == null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
    {
        return Unauthorized(new { message = "Invalid credentials" });
    }

    var token = _jwtTokenGenerator.GenerateToken(
        userId: user.Id.ToString(),
        email: user.Email,
        roles: user.Roles,
        claims: new Dictionary<string, string>
        {
            { "TenantId", user.TenantId.ToString() },
            { "FullName", user.FullName }
        }
    );

    return Ok(new { token, expiresIn = 3600 });
}

// Protected endpoint - Require authentication
[Authorize]
[HttpGet("profile")]
public async Task<IActionResult> GetProfile()
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
    return Ok(user);
}

// Permission-based authorization
[Authorize]
[RequirePermission("products.create")]
[HttpPost("products")]
public async Task<IActionResult> CreateProduct(CreateProductCommand command)
{
    var result = await _mediator.Send(command);
    return Ok(result);
}

// Multiple permissions (user must have ALL)
[Authorize]
[RequirePermission("orders.view", "orders.edit")]
[HttpPut("orders/{id}")]
public async Task<IActionResult> UpdateOrder(Guid id, UpdateOrderCommand command)
{
    var result = await _mediator.Send(command);
    return Ok(result);
}

// Role-based authorization
[Authorize(Roles = "Admin,Manager")]
[HttpDelete("products/{id}")]
public async Task<IActionResult> DeleteProduct(Guid id)
{
    await _productService.DeleteAsync(id);
    return NoContent();
}

// Custom authorization in services
public class OrderService
{
    private readonly IPermissionService _permissionService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public async Task<Result<Order>> CancelOrderAsync(Guid orderId)
    {
        var userId = _httpContextAccessor.HttpContext.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Check if user has permission
        if (!await _permissionService.HasPermissionAsync(userId, "orders.cancel"))
        {
            return Result.Failure<Order>("Insufficient permissions", "FORBIDDEN");
        }

        // Business logic...
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

### **Result Pattern**

```csharp
// Result Pattern provides type-safe error handling without exceptions

// Success result
public async Task<Result<Order>> CreateOrderAsync(CreateOrderDto dto)
{
    var order = new Order { /* ... */ };
    await _repository.AddAsync(order);
    await _unitOfWork.SaveChangesAsync();

    return Result.Success(order);
}

// Failure result with error message and code
public async Task<Result<Product>> GetProductAsync(Guid id)
{
    var product = await _repository.GetByIdAsync(id);
    if (product == null)
    {
        return Result.Failure<Product>("Product not found", "NOT_FOUND");
    }

    return Result.Success(product);
}

// Multiple validation errors
public async Task<Result<User>> RegisterUserAsync(RegisterDto dto)
{
    var errors = new List<string>();

    if (await _userRepository.ExistsAsync(u => u.Email == dto.Email))
        errors.Add("Email already exists");

    if (dto.Password.Length < 8)
        errors.Add("Password must be at least 8 characters");

    if (errors.Any())
    {
        return Result.Failure<User>(
            string.Join(", ", errors),
            "VALIDATION_ERROR"
        );
    }

    var user = new User { /* ... */ };
    await _repository.AddAsync(user);

    return Result.Success(user);
}

// Handling results in controllers
[HttpPost]
public async Task<IActionResult> CreateOrder(CreateOrderCommand command)
{
    var result = await _mediator.Send(command);

    if (!result.IsSuccess)
    {
        // Log the error
        _logger.LogWarning("Order creation failed: {Error}", result.ErrorMessage);

        // Return appropriate response
        var response = ApiResponseFactory.FromResult(result);
        return result.ErrorCode switch
        {
            "NOT_FOUND" => NotFound(response),
            "VALIDATION_ERROR" => BadRequest(response),
            "UNAUTHORIZED" => Unauthorized(response),
            _ => StatusCode(500, response)
        };
    }

    return Ok(ApiResponseFactory.FromResult(result));
}

// Chaining results
public async Task<Result<OrderConfirmation>> ProcessOrderAsync(Guid orderId)
{
    var orderResult = await GetOrderAsync(orderId);
    if (!orderResult.IsSuccess)
        return Result.Failure<OrderConfirmation>(orderResult.ErrorMessage, orderResult.ErrorCode);

    var paymentResult = await ProcessPaymentAsync(orderResult.Data);
    if (!paymentResult.IsSuccess)
        return Result.Failure<OrderConfirmation>(paymentResult.ErrorMessage, paymentResult.ErrorCode);

    var confirmation = new OrderConfirmation { /* ... */ };
    return Result.Success(confirmation);
}
```

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

### **Password Hashing & Encryption**

```csharp
// Password Hashing with BCrypt
public class UserService
{
    // Hash password during registration
    public async Task<Result<User>> RegisterAsync(RegisterDto dto)
    {
        var passwordHash = PasswordHasher.Hash(dto.Password);

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = passwordHash,
            FullName = dto.FullName
        };

        await _repository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success(user);
    }

    // Verify password during login
    public async Task<Result<string>> LoginAsync(LoginDto dto)
    {
        var user = await _repository.GetByEmailAsync(dto.Email);
        if (user == null)
        {
            return Result.Failure<string>("Invalid credentials", "UNAUTHORIZED");
        }

        // Verify password
        if (!PasswordHasher.Verify(dto.Password, user.PasswordHash))
        {
            return Result.Failure<string>("Invalid credentials", "UNAUTHORIZED");
        }

        var token = _jwtTokenGenerator.GenerateToken(user.Id.ToString(), user.Email);
        return Result.Success(token);
    }

    // Change password
    public async Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
    {
        var user = await _repository.GetByIdAsync(userId);

        // Verify current password
        if (!PasswordHasher.Verify(dto.CurrentPassword, user.PasswordHash))
        {
            return Result.Failure("Current password is incorrect", "VALIDATION_ERROR");
        }

        // Hash and save new password
        user.PasswordHash = PasswordHasher.Hash(dto.NewPassword);
        await _repository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}

// AES Encryption for sensitive data
public class PaymentService
{
    private readonly string _encryptionKey = "your-32-character-encryption-key!!"; // Store in appsettings

    // Encrypt credit card number before storing
    public async Task SavePaymentMethodAsync(PaymentMethodDto dto)
    {
        var encryptedCardNumber = AesEncryption.Encrypt(dto.CardNumber, _encryptionKey);

        var paymentMethod = new PaymentMethod
        {
            UserId = dto.UserId,
            CardNumberEncrypted = encryptedCardNumber,
            ExpiryDate = dto.ExpiryDate
        };

        await _repository.AddAsync(paymentMethod);
    }

    // Decrypt when needed (e.g., for payment processing)
    public async Task<string> GetDecryptedCardNumberAsync(Guid paymentMethodId)
    {
        var paymentMethod = await _repository.GetByIdAsync(paymentMethodId);
        return AesEncryption.Decrypt(paymentMethod.CardNumberEncrypted, _encryptionKey);
    }

    // Encrypt sensitive configuration data
    public void StoreApiKey(string apiKey)
    {
        var encrypted = AesEncryption.Encrypt(apiKey, _encryptionKey);
        // Store encrypted value in database
    }

    public string GetApiKey(string encryptedApiKey)
    {
        return AesEncryption.Decrypt(encryptedApiKey, _encryptionKey);
    }
}

// Best practices
public class SecurityBestPractices
{
    // ✅ DO: Hash passwords (one-way)
    var passwordHash = PasswordHasher.Hash(password); // Cannot be decrypted

    // ✅ DO: Encrypt sensitive data that needs to be retrieved (two-way)
    var encrypted = AesEncryption.Encrypt(creditCard, key); // Can be decrypted
    var decrypted = AesEncryption.Decrypt(encrypted, key);

    // ❌ DON'T: Store passwords in plain text
    // ❌ DON'T: Store encryption keys in code (use appsettings, Azure Key Vault, etc.)
    // ❌ DON'T: Use weak passwords or short encryption keys
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

### **API Response Examples**

```csharp
// Standardized API Response
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorCode { get; set; }
    public string? Message { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
}

// Usage in Controllers
[HttpPost]
public async Task<IActionResult> Create(CreateProductCommand command)
{
    var result = await _mediator.Send(command);
    var response = ApiResponseFactory.FromResult(result);
    return result.IsSuccess ? Ok(response) : BadRequest(response);
}

// Success Response (200 OK)
{
  "success": true,
  "data": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "message": null,
  "errorCode": null,
  "errors": null
}

// Error Response (400 Bad Request)
{
  "success": false,
  "data": null,
  "message": "Product not found",
  "errorCode": "NOT_FOUND",
  "errors": null
}
```

### **FluentValidation Usage**

```csharp
// Define validation rules
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");
    }
}

// Register validators in Program.cs
builder.Services.AddMarventaValidation(typeof(Program).Assembly);

// Validation happens automatically via MediatR pipeline
// Validation errors return 400 Bad Request with detailed error messages:
{
  "success": false,
  "data": null,
  "message": "Validation failed",
  "errorCode": "VALIDATION_ERROR",
  "errors": {
    "Name": ["Product name is required"],
    "Price": ["Price must be greater than zero"]
  }
}
```

### **Global Exception Handling**

```csharp
// UseMarventaFramework() automatically adds global exception handling
app.UseMarventaFramework(app.Environment);

// All unhandled exceptions are caught and returned as standardized responses:
// Development Environment - Detailed error info
{
  "success": false,
  "data": null,
  "message": "An error occurred: Division by zero",
  "errorCode": "INTERNAL_ERROR",
  "errors": {
    "StackTrace": ["at MyApp.Services.CalculationService..."]
  }
}

// Production Environment - Generic error message
{
  "success": false,
  "data": null,
  "message": "An internal error occurred",
  "errorCode": "INTERNAL_ERROR",
  "errors": null
}
```

### **Swagger/OpenAPI Configuration**

```csharp
// Add Swagger with JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
        Description = "API documentation with Marventa Framework"
    });

    // JWT Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Enable Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

// Access at: https://localhost:5001/swagger
```

### **API Versioning**

```csharp
// Configure API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"),
        new QueryStringApiVersionReader("api-version")
    );
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Version 1 Controller
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsV1Controller : ControllerBase
{
    [HttpGet]
    public IActionResult GetProducts()
    {
        return Ok(new { version = "1.0", products = new[] { "Product1", "Product2" } });
    }
}

// Version 2 Controller
[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsV2Controller : ControllerBase
{
    [HttpGet]
    public IActionResult GetProducts()
    {
        return Ok(new { version = "2.0", products = new[] { "Product1", "Product2", "Product3" } });
    }
}

// Access endpoints:
// GET /api/v1/products
// GET /api/v2/products
// GET /api/products?api-version=1.0
// GET /api/products (with header: X-Api-Version: 2.0)
```

### **CORS Configuration**

```csharp
// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });

    options.AddPolicy("AllowSpecific", builder =>
    {
        builder.WithOrigins("https://example.com", "https://app.example.com")
               .WithMethods("GET", "POST", "PUT", "DELETE")
               .WithHeaders("Content-Type", "Authorization")
               .AllowCredentials();
    });
});

// Enable CORS
app.UseCors("AllowSpecific");
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
