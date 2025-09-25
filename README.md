# Marventa.Framework v2.0.0

A comprehensive .NET 9.0 enterprise e-commerce framework following Clean Architecture and SOLID principles with multi-tenancy, JWT authentication, CQRS, Saga patterns, messaging infrastructure, and complete e-commerce domain modules.

## ✨ What's New in v2.0.0 - SOLID Compliant Enterprise Framework

### 🏗️ **SOLID Principles Architecture**
- **Single Responsibility** - Each class/interface has one focused responsibility
- **Open/Closed** - Extensible design without modifying existing code
- **Liskov Substitution** - Proper inheritance and polymorphism
- **Interface Segregation** - Small, focused interfaces
- **Dependency Inversion** - Proper dependency injection patterns

### 🏢 **Enterprise Features**
- ✅ **Multi-Tenancy** - Complete tenant isolation with policy-based authorization
- ✅ **Saga/Process Manager** - MassTransit state machine orchestration
- ✅ **Outbox/Inbox Patterns** - Reliable messaging with eventual consistency
- ✅ **Read Model Projections** - MSSQL → MongoDB projections
- ✅ **Domain Events** - Rich domain event system with integration events
- ✅ **Money/Currency** - Value objects with multi-currency support
- ✅ **HTTP Idempotency** - Correlation tracking for safe retries
- ✅ **OpenTelemetry** - Complete observability and tracing
- ✅ **Background Jobs** - Hangfire/Quartz integration
- ✅ **Search & Analytics** - Elasticsearch/ClickHouse abstractions
- ✅ **Storage Abstraction** - S3/Azure Blob/GCS unified interface
- ✅ **Security Hardening** - Data Protection, CSP, and audit logging
- ✅ **FluentValidation** - RFC 7807 Problem Details pipeline

### 🛒 **E-Commerce Domain Modules**
- ✅ **Payment Aggregate** - Payment processing with domain events
- ✅ **Shipping Aggregate** - Complete shipping lifecycle management
- ✅ **Order Saga** - End-to-end order orchestration
- ✅ **Cargo Domain** - Logistics and tracking
- ✅ **Fraud Detection** - Risk assessment patterns

### 🔧 **Core Features**
- ✅ **Clean Architecture** - Proper separation of concerns with Core, Domain, Application, Infrastructure, and Web layers
- ✅ **JWT Authentication** - Complete token-based authentication and authorization
- ✅ **CQRS Pattern** - Command Query Responsibility Segregation with MediatR
- ✅ **Tenant-Scoped Caching** - Multi-tenant Redis/Memory caching
- ✅ **Rate Limiting** - Tenant-aware rate limiting middleware
- ✅ **Health Checks** - Database and cache health monitoring
- ✅ **API Versioning** - Multiple versioning strategies support
- ✅ **Exception Handling** - Global exception handling middleware
- ✅ **Repository Pattern** - Generic repository with Unit of Work
- ✅ **Security** - Encryption services and secure token management
- ✅ **Communication** - Email and SMS services
- ✅ **HTTP Client** - Circuit breaker pattern implementation
- ✅ **Feature Flags** - Dynamic feature toggle support
- ✅ **Logging** - Comprehensive logging infrastructure
- ✅ **Messaging** - RabbitMQ+MassTransit and Kafka message queue infrastructure

## 🏗️ SOLID Architecture

The framework strictly follows SOLID principles with proper separation of concerns:

### 📁 Project Structure
```
Marventa.Framework/
├── 📁 Marventa.Framework.Core/           # Core abstractions and interfaces
│   ├── 📁 Events/                        # Domain & Integration event contracts
│   │   ├── IDomainEvent.cs              # Domain event interface
│   │   ├── IIntegrationEvent.cs         # Integration event interface
│   │   ├── IDomainEventHandler.cs       # Domain event handler
│   │   └── IEventBus.cs                 # Event bus abstraction
│   ├── 📁 Interfaces/                   # Core service contracts
│   │   ├── ITenant.cs                   # Tenant entity contract
│   │   ├── ITenantContext.cs            # Tenant context operations
│   │   ├── ITenantResolver.cs           # Tenant resolution logic
│   │   └── ITenantStore.cs              # Tenant persistence
│   └── 📁 Security/                     # Security abstractions
│       ├── ITokenService.cs             # Token service contract
│       └── TokenInfo.cs                 # Token data structure
│
├── 📁 Marventa.Framework.Domain/         # Domain entities and business logic
│   ├── 📁 ECommerce/                    # E-commerce domain modules
│   │   ├── Payment/PaymentAggregate.cs  # Payment domain logic
│   │   └── Shipping/ShippingAggregate.cs # Shipping domain logic
│   ├── 📁 Entities/                     # Domain entities
│   │   ├── OutboxMessage.cs             # Outbox pattern implementation
│   │   └── InboxMessage.cs              # Inbox pattern implementation
│   └── 📁 Events/                       # Domain event implementations
│       └── DomainEvent.cs               # Base domain event class
│
├── 📁 Marventa.Framework.Application/    # Application services and CQRS
│   ├── 📁 Commands/                     # Command pattern
│   │   ├── ICommand.cs                  # Command contracts
│   │   └── ICommandHandler.cs           # Command handlers
│   └── 📁 Queries/                      # Query pattern
│       ├── IQuery.cs                    # Query contracts
│       └── IQueryHandler.cs             # Query handlers
│
├── 📁 Marventa.Framework.Infrastructure/ # External concerns implementation
│   ├── 📁 Caching/                      # Caching implementations
│   │   ├── ITenantScopedCache.cs       # Tenant cache contract
│   │   ├── TenantScopedCache.cs        # Tenant cache implementation
│   │   └── TenantCacheOptions.cs       # Cache configuration
│   ├── 📁 Sagas/                       # Process manager implementations
│   └── 📁 Messaging/                   # Messaging infrastructure
│
└── 📁 Marventa.Framework.Web/           # Web layer and middleware
    ├── 📁 RateLimiting/                # Rate limiting middleware
    │   ├── RateLimitingMiddleware.cs   # Rate limit implementation
    │   └── RateLimitOptions.cs         # Rate limit configuration
    └── 📁 Versioning/                  # API versioning
        ├── ApiVersioningOptions.cs    # Versioning configuration
        └── ApiVersionReader.cs         # Version reader enum
```

