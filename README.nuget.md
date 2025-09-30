# 🚀 Marventa Framework

[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/AdemKinatas/Marventa.Framework/blob/master/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Marventa.Framework.svg)](https://www.nuget.org/packages/Marventa.Framework)

> **Enterprise-grade .NET framework with Clean Architecture, CQRS, and 47+ modular features**

---

## 📋 Table of Contents

- [Quick Start](#-quick-start)
- [Core Features](#-core-features)
- [Installation & Setup](#-installation--setup)
- [Entity Base Classes](#-entity-base-classes)
- [CQRS Pattern](#-cqrs-pattern)
- [Repository Pattern](#-repository-pattern)
- [Configuration Options](#-configuration-options)
- [Real-World Example](#-real-world-example)
- [Documentation](#-documentation)
- [Support](#-support)

---

## ⚡ Quick Start

### 1. Installation

```bash
dotnet add package Marventa.Framework
```

### 2. Configure Services

```csharp
using Marventa.Framework.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add Marventa Framework with CQRS
builder.Services.AddMarventaFramework(builder.Configuration, options =>
{
    // Core Infrastructure
    options.EnableLogging = true;
    options.EnableCaching = true;
    options.EnableRepository = true;
    options.EnableHealthChecks = true;

    // CQRS + MediatR
    options.EnableCQRS = true;
    options.CqrsOptions.Assemblies.Add(typeof(Program).Assembly);
    options.CqrsOptions.EnableValidationBehavior = true;
    options.CqrsOptions.EnableLoggingBehavior = true;
    options.CqrsOptions.EnableTransactionBehavior = true;
});

var app = builder.Build();
app.UseMarventaFramework(builder.Configuration);
app.Run();
```

### 3. Create Your First Entity

```csharp
using Marventa.Framework.Core.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }

    // BaseEntity provides:
    // - Guid Id (auto-generated)
    // - DateTime CreatedDate, UpdatedDate
    // - bool IsDeleted (soft delete)
    // - Audit tracking (CreatedBy, UpdatedBy)
}
```

### 4. Done! 🎉

You now have:
- ✅ Repository pattern with Unit of Work
- ✅ CQRS with automatic validation
- ✅ Soft delete and audit tracking
- ✅ Logging and health checks
- ✅ Clean Architecture structure

---

## 🎯 Core Features

### **1. Base Entity Classes**

| Class | Purpose | When to Use |
|-------|---------|-------------|
| `BaseEntity` | Basic entity with audit tracking | Default for all entities |
| `AuditableEntity` | Adds versioning & concurrency | Entities needing version control |
| `TenantBaseEntity` | Multi-tenant isolation | Multi-tenant applications |

### **2. CQRS + MediatR Pipeline**

```csharp
// Command with automatic validation & transaction
public class CreateProductCommand : ICommand<Guid>
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// Validator (automatically executed)
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}

// Handler (transaction managed automatically)
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = new Product { Name = request.Name, Price = request.Price };
        await _unitOfWork.Repository<Product>().AddAsync(product, ct);
        // SaveChanges called automatically by TransactionBehavior
        return product.Id;
    }
}
```

### **3. Repository Pattern**

```csharp
public class ProductService
{
    private readonly IRepository<Product> _repository;

    public ProductService(IRepository<Product> repository)
    {
        _repository = repository;
    }

    // Simple queries
    public async Task<Product?> GetByIdAsync(Guid id)
        => await _repository.GetByIdAsync(id);

    // Advanced queries with specifications
    public async Task<IEnumerable<Product>> GetExpensiveProductsAsync()
        => await _repository.FindAsync(p => p.Price > 1000);

    // Pagination
    public async Task<IEnumerable<Product>> GetPagedAsync(int page, int size)
        => await _repository.GetPagedAsync(page, size);
}
```

### **4. Built-in Pipeline Behaviors**

| Behavior | Purpose | Automatic Actions |
|----------|---------|-------------------|
| `ValidationBehavior` | Input validation | Validates using FluentValidation |
| `LoggingBehavior` | Performance monitoring | Logs execution time, warns if >500ms |
| `TransactionBehavior` | Transaction management | Auto SaveChanges, rollback on error |
| `IdempotencyBehavior` | Duplicate prevention | Prevents duplicate command execution |

---

## 📦 Installation & Setup

### Step 1: Install Package

```bash
dotnet add package Marventa.Framework
```

### Step 2: Configure appsettings.json

```json
{
  "Marventa": {
    "ApiKey": "your-secret-key",
    "RateLimit": {
      "MaxRequests": 100,
      "WindowMinutes": 15
    },
    "Caching": {
      "Provider": "Memory"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "your-database-connection"
  }
}
```

### Step 3: Setup DbContext

```csharp
using Marventa.Framework.Infrastructure.Data;

public class ApplicationDbContext : BaseDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ITenantContext tenantContext)
        : base(options, tenantContext)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // IMPORTANT: Apply base configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
```

**BaseDbContext provides:**
- ✅ Automatic audit tracking (CreatedDate, UpdatedDate)
- ✅ Soft delete with global query filters
- ✅ Multi-tenancy support with automatic isolation
- ✅ Domain event dispatching
- ✅ Optimistic concurrency with RowVersion

---

## 🏗️ Entity Base Classes

### BaseEntity - Standard Entities

```csharp
using Marventa.Framework.Core.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

// Provides automatically:
// - Guid Id
// - DateTime CreatedDate, UpdatedDate
// - string? CreatedBy, UpdatedBy
// - bool IsDeleted (soft delete)
// - DateTime? DeletedDate, string? DeletedBy
```

### AuditableEntity - With Versioning

```csharp
public class Order : AuditableEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}

// Includes BaseEntity + versioning:
// - string Version (semantic versioning)
// - byte[] RowVersion (optimistic concurrency)
```

### TenantBaseEntity - Multi-Tenant

```csharp
public class Customer : TenantBaseEntity
{
    public string CompanyName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

// Includes BaseEntity + tenant isolation:
// - Guid TenantId
// - Automatic filtering by tenant in all queries
```

---

## ⚡ CQRS Pattern

### Commands (Write Operations)

```csharp
using Marventa.Framework.Application.Commands;

// 1. Define Command
public class UpdateProductPriceCommand : ICommand<bool>
{
    public Guid ProductId { get; set; }
    public decimal NewPrice { get; set; }
}

// 2. Validator (optional but recommended)
public class UpdateProductPriceCommandValidator : AbstractValidator<UpdateProductPriceCommand>
{
    public UpdateProductPriceCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.NewPrice).GreaterThan(0).LessThan(1000000);
    }
}

// 3. Handler
public class UpdateProductPriceCommandHandler : IRequestHandler<UpdateProductPriceCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public async Task<bool> Handle(UpdateProductPriceCommand request, CancellationToken ct)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.ProductId);
        if (product == null) return false;

        product.Price = request.NewPrice;
        await _unitOfWork.Repository<Product>().UpdateAsync(product);
        return true;
    }
}
```

### Queries (Read Operations)

```csharp
using Marventa.Framework.Application.Queries;

// 1. Define Query
public class GetProductByIdQuery : IQuery<ProductDto>
{
    public Guid Id { get; set; }
}

// 2. Handler
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IRepository<Product> _repository;

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(request.Id);
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price
        };
    }
}
```

### Using in Controllers

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> Get(Guid id)
    {
        var query = new GetProductByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateProductCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { id }, id);
    }
}
```

---

## 💾 Repository Pattern

### Basic Usage

```csharp
public class ProductService
{
    private readonly IRepository<Product> _repository;

    // CRUD Operations
    public async Task<Product> CreateAsync(Product product)
        => await _repository.AddAsync(product);

    public async Task<Product?> GetAsync(Guid id)
        => await _repository.GetByIdAsync(id);

    public async Task<IEnumerable<Product>> GetAllAsync()
        => await _repository.GetAllAsync();

    public async Task UpdateAsync(Product product)
        => await _repository.UpdateAsync(product);

    public async Task DeleteAsync(Product product)
        => await _repository.DeleteAsync(product); // Soft delete
}
```

### Advanced Queries

```csharp
// Find with predicate
var products = await _repository.FindAsync(p => p.Price > 100);

// Pagination
var pagedProducts = await _repository.GetPagedAsync(pageNumber: 1, pageSize: 20);

// Count
var count = await _repository.CountAsync(p => p.IsDeleted == false);

// Any
var hasExpensive = await _repository.AnyAsync(p => p.Price > 1000);
```

### Unit of Work Pattern

```csharp
public class OrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Order> CreateOrderAsync(OrderDto dto)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // Create order
            var order = new Order { OrderNumber = dto.OrderNumber };
            await _unitOfWork.Repository<Order>().AddAsync(order);

            // Update inventory
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(dto.ProductId);
            product.StockQuantity -= dto.Quantity;

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
            return order;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
```

---

## ⚙️ Configuration Options

### Feature Flags (47 Total)

```csharp
builder.Services.AddMarventaFramework(builder.Configuration, options =>
{
    // 🏗️ Core Infrastructure
    options.EnableLogging = true;
    options.EnableCaching = true;
    options.EnableRepository = true;
    options.EnableHealthChecks = true;
    options.EnableValidation = true;
    options.EnableExceptionHandling = true;

    // 🛡️ Security
    options.EnableSecurity = true;
    options.EnableJWT = true;
    options.EnableApiKeys = true;
    options.EnableEncryption = true;

    // ⚡ CQRS + MediatR
    options.EnableCQRS = true;
    options.CqrsOptions.EnableValidationBehavior = true;
    options.CqrsOptions.EnableLoggingBehavior = true;
    options.CqrsOptions.EnableTransactionBehavior = true;

    // 🌐 API Management
    options.EnableVersioning = true;
    options.EnableRateLimiting = true;
    options.EnableCompression = true;
    options.EnableIdempotency = true;

    // 📊 Monitoring
    options.EnableAnalytics = true;
    options.EnableObservability = true;

    // 🔄 Event-Driven
    options.EnableEventDriven = true;
    options.EnableMessaging = true;
    options.EnableSagas = true;

    // 🏢 Multi-Tenancy
    options.EnableMultiTenancy = true;
});
```

---

## 🌟 Real-World Example

Complete working example of a Product Management API:

```csharp
// 1. Entity
public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string Category { get; set; } = string.Empty;
}

// 2. Command
public class CreateProductCommand : ICommand<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string Category { get; set; } = string.Empty;
}

// 3. Validator
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Category).NotEmpty();
    }
}

// 4. Handler
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            Category = request.Category
        };

        await _unitOfWork.Repository<Product>().AddAsync(product, ct);
        // TransactionBehavior automatically calls SaveChangesAsync
        return product.Id;
    }
}

// 5. Query
public class GetAllProductsQuery : IQuery<IEnumerable<ProductDto>>
{
    public string? Category { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}

// 6. Query Handler
public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IRepository<Product> _repository;

    public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken ct)
    {
        var query = _repository.Query();

        if (!string.IsNullOrEmpty(request.Category))
            query = query.Where(p => p.Category == request.Category);

        if (request.MinPrice.HasValue)
            query = query.Where(p => p.Price >= request.MinPrice.Value);

        if (request.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= request.MaxPrice.Value);

        var products = await query.ToListAsync(ct);

        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            Category = p.Category
        });
    }
}

