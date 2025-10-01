# Marventa.Framework v4.1.0

**Enterprise .NET 8.0 & 9.0 framework - Convention over Configuration**

[![NuGet](https://img.shields.io/nuget/v/Marventa.Framework.svg)](https://www.nuget.org/packages/Marventa.Framework/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

---

## 📖 What is Marventa.Framework?

Marventa.Framework is an **enterprise-grade .NET framework** that provides:

### Core Architecture Patterns (Always Available)
- **CQRS** - Command/Query separation with MediatR
- **DDD** - Entity, AggregateRoot, ValueObject, DomainEvent
- **Repository & UnitOfWork** - Generic and custom repositories
- **FluentValidation** - Request validation pipeline
- **Result Pattern** - Type-safe error handling
- **API Responses** - Standardized response format
- **Exception Handling** - Global error handling middleware

### Infrastructure Features (Auto-Configured)

**🔒 Security & Authentication**
- JWT Authentication & Authorization (permission-based)
- Rate Limiting (IP/User/ApiKey strategies)
- Password Hashing (BCrypt)
- AES Encryption

**💾 Data & Storage**
- Caching (InMemory, Redis, Hybrid)
- Local File Storage
- Azure Blob Storage
- AWS S3 Storage
- MongoDB
- Elasticsearch

**📨 Event-Driven Architecture**
- RabbitMQ Event Bus
- Kafka Event Streaming
- MassTransit Integration

**🏢 Enterprise Features**
- Multi-Tenancy (Header/Subdomain/Claim)
- Health Checks (Database, Redis, RabbitMQ)
- Serilog Structured Logging
- OpenTelemetry Tracing
- Polly Resilience Patterns

---

## 🚀 How to Use

### Installation

```bash
dotnet add package Marventa.Framework
```

---

## 📋 Feature Categories

### 1️⃣ Features That Work Without Configuration

These features are **always available** after installing the package - no configuration needed:

```csharp
// In Program.cs
builder.Services.AddMarventa(builder.Configuration);
```

**What you get immediately:**
- ✅ Exception Handling (global error handling)
- ✅ API Response standardization
- ✅ CQRS patterns (Entity, ValueObject, Result)
- ✅ In-Memory Caching (default)

**Usage:**
```csharp
// Use Result pattern
public Result<User> GetUser(Guid id)
{
    var user = _repository.GetById(id);
    if (user == null)
        return Result<User>.Failure("User not found");

    return Result<User>.Success(user);
}

// Use In-Memory Cache
await _cache.SetAsync("key", value, TimeSpan.FromMinutes(10));
var cached = await _cache.GetAsync<MyType>("key");
```

---

### 2️⃣ Features That Require Configuration

These features **auto-activate** when you add their configuration to `appsettings.json`:

#### JWT Authentication

**Add to appsettings.json:**
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

**Usage:**
```csharp
// Generate token
var token = _jwtTokenGenerator.GenerateToken(
    userId: user.Id.ToString(),
    email: user.Email,
    roles: new[] { "Admin" },
    permissions: new[] { "users.read", "users.write" }
);

// Protect endpoints
[Authorize]
[RequirePermission("users.write")]
public async Task<IActionResult> CreateUser(CreateUserCommand command)
{
    var result = await _mediator.Send(command);
    return Ok(result);
}
```

---

#### Redis Caching

**Add to appsettings.json:**
```json
{
  "Caching": {
    "Type": "Redis"
  },
  "Redis": {
    "ConnectionString": "localhost:6379",
    "InstanceName": "MyApp:"
  }
}
```

**Usage:** (Same interface as in-memory cache)
```csharp
await _cache.SetAsync("user:123", user, TimeSpan.FromHours(1));
var user = await _cache.GetAsync<User>("user:123");
```

---

#### Multi-Tenancy

**Add to appsettings.json:**
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
public class MyService
{
    private readonly ITenantContext _tenantContext;

    public MyService(ITenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    public void DoSomething()
    {
        var tenantId = _tenantContext.TenantId;
        var tenantName = _tenantContext.TenantName;
        // Your tenant-specific logic
    }
}
```

---

#### Rate Limiting

**Add to appsettings.json:**
```json
{
  "RateLimiting": {
    "Strategy": "IpAddress",
    "RequestsPerMinute": 60,
    "Enabled": true
  }
}
```

**What it does:** Automatically limits requests to 60 per minute per IP address.

---

#### RabbitMQ Event Bus

**Add to appsettings.json:**
```json
{
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  }
}
```

**Usage:**
```csharp
// Publish event
var @event = new UserCreatedEvent(userId, email);
await _eventBus.PublishAsync(@event);

// Subscribe to event
public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
{
    public async Task HandleAsync(UserCreatedEvent @event)
    {
        // Handle the event
    }
}
```

---

#### Kafka Event Streaming

**Add to appsettings.json:**
```json
{
  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "GroupId": "my-consumer-group"
  }
}
```

**Usage:**
```csharp
// Produce
await _kafkaProducer.ProduceAsync("my-topic", new { UserId = 123, Action = "Created" });

// Consume
await _kafkaConsumer.ConsumeAsync("my-topic", async message =>
{
    Console.WriteLine($"Received: {message}");
});
```

---

#### MassTransit

**Add to appsettings.json:**
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

**Usage:**
```csharp
// Consumer
public class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        await Console.Out.WriteLineAsync($"Order {context.Message.OrderId} created");
    }
}