### 🎯 SOLID Principles Applied

#### 🔹 Single Responsibility Principle (SRP)
- Each class/interface has **ONE** focused responsibility
- Interfaces separated by concern (ITenant, ITenantContext, ITenantStore)
- Commands and Queries have separate handlers
- Configuration classes separated from implementation

#### 🔹 Open/Closed Principle (OCP)
- Framework is **open for extension**, **closed for modification**
- Plugin architecture for custom implementations
- Strategy pattern for different tenant resolution methods
- Extensible event system

#### 🔹 Liskov Substitution Principle (LSP)
- All implementations are fully substitutable
- Proper inheritance hierarchies (BaseEntity, DomainEvent)
- Interface contracts respected by all implementations

#### 🔹 Interface Segregation Principle (ISP)
- **Small, focused interfaces** instead of large ones
- ITenant, ITenantContext, ITenantResolver are separate
- Command/Query handlers separated
- Event interfaces segregated by responsibility

#### 🔹 Dependency Inversion Principle (DIP)
- **Depend on abstractions, not concretions**
- All dependencies injected via interfaces
- Infrastructure depends on Core abstractions
- Web layer depends on Application contracts

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package Marventa.Framework
```

Or via Package Manager Console:

```powershell
Install-Package Marventa.Framework
```

## Quick Start

Add Marventa Framework to your ASP.NET Core application:

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

## Configuration

Configure framework options in your `appsettings.json`:

```json
{
  "JWT": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "your-issuer",
    "Audience": "your-audience",
    "ExpiryInMinutes": 60
  },
  "RateLimit": {
    "EnableRateLimiting": true,
    "MaxRequests": 100,
    "WindowSizeInMinutes": 1
  },
  "ApiVersioning": {
    "DefaultVersion": "1.0",
    "Strategy": "Header"
  }
}
```

## Detailed Usage Guide

### 🔐 JWT Authentication & Authorization

#### Basic Setup
```csharp
// Configure JWT settings in appsettings.json
{
  "JWT": {
    "SecretKey": "your-super-secret-key-at-least-32-characters-long",
    "Issuer": "your-app-name",
    "Audience": "your-app-users",
    "ExpiryInMinutes": 60,
    "RefreshTokenExpiryInDays": 7
  }
}

// Enable JWT Authentication
builder.Services.AddMarventaJwtAuthentication(builder.Configuration);
app.UseMarventaJwtAuthentication();
```

#### JWT Token Management
```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly ICurrentUserService _currentUser;

    public AuthController(ITokenService tokenService, ICurrentUserService currentUser)
    {
        _tokenService = tokenService;
        _currentUser = currentUser;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        // Your authentication logic here
        var user = await AuthenticateUser(request);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = await _tokenService.GenerateAccessTokenAsync(claims);
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id.ToString());

        return Ok(new
        {
            AccessToken = token,
            RefreshToken = refreshToken,
            ExpiresIn = 3600,
            TokenType = "Bearer"
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
    {
        var principal = await _tokenService.ValidateTokenAsync(request.Token);
        if (principal == null)
            return Unauthorized();

        // Generate new tokens
        var newToken = await _tokenService.GenerateAccessTokenAsync(principal.Claims);
        return Ok(new { AccessToken = newToken });
    }

    [HttpGet("profile")]
    [Authorize]
    public IActionResult GetProfile()
    {
        return Ok(new
        {
            UserId = _currentUser.UserId,
            Username = _currentUser.UserName,
            Email = _currentUser.Email,
            Roles = _currentUser.Roles,
            IsAuthenticated = _currentUser.IsAuthenticated
        });
    }
}
```

### 🔑 API Key Authentication
```csharp
// Configure API Key
builder.Services.AddMarventaApiKey(options =>
{
    options.ApiKeyHeaderName = "X-API-Key";
    options.AllowedApiKeys = new[] { "your-api-key-here" };
});

app.UseMarventaApiKey();

// Protect endpoints with API Key
[ApiKey]
[HttpGet("protected")]
public IActionResult ProtectedEndpoint()
{
    return Ok("This endpoint requires API key");
}
```

### 🗃️ Repository Pattern & Unit of Work

#### Entity Definition
```csharp
public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public List<Order> Orders { get; set; } = new();
}
```

#### Service Implementation
```csharp
public class UserService
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<User> CreateUserAsync(CreateUserRequest request)
    {
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = HashPassword(request.Password)
        };

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("User created: {UserId}", user.Id);
        return user;
    }

    public async Task<PagedResult<User>> GetUsersAsync(int page = 1, int pageSize = 10)
    {
        return await _userRepository.GetPagedAsync(
            page: page,
            pageSize: pageSize,
            predicate: u => u.IsActive,
            orderBy: u => u.CreatedDate,
            include: u => u.Orders
        );
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _userRepository.GetByExpressionAsync(u => u.Email == email);
    }

    public async Task<bool> UpdateUserAsync(Guid userId, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        user.Username = request.Username;
        user.Email = request.Email;
        user.UpdatedDate = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> SoftDeleteUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        await _userRepository.SoftDeleteAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
```

### ⚡ CQRS Pattern

#### Commands and Queries
```csharp
// Command
public record CreateUserCommand(string Username, string Email, string Password) : ICommand<User>;

public record UpdateUserCommand(Guid UserId, string Username, string Email) : ICommand<bool>;

// Query
public record GetUserByIdQuery(Guid UserId) : IQuery<User?>;

public record GetUsersQuery(int Page = 1, int PageSize = 10, string? SearchTerm = null) : IQuery<PagedResult<User>>;
```

#### Command Handlers
```csharp
public class CreateUserHandler : ICommandHandler<CreateUserCommand, User>
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEncryptionService _encryption;

    public CreateUserHandler(
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork,
        IEncryptionService encryption)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _encryption = encryption;
    }

    public async Task<User> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Username = command.Username,
            Email = command.Email,
            PasswordHash = _encryption.HashPassword(command.Password)
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user;
    }
}
```

#### Query Handlers
```csharp
public class GetUserByIdHandler : IQueryHandler<GetUserByIdQuery, User?>
{
    private readonly IRepository<User> _userRepository;

