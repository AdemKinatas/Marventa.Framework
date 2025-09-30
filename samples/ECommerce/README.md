# E-Commerce Sample Application

A complete e-commerce platform built with Marventa Framework demonstrating advanced features.

## Features

✅ **Product Catalog** - Browse and search products
✅ **Shopping Cart** - Add/remove items, update quantities
✅ **Order Management** - Create orders, track status
✅ **Payment Processing** - Payment integration
✅ **User Management** - Customer accounts and authentication
✅ **Admin Dashboard** - Manage products, orders, customers
✅ **Multi-tenancy** - Support multiple stores
✅ **Event-Driven** - Domain events for order processing
✅ **Caching** - Redis caching for product catalog
✅ **Background Jobs** - Order processing, email notifications

## Architecture

```
ECommerce/
├── src/
│   ├── Products/
│   │   ├── Commands/
│   │   ├── Queries/
│   │   ├── Entities/
│   │   └── Events/
│   ├── Orders/
│   │   ├── Commands/
│   │   ├── Queries/
│   │   ├── Entities/
│   │   └── Events/
│   ├── Customers/
│   ├── Payments/
│   ├── Shared/
│   │   ├── DTOs/
│   │   ├── ValueObjects/
│   │   └── Specifications/
│   └── Infrastructure/
│       ├── Data/
│       ├── Services/
│       └── BackgroundJobs/
└── tests/
    ├── Unit/
    └── Integration/
```

## Domain Models

### Product Aggregate

```csharp
public class Product : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Money Price { get; private set; }
    public int Stock { get; private set; }
    public string Category { get; private set; }
    public string ImageUrl { get; private set; }

    public void UpdatePrice(Money newPrice)
    {
        if (newPrice.Amount <= 0)
            throw new InvalidOperationException("Price must be positive");

        Price = newPrice;
        AddDomainEvent(new ProductPriceChangedEvent(Id, Price));
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity > Stock)
            throw new InvalidOperationException("Insufficient stock");

        Stock -= quantity;
        AddDomainEvent(new ProductStockDecreasedEvent(Id, quantity));
    }
}
```

### Order Aggregate

```csharp
public class Order : AggregateRoot
{
    public string OrderNumber { get; private set; }
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public Money TotalAmount { get; private set; }
    public List<OrderItem> Items { get; private set; } = new();

    public void AddItem(Guid productId, string productName, Money price, int quantity)
    {
        var item = new OrderItem(productId, productName, price, quantity);
        Items.Add(item);
        RecalculateTotalAmount();
        AddDomainEvent(new OrderItemAddedEvent(Id, item));
    }

    public void PlaceOrder()
    {
        if (Items.Count == 0)
            throw new InvalidOperationException("Cannot place order with no items");

        Status = OrderStatus.Placed;
        AddDomainEvent(new OrderPlacedEvent(Id, CustomerId, TotalAmount));
    }
}
```

## Key Use Cases

### 1. Create Order Flow

```
Customer places order
  ↓
CreateOrderCommand → ValidationBehavior
  ↓
CreateOrderCommandHandler
  ↓
Order.PlaceOrder() → OrderPlacedEvent
  ↓
TransactionBehavior → SaveChanges
  ↓
OrderPlacedEventHandler
  ├── Decrease product stock
  ├── Send confirmation email
  └── Create payment record
```

### 2. Domain Events

```csharp
public class OrderPlacedEventHandler : INotificationHandler<OrderPlacedEvent>
{
    private readonly IRepository<Product> _productRepository;
    private readonly IEmailService _emailService;
    private readonly IPaymentService _paymentService;

    public async Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
    {
        // Decrease stock for all ordered products
        foreach (var item in notification.OrderItems)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            product.DecreaseStock(item.Quantity);
        }

        // Send confirmation email
        await _emailService.SendOrderConfirmationAsync(notification.CustomerId, notification.OrderId);

        // Process payment
        await _paymentService.ProcessPaymentAsync(notification.OrderId, notification.TotalAmount);
    }
}
```

### 3. Advanced Queries with Specifications

