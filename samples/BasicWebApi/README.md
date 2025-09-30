# Basic Web API Sample

This sample demonstrates a simple CRUD API using Marventa Framework with CQRS pattern.

## Features Demonstrated

✅ **CQRS with MediatR** - Commands and Queries
✅ **Repository Pattern** - Data access abstraction
✅ **Validation** - FluentValidation integration
✅ **API Response** - Consistent response format
✅ **Caching** - Memory caching for queries
✅ **Health Checks** - Built-in health monitoring

## Project Structure

```
BasicWebApi/
├── Commands/
│   ├── CreateProductCommand.cs
│   ├── UpdateProductCommand.cs
│   └── DeleteProductCommand.cs
├── Queries/
│   ├── GetProductByIdQuery.cs
│   └── GetAllProductsQuery.cs
├── Validators/
│   └── CreateProductCommandValidator.cs
├── DTOs/
│   └── ProductDto.cs
├── Entities/
│   └── Product.cs
├── Controllers/
│   └── ProductsController.cs
├── Data/
│   └── ApplicationDbContext.cs
├── Program.cs
└── appsettings.json
```

## Getting Started

### 1. Install Dependencies

```bash
dotnet restore
```

### 2. Update Database

```bash
dotnet ef database update
```

### 3. Run the Application

```bash
dotnet run
```

The API will be available at `https://localhost:5001`

## API Endpoints

### Create Product
```http
POST /api/products
Content-Type: application/json

{
  "name": "Laptop",
  "price": 999.99,
  "stock": 10
}
```

### Get Product
```http
GET /api/products/{id}
```

### Get All Products
```http
GET /api/products
```

### Update Product
```http
PUT /api/products/{id}
Content-Type: application/json

{
  "name": "Updated Laptop",
  "price": 899.99,
  "stock": 15
}
```

### Delete Product
```http
DELETE /api/products/{id}
```

## Key Concepts

### 1. Command Example

```csharp
public class CreateProductCommand : ICommand<ApiResponse<ProductDto>>
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ApiResponse<ProductDto>>
{
    private readonly IRepository<Product> _repository;
    private readonly IMapper _mapper;

    public async Task<ApiResponse<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Price = request.Price,
            Stock = request.Stock
        };

        await _repository.AddAsync(product, cancellationToken);

        return ApiResponse<ProductDto>.SuccessResult(_mapper.Map<ProductDto>(product));
    }
}
```

### 2. Query Example

```csharp
public class GetProductByIdQuery : IQuery<ApiResponse<ProductDto>>
{
    public Guid Id { get; set; }
}

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ApiResponse<ProductDto>>
{
    private readonly IRepository<Product> _repository;

    public async Task<ApiResponse<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (product == null)
            return ApiResponse<ProductDto>.FailureResult("Product not found");

        return ApiResponse<ProductDto>.SuccessResult(_mapper.Map<ProductDto>(product));
    }
}
```

### 3. Validation Example

```csharp
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative");
    }
}
```

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BasicWebApiDb;Trusted_Connection=True;"
  },
  "Marventa": {
    "ApiKey": "your-api-key-here",
    "Caching": {
      "Provider": "Memory"
    }
  }
}
```

### Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Marventa Framework
builder.Services.AddMarventaFramework(builder.Configuration, options =>
{
    options.EnableCQRS = true;
    options.EnableCaching = true;
    options.EnableValidation = true;
    options.EnableHealthChecks = true;
    options.EnableExceptionHandling = true;

    options.CqrsOptions.Assemblies.Add(typeof(Program).Assembly);
});

// Add controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
```

## Testing

Run the tests:
```bash
dotnet test
```

## Learn More

- [Marventa Framework Documentation](../../README.md)
- [CQRS Pattern Guide](../../docs/features/cqrs.md)
- [Repository Pattern Guide](../../docs/architecture/repository-pattern.md)