    public GetUserByIdHandler(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        return await _userRepository.GetByIdAsync(query.UserId, cancellationToken);
    }
}
```

#### Controller Usage
```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserCommand command)
    {
        var user = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await _mediator.Send(new GetUserByIdQuery(id));
        return user == null ? NotFound() : Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, UpdateUserCommand command)
    {
        if (id != command.UserId)
            return BadRequest();

        var success = await _mediator.Send(command);
        return success ? NoContent() : NotFound();
    }
}
```

### 🚀 Caching

#### Memory Cache Usage
```csharp
public class ProductService
{
    private readonly ICacheService _cache;
    private readonly IRepository<Product> _productRepository;

    public ProductService(ICacheService cache, IRepository<Product> productRepository)
    {
        _cache = cache;
        _productRepository = productRepository;
    }

    public async Task<Product?> GetProductAsync(Guid productId)
    {
        var cacheKey = $"product:{productId}";
        var cachedProduct = await _cache.GetAsync<Product>(cacheKey);

        if (cachedProduct != null)
            return cachedProduct;

        var product = await _productRepository.GetByIdAsync(productId);
        if (product != null)
        {
            await _cache.SetAsync(cacheKey, product, TimeSpan.FromMinutes(30));
        }

        return product;
    }

    public async Task<List<Product>> GetFeaturedProductsAsync()
    {
        const string cacheKey = "products:featured";
        var cachedProducts = await _cache.GetAsync<List<Product>>(cacheKey);

        if (cachedProducts != null)
            return cachedProducts;

        var products = await _productRepository.GetManyAsync(p => p.IsFeatured);
        await _cache.SetAsync(cacheKey, products, TimeSpan.FromHours(1));

        return products.ToList();
    }

    public async Task InvalidateProductCacheAsync(Guid productId)
    {
        await _cache.RemoveAsync($"product:{productId}");
        await _cache.RemoveAsync("products:featured");
    }
}
```

### 🛡️ Rate Limiting
```csharp
// Configure rate limiting
builder.Services.AddMarventaRateLimiting(options =>
{
    options.EnableRateLimiting = true;
    options.MaxRequests = 100;
    options.WindowSize = TimeSpan.FromMinutes(1);
});

app.UseMarventaRateLimiting();

// Custom rate limiting per endpoint
[HttpGet]
[RateLimit(MaxRequests = 10, WindowSizeInMinutes = 1)]
public IActionResult GetSensitiveData()
{
    return Ok("This endpoint has stricter rate limits");
}
```

### 🔄 API Versioning
```csharp
// Configure API versioning
builder.Services.AddMarventaApiVersioning(options =>
{
    options.DefaultVersion = "1.0";
    options.Strategy = VersioningStrategy.Header; // Header, Query, MediaType, Url
    options.HeaderName = "Api-Version";
});

app.UseMarventaApiVersioning();

// Version-specific controllers
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    [MapToApiVersion("1.0")]
    public IActionResult GetV1()
    {
        return Ok("Version 1.0 response");
    }

    [HttpGet]
    [MapToApiVersion("2.0")]
    public IActionResult GetV2()
    {
        return Ok("Version 2.0 response with new features");
    }
}
```

### 🔒 Encryption Services
```csharp
public class UserService
{
    private readonly IEncryptionService _encryption;

    public UserService(IEncryptionService encryption)
    {
        _encryption = encryption;
    }

    public async Task<User> CreateUserAsync(CreateUserRequest request)
    {
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _encryption.HashPassword(request.Password),
            // Encrypt sensitive data
            SocialSecurityNumber = _encryption.Encrypt(request.SSN),
            CreditCardNumber = _encryption.Encrypt(request.CreditCard)
        };

        return user;
    }

    public async Task<string> GetDecryptedSSNAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return _encryption.Decrypt(user.SocialSecurityNumber);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return _encryption.VerifyPassword(password, hash);
    }
}
```

### 📧 Email & SMS Services
```csharp
public class NotificationService
{
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;

    public NotificationService(IEmailService emailService, ISmsService smsService)
    {
        _emailService = emailService;
        _smsService = smsService;
    }

    public async Task SendWelcomeEmailAsync(User user)
    {
        var emailRequest = new EmailRequest
        {
            To = new[] { user.Email },
            Subject = "Welcome to Our Platform!",
            Body = $"Hello {user.Username}, welcome to our platform!",
            IsHtml = true
        };

        await _emailService.SendEmailAsync(emailRequest);
    }

    public async Task SendSmsVerificationAsync(string phoneNumber, string code)
    {
        var smsRequest = new SmsRequest
        {
            PhoneNumber = phoneNumber,
            Message = $"Your verification code is: {code}"
        };

        await _smsService.SendSmsAsync(smsRequest);
    }

    public async Task SendBulkEmailAsync(List<User> users, string subject, string body)
    {
        var emailRequest = new EmailRequest
        {
            To = users.Select(u => u.Email).ToArray(),
            Subject = subject,
            Body = body,
            IsHtml = true
        };

        await _emailService.SendBulkEmailAsync(emailRequest);
    }
}
```

### 🌐 HTTP Client with Circuit Breaker
```csharp
public class ExternalApiService
{
    private readonly IHttpClientService _httpClient;
    private readonly ILogger<ExternalApiService> _logger;