```csharp
public class ProductsByCategorySpec : BaseSpecification<Product>
{
    public ProductsByCategorySpec(string category, ProductFilter filter)
    {
        // Filtering
        Criteria = p => p.Category == category && p.Stock > 0 && !p.IsDeleted;

        // Price range filter
        if (filter.MinPrice.HasValue)
            ApplyAnd(p => p.Price.Amount >= filter.MinPrice.Value);

        if (filter.MaxPrice.HasValue)
            ApplyAnd(p => p.Price.Amount <= filter.MaxPrice.Value);

        // Eager loading
        AddInclude(p => p.Reviews);
        AddInclude(p => p.Category);

        // Sorting
        switch (filter.SortBy)
        {
            case "price":
                ApplyOrderBy(p => p.Price.Amount);
                break;
            case "name":
                ApplyOrderBy(p => p.Name);
                break;
            default:
                ApplyOrderByDescending(p => p.CreatedDate);
                break;
        }

        // Pagination
        ApplyPaging(filter.PageNumber, filter.PageSize);
    }
}

// Usage
var spec = new ProductsByCategorySpec("Electronics", filter);
var products = await _repository.GetWithSpecificationAsync(spec);
```

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ECommerceDb;",
    "Redis": "localhost:6379"
  },
  "Marventa": {
    "ApiKey": "your-api-key-here",
    "RateLimit": {
      "MaxRequests": 1000,
      "WindowMinutes": 15
    },
    "Caching": {
      "Provider": "Redis"
    },
    "Storage": {
      "Provider": "AzureBlob",
      "ConnectionString": "your-azure-storage-connection"
    }
  },
  "Email": {
    "Provider": "SendGrid",
    "ApiKey": "your-sendgrid-api-key",
    "FromEmail": "noreply@yourecommerce.com"
  },
  "Payment": {
    "Provider": "Stripe",
    "SecretKey": "your-stripe-secret-key"
  }
}
```

### Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add DbContext with multi-tenancy
builder.Services.AddDbContext<ECommerceDbContext>((sp, options) =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add Marventa Framework with full features
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
    options.EnableEncryption = true;

    // Communication
    options.EnableEmail = true;
    options.EnableHttpClient = true;

    // Data & Storage
    options.EnableStorage = true;
    options.EnableFileProcessor = true;

    // API Management
    options.EnableVersioning = true;
    options.EnableRateLimiting = true;
    options.EnableCompression = true;
    options.EnableIdempotency = true;

    // Performance
    options.EnableCircuitBreaker = true;
    options.EnableAdvancedCaching = true;

    // Enterprise Architecture
    options.EnableMultiTenancy = true;
    options.EnableEventDriven = true;
    options.EnableCQRS = true;

    // CQRS Configuration
    options.CqrsOptions.Assemblies.Add(typeof(Program).Assembly);
    options.CqrsOptions.EnableValidationBehavior = true;
    options.CqrsOptions.EnableLoggingBehavior = true;
    options.CqrsOptions.EnableTransactionBehavior = true;

    // Background Jobs
    options.EnableBackgroundJobs = true;
    options.EnableMessaging = true;
});

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
    await seeder.SeedAsync();
}

app.Run();
```

## API Endpoints

### Products

```http
GET    /api/v1/products
GET    /api/v1/products/{id}
POST   /api/v1/products
PUT    /api/v1/products/{id}
DELETE /api/v1/products/{id}
GET    /api/v1/products/category/{category}
GET    /api/v1/products/search?q={query}
```

### Orders

```http
GET    /api/v1/orders
GET    /api/v1/orders/{id}
POST   /api/v1/orders
PUT    /api/v1/orders/{id}/status
GET    /api/v1/orders/customer/{customerId}
```

### Shopping Cart

```http
GET    /api/v1/cart
POST   /api/v1/cart/items
PUT    /api/v1/cart/items/{itemId}
DELETE /api/v1/cart/items/{itemId}
POST   /api/v1/cart/checkout
```

## Testing

The sample includes comprehensive tests:

```bash
# Run all tests
dotnet test

# Run unit tests only
dotnet test --filter Category=Unit

# Run integration tests
dotnet test --filter Category=Integration

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## Performance

- Product catalog queries: **< 50ms** (with Redis caching)
- Order creation: **< 200ms** (including event processing)
- Payment processing: **< 500ms** (with circuit breaker)
- Handles: **10,000+ requests/min** per instance

## Learn More

- [Main Documentation](../../README.md)
- [Multi-tenancy Guide](../../docs/features/multi-tenancy.md)
- [Event-Driven Architecture](../../docs/architecture/event-driven.md)
- [Domain-Driven Design](../../docs/architecture/ddd-patterns.md)