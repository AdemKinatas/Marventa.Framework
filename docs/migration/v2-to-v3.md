# Migration Guide: v2.x to v3.x

This guide will help you migrate from Marventa Framework v2.x to v3.x.

## Overview

Version 3.0 introduces significant architectural improvements while maintaining backward compatibility where possible. This guide covers all breaking changes and new features.

## Breaking Changes

### 1. Namespace Reorganization

**Old (v2.x):**
```csharp
using Marventa.Framework.Interfaces;
```

**New (v3.x):**
```csharp
using Marventa.Framework.Core.Interfaces.Data;
using Marventa.Framework.Core.Interfaces.Messaging;
using Marventa.Framework.Core.Interfaces.Sagas;
```

**Migration Steps:**
1. Update all `using` statements
2. Use IDE's "Find and Replace" for bulk updates
3. Common mappings:
   - `IRepository` → `Marventa.Framework.Core.Interfaces.Data.IRepository`
   - `IUnitOfWork` → `Marventa.Framework.Core.Interfaces.Data.IUnitOfWork`
   - `IMessageBus` → `Marventa.Framework.Core.Interfaces.Messaging.IMessageBus`

### 2. BaseDbContext Changes

**Old (v2.x):**
```csharp
public class ApplicationDbContext : DbContext
{
    // Manual audit tracking
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Manual implementation
    }
}
```

**New (v3.x):**
```csharp
public class ApplicationDbContext : BaseDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ITenantContext tenantContext)
        : base(options, tenantContext)
    {
    }

    // Automatic audit tracking, soft delete, multi-tenancy
}
```

**Migration Steps:**
1. Change base class from `DbContext` to `BaseDbContext`
2. Add `ITenantContext` parameter to constructor
3. Remove manual audit tracking code
4. Remove manual soft delete implementation
5. Update `SaveChangesAsync` overrides (if any)

### 3. Repository Pattern Updates

**Old (v2.x):**
```csharp
public class ProductRepository : IRepository<Product>
{
    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products.FindAsync(id);
    }
}
```

**New (v3.x):**
```csharp
public class ProductRepository : BaseRepository<Product>
{
    public ProductRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    // BaseRepository provides all CRUD operations
    // Add only custom methods here
}
```

**Migration Steps:**
1. Inherit from `BaseRepository<T>` instead of implementing `IRepository<T>`
2. Remove boilerplate CRUD methods
3. Keep only custom repository methods

### 4. CQRS Configuration

**Old (v2.x):**
```csharp
// No built-in CQRS support
services.AddMediatR(typeof(Program).Assembly);
services.AddValidatorsFromAssembly(typeof(Program).Assembly);
```

**New (v3.x):**
```csharp
builder.Services.AddMarventaFramework(configuration, options =>
{
    options.EnableCQRS = true;
    options.CqrsOptions.Assemblies.Add(typeof(Program).Assembly);
    options.CqrsOptions.EnableValidationBehavior = true;
    options.CqrsOptions.EnableLoggingBehavior = true;
    options.CqrsOptions.EnableTransactionBehavior = true;
});
```

**Migration Steps:**
1. Remove manual MediatR registration
2. Enable CQRS in framework options
3. Configure pipeline behaviors
4. Update command/query interfaces

## New Features in v3.x

### 1. BaseDbContext

Automatic features:
- ✅ Audit tracking (CreatedDate, UpdatedDate)
- ✅ Soft delete with global query filters
- ✅ Multi-tenancy support
- ✅ Domain event dispatching

**Usage:**
```csharp
public class ApplicationDbContext : BaseDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ITenantContext tenantContext)
        : base(options, tenantContext)
    {
    }

    public DbSet<Product> Products { get; set; }

    protected override async Task PublishDomainEventAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        // Optional: Custom domain event publishing
        await _eventBus.PublishAsync(domainEvent, cancellationToken);
    }
}
```

### 2. MediatR Pipeline Behaviors

Three new behaviors automatically applied:

**ValidationBehavior:**
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

**LoggingBehavior:**
- Automatic performance monitoring
- Logs all requests with execution time
- Warns about slow operations (>500ms)

**TransactionBehavior:**
- Automatic transaction management for commands
- Automatic `SaveChangesAsync()` on success
- Rollback on exceptions