// Publish
await _publishEndpoint.Publish(new OrderCreated { OrderId = 123 });
```

---

#### Local File Storage

**Add to appsettings.json:**
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
// Returns: "https://myapp.com/files/documents/file.pdf"
```

---

#### Azure Blob Storage

**Add to appsettings.json:**
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

**Usage:** Same interface as Local Storage.

---

#### AWS S3 Storage

**Add to appsettings.json:**
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

**Usage:** Same interface as Local Storage.

---

#### MongoDB

**Add to appsettings.json:**
```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "MyDatabase"
  }
}
```

**Usage:**
```csharp
public class MyService
{
    private readonly IMongoDatabase _database;

    public MyService(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<User> GetUser(string id)
    {
        var collection = _database.GetCollection<User>("users");
        return await collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }
}
```

---

#### Elasticsearch

**Add to appsettings.json:**
```json
{
  "Elasticsearch": {
    "Uri": "http://localhost:9200"
  }
}
```

**Usage:**
```csharp
// Index document
await _elasticsearchService.IndexAsync("products", product);

// Search
var results = await _elasticsearchService.SearchAsync<Product>("products", "laptop");
```

---

#### Health Checks

**Add to appsettings.json:**
```json
{
  "HealthChecks": {
    "Enabled": "true"
  }
}
```

**What it does:**
- Automatically adds `/health` endpoint
- Auto-detects Database, Redis, RabbitMQ and monitors them

**Check health:**
```bash
curl http://localhost:5000/health
```

---

### 3️⃣ Features That Require Manual Setup

These features need **your application-specific code**, so they require manual configuration:

#### CQRS & MediatR

**Setup in Program.cs:**
```csharp
builder.Services.AddMarventa(builder.Configuration);
builder.Services.AddMarventaMediatR(typeof(Program).Assembly);
builder.Services.AddMarventaValidation(typeof(Program).Assembly);
```

**Usage:**
```csharp
// Command
public record CreateUserCommand(string Email, string Password) : IRequest<Result<Guid>>;

// Handler
public class CreateUserHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = User.Create(request.Email, request.Password);
        await _repository.AddAsync(user);
        return Result<Guid>.Success(user.Id);
    }
}

// Validator
public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).MinimumLength(8);
    }
}

// Controller
[HttpPost]
public async Task<IActionResult> CreateUser(CreateUserCommand command)
{
    var result = await _mediator.Send(command);
    return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
}
```

---

#### Database & Repository

**Setup in Program.cs:**
```csharp
builder.Services.AddMarventa(builder.Configuration);

builder.Services.AddMarventaDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
```

**DbContext:**
```csharp
public class AppDbContext : BaseDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor)
        : base(options, httpContextAccessor)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
```

**Repository:**
```csharp
public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Email == email);
    }
}
```

---

#### Logging (Serilog)

**Setup in Program.cs:**
```csharp
builder.Services.AddMarventaLogging(builder.Configuration, "MyApp");
```

**Add to appsettings.json:**
```json
{
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
public class MyService
{
    private readonly ILogger<MyService> _logger;

    public MyService(ILogger<MyService> logger)
    {
        _logger = logger;
    }

    public void DoSomething()
    {
        _logger.LogInformation("Operation started with {UserId}", userId);
    }
}
```

---

## 🎯 Complete Setup Guide