    public ExternalApiService(IHttpClientService httpClient, ILogger<ExternalApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<T?> GetDataFromExternalApiAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync($"https://api.external.com/{endpoint}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json);
            }
        }
        catch (CircuitBreakerOpenException)
        {
            _logger.LogWarning("Circuit breaker is open for external API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling external API");
        }

        return default;
    }

    public async Task<bool> PostDataAsync<T>(string endpoint, T data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"https://api.external.com/{endpoint}", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error posting to external API");
            return false;
        }
    }
}
```

### 🏥 Health Checks
```csharp
// Configure health checks
builder.Services.AddMarventaHealthChecks(builder.Configuration);

app.UseMarventaHealthChecks(); // Adds /health endpoint

// Custom health check
public class CustomHealthCheck : IHealthCheck
{
    private readonly IHttpClientService _httpClient;

    public CustomHealthCheck(IHttpClientService httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("https://api.dependency.com/health");
            return response.IsSuccessStatusCode
                ? HealthCheckResult.Healthy("External API is healthy")
                : HealthCheckResult.Unhealthy("External API is down");
        }
        catch
        {
            return HealthCheckResult.Unhealthy("Cannot reach external API");
        }
    }
}
```

### 🚩 Feature Flags
```csharp
public class OrderService
{
    private readonly IFeatureFlagService _featureFlags;
    private readonly IRepository<Order> _orderRepository;

    public OrderService(IFeatureFlagService featureFlags, IRepository<Order> orderRepository)
    {
        _featureFlags = featureFlags;
        _orderRepository = orderRepository;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        var order = new Order
        {
            UserId = request.UserId,
            Items = request.Items,
            Total = request.Items.Sum(i => i.Price * i.Quantity)
        };

        // Check feature flag for new discount system
        if (await _featureFlags.IsEnabledAsync("NewDiscountSystem"))
        {
            order.Discount = CalculateNewDiscount(order);
        }
        else
        {
            order.Discount = CalculateOldDiscount(order);
        }

        // Check feature flag for free shipping
        if (await _featureFlags.IsEnabledAsync("FreeShipping", request.UserId))
        {
            order.ShippingCost = 0;
        }

        await _orderRepository.AddAsync(order);
        return order;
    }

    public async Task<List<Order>> GetOrdersAsync(Guid userId)
    {
        var orders = await _orderRepository.GetManyAsync(o => o.UserId == userId);

        // Feature flag for enhanced order details
        if (await _featureFlags.IsEnabledAsync("EnhancedOrderDetails"))
        {
            foreach (var order in orders)
            {
                // Load additional details
                order.TrackingInfo = await GetTrackingInfoAsync(order.Id);
                order.EstimatedDelivery = CalculateEstimatedDelivery(order);
            }
        }

        return orders.ToList();
    }
}
```

### 📨 Messaging Infrastructure

The framework provides a unified messaging infrastructure supporting both RabbitMQ+MassTransit and Apache Kafka through a common `IMessageBus` interface.

#### Core Interfaces
```csharp
// Core messaging interface
public interface IMessageBus
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
    Task SendAsync<T>(T command, CancellationToken cancellationToken = default) where T : class;
    Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class where TResponse : class;
}

// Message handler interface
public interface IMessageHandler<in T> where T : class
{
    Task Handle(T message, CancellationToken cancellationToken = default);
}
```

#### Message Base Classes
```csharp
// Base event class
public abstract record BaseEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public string EventType { get; init; } = string.Empty;
    public Dictionary<string, object> Metadata { get; init; } = new();
}

// Base command class
public abstract record BaseCommand
{
    public Guid CommandId { get; init; } = Guid.NewGuid();
    public DateTime IssuedAt { get; init; } = DateTime.UtcNow;
    public string CommandType { get; init; } = string.Empty;
    public Dictionary<string, object> Metadata { get; init; } = new();
}

// Example implementations
public record UserCreatedEvent(Guid UserId, string Username, string Email) : BaseEvent
{
    public UserCreatedEvent() : this(Guid.Empty, string.Empty, string.Empty) { }
}

public record CreateOrderCommand(Guid UserId, List<OrderItem> Items) : BaseCommand
{
    public CreateOrderCommand() : this(Guid.Empty, new List<OrderItem>()) { }
}
```

#### RabbitMQ + MassTransit Setup

##### Configuration
```json
{
  "ConnectionStrings": {
    "RabbitMQ": "amqp://guest:guest@localhost:5672/"
  },
  "Messaging": {
    "ServiceName": "my-service"
  }
}
```

##### Basic Setup
```csharp
// Program.cs - Basic setup with configuration
builder.Services.AddMarventaRabbitMQ(builder.Configuration);

// Or with explicit parameters
builder.Services.AddMarventaRabbitMQ(
    connectionString: "amqp://guest:guest@localhost:5672/",
    serviceName: "my-service",
    assemblies: typeof(Program).Assembly
);

// For testing - In-memory transport
builder.Services.AddMarventaInMemoryMessaging(typeof(Program).Assembly);
```

##### Advanced Configuration
```csharp
// Advanced RabbitMQ configuration
builder.Services.AddMarventaRabbitMQ(
    configure: x =>
    {
        // Add consumers from multiple assemblies
        x.AddConsumers(typeof(Program).Assembly, typeof(OrderHandler).Assembly);

        // Add specific consumers
        x.AddConsumer<UserCreatedEventHandler>();
        x.AddConsumer<OrderProcessingHandler>();
    },
    configureRabbitMq: cfg =>
    {
        // Custom endpoint configuration
        cfg.ReceiveEndpoint("user-events", ep =>
        {
            ep.ConfigureConsumer<UserCreatedEventHandler>(context);
            ep.UseMessageRetry(r => r.Exponential(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2)));
        });

        // Custom message serialization
        cfg.UseJsonSerializer();

        // Advanced retry policies
        cfg.UseDelayedRedelivery(r => r.Intervals(
            TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(15),
            TimeSpan.FromMinutes(30)
        ));
    }
);
```

##### Message Consumers
```csharp
// MassTransit Consumer
public class UserCreatedEventHandler : IConsumer<UserCreatedEvent>
{
    private readonly ILogger<UserCreatedEventHandler> _logger;
    private readonly IEmailService _emailService;