### 3. Enhanced Repository Pattern

**Specifications:**
```csharp
public class ProductsInCategorySpec : BaseSpecification<Product>
{
    public ProductsInCategorySpec(string category)
    {
        Criteria = p => p.Category == category && !p.IsDeleted;
        AddInclude(p => p.Reviews);
        ApplyOrderByDescending(p => p.CreatedDate);
        ApplyPaging(0, 20);
    }
}

// Usage
var spec = new ProductsInCategorySpec("Electronics");
var products = await _repository.GetWithSpecificationAsync(spec);
```

### 4. Organized Interfaces

88 types reorganized into 17 domain-specific namespaces:

```
Marventa.Framework.Core.Interfaces.
├── Data/              # Repository, UnitOfWork, DatabaseSeeder
├── Messaging/         # MessageBus, Handlers
├── Sagas/             # Saga orchestration
├── Projections/       # CQRS read models
├── Storage/           # Storage, CDN
├── Services/          # Email, SMS, Search
├── MultiTenancy/      # Tenant management
├── Caching/           # Cache services
├── Security/          # JWT, Encryption
└── ... (13 more namespaces)
```

## Step-by-Step Migration

### Step 1: Update NuGet Package

```bash
dotnet add package Marventa.Framework --version 3.2.0
dotnet restore
```

### Step 2: Update Namespaces

Create a mapping file for Find & Replace:

| Old | New |
|-----|-----|
| `using Marventa.Framework.Interfaces;` | `using Marventa.Framework.Core.Interfaces.Data;` |
| `using Marventa.Framework.Data;` | `using Marventa.Framework.Infrastructure.Data;` |

### Step 3: Update DbContext

```csharp
// Before
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}

// After
public class ApplicationDbContext : BaseDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ITenantContext tenantContext)
        : base(options, tenantContext)
    {
    }
}
```

### Step 4: Update Repositories

```csharp
// Before
public class ProductRepository : IRepository<Product>
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products.FindAsync(id);
    }

    // ... more boilerplate CRUD methods
}

// After
public class ProductRepository : BaseRepository<Product>
{
    public ProductRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    // Only custom methods needed
    public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
    {
        return await Query()
            .Where(p => p.IsFeatured)
            .OrderByDescending(p => p.Rating)
            .Take(10)
            .ToListAsync();
    }
}
```

### Step 5: Enable CQRS (Optional)

```csharp
builder.Services.AddMarventaFramework(builder.Configuration, options =>
{
    options.EnableCQRS = true;
    options.CqrsOptions.Assemblies.Add(typeof(Program).Assembly);
});
```

### Step 6: Test Everything

```bash
dotnet test
```

## Common Issues

### Issue: Namespace not found

**Error:** `The type or namespace name 'IRepository' could not be found`

**Solution:** Update namespace:
```csharp
using Marventa.Framework.Core.Interfaces.Data;
```

### Issue: BaseDbContext requires ITenantContext

**Error:** `There is no argument given that corresponds to the required parameter 'tenantContext'`

**Solution:** Add ITenantContext to constructor:
```csharp
public ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    ITenantContext tenantContext)
    : base(options, tenantContext)
{
}
```

If not using multi-tenancy:
```csharp
options.EnableMultiTenancy = false;
```

### Issue: ValidationException not handled

**Error:** Unhandled `FluentValidation.ValidationException`

**Solution:** Enable exception handling:
```csharp
options.EnableExceptionHandling = true;
```

## Rollback Plan

If you need to rollback to v2.x:

```bash
# 1. Revert code changes
git checkout <previous-commit>

# 2. Downgrade package
dotnet add package Marventa.Framework --version 2.8.0

# 3. Restore
dotnet restore

# 4. Test
dotnet test
```

## Need Help?

- 💬 [GitHub Discussions](https://github.com/AdemKinatas/Marventa.Framework/discussions)
- 🐛 [Report Issues](https://github.com/AdemKinatas/Marventa.Framework/issues)
- 📧 Email: ademkinatas@gmail.com

## What's Next?

- [Explore New Features](../features/)
- [Read Architecture Guide](../architecture/)
- [Check Code Samples](../../samples/)
- [Review Breaking Changes](breaking-changes.md)