# Marventa Framework

Enterprise-grade .NET framework with Clean Architecture, CQRS, and 47+ modular features for .NET 8.0/9.0

[![NuGet](https://img.shields.io/nuget/v/Marventa.Framework.svg)](https://www.nuget.org/packages/Marventa.Framework/)
[![Downloads](https://img.shields.io/nuget/dt/Marventa.Framework.svg)](https://www.nuget.org/packages/Marventa.Framework/)

## What's New in v3.5.2

🔧 **CRITICAL PACKAGING FIX - True Single Package**

- ✅ **FIXED**: No more NU1102 errors - works globally!
- ✅ **FIXED**: All 5 framework DLLs embedded (Core, Domain, Application, Infrastructure, Web)
- ✅ **FIXED**: All dependencies included (Redis, Serilog, EF Core, MediatR, Hangfire, etc.)
- ✅ **WORKS WORLDWIDE**: Install from anywhere - China, Europe, US, anywhere!

**Just one command:**
```bash
dotnet add package Marventa.Framework
```

Everything included - no sub-packages, no manual dependencies!

## Quick Start

### Installation
```bash
dotnet add package Marventa.Framework
# That's it! All dependencies included automatically.
```

### Migrating from v3.5.1 or earlier?
```bash
# Just update - no breaking changes
dotnet add package Marventa.Framework
dotnet clean
dotnet restore
dotnet build
```

### Basic Setup
```csharp
using Marventa.Framework.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMarventaFramework(builder.Configuration, options =>
{
    options.EnableCQRS = true;
    options.EnableRepository = true;
    options.EnableLogging = true;
    options.EnableCaching = true;
});

var app = builder.Build();
app.UseMarventaFramework(builder.Configuration);
app.Run();
```

## Core Features

**Base Entity Classes**
- `BaseEntity` - Audit tracking, soft delete, timestamps
- `AuditableEntity` - Version control and concurrency
- `TenantBaseEntity` - Multi-tenant isolation

**CQRS with MediatR**
- Automatic validation with FluentValidation
- Transaction management
- Logging and performance tracking
- Idempotency support

**Repository Pattern**
- Generic repository with common operations
- Unit of Work pattern
- Specification pattern support
- Soft delete handling

**Pipeline Behaviors**
- `ValidationBehavior` - Automatic input validation
- `LoggingBehavior` - Performance monitoring
- `TransactionBehavior` - Automatic transaction management
- `IdempotencyBehavior` - Duplicate prevention (implemented, manual registration required)

## Example: Product CRUD

**Entity**
```csharp
public class Product : BaseEntity
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

**Command**
```csharp
public class CreateProductCommand : ICommand<Guid>
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = new Product { Name = request.Name, Price = request.Price };
        await _unitOfWork.Repository<Product>().AddAsync(product, ct);
        return product.Id;
    }
}
```

**Query**
```csharp
public class GetProductByIdQuery : IQuery<ProductDto>
{
    public Guid Id { get; set; }
}

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IRepository<Product> _repository;

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(request.Id);
        return new ProductDto { Id = product.Id, Name = product.Name, Price = product.Price };
    }
}
```

**Controller**
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> Get(Guid id)
        => Ok(await _mediator.Send(new GetProductByIdQuery { Id = id }));

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateProductCommand command)
        => CreatedAtAction(nameof(Get), new { id = await _mediator.Send(command) }, null);
}
```

## Advanced Features

**Multi-Tenancy**
```csharp
public class Customer : TenantBaseEntity
{
    public string CompanyName { get; set; }
}
```

**Event Sourcing**
```csharp
public class OrderCreatedEvent : IDomainEvent
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
}
```

**Saga Pattern**
```csharp
public class OrderSaga : ISaga<OrderSagaState>
{
    public async Task ExecuteAsync(OrderSagaState state, CancellationToken ct)
    {
        // Distributed transaction logic
    }
}
```

**CDN Integration**
```csharp
public class FileService
{
    private readonly IMarventaCDN _cdn;

    public async Task<string> UploadAsync(Stream file)
        => await _cdn.UploadAsync(file, new CDNUploadOptions());
}
```

## Feature Status (v3.5.2)

✅ **Production Ready (27 features)**
- BaseDbContext, Repository, Unit of Work, CQRS, Saga, Outbox/Inbox, Projections
- Caching (Memory/Redis), Elasticsearch
- Email, SMS, Storage (Local/Cloud), CDN (Azure/AWS/CloudFlare)
- JWT, Encryption, Multi-tenancy, Health Checks

⚠️ **Mock/Development (6 features)**
- ML Service, Analytics, Mock CDN/Storage (for testing)

🚧 **Roadmap (14 features)**
- Event Sourcing (infrastructure ready), Background Jobs, E-commerce features

**See full README** for detailed feature breakdown and usage examples.

## Resources

- **Documentation**: https://github.com/AdemKinatas/Marventa.Framework#readme
- **GitHub**: https://github.com/AdemKinatas/Marventa.Framework
- **Issues**: https://github.com/AdemKinatas/Marventa.Framework/issues
- **Email**: ademkinatas@gmail.com

## License

MIT License - Free for personal and commercial use.

Built with love by Adem Kinatas