    public UserCreatedEventHandler(ILogger<UserCreatedEventHandler> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        var userEvent = context.Message;
        _logger.LogInformation("Processing UserCreatedEvent for user: {UserId}", userEvent.UserId);

        // Send welcome email
        await _emailService.SendWelcomeEmailAsync(userEvent.Email, userEvent.Username);

        _logger.LogInformation("UserCreatedEvent processed successfully for user: {UserId}", userEvent.UserId);
    }
}

// Order processing with retry and error handling
public class OrderProcessingHandler : IConsumer<CreateOrderCommand>
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderProcessingHandler> _logger;

    public OrderProcessingHandler(IOrderService orderService, ILogger<OrderProcessingHandler> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CreateOrderCommand> context)
    {
        try
        {
            var command = context.Message;
            _logger.LogInformation("Processing order for user: {UserId}", command.UserId);

            var order = await _orderService.CreateOrderAsync(command);

            // Publish order created event
            await context.Publish(new OrderCreatedEvent(order.Id, command.UserId, order.Total));

            _logger.LogInformation("Order created successfully: {OrderId}", order.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process order for user: {UserId}", context.Message.UserId);
            throw; // Let MassTransit handle retry/error policies
        }
    }
}
```

#### Apache Kafka Setup

##### Configuration
```json
{
  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "GroupId": "my-service-consumers",
    "TopicPrefix": "my-app-",
    "TopicMappings": {
      "UserCreatedEvent": "user-events",
      "OrderCreatedEvent": "order-events"
    },
    "AutoOffsetReset": "earliest",
    "EnableAutoCommit": false,
    "SessionTimeoutMs": 6000,
    "SecurityProtocol": "SaslSsl",
    "SaslMechanism": "Plain",
    "SaslUsername": "your-username",
    "SaslPassword": "your-password"
  }
}
```

##### Basic Setup
```csharp
// Program.cs - Basic Kafka setup
builder.Services.AddMarventaKafka(builder.Configuration);

// Or with explicit configuration
builder.Services.AddMarventaKafka(options =>
{
    options.BootstrapServers = "localhost:9092";
    options.GroupId = "my-service-consumers";
    options.TopicPrefix = "my-app-";
    options.EnableAutoCommit = false;
});
```

##### Kafka Message Handlers
```csharp
// Kafka message handler
public class UserEventKafkaHandler : BaseKafkaHandler<UserCreatedEvent>
{
    private readonly IEmailService _emailService;

    public UserEventKafkaHandler(
        IOptions<KafkaOptions> options,
        ILogger<UserEventKafkaHandler> logger,
        IEmailService emailService) : base(options, logger)
    {
        _emailService = emailService;
    }

    public override async Task Handle(UserCreatedEvent message, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Processing UserCreatedEvent from Kafka: {UserId}", message.UserId);

        // Send welcome email
        await _emailService.SendWelcomeEmailAsync(message.Email, message.Username);

        Logger.LogInformation("UserCreatedEvent processed successfully: {UserId}", message.UserId);
    }
}

// Register Kafka handlers
builder.Services.AddKafkaHandler<UserEventKafkaHandler, UserCreatedEvent>();
builder.Services.AddKafkaHandler<OrderEventKafkaHandler, OrderCreatedEvent>();
```

#### Using the Message Bus

##### Publishing Messages
```csharp
public class UserService
{
    private readonly IMessageBus _messageBus;
    private readonly IRepository<User> _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IMessageBus messageBus, IRepository<User> userRepository, ILogger<UserService> logger)
    {
        _messageBus = messageBus;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<User> CreateUserAsync(CreateUserRequest request)
    {
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = HashPassword(request.Password)
        };

        await _userRepository.AddAsync(user);

        // Publish event (fire-and-forget)
        var userCreatedEvent = new UserCreatedEvent(user.Id, user.Username, user.Email);
        await _messageBus.PublishAsync(userCreatedEvent);

        _logger.LogInformation("User created and event published: {UserId}", user.Id);
        return user;
    }
}

public class OrderService
{
    private readonly IMessageBus _messageBus;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IMessageBus messageBus, ILogger<OrderService> logger)
    {
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task ProcessOrderAsync(ProcessOrderRequest request)
    {
        // Send command (ensure delivery)
        var command = new CreateOrderCommand(request.UserId, request.Items);
        await _messageBus.SendAsync(command);

        _logger.LogInformation("Order processing command sent for user: {UserId}", request.UserId);
    }

    // Note: Request-Response pattern is not implemented in Kafka
    // Use RabbitMQ for request-response scenarios
    public async Task<OrderStatus> GetOrderStatusAsync(Guid orderId)
    {
        var request = new GetOrderStatusRequest(orderId);
        return await _messageBus.RequestAsync<GetOrderStatusRequest, OrderStatus>(request);
    }
}
```

##### Batch Publishing
```csharp
public class BulkNotificationService
{
    private readonly IMessageBus _messageBus;