### Option 1: Use Everything Together (Recommended)

**Program.cs:**
```csharp
var builder = WebApplication.CreateBuilder(args);

// 1. Auto-configured infrastructure
builder.Services.AddMarventa(builder.Configuration, builder.Environment);

// 2. Manual features (require your code)
builder.Services.AddMarventaMediatR(typeof(Program).Assembly);
builder.Services.AddMarventaValidation(typeof(Program).Assembly);
builder.Services.AddMarventaLogging(builder.Configuration, "MyApp");

builder.Services.AddMarventaDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Your repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddControllers();

var app = builder.Build();

// 4. Middleware pipeline (auto-configured)
app.UseMarventa();
app.MapControllers();

app.Run();
```

**appsettings.json:**
```json
{
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
    "Type": "Redis"
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
  "MultiTenancy": {
    "Strategy": "Header",
    "HeaderName": "X-Tenant-Id"
  },
  "RateLimiting": {
    "Strategy": "IpAddress",
    "RequestsPerMinute": 60,
    "Enabled": true
  },
  "LocalStorage": {
    "BasePath": "D:/uploads",
    "BaseUrl": "https://myapp.com/files"
  },
  "HealthChecks": {
    "Enabled": "true"
  }
}
```

---

### Option 2: Use Individual Features

You can also configure features individually instead of using `AddMarventa()`:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Configure only what you need
builder.Services.AddMarventaJwtAuthentication(builder.Configuration);
builder.Services.AddMarventaRabbitMq(builder.Configuration);
builder.Services.AddMarventaElasticsearch(builder.Configuration);
builder.Services.AddMarventaMultiTenancy(builder.Configuration);
builder.Services.AddMarventaLocalStorage(builder.Configuration);
builder.Services.AddMarventaHealthChecks(builder.Configuration);

builder.Services.AddControllers();

var app = builder.Build();

// Manually configure middleware
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

---

## 📊 Configuration Reference

| Feature | Configuration Section | Required Fields | Auto-Activated |
|---------|---------------------|-----------------|----------------|
| **JWT Auth** | `Jwt` | `Secret`, `Issuer`, `Audience` | ✅ Yes |
| **Caching** | `Caching`, `Redis` | `Type`, `ConnectionString` | ✅ Yes |
| **Multi-Tenancy** | `MultiTenancy` | `Strategy` | ✅ Yes |
| **Rate Limiting** | `RateLimiting` | `Strategy`, `RequestsPerMinute` | ✅ Yes |
| **RabbitMQ** | `RabbitMQ` | `Host` | ✅ Yes |
| **Kafka** | `Kafka` | `BootstrapServers` | ✅ Yes |
| **MassTransit** | `MassTransit` | `Enabled: "true"` | ✅ Yes |
| **Elasticsearch** | `Elasticsearch` | `Uri` | ✅ Yes |
| **MongoDB** | `MongoDB` | `ConnectionString`, `DatabaseName` | ✅ Yes |
| **LocalStorage** | `LocalStorage` | `BasePath` | ✅ Yes |
| **Azure Storage** | `Azure:Storage` | `ConnectionString` | ✅ Yes |
| **AWS S3** | `AWS` | `AccessKey`, `SecretKey`, `BucketName` | ✅ Yes |
| **Health Checks** | `HealthChecks` | `Enabled: "true"` | ✅ Yes |
| **CQRS/MediatR** | - | Manual setup in Program.cs | ❌ No |
| **Database/EF** | - | Manual setup in Program.cs | ❌ No |
| **Logging** | `Serilog` | Manual setup in Program.cs | ❌ No |

---

## 🔄 Migration Guide

### From v4.0.x to v4.1.0

**New Features:**
- ✅ Local File Storage support
- ✅ MassTransit integration
- ✅ Health Checks auto-configuration

**Breaking Changes:**
- None

**To Use New Features:**
```json
{
  "LocalStorage": {
    "BasePath": "D:/uploads"
  },
  "MassTransit": {
    "Enabled": "true"
  },
  "HealthChecks": {
    "Enabled": "true"
  }
}
```

---

## 📄 License

MIT License - see [LICENSE](LICENSE) for details

---

## 🤝 Contributing

Contributions are welcome! Please open an issue or submit a pull request.

---

## 📧 Support

For questions and support, please open an issue on [GitHub](https://github.com/AdemKinatas/Marventa.Framework/issues).
