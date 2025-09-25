# Marventa.Framework

A comprehensive .NET 9.0 enterprise e-commerce framework with multi-tenancy, JWT authentication, CQRS, messaging infrastructure, and complete e-commerce domain modules.

## 🚀 Features

- **🏢 Multi-Tenancy** - Complete tenant isolation with policy-based authorization
- **🔐 JWT Authentication** - Token-based authentication and authorization
- **📋 CQRS & MediatR** - Command Query Responsibility Segregation
- **💰 Payment Processing** - Complete payment domain with events
- **📦 Shipping Management** - End-to-end shipping lifecycle
- **🔄 Saga Patterns** - MassTransit state machine orchestration
- **📨 Messaging** - RabbitMQ+MassTransit and Kafka support
- **📤 Outbox/Inbox** - Reliable messaging patterns
- **💵 Money/Currency** - Multi-currency value objects
- **🗄️ Caching** - Redis and in-memory caching with tenant scoping
- **🚦 Rate Limiting** - Tenant-aware rate limiting
- **🏥 Health Checks** - Database and service monitoring
- **📊 API Versioning** - Multiple versioning strategies
- **🔒 Security** - Encryption, data protection, CORS
- **📧 Communication** - Email and SMS services
- **⚡ Circuit Breaker** - HTTP resilience patterns
- **🎛️ Feature Flags** - Dynamic feature toggles
- **📝 Validation** - FluentValidation with RFC 7807 Problem Details
- **🔍 Observability** - OpenTelemetry tracing and metrics

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
  },
  "MultiTenancy": {
    "ResolutionStrategy": "Header",
    "HeaderName": "X-Tenant-Id"
  }
}
```

## Usage Examples

### JWT Authentication

```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var token = await _tokenService.GenerateAccessTokenAsync(claims);
        return Ok(new { Token = token });
    }
}
```

### Multi-Tenant CQRS

```csharp
// Command
public record CreateProductCommand(string Name, decimal Price) : ICommand<Guid>;

// Handler
public class CreateProductHandler : ICommandHandler<CreateProductCommand, Guid>
{
    private readonly ITenantContext _tenantContext;
    private readonly IRepository<Product> _repository;

    public async Task<Guid> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = command.Name,
            Price = new Money(command.Price, Currency.USD),
            TenantId = _tenantContext.TenantId
        };

        await _repository.AddAsync(product);
        return product.Id;
    }
}
```

### Payment Processing

```csharp
public class PaymentService
{
    private readonly IRepository<Payment> _paymentRepository;

    public async Task<Payment> ProcessPaymentAsync(decimal amount, Currency currency, string orderId)
    {
        var payment = new Payment(
            orderId: orderId,
            amount: new Money(amount, currency),
            method: PaymentMethod.CreditCard
        );

        payment.MarkAsProcessing();
        await _paymentRepository.AddAsync(payment);

        // Domain event will be automatically dispatched
        payment.MarkAsSuccessful("gateway-transaction-id");

        return payment;
    }
}
```

### Saga Pattern

```csharp
public class OrderSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string OrderId { get; set; }
    public bool PaymentProcessed { get; set; }
    public bool InventoryReserved { get; set; }
    public bool ShippingArranged { get; set; }
}

public class OrderSagaStateMachine : MassTransitStateMachine<OrderSagaState>
{
    public OrderSagaStateMachine()
    {
        Initially(
            When(OrderSubmitted)
                .Then(context => context.Saga.OrderId = context.Message.OrderId.ToString())
                .Publish(context => new ProcessPaymentCommand { OrderId = context.Message.OrderId })
        );
    }
}
```

## Architecture

The framework follows Clean Architecture principles with these layers:

- **Core** - Domain entities, interfaces, and shared abstractions
- **Domain** - Business logic, aggregates, and domain events
- **Application** - CQRS handlers, validators, and application services
- **Infrastructure** - External service implementations (database, messaging, caching)
- **Web** - Controllers, middleware, and API concerns

## Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/my-feature`
3. Commit your changes: `git commit -am 'Add my feature'`
4. Push to the branch: `git push origin feature/my-feature`
5. Submit a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

- Documentation: [GitHub Wiki](https://github.com/AdemKinatas/Marventa.Framework/wiki)
- Issues: [GitHub Issues](https://github.com/AdemKinatas/Marventa.Framework/issues)
- Discussions: [GitHub Discussions](https://github.com/AdemKinatas/Marventa.Framework/discussions)

---

Built with ❤️ for .NET developers by [Adem Kınataş](https://github.com/AdemKinatas)