    public BulkNotificationService(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task SendBulkNotificationsAsync(List<User> users, string message)
    {
        var tasks = users.Select(async user =>
        {
            var notification = new NotificationEvent(user.Id, message, NotificationType.Email);
            await _messageBus.PublishAsync(notification);
        });

        await Task.WhenAll(tasks);
    }
}
```

#### Message Patterns & Best Practices

##### 1. Event-Driven Architecture
```csharp
// Domain events for business processes
public record OrderPaymentProcessedEvent(Guid OrderId, decimal Amount, string PaymentMethod) : BaseEvent;
public record OrderShippedEvent(Guid OrderId, string TrackingNumber, DateTime ShippedAt) : BaseEvent;
public record InventoryUpdatedEvent(Guid ProductId, int NewQuantity, int PreviousQuantity) : BaseEvent;

// Event handlers chain business processes
public class PaymentProcessedHandler : IConsumer<OrderPaymentProcessedEvent>
{
    public async Task Consume(ConsumeContext<OrderPaymentProcessedEvent> context)
    {
        var payment = context.Message;

        // Update order status
        await _orderService.MarkAsPaidAsync(payment.OrderId);

        // Trigger fulfillment process
        await context.Publish(new FulfillmentRequestedEvent(payment.OrderId));

        // Update customer points
        await context.Publish(new CustomerPointsUpdatedEvent(payment.OrderId, payment.Amount));
    }
}
```

##### 2. Saga Pattern (with MassTransit)
```csharp
// Order processing saga
public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public State Submitted { get; private set; }
    public State PaymentProcessing { get; private set; }
    public State Completed { get; private set; }

    public Event<OrderSubmittedEvent> OrderSubmitted { get; private set; }
    public Event<PaymentProcessedEvent> PaymentProcessed { get; private set; }

    public OrderStateMachine()
    {
        Initially(
            When(OrderSubmitted)
                .Then(context => context.Instance.OrderId = context.Data.OrderId)
                .TransitionTo(PaymentProcessing)
                .Publish(context => new ProcessPaymentCommand(context.Data.OrderId, context.Data.Amount))
        );

        During(PaymentProcessing,
            When(PaymentProcessed)
                .TransitionTo(Completed)
                .Publish(context => new OrderCompletedEvent(context.Instance.OrderId))
        );
    }
}
```

##### 3. Dead Letter Queue Handling
```csharp
// Configure dead letter handling
builder.Services.AddMarventaRabbitMQ(
    configure: x => x.AddConsumer<UserCreatedEventHandler>(),
    configureRabbitMq: cfg =>
    {
        cfg.ReceiveEndpoint("user-events", ep =>
        {
            ep.ConfigureConsumer<UserCreatedEventHandler>(context);

            // Configure retry policy
            ep.UseMessageRetry(r => r.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2)));

            // Configure error handling
            ep.UseDeadLetterQueue("user-events-error");
        });
    }
);

// Dead letter message handler
public class DeadLetterMessageHandler : IConsumer<UserCreatedEvent>
{
    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        // Log the failed message
        _logger.LogError("Processing failed message from dead letter queue: {@Message}", context.Message);

        // Implement custom recovery logic
        await HandleFailedUserCreation(context.Message);
    }
}
```

#### Configuration Examples

##### Development Configuration
```json
{
  "ConnectionStrings": {
    "RabbitMQ": "amqp://guest:guest@localhost:5672/"
  },
  "Messaging": {
    "ServiceName": "dev-service"
  },
  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "GroupId": "dev-consumers",
    "TopicPrefix": "dev-",
    "EnableAutoCommit": true,
    "AutoOffsetReset": "latest"
  }
}
```

##### Production Configuration
```json
{
  "ConnectionStrings": {
    "RabbitMQ": "amqps://user:password@rabbitmq.production.com:5671/production"
  },
  "Messaging": {
    "ServiceName": "production-service"
  },
  "Kafka": {
    "BootstrapServers": "kafka1.production.com:9092,kafka2.production.com:9092,kafka3.production.com:9092",
    "GroupId": "production-consumers",
    "TopicPrefix": "prod-",
    "EnableAutoCommit": false,
    "AutoOffsetReset": "earliest",
    "SecurityProtocol": "SaslSsl",
    "SaslMechanism": "ScramSha512",
    "SaslUsername": "your-username",
    "SaslPassword": "your-password",
    "SessionTimeoutMs": 30000,
    "HeartbeatIntervalMs": 3000,
    "MaxPollIntervalMs": 300000,
    "EnableIdempotence": true,
    "Acks": "All",
    "MessageTimeoutMs": 300000
  }
}
```

#### Testing

##### Unit Testing Message Handlers
```csharp
public class UserCreatedEventHandlerTests
{
    [Test]
    public async Task Handle_ValidEvent_SendsWelcomeEmail()
    {
        // Arrange
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<UserCreatedEventHandler>>();
        var handler = new UserCreatedEventHandler(mockLogger.Object, mockEmailService.Object);

        var userEvent = new UserCreatedEvent(Guid.NewGuid(), "john.doe", "john@example.com");
        var mockContext = new Mock<ConsumeContext<UserCreatedEvent>>();
        mockContext.Setup(x => x.Message).Returns(userEvent);

        // Act
        await handler.Consume(mockContext.Object);

        // Assert
        mockEmailService.Verify(x => x.SendWelcomeEmailAsync(userEvent.Email, userEvent.Username), Times.Once);
    }
}
```

##### Integration Testing with In-Memory Transport
```csharp
public class MessagingIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public MessagingIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Test]
    public async Task PublishEvent_ShouldBeProcessedByHandler()
    {
        // Arrange
        var scope = _factory.Services.CreateScope();
        var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

        // Act
        var userEvent = new UserCreatedEvent(Guid.NewGuid(), "test.user", "test@example.com");
        await messageBus.PublishAsync(userEvent);

        // Assert - verify handler was called (through side effects or mocks)
        await Task.Delay(100); // Allow message processing
        // Verify expected side effects occurred
    }
}
```

### 📊 Logging
```csharp
public class OrderController : ControllerBase
{
    private readonly ILoggerService _logger;
    private readonly IMediator _mediator;