// 7. Controller
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll([FromQuery] GetAllProductsQuery query)
    {
        var products = await _mediator.Send(query);
        return Ok(products);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateProductCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        var query = new GetProductByIdQuery { Id = id };
        var product = await _mediator.Send(query);
        return product != null ? Ok(product) : NotFound();
    }
}
```

---

## 📚 Documentation

### Complete Resources

- 📖 **[Full Documentation](https://github.com/AdemKinatas/Marventa.Framework#readme)** - Comprehensive guide
- 🎓 **[Getting Started Tutorial](https://github.com/AdemKinatas/Marventa.Framework/blob/master/docs/getting-started.md)** - Step-by-step guide
- 💡 **[Sample Projects](https://github.com/AdemKinatas/Marventa.Framework/tree/master/samples)** - Working examples
- 🔧 **[API Reference](https://github.com/AdemKinatas/Marventa.Framework/blob/master/docs/api/)** - Complete API docs
- 📝 **[Migration Guide](https://github.com/AdemKinatas/Marventa.Framework/blob/master/docs/migration/)** - Upgrade instructions

### Key Topics

- **Clean Architecture Implementation**
- **CQRS Pattern with MediatR**
- **Repository & Unit of Work**
- **Multi-Tenancy Setup**
- **Event-Driven Architecture**
- **Saga Pattern for Distributed Transactions**
- **CDN Integration (Azure, AWS, CloudFlare)**
- **Performance Optimization**

---

## 🆘 Support

### Get Help

- 💬 **[GitHub Discussions](https://github.com/AdemKinatas/Marventa.Framework/discussions)** - Ask questions
- 🐛 **[Report Issues](https://github.com/AdemKinatas/Marventa.Framework/issues)** - Bug reports
- 📧 **Email**: ademkinatas@gmail.com
- ⭐ **[Star on GitHub](https://github.com/AdemKinatas/Marventa.Framework)** - Show your support!

### Quick Links

- [Changelog](https://github.com/AdemKinatas/Marventa.Framework/releases)
- [Contributing Guide](https://github.com/AdemKinatas/Marventa.Framework/blob/master/CONTRIBUTING.md)
- [Code of Conduct](https://github.com/AdemKinatas/Marventa.Framework/blob/master/CODE_OF_CONDUCT.md)

---

## 📄 License

MIT License - Free for personal and commercial use.

See [LICENSE](https://github.com/AdemKinatas/Marventa.Framework/blob/master/LICENSE) for details.

---

<div align="center">

**Built with ❤️ by [Adem Kınataş](https://github.com/AdemKinatas)**

⭐ **[Star us on GitHub](https://github.com/AdemKinatas/Marventa.Framework)** if you find this useful!

</div>