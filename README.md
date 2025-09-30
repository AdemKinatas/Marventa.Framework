# 📘 Marventa Framework - Complete Guide

[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![NuGet](https://img.shields.io/badge/NuGet-v3.3.2-blue)](https://www.nuget.org/packages/Marventa.Framework)

> **Enterprise-grade .NET framework with Clean Architecture, CQRS, and 47+ modular features**

---

## 📖 Table of Contents

### Getting Started
1. [Introduction](#-1-introduction)
2. [Installation](#-2-installation)
3. [Quick Start](#-3-quick-start)
4. [Project Structure](#-4-project-structure)

### Core Concepts
5. [Entity Base Classes](#-5-entity-base-classes)
6. [DbContext Setup](#-6-dbcontext-setup)
7. [Repository Pattern](#-7-repository-pattern)
8. [Unit of Work](#-8-unit-of-work)

### CQRS & MediatR
9. [Commands](#-9-commands)
10. [Queries](#-10-queries)
11. [Validation](#-11-validation)
12. [Pipeline Behaviors](#-12-pipeline-behaviors)

### Advanced Features
13. [Multi-Tenancy](#-13-multi-tenancy)
14. [Soft Delete](#-14-soft-delete)
15. [Audit Tracking](#-15-audit-tracking)
16. [Event Sourcing](#-16-event-sourcing)
17. [Saga Pattern](#-17-saga-pattern)

### Infrastructure
18. [Caching](#-18-caching)
19. [Logging](#-19-logging)
20. [Health Checks](#-20-health-checks)
21. [CDN Integration](#-21-cdn-integration)

### Configuration
22. [Feature Flags](#-22-feature-flags)
23. [appsettings.json](#-23-appsettingsjson)

### Examples
24. [Complete Example](#-24-complete-example)
25. [Best Practices](#-25-best-practices)

---

## 📚 1. Introduction

Marventa Framework is a complete enterprise solution implementing **Clean Architecture** principles. It helps you build scalable, maintainable applications faster.

### What You Get

✅ **Clean Architecture** - Proper separation of concerns
✅ **CQRS Pattern** - Command Query Responsibility Segregation
✅ **Repository Pattern** - Clean data access
✅ **MediatR Behaviors** - Automatic validation, logging, transactions
✅ **Multi-Tenancy** - Built-in tenant isolation
✅ **Soft Delete** - Never lose data
✅ **Audit Tracking** - Who did what, when
✅ **47 Features** - Enable only what you need

### When to Use

- Building enterprise applications
- Need Clean Architecture structure
- Want CQRS with minimal boilerplate
- Require multi-tenancy support
- Need audit trails and soft delete

---

## 🚀 2. Installation

### Step 1: Install NuGet Package

```bash
dotnet add package Marventa.Framework
```

### Step 2: Install Database Provider

```bash
# For SQL Server
dotnet add package Microsoft.EntityFrameworkCore.SqlServer

# For PostgreSQL
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

# For MySQL
dotnet add package Pomelo.EntityFrameworkCore.MySql
```

### Step 3: Verify Installation

```bash
dotnet restore
dotnet build
```

---

## ⚡ 3. Quick Start

### Minimal Setup (3 Steps)

#### Step 1: Configure Services

```csharp
using Marventa.Framework.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add Marventa Framework
builder.Services.AddMarventaFramework(builder.Configuration, options =>
{
    options.EnableRepository = true;
    options.EnableCQRS = true;
    options.EnableValidation = true;
});

var app = builder.Build();
app.UseMarventaFramework(builder.Configuration);
app.Run();
```

#### Step 2: Create Your First Entity

```csharp
using Marventa.Framework.Core.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
```

#### Step 3: Setup DbContext

```csharp
using Marventa.Framework.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : BaseDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ITenantContext tenantContext)
        : base(options, tenantContext)
    {
    }

    public DbSet<Product> Products { get; set; }
}
```

**Done!** You now have:
- ✅ Repository pattern ready
- ✅ Soft delete enabled
- ✅ Audit tracking active
- ✅ CQRS setup complete

---

## 📁 4. Project Structure

### Recommended Structure

```
YourProject/
├── YourProject.Api/           # Web API (Controllers, Middleware)
├── YourProject.Application/   # CQRS (Commands, Queries, Handlers)
├── YourProject.Domain/        # Entities, Interfaces
├── YourProject.Infrastructure/# DbContext, Repositories
└── YourProject.Tests/         # Unit & Integration Tests
```

### Layer Responsibilities

| Layer | Responsibility | References |
|-------|---------------|------------|
| **Api** | HTTP endpoints, DTOs | Application |
| **Application** | Business logic, CQRS | Domain |
| **Domain** | Entities, interfaces | None |
| **Infrastructure** | Data access, external services | Application, Domain |

---

## 🏗️ 5. Entity Base Classes

### BaseEntity - Standard Entities

Use `BaseEntity` for regular entities that need audit tracking and soft delete.

```csharp
using Marventa.Framework.Core.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
```

**What BaseEntity Provides:**

| Property | Type | Description |
|----------|------|-------------|
| `Id` | Guid | Auto-generated unique identifier |
| `CreatedDate` | DateTime | When entity was created (UTC) |
| `UpdatedDate` | DateTime? | When entity was last updated |
| `CreatedBy` | string? | Who created this entity |
| `UpdatedBy` | string? | Who last updated this entity |
| `IsDeleted` | bool | Soft delete flag |
| `DeletedDate` | DateTime? | When entity was deleted |
| `DeletedBy` | string? | Who deleted this entity |

### AuditableEntity - With Versioning

Use when you need version tracking and optimistic concurrency.

```csharp
public class Order : AuditableEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}
```

**Additional Properties:**

| Property | Type | Description |
|----------|------|-------------|
| `Version` | string | Semantic version (e.g., "1.0", "2.1") |
| `RowVersion` | byte[] | For optimistic concurrency |

### TenantBaseEntity - Multi-Tenant

Use for multi-tenant applications.

```csharp
public class Customer : TenantBaseEntity
{
    public string CompanyName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}
```

**Additional Properties:**

| Property | Type | Description |
|----------|------|-------------|
| `TenantId` | string? | Tenant identifier for isolation |

**Automatic Features:**
- ✅ All queries automatically filtered by tenant
- ✅ No manual tenant filtering needed
- ✅ Complete data isolation

---

## 💾 6. DbContext Setup

### Basic Setup

```csharp
using Marventa.Framework.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : BaseDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ITenantContext tenantContext)
        : base(options, tenantContext)
    {
    }

    // Your entities
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // IMPORTANT: Call base first
        base.OnModelCreating(modelBuilder);

        // Your configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
```

### Register in Program.cs

```csharp
// Add database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register as base context
builder.Services.AddScoped<BaseDbContext>(provider =>
    provider.GetRequiredService<ApplicationDbContext>());
```

### Connection String

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=YourDb;User Id=sa;Password=YourPassword;TrustServerCertificate=true"
  }
}
```

### What BaseDbContext Provides

✅ **Automatic Audit Tracking** - CreatedDate, UpdatedDate filled automatically
✅ **Soft Delete** - Global query filter excludes deleted entities
✅ **Multi-Tenancy** - Automatic tenant filtering
✅ **Domain Events** - Dispatches events on SaveChanges
✅ **Optimistic Concurrency** - RowVersion handling

---

## 🗄️ 7. Repository Pattern

### Basic Usage

```csharp
public class ProductService
{
    private readonly IRepository<Product> _productRepository;

    public ProductService(IRepository<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    // Create
    public async Task<Product> CreateProductAsync(Product product)
    {
        await _productRepository.AddAsync(product);
        return product;
    }

    // Read
    public async Task<Product?> GetProductAsync(Guid id)
    {
        return await _productRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _productRepository.GetAllAsync();
    }

    // Update
    public async Task UpdateProductAsync(Product product)
    {
        await _productRepository.UpdateAsync(product);
    }

    // Delete (soft delete)
    public async Task DeleteProductAsync(Product product)
    {
        await _productRepository.DeleteAsync(product);
    }
}
```

### Advanced Queries

```csharp
public class ProductService
{
    private readonly IRepository<Product> _repository;

    // Find with conditions
    public async Task<IEnumerable<Product>> GetExpensiveProductsAsync()
    {
        return await _repository.FindAsync(p => p.Price > 1000);
    }

    // Pagination
    public async Task<IEnumerable<Product>> GetProductsPageAsync(int page, int size)
    {
        return await _repository.GetPagedAsync(page, size);
    }

    // Count
    public async Task<int> CountActiveProductsAsync()
    {
        return await _repository.CountAsync(p => p.Stock > 0);
    }

    // Any
    public async Task<bool> HasExpensiveProductsAsync()
    {
        return await _repository.AnyAsync(p => p.Price > 5000);
    }

    // Complex queries with LINQ
    public async Task<IEnumerable<Product>> SearchProductsAsync(string keyword)
    {
        return await _repository.Query()
            .Where(p => p.Name.Contains(keyword) || p.Description.Contains(keyword))
            .OrderBy(p => p.Name)
            .Take(20)
            .ToListAsync();
    }
}
```

### Repository Methods Reference

| Method | Description | Returns |
|--------|-------------|---------|
| `GetByIdAsync(id)` | Get entity by ID | Entity or null |
| `GetAllAsync()` | Get all entities | IEnumerable |
| `FindAsync(predicate)` | Filter by condition | IEnumerable |
| `GetPagedAsync(page, size)` | Get paginated results | IEnumerable |
| `AddAsync(entity)` | Add new entity | Task |
| `UpdateAsync(entity)` | Update existing entity | Task |
| `DeleteAsync(entity)` | Soft delete entity | Task |
| `CountAsync(predicate)` | Count matching entities | int |
| `AnyAsync(predicate)` | Check if any exist | bool |
| `Query()` | Get queryable for LINQ | IQueryable |

---

## 🔄 8. Unit of Work

### Why Unit of Work?

Use Unit of Work when you need **transactions across multiple repositories**.

### Example: Order Processing

```csharp
public class OrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderDto dto)
    {
        // Start transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // 1. Create order
            var order = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                CustomerId = dto.CustomerId,
                TotalAmount = dto.Items.Sum(i => i.Price * i.Quantity)
            };
            await _unitOfWork.Repository<Order>().AddAsync(order);

            // 2. Update product stock
            foreach (var item in dto.Items)
            {
                var product = await _unitOfWork.Repository<Product>()
                    .GetByIdAsync(item.ProductId);

                if (product == null)
                    throw new Exception($"Product {item.ProductId} not found");

                if (product.Stock < item.Quantity)
                    throw new Exception($"Insufficient stock for {product.Name}");

                product.Stock -= item.Quantity;
                await _unitOfWork.Repository<Product>().UpdateAsync(product);
            }

            // 3. Save all changes
            await _unitOfWork.SaveChangesAsync();

            // 4. Commit transaction
            await _unitOfWork.CommitTransactionAsync();

            return order;
        }
        catch
        {
            // Rollback on error
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
```

### Unit of Work Methods

| Method | Description |
|--------|-------------|
| `Repository<T>()` | Get repository for entity type T |
| `SaveChangesAsync()` | Save all changes to database |
| `BeginTransactionAsync()` | Start a transaction |
| `CommitTransactionAsync()` | Commit transaction |
| `RollbackTransactionAsync()` | Rollback transaction |

---

## ⚡ 9. Commands

Commands represent **write operations** (Create, Update, Delete).

### Step 1: Define Command

```csharp
using Marventa.Framework.Application.Commands;

public class CreateProductCommand : ICommand<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int InitialStock { get; set; }
}
```

### Step 2: Create Validator (Optional but Recommended)

```csharp
using FluentValidation;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Name too long");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be positive");

        RuleFor(x => x.InitialStock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative");
    }
}
```

### Step 3: Create Handler

```csharp
using MediatR;

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
            Stock = request.InitialStock
        };

        await _unitOfWork.Repository<Product>().AddAsync(product, ct);
        // TransactionBehavior automatically calls SaveChangesAsync

        return product.Id;
    }
}
```

### Step 4: Use in Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateProduct([FromBody] CreateProductCommand command)
    {
        var productId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetProduct), new { id = productId }, productId);
    }
}
```

### What Happens Automatically

When you send a command:
1. ✅ **ValidationBehavior** - Validates input using FluentValidation
2. ✅ **LoggingBehavior** - Logs execution time
3. ✅ **TransactionBehavior** - Wraps in transaction, calls SaveChanges
4. ✅ **Your Handler** - Executes business logic

---

## 🔍 10. Queries

Queries represent **read operations** (Get, List, Search).

### Step 1: Define Query

```csharp
using Marventa.Framework.Application.Queries;

public class GetProductByIdQuery : IQuery<ProductDto>
{
    public Guid Id { get; set; }
}

public class GetAllProductsQuery : IQuery<List<ProductDto>>
{
    public string? SearchTerm { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
```

### Step 2: Create DTO

```csharp
public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedDate { get; set; }
}
```

### Step 3: Create Handler

```csharp
using MediatR;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IRepository<Product> _repository;

    public GetProductByIdQueryHandler(IRepository<Product> repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(request.Id);
        if (product == null) return null;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            CreatedDate = product.CreatedDate
        };
    }
}

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
{
    private readonly IRepository<Product> _repository;

    public async Task<List<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken ct)
    {
        var query = _repository.Query();

        // Apply filters
        if (!string.IsNullOrEmpty(request.SearchTerm))
            query = query.Where(p => p.Name.Contains(request.SearchTerm));

        if (request.MinPrice.HasValue)
            query = query.Where(p => p.Price >= request.MinPrice.Value);

        if (request.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= request.MaxPrice.Value);

        // Pagination
        var products = await query
            .OrderBy(p => p.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock,
            CreatedDate = p.CreatedDate
        }).ToList();
    }
}
```

### Step 4: Use in Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
    {
        var query = new GetProductByIdQuery { Id = id };
        var product = await _mediator.Send(query);

        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetAllProducts([FromQuery] GetAllProductsQuery query)
    {
        var products = await _mediator.Send(query);
        return Ok(products);
    }
}
```

---

## ✅ 11. Validation

Validation happens **automatically** before your handler executes.

### FluentValidation Rules

```csharp
using FluentValidation;

public class UpdateProductCommand : ICommand<bool>
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be positive")
            .LessThan(1000000).WithMessage("Price is too high");
    }
}
```

### Common Validation Rules

```csharp
// Required
RuleFor(x => x.Email).NotEmpty();

// String length
RuleFor(x => x.Name).Length(3, 100);

// Numeric range
RuleFor(x => x.Age).InclusiveBetween(18, 65);

// Email format
RuleFor(x => x.Email).EmailAddress();

// Custom condition
RuleFor(x => x.Password)
    .Must(BeStrongPassword)
    .WithMessage("Password must contain uppercase, lowercase, and number");

// Conditional validation
RuleFor(x => x.CompanyName)
    .NotEmpty()
    .When(x => x.IsCompany);

// Complex object
RuleFor(x => x.Address).SetValidator(new AddressValidator());

private bool BeStrongPassword(string password)
{
    return password.Any(char.IsUpper) &&
           password.Any(char.IsLower) &&
           password.Any(char.IsDigit);
}
```

### Enable Validation

```csharp
builder.Services.AddMarventaFramework(builder.Configuration, options =>
{
    options.EnableCQRS = true;
    options.CqrsOptions.EnableValidationBehavior = true; // Enable validation
    options.CqrsOptions.Assemblies.Add(typeof(Program).Assembly);
});
```

---

## 🔧 12. Pipeline Behaviors

Behaviors wrap your handlers with cross-cutting concerns.

### Available Behaviors

| Behavior | Purpose | When Executes |
|----------|---------|---------------|
| **ValidationBehavior** | Validates input | Before handler |
| **LoggingBehavior** | Logs execution time | Before/After handler |
| **TransactionBehavior** | Manages transactions | Wraps handler |
| **IdempotencyBehavior** | Prevents duplicates | Before handler |

### Enable Behaviors

```csharp
builder.Services.AddMarventaFramework(builder.Configuration, options =>
{
    options.EnableCQRS = true;
    options.CqrsOptions.EnableValidationBehavior = true;
    options.CqrsOptions.EnableLoggingBehavior = true;
    options.CqrsOptions.EnableTransactionBehavior = true;
});
```

### What Each Behavior Does

**ValidationBehavior**
- Runs FluentValidation validators
- Returns validation errors before handler executes
- Saves database round-trips for invalid data

**LoggingBehavior**
- Logs request start/completion
- Measures execution time
- Warns if operation takes > 500ms
- Logs errors with full details

**TransactionBehavior**
- Automatically wraps commands in transactions
- Calls `SaveChangesAsync()` after handler
- Rollbacks on exceptions
- Skips queries (read-only)

**IdempotencyBehavior**
- Prevents duplicate command execution
- Uses command hash as key
- Returns cached result for duplicates

### Execution Order

```
Request
  ↓
ValidationBehavior (validates)
  ↓
LoggingBehavior (starts timer)
  ↓
TransactionBehavior (begins transaction)
  ↓
Your Handler (business logic)
  ↓
TransactionBehavior (SaveChanges, commit)
  ↓
LoggingBehavior (logs time)
  ↓
Response
```

---

## 🏢 13. Multi-Tenancy

### Step 1: Use TenantBaseEntity

```csharp
public class Customer : TenantBaseEntity
{
    public string CompanyName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class Invoice : TenantBaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
```

### Step 2: Implement ITenantContext

```csharp
using Marventa.Framework.Core.Interfaces.MultiTenancy;

public class TenantContext : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetCurrentTenantId()
    {
        // Get tenant from JWT claim
        return _httpContextAccessor.HttpContext?.User
            .FindFirst("TenantId")?.Value;

        // Or from header
        // return _httpContextAccessor.HttpContext?.Request
        //     .Headers["X-Tenant-ID"].FirstOrDefault();

        // Or from subdomain
        // var host = _httpContextAccessor.HttpContext?.Request.Host.Host;
        // return host?.Split('.').FirstOrDefault();
    }
}
```

### Step 3: Register Services

```csharp
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantContext, TenantContext>();

builder.Services.AddMarventaFramework(builder.Configuration, options =>
{
    options.EnableMultiTenancy = true;
});
```

### What Happens Automatically

```csharp
// All queries automatically filtered by tenant
var customers = await _repository.GetAllAsync();
// SELECT * FROM Customers WHERE TenantId = 'current-tenant' AND IsDeleted = 0

// New entities automatically get TenantId
var customer = new Customer { CompanyName = "Acme Corp" };
await _repository.AddAsync(customer);
// customer.TenantId is automatically set from ITenantContext
```

### Admin Queries (All Tenants)

```csharp
// To query across all tenants (admin only)
var allCustomers = await _context.Customers
    .IgnoreQueryFilters() // Skip tenant filter
    .ToListAsync();
```

---

## 🗑️ 14. Soft Delete

Entities are **never physically deleted** from the database.

### How It Works

```csharp
// Soft delete
var product = await _repository.GetByIdAsync(productId);
await _repository.DeleteAsync(product);
await _unitOfWork.SaveChangesAsync();

// Product still in database, but:
// - IsDeleted = true
// - DeletedDate = DateTime.UtcNow
// - DeletedBy = "current-user-id"

// This query won't find it (automatic filter)
var products = await _repository.GetAllAsync();
// SELECT * FROM Products WHERE IsDeleted = 0

// To include deleted (admin queries)
var allProducts = await _context.Products
    .IgnoreQueryFilters()
    .ToListAsync();
```

### Benefits

✅ Data recovery possible
✅ Audit trail preserved
✅ Referential integrity maintained
✅ Compliance with data retention laws

### Hard Delete (Permanent)

```csharp
// Only if you really need to permanently delete
var product = await _context.Products
    .IgnoreQueryFilters()
    .FirstOrDefaultAsync(p => p.Id == productId);

if (product != null)
{
    _context.Products.Remove(product); // Real delete
    await _context.SaveChangesAsync();
}
```

---

## 📝 15. Audit Tracking

Every change is **automatically tracked**.

### Automatic Tracking

```csharp
// Create
var product = new Product { Name = "Laptop", Price = 999 };
await _repository.AddAsync(product);
await _unitOfWork.SaveChangesAsync();

// Automatically filled:
// product.Id = Guid.NewGuid()
// product.CreatedDate = DateTime.UtcNow
// product.CreatedBy = "current-user-id"

// Update
product.Price = 899;
await _repository.UpdateAsync(product);
await _unitOfWork.SaveChangesAsync();

// Automatically filled:
// product.UpdatedDate = DateTime.UtcNow
// product.UpdatedBy = "current-user-id"

// Delete
await _repository.DeleteAsync(product);
await _unitOfWork.SaveChangesAsync();

// Automatically filled:
// product.IsDeleted = true
// product.DeletedDate = DateTime.UtcNow
// product.DeletedBy = "current-user-id"
```

### Set Current User

Implement `IUserContext`:

```csharp
using Marventa.Framework.Core.Interfaces.Common;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public string? GetCurrentUserName()
    {
        return _httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.Name)?.Value;
    }
}

// Register
builder.Services.AddScoped<IUserContext, UserContext>();
```

### Query Audit Trail

```csharp
// Who created this product?
Console.WriteLine($"Created by: {product.CreatedBy} on {product.CreatedDate}");

// When was it last updated?
if (product.UpdatedDate.HasValue)
    Console.WriteLine($"Updated by: {product.UpdatedBy} on {product.UpdatedDate}");

// Is it deleted?
if (product.IsDeleted)
    Console.WriteLine($"Deleted by: {product.DeletedBy} on {product.DeletedDate}");
```

---

## 🎯 16. Event Sourcing

Capture **all changes** as events.

### Step 1: Define Domain Event

```csharp
using Marventa.Framework.Core.Events;

public class ProductCreatedEvent : IDomainEvent
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}

public class ProductPriceChangedEvent : IDomainEvent
{
    public Guid ProductId { get; set; }
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}
```

### Step 2: Raise Events in Entity

```csharp
public class Product : BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public string Name { get; set; } = string.Empty;
    public decimal Price { get; private set; }

    public void ChangePrice(decimal newPrice)
    {
        if (newPrice == Price) return;

        var oldPrice = Price;
        Price = newPrice;

        // Raise event
        _domainEvents.Add(new ProductPriceChangedEvent
        {
            ProductId = Id,
            OldPrice = oldPrice,
            NewPrice = newPrice
        });
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}
```

### Step 3: Create Event Handler

```csharp
using MediatR;

public class ProductPriceChangedEventHandler : INotificationHandler<ProductPriceChangedEvent>
{
    private readonly ILogger<ProductPriceChangedEventHandler> _logger;
    private readonly IEmailService _emailService;

    public ProductPriceChangedEventHandler(
        ILogger<ProductPriceChangedEventHandler> logger,
        IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Handle(ProductPriceChangedEvent notification, CancellationToken ct)
    {
        _logger.LogInformation(
            "Product {ProductId} price changed from {OldPrice} to {NewPrice}",
            notification.ProductId, notification.OldPrice, notification.NewPrice);

        // Notify customers
        if (notification.NewPrice < notification.OldPrice)
        {
            await _emailService.SendPriceDropNotificationAsync(
                notification.ProductId,
                notification.NewPrice);
        }
    }
}
```

### Step 4: Enable Event Sourcing

```csharp
builder.Services.AddMarventaFramework(builder.Configuration, options =>
{
    options.EnableEventDriven = true;
    options.EnableEventSourcing = true;
});
```

### What Happens

1. You change product price
2. Event is raised in domain
3. SaveChanges() dispatches events
4. All handlers execute
5. Events stored in database (if enabled)

---

## 🔄 17. Saga Pattern

Manage **distributed transactions** across multiple services.

### Example: Order Processing Saga

```csharp
using Marventa.Framework.Core.Sagas;

public class OrderSaga : ISaga
{
    public Guid Id { get; set; }
    public string State { get; set; } = "Started";
    public Dictionary<string, object> Data { get; set; } = new();

    // Step 1: Reserve inventory
    public async Task<bool> ReserveInventory(IInventoryService inventory)
    {
        var orderId = (Guid)Data["OrderId"];
        var items = (List<OrderItem>)Data["Items"];

        var reserved = await inventory.ReserveAsync(orderId, items);
        if (!reserved)
        {
            State = "InventoryFailed";
            return false;
        }

        Data["InventoryReserved"] = true;
        State = "InventoryReserved";
        return true;
    }

    // Step 2: Process payment
    public async Task<bool> ProcessPayment(IPaymentService payment)
    {
        var orderId = (Guid)Data["OrderId"];
        var amount = (decimal)Data["Amount"];

        var paid = await payment.ChargeAsync(orderId, amount);
        if (!paid)
        {
            State = "PaymentFailed";
            await CompensateInventory(inventory); // Rollback
            return false;
        }

        Data["PaymentProcessed"] = true;
        State = "PaymentCompleted";
        return true;
    }

    // Step 3: Ship order
    public async Task<bool> ShipOrder(IShippingService shipping)
    {
        var orderId = (Guid)Data["OrderId"];
        var address = (string)Data["ShippingAddress"];

        var shipped = await shipping.CreateShipmentAsync(orderId, address);
        if (!shipped)
        {
            State = "ShippingFailed";
            await CompensatePayment(payment); // Refund
            await CompensateInventory(inventory); // Release
            return false;
        }

        State = "Completed";
        return true;
    }

    // Compensation: Release inventory
    private async Task CompensateInventory(IInventoryService inventory)
    {
        if (Data.ContainsKey("InventoryReserved"))
        {
            var orderId = (Guid)Data["OrderId"];
            await inventory.ReleaseAsync(orderId);
        }
    }

    // Compensation: Refund payment
    private async Task CompensatePayment(IPaymentService payment)
    {
        if (Data.ContainsKey("PaymentProcessed"))
        {
            var orderId = (Guid)Data["OrderId"];
            await payment.RefundAsync(orderId);
        }
    }
}
```

### Use Saga

```csharp
public class OrderService
{
    private readonly ISagaOrchestrator _orchestrator;

    public async Task<bool> ProcessOrderAsync(Order order)
    {
        var saga = new OrderSaga
        {
            Id = Guid.NewGuid(),
            Data = new Dictionary<string, object>
            {
                ["OrderId"] = order.Id,
                ["Items"] = order.Items,
                ["Amount"] = order.TotalAmount,
                ["ShippingAddress"] = order.ShippingAddress
            }
        };

        // Execute saga
        var result = await _orchestrator.ExecuteAsync(saga);

        if (result.IsSuccess)
        {
            order.Status = "Completed";
        }
        else
        {
            order.Status = "Failed";
            order.FailureReason = result.Error;
        }

        return result.IsSuccess;
    }
}
```

---

## 💨 18. Caching

Speed up your application with **built-in caching**.

### Memory Cache

```csharp
// Enable caching
builder.Services.AddMarventaFramework(builder.Configuration, options =>
{
    options.EnableCaching = true;
    options.CachingOptions.Provider = "Memory";
});

// Use in code
public class ProductService
{
    private readonly ICacheService _cache;
    private readonly IRepository<Product> _repository;

    public async Task<Product?> GetProductAsync(Guid id)
    {
        var cacheKey = $"product:{id}";

        // Try cache first
        var cached = await _cache.GetAsync<Product>(cacheKey);
        if (cached != null) return cached;

        // Load from database
        var product = await _repository.GetByIdAsync(id);
        if (product == null) return null;

        // Cache for 5 minutes
        await _cache.SetAsync(cacheKey, product, TimeSpan.FromMinutes(5));

        return product;
    }
}
```

### Redis Cache

```csharp
// appsettings.json
{
  "Marventa": {
    "Caching": {
      "Provider": "Redis",
      "ConnectionString": "localhost:6379"
    }
  }
}

// Program.cs
builder.Services.AddMarventaFramework(builder.Configuration, options =>
{
    options.EnableCaching = true;
    options.CachingOptions.Provider = "Redis";
});
```

### Cache Methods

```csharp
// Set
await _cache.SetAsync("key", value, TimeSpan.FromMinutes(10));

// Get
var value = await _cache.GetAsync<MyType>("key");

// Remove
await _cache.RemoveAsync("key");

// Exists
var exists = await _cache.ExistsAsync("key");

// Pattern remove
await _cache.RemoveByPatternAsync("product:*");
```

---

## 📊 19. Logging

Structured logging with **Serilog**.

### Enable Logging

```csharp
builder.Services.AddMarventaFramework(builder.Configuration, options =>
{
    options.EnableLogging = true;
    options.EnableCQRS = true;
    options.CqrsOptions.EnableLoggingBehavior = true; // Auto-log all requests
});
```

### Manual Logging

```csharp
public class ProductService
{
    private readonly ILogger<ProductService> _logger;

    public async Task<Product> CreateProductAsync(Product product)
    {
        _logger.LogInformation("Creating product {ProductName}", product.Name);

        try
        {
            await _repository.AddAsync(product);

            _logger.LogInformation(
                "Product {ProductId} created successfully",
                product.Id);

            return product;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to create product {ProductName}",
                product.Name);
            throw;
        }
    }
}
```

### Log Levels

```csharp
_logger.LogTrace("Very detailed information");
_logger.LogDebug("Debugging information");
_logger.LogInformation("General information");
_logger.LogWarning("Warning message");
_logger.LogError(exception, "Error occurred");
_logger.LogCritical(exception, "Critical error");
```

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  }
}
```

---

## ❤️ 20. Health Checks

Monitor application health.

### Enable Health Checks

```csharp
builder.Services.AddMarventaFramework(builder.Configuration, options =>
{
    options.EnableHealthChecks = true;
});

// In middleware
app.UseMarventaFramework(builder.Configuration);
// Adds endpoints: /health, /health/ready, /health/live
```

### Custom Health Check

```csharp
using Microsoft.Extensions.Diagnostics.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly ApplicationDbContext _context;

    public DatabaseHealthCheck(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken ct = default)
    {
        try
        {
            await _context.Database.CanConnectAsync(ct);
            return HealthCheckResult.Healthy("Database is accessible");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Database is not accessible",
                ex);
        }
    }
}

// Register
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database");
```

### Check Endpoints

```bash
# Overall health
GET /health

# Readiness (is app ready to serve requests?)
GET /health/ready

# Liveness (is app alive?)
GET /health/live
```

---

## 🌐 21. CDN Integration

Integrate with Azure, AWS, or CloudFlare CDN.

### Azure CDN

```csharp
// appsettings.json
{
  "Marventa": {
    "CDN": {
      "Provider": "Azure",
      "AccountName": "yourstorageaccount",
      "AccountKey": "your-key",
      "ContainerName": "cdn",
      "CdnEndpoint": "https://yourcdn.azureedge.net"
    }
  }
}

// Enable
builder.Services.AddMarventaFramework(builder.Configuration, options =>
{
    options.EnableCDN = true;
});

// Use
public class FileService
{
    private readonly ICDNService _cdn;

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var fileName = $"images/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        var url = await _cdn.UploadAsync(fileName, stream, file.ContentType);
        return url; // https://yourcdn.azureedge.net/images/abc123.jpg
    }
}
```

---

## ⚙️ 22. Feature Flags

Enable only the features you need.

### All Features

```csharp
builder.Services.AddMarventaFramework(builder.Configuration, options =>
{
    // Core Infrastructure
    options.EnableLogging = true;
    options.EnableCaching = true;
    options.EnableRepository = true;
    options.EnableHealthChecks = true;
    options.EnableValidation = true;
    options.EnableExceptionHandling = true;

    // Security
    options.EnableSecurity = true;
    options.EnableJWT = true;
    options.EnableApiKeys = true;
    options.EnableEncryption = true;

    // CQRS + MediatR
    options.EnableCQRS = true;
    options.CqrsOptions.Assemblies.Add(typeof(Program).Assembly);
    options.CqrsOptions.EnableValidationBehavior = true;
    options.CqrsOptions.EnableLoggingBehavior = true;
    options.CqrsOptions.EnableTransactionBehavior = true;
    options.CqrsOptions.EnableIdempotencyBehavior = false;

    // API Management
    options.EnableVersioning = true;
    options.EnableRateLimiting = true;
    options.EnableCompression = true;
    options.EnableCORS = true;
    options.EnableSwagger = true;

    // Monitoring
    options.EnableAnalytics = true;
    options.EnableObservability = true;
    options.EnableMetrics = true;

    // Event-Driven
    options.EnableEventDriven = true;
    options.EnableMessaging = false; // Requires RabbitMQ/Kafka
    options.EnableEventSourcing = true;
    options.EnableSagas = false; // Advanced feature

    // Multi-Tenancy
    options.EnableMultiTenancy = true;

    // File Services
    options.EnableFileService = false;
    options.EnableCDN = false;

    // Background Jobs
    options.EnableBackgroundJobs = false; // Requires Hangfire
});
```

---

## 📄 23. appsettings.json

Complete configuration example.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyApp;User Id=sa;Password=YourPassword;TrustServerCertificate=true"
  },
  "Marventa": {
    "ApiKey": "your-api-key-for-external-services",
    "Caching": {
      "Provider": "Memory",
      "ConnectionString": "localhost:6379",
      "DefaultExpiration": 300
    },
    "RateLimit": {
      "MaxRequests": 100,
      "WindowMinutes": 15
    },
    "JWT": {
      "SecretKey": "your-super-secret-key-min-32-chars",
      "Issuer": "https://yourapp.com",
      "Audience": "https://yourapp.com",
      "ExpirationMinutes": 60
    },
    "CDN": {
      "Provider": "Azure",
      "AccountName": "yourstorageaccount",
      "AccountKey": "your-key",
      "ContainerName": "cdn",
      "CdnEndpoint": "https://yourcdn.azureedge.net"
    },
    "Messaging": {
      "Provider": "RabbitMQ",
      "ConnectionString": "amqp://localhost:5672"
    }
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

---

## 🎯 24. Complete Example

Full working example of a Product Management API.

### 1. Entity

```csharp
public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
}
```

### 2. DbContext

```csharp
public class ApplicationDbContext : BaseDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ITenantContext tenantContext)
        : base(options, tenantContext) { }

    public DbSet<Product> Products { get; set; }
}
```

### 3. Commands

```csharp
// Create
public class CreateProductCommand : ICommand<Guid>
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = new Product
        {
            Name = request.Name,
            Price = request.Price,
            Stock = request.Stock
        };
        await _unitOfWork.Repository<Product>().AddAsync(product, ct);
        return product.Id;
    }
}

// Update
public class UpdateProductCommand : ICommand<bool>
{
    public Guid Id { get; set; }
    public decimal Price { get; set; }
}

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.Id);
        if (product == null) return false;

        product.Price = request.Price;
        await _unitOfWork.Repository<Product>().UpdateAsync(product);
        return true;
    }
}
```

### 4. Queries

```csharp
public class GetAllProductsQuery : IQuery<List<ProductDto>>
{
    public string? Category { get; set; }
}

public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
{
    private readonly IRepository<Product> _repository;

    public async Task<List<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken ct)
    {
        var query = _repository.Query();

        if (!string.IsNullOrEmpty(request.Category))
            query = query.Where(p => p.Category == request.Category);

        var products = await query.ToListAsync(ct);

        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Stock = p.Stock
        }).ToList();
    }
}
```

### 5. Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetAll([FromQuery] GetAllProductsQuery query)
        => Ok(await _mediator.Send(query));

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateProductCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateProductCommand command)
    {
        command.Id = id;
        var success = await _mediator.Send(command);
        return success ? NoContent() : NotFound();
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

### 6. Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Marventa Framework
builder.Services.AddMarventaFramework(builder.Configuration, options =>
{
    options.EnableRepository = true;
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

---

## ✨ 25. Best Practices

### 1. Always Use BaseEntity

```csharp
// ✅ Good
public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
}

// ❌ Bad
public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
```

### 2. Validate All Commands

```csharp
// ✅ Good - Always validate
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
    }
}

// ❌ Bad - No validation
// Command without validator = potential bad data in database
```

### 3. Use Unit of Work for Transactions

```csharp
// ✅ Good - Multiple operations in transaction
public async Task ProcessOrderAsync(Order order)
{
    await _unitOfWork.BeginTransactionAsync();
    try
    {
        await _unitOfWork.Repository<Order>().AddAsync(order);
        // Update inventory
        // Update customer credit
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitTransactionAsync();
    }
    catch
    {
        await _unitOfWork.RollbackTransactionAsync();
        throw;
    }
}

// ❌ Bad - No transaction
// If one operation fails, others are already saved = data inconsistency
```

### 4. Don't Expose Entities Directly

```csharp
// ✅ Good - Use DTOs
public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

[HttpGet("{id}")]
public async Task<ActionResult<ProductDto>> Get(Guid id)
{
    var product = await _repository.GetByIdAsync(id);
    return new ProductDto { /* map */ };
}

// ❌ Bad - Returning entities
[HttpGet("{id}")]
public async Task<ActionResult<Product>> Get(Guid id)
{
    return await _repository.GetByIdAsync(id);
    // Exposes audit fields, allows circular references, performance issues
}
```

### 5. Enable Pipeline Behaviors

```csharp
// ✅ Good - All behaviors enabled
options.CqrsOptions.EnableValidationBehavior = true;
options.CqrsOptions.EnableLoggingBehavior = true;
options.CqrsOptions.EnableTransactionBehavior = true;

// ❌ Bad - Manual validation, logging, transactions everywhere
// Duplicated code, easy to forget, inconsistent
```

### 6. Use Soft Delete

```csharp
// ✅ Good - Soft delete (default)
await _repository.DeleteAsync(product);
// Data preserved, can be restored

// ❌ Bad - Hard delete
_context.Products.Remove(product);
// Data permanently lost, no audit trail
```

### 7. Log Important Operations

```csharp
// ✅ Good - Structured logging
_logger.LogInformation(
    "Order {OrderId} created by {UserId} for {Amount:C}",
    order.Id, userId, order.TotalAmount);

// ❌ Bad - String concatenation
_logger.LogInformation($"Order {order.Id} created");
// Loses structured data, hard to query logs
```

### 8. Cache Expensive Queries

```csharp
// ✅ Good - Cache expensive operations
public async Task<List<ProductDto>> GetFeaturedProductsAsync()
{
    var cached = await _cache.GetAsync<List<ProductDto>>("featured-products");
    if (cached != null) return cached;

    var products = await _repository.Query()
        .Where(p => p.IsFeatured)
        .OrderBy(p => p.Rank)
        .Take(10)
        .ToListAsync();

    var dtos = products.Select(/* map to DTO */).ToList();
    await _cache.SetAsync("featured-products", dtos, TimeSpan.FromHours(1));
    return dtos;
}

// ❌ Bad - Query database every time
// Slow, expensive, unnecessary load
```

---

## 📞 Support & Resources

### Documentation
- 📖 [This Guide](README.md)
- 📦 [NuGet Package](https://www.nuget.org/packages/Marventa.Framework)
- 💡 [Sample Projects](samples/)

### Community
- 💬 [GitHub Discussions](https://github.com/AdemKinatas/Marventa.Framework/discussions)
- 🐛 [Report Issues](https://github.com/AdemKinatas/Marventa.Framework/issues)
- ⭐ [Star on GitHub](https://github.com/AdemKinatas/Marventa.Framework)

### Contact
- 📧 Email: ademkinatas@gmail.com
- 🌐 GitHub: [@AdemKinatas](https://github.com/AdemKinatas)

---

## 📄 License

MIT License - Free for personal and commercial use.

---

<div align="center">

**Built with ❤️ by Adem Kınataş**

⭐ **Star us on GitHub** if this helped you!

[NuGet](https://www.nuget.org/packages/Marventa.Framework) •
[GitHub](https://github.com/AdemKinatas/Marventa.Framework) •
[Issues](https://github.com/AdemKinatas/Marventa.Framework/issues)

</div>