    public OrderController(ILoggerService logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderCommand command)
    {
        using var activity = _logger.StartActivity("CreateOrder");

        try
        {
            _logger.LogInformation("Creating order for user {UserId}", command.UserId);

            var order = await _mediator.Send(command);

            _logger.LogInformation("Order created successfully: {OrderId}", order.Id);

            // Log business metrics
            _logger.LogMetric("order.created", 1, new Dictionary<string, object>
            {
                ["user_id"] = command.UserId,
                ["order_total"] = order.Total,
                ["item_count"] = order.Items.Count
            });

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create order for user {UserId}", command.UserId);
            throw;
        }
    }
}
```

## Complete Configuration Guide

### Full appsettings.json Example
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyApp;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "JWT": {
    "SecretKey": "your-super-secret-key-at-least-32-characters-long-for-security",
    "Issuer": "MyApplication",
    "Audience": "MyApplicationUsers",
    "ExpiryInMinutes": 60,
    "RefreshTokenExpiryInDays": 7,
    "ClockSkewInMinutes": 5
  },
  "RateLimit": {
    "EnableRateLimiting": true,
    "MaxRequests": 100,
    "WindowSizeInMinutes": 1
  },
  "ApiVersioning": {
    "DefaultVersion": "1.0",
    "Strategy": "Header",
    "HeaderName": "Api-Version",
    "AllowedVersions": ["1.0", "1.1", "2.0"]
  },
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "FromName": "My Application",
    "FromEmail": "noreply@myapp.com",
    "EnableSsl": true
  },
  "SMS": {
    "Provider": "Twilio",
    "AccountSid": "your-twilio-sid",
    "AuthToken": "your-twilio-token",
    "FromNumber": "+1234567890"
  },
  "Cache": {
    "DefaultExpirationMinutes": 30,
    "EnableDistributedCache": false,
    "RedisConnectionString": "localhost:6379"
  },
  "FeatureFlags": {
    "NewDiscountSystem": true,
    "FreeShipping": false,
    "EnhancedOrderDetails": true
  },
  "CircuitBreaker": {
    "FailureThreshold": 5,
    "TimeoutInSeconds": 30,
    "RetryIntervalInSeconds": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Marventa.Framework": "Debug"
    }
  }
}
```

### Complete Program.cs Setup
```csharp
using Marventa.Framework;

var builder = WebApplication.CreateBuilder(args);

// Add Marventa Framework (includes all core services)
builder.Services.AddMarventa();

// Add specific features
builder.Services.AddMarventaRateLimiting(builder.Configuration.GetSection("RateLimit"));
builder.Services.AddMarventaApiVersioning(builder.Configuration.GetSection("ApiVersioning"));
builder.Services.AddMarventaJwtAuthentication(builder.Configuration);
builder.Services.AddMarventaHealthChecks(builder.Configuration);

// Add your custom services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Configure Entity Framework (optional)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Marventa middleware (order matters!)
app.UseMarventa(); // Includes exception handling
app.UseMarventaRateLimiting();
app.UseMarventaApiVersioning();
app.UseMarventaJwtAuthentication();
app.UseMarventaHealthChecks(); // Adds /health, /health/ready, /health/live

// Standard ASP.NET Core middleware
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed data (optional)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.EnsureCreatedAsync();
    // Add your seed data here
}

app.Run();
```

## Best Practices

### 🏗️ Architecture Guidelines

1. **Follow Clean Architecture**
```csharp
// ✅ Good: Dependencies flow inward
public class OrderService
{
    private readonly IRepository<Order> _repository; // Core abstraction
    private readonly IEmailService _emailService;   // Core abstraction
}

// ❌ Bad: Depending on concrete implementations
public class OrderService
{
    private readonly SqlOrderRepository _repository; // Infrastructure detail
    private readonly SmtpEmailService _emailService; // Infrastructure detail
}
```

2. **Use Repository Pattern Correctly**
```csharp
// ✅ Good: Generic repository for simple CRUD
public class UserService
{
    private readonly IRepository<User> _userRepository;

    public async Task<User> GetUserAsync(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
    }
}

// ✅ Good: Custom repository for complex queries
public interface IOrderRepository : IRepository<Order>
{
    Task<List<Order>> GetOrdersByDateRangeAsync(DateTime start, DateTime end);
    Task<decimal> GetTotalRevenueAsync(int year);
}
```

3. **CQRS Implementation**
```csharp
// ✅ Good: Separate commands and queries
public record CreateUserCommand(string Email, string Name) : ICommand<User>;
public record GetUserQuery(Guid Id) : IQuery<User>;

// ❌ Bad: Mixing command and query in one method
public interface IUserService
{
    Task<User> CreateAndReturnUserWithStatistics(CreateUserRequest request); // Too complex
}
```

4. **Error Handling**
```csharp
// ✅ Good: Use specific exceptions
if (user == null)
    throw new NotFoundException($"User with ID {userId} not found");

if (order.Total <= 0)
    throw new BusinessException("Order total must be greater than zero");

// ✅ Good: Handle exceptions in handlers
public class CreateOrderHandler : ICommandHandler<CreateOrderCommand, Order>
{
    public async Task<Order> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        try
        {
            // Business logic
            return order;
        }
        catch (ValidationException)
        {
            throw; // Re-throw validation errors
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create order");
            throw new BusinessException("Failed to create order", ex);
        }
    }
}
```

5. **Caching Strategy**
```csharp
// ✅ Good: Cache frequently accessed, rarely changing data
public async Task<List<Category>> GetCategoriesAsync()
{
    return await _cache.GetOrSetAsync(
        "categories:all",
        () => _repository.GetAllAsync(),
        TimeSpan.FromHours(1)
    );
}

// ❌ Bad: Don't cache user-specific or rapidly changing data
public async Task<decimal> GetCurrentUserBalanceAsync(Guid userId)
{
    // Don't cache this - it changes frequently and is user-specific
    return await _repository.GetUserBalanceAsync(userId);
}
```

### 🔒 Security Best Practices

1. **JWT Token Security**
```csharp
// ✅ Good: Use strong secret keys (256-bit minimum)
"SecretKey": "your-super-secret-key-at-least-32-characters-long-for-security-purposes-2024"

// ✅ Good: Set appropriate expiration times
"ExpiryInMinutes": 15,  // Short for access tokens
"RefreshTokenExpiryInDays": 7  // Longer for refresh tokens

// ✅ Good: Validate all claims
[Authorize]
public async Task<IActionResult> UpdateUser(Guid userId, UpdateUserRequest request)
{
    if (_currentUser.UserId != userId.ToString() && !_currentUser.IsInRole("Admin"))
    {
        return Forbid();
    }
    // Update logic
}
```

2. **API Security**
```csharp
// ✅ Good: Rate limit sensitive endpoints
[HttpPost("login")]
[RateLimit(MaxRequests = 5, WindowSizeInMinutes = 15)]
public async Task<IActionResult> Login(LoginRequest request) { }

// ✅ Good: Validate input
public record CreateUserRequest
{
    [Required, EmailAddress]
    public string Email { get; init; } = "";

    [Required, StringLength(100, MinimumLength = 8)]
    public string Password { get; init; } = "";
}
```

### 📊 Performance Optimization

1. **Database Queries**
```csharp
// ✅ Good: Use pagination for large datasets
public async Task<PagedResult<Order>> GetOrdersAsync(int page = 1, int pageSize = 20)
{
    return await _orderRepository.GetPagedAsync(
        page: page,
        pageSize: Math.Min(pageSize, 100), // Limit max page size
        orderBy: o => o.CreatedDate,
        descending: true
    );
}

// ✅ Good: Include related data efficiently
public async Task<Order> GetOrderWithDetailsAsync(Guid orderId)
{
    return await _orderRepository.GetByIdAsync(
        orderId,
        include: o => o.Items.ThenInclude(i => i.Product)
    );
}
```

2. **Caching Patterns**
```csharp
// ✅ Good: Cache-aside pattern
public async Task<Product> GetProductAsync(Guid productId)
{
    var cacheKey = $"product:{productId}";
    var product = await _cache.GetAsync<Product>(cacheKey);

    if (product == null)
    {
        product = await _repository.GetByIdAsync(productId);
        if (product != null)
        {
            await _cache.SetAsync(cacheKey, product, TimeSpan.FromMinutes(30));
        }
    }

    return product;
}

// ✅ Good: Invalidate cache on updates
public async Task UpdateProductAsync(Product product)
{
    await _repository.UpdateAsync(product);
    await _cache.RemoveAsync($"product:{product.Id}");
    await _cache.RemoveAsync("products:featured"); // Remove related cache
}
```

## Troubleshooting & FAQ

### Common Issues

#### 1. JWT Authentication Not Working
```
Problem: 401 Unauthorized even with valid token
```
**Solutions:**
- Check JWT secret key configuration
- Verify token expiration time
- Ensure middleware order: `UseAuthentication()` before `UseAuthorization()`
- Check clock skew settings

#### 2. Rate Limiting Too Restrictive
```
Problem: Getting 429 Too Many Requests too frequently
```
**Solutions:**
```csharp
// Increase limits for specific endpoints
[RateLimit(MaxRequests = 1000, WindowSizeInMinutes = 1)]

// Or disable for development
builder.Services.AddMarventaRateLimiting(options =>
{
    options.EnableRateLimiting = !builder.Environment.IsDevelopment();
});
```

#### 3. CQRS Handler Not Found
```
Problem: Handler for 'CreateUserCommand' was not found
```
**Solutions:**
- Ensure handler is registered in DI container
- Check handler implements correct interface
- Verify assembly scanning is working

#### 4. Database Connection Issues
```
Problem: Unable to connect to database
```
**Solutions:**
- Verify connection string in appsettings.json
- Check database server is running
- Ensure Entity Framework is properly configured
- Run database migrations

#### 5. Email/SMS Not Sending
```
Problem: Notifications not being delivered
```
**Solutions:**
- Check SMTP/SMS provider configuration
- Verify credentials and API keys
- Check firewall/network settings
- Enable logging to see detailed errors

### FAQ

**Q: Can I use Redis instead of MemoryCache?**
A: Yes! Implement `ICacheService` interface:
```csharp
public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;

    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await _distributedCache.GetStringAsync(key);
        return json == null ? default : JsonSerializer.Deserialize<T>(json);
    }

    // Implement other methods...
}

// Register in DI
builder.Services.AddScoped<ICacheService, RedisCacheService>();
```

**Q: How do I add custom validation?**
A: Create custom validators:
```csharp
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Username).NotEmpty().Length(3, 50);
        RuleFor(x => x.Password).MinimumLength(8).Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)");
    }
}
```

**Q: Can I customize exception handling?**
A: Yes! Implement `IExceptionHandler`:
```csharp
public class CustomExceptionHandler : IExceptionHandler
{
    public async Task<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        // Custom logic
        return true; // Handled
    }
}

builder.Services.AddScoped<IExceptionHandler, CustomExceptionHandler>();
```

**Q: How do I add custom health checks?**
A: Implement `IHealthCheck`:
```csharp
public class DatabaseHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        // Check database connectivity
        return HealthCheckResult.Healthy("Database is responsive");
    }
}

builder.Services.AddScoped<IHealthCheck, DatabaseHealthCheck>();
```

## Documentation

For detailed documentation and advanced usage examples, visit our [GitHub repository](https://github.com/AdemKinatas/Marventa.Framework).

## Contributing

Contributions are welcome! Please read our contributing guidelines and submit pull requests to our GitHub repository.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

If you encounter any issues or have questions:

- 🐛 [Report bugs](https://github.com/AdemKinatas/Marventa.Framework/issues)
- 💬 [Ask questions](https://github.com/AdemKinatas/Marventa.Framework/discussions)
- 📖 [Read documentation](https://github.com/AdemKinatas/Marventa.Framework)

---

Made with ❤️ by the Adem Kınataş
