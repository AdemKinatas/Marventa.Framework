# Serilog Complete Implementation Guide

## ✅ Serilog Fully Implemented!

The Marventa.Framework now has a complete, production-ready Serilog implementation with:

### Features Implemented
- ✅ **Structured logging** - Full structured logging support
- ✅ **Multiple sinks** - Console, File, Elasticsearch, Seq
- ✅ **Enrichers** - CorrelationId, Tenant, Environment, Thread, Machine
- ✅ **Request logging** - Automatic HTTP request/response logging
- ✅ **Performance logging** - Built-in performance measurement
- ✅ **Exception details** - Detailed exception logging with stack traces
- ✅ **Multi-tenant support** - Tenant-aware logging

## Configuration Examples

### 1. appsettings.json Configuration

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning",
        "System": "Warning"
      }
    },
    "FilePath": "logs/log-.txt",
    "Elasticsearch": {
      "Url": "http://localhost:9200"
    },
    "Seq": {
      "Url": "http://localhost:5341",
      "ApiKey": "your-api-key"
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "theme": "Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme::Code, Serilog.Sinks.Console"
        }
      }
    ],
    "Enrich": ["FromLogContext", "WithMachineName", "WithThreadId"]
  }
}
```

### 2. Program.cs Setup

```csharp
using Marventa.Framework.Infrastructure.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.AddMarventaSerilog();

// Add services
builder.Services.AddControllers();
// ... other services

var app = builder.Build();

// Add Serilog request logging
app.UseMarventaSerilogRequestLogging();

// Add custom logging middleware (optional)
app.UseMiddleware<LoggingMiddleware>();

// ... other middleware

app.Run();
```

### 3. Using Structured Logging in Services

```csharp
using Marventa.Framework.Infrastructure.Logging;

public class OrderService
{
    private readonly ILogger<OrderService> _logger;

    public OrderService(ILogger<OrderService> logger)
    {
        _logger = logger;
    }

    // Basic structured logging
    public async Task<Order> CreateOrderAsync(CreateOrderDto dto)
    {
        _logger.LogInformation("Creating order for customer {CustomerId} with {ItemCount} items",
            dto.CustomerId, dto.Items.Count);

        try
        {
            var order = await ProcessOrder(dto);

            _logger.LogInformation("Order {OrderId} created successfully for customer {CustomerId}",
                order.Id, order.CustomerId);

            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create order for customer {CustomerId}", dto.CustomerId);
            throw;
        }
    }

    // Using performance logging
    public async Task<Order> GetOrderWithPerformanceAsync(int orderId)
    {
        using var perfScope = _logger.BeginPerformanceScope("GetOrder", thresholdMs: 100)
            .AddProperty("OrderId", orderId);

        var order = await _repository.GetByIdAsync(orderId);
        return order;
        // Automatically logs if operation takes > 100ms
    }

    // Using method entry/exit logging
    public async Task ProcessPaymentAsync(PaymentDto payment)
    {
        using (_logger.LogMethodEntry(new { payment.OrderId, payment.Amount }))
        {
            // Method implementation
            await _paymentService.ProcessAsync(payment);
        }
        // Automatically logs method exit with duration
    }

    // Using pre-defined structured log methods
    public async Task<Product> GetProductFromCacheAsync(int productId)
    {
        var cacheKey = $"product:{productId}";

        var cached = await _cache.GetAsync<Product>(cacheKey);
        if (cached != null)
        {
            _logger.LogCacheHit(cacheKey, "Redis");
            return cached;
        }

        _logger.LogCacheMiss(cacheKey, "Redis");

        var product = await _repository.GetProductAsync(productId);
        await _cache.SetAsync(cacheKey, product);

        return product;
    }
}
```

### 4. Event Bus Integration

```csharp
public class EventPublisher
{
    private readonly ILogger<EventPublisher> _logger;

    public async Task PublishAsync<T>(T @event) where T : IIntegrationEvent
    {
        var correlationId = Guid.NewGuid().ToString();

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            _logger.LogEventPublished(
                typeof(T).Name,
                "RabbitMQ",
                correlationId);

            await _bus.PublishAsync(@event);
        }
    }
}
```

### 5. Database Query Logging

```csharp
public class Repository<T> : IRepository<T>
{
    private readonly ILogger<Repository<T>> _logger;
    private readonly DbContext _context;

    public async Task<T> GetByIdAsync(int id)
    {
        var query = $"SELECT * FROM {typeof(T).Name} WHERE Id = {id}";

        _logger.LogDatabaseQueryExecuting(query, _context.Database.GetDbConnection().Database);

        var sw = Stopwatch.StartNew();
        var result = await _context.Set<T>().FindAsync(id);
        sw.Stop();

        _logger.LogDatabaseQueryExecuted(
            _context.Database.GetDbConnection().Database,
            sw.ElapsedMilliseconds);

        return result;
    }
}
```

## Log Output Examples

### Console (Development)
```
[14:23:45 INF] OrderService: Creating order for customer 12345 with 3 items
[14:23:45 DBG] Repository: Executing query: SELECT * FROM Orders WHERE CustomerId = 12345 on database: OrderDB
[14:23:45 INF] Repository: Query executed on database: OrderDB in 45ms
[14:23:45 INF] OrderService: Order 98765 created successfully for customer 12345
```

### JSON (Production)
```json
{
  "@t": "2024-01-20T14:23:45.1234567Z",
  "@l": "Information",
  "@mt": "Order {OrderId} created successfully for customer {CustomerId}",
  "OrderId": 98765,
  "CustomerId": 12345,
  "CorrelationId": "abc-123-def",
  "TenantId": "tenant-001",
  "TenantName": "Acme Corp",
  "MachineName": "PROD-WEB-01",
  "Environment": "Production",
  "Application": "OrderService"
}
```

### Elasticsearch Index
```
Index: orderservice-production-2024.01.20
Document:
{
  "timestamp": "2024-01-20T14:23:45.123Z",
  "level": "Information",
  "messageTemplate": "Order {OrderId} created successfully",
  "fields": {
    "OrderId": 98765,
    "CustomerId": 12345,
    "CorrelationId": "abc-123-def",
    "TenantId": "tenant-001"
  }
}
```

## Multi-Tenant Logging

```csharp
public class TenantAwareService
{
    private readonly ILogger<TenantAwareService> _logger;
    private readonly ITenantContext _tenantContext;

    public async Task ProcessTenantDataAsync()
    {
        // Tenant info automatically added to all logs
        _logger.LogInformation("Processing data for current tenant");
        // Output includes: TenantId: tenant-123, TenantName: Acme Corp
    }
}
```

## Performance Monitoring

```csharp
// Automatic performance warnings
using (var scope = _logger.BeginPerformanceScope("SlowOperation", thresholdMs: 500))
{
    await Task.Delay(1000); // Simulate slow operation
}
// Logs: WARNING: Operation SlowOperation completed in 1005ms
```

## Correlation Tracking

```csharp
// Correlation ID flows through entire request
public class ApiController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderDto order)
    {
        // Correlation ID automatically added from HTTP header or generated
        _logger.LogInformation("Received order request");

        // Same correlation ID in all subsequent logs
        await _orderService.CreateOrderAsync(order);

        // Response includes X-Correlation-Id header
        return Ok();
    }
}
```

## Health Checks

```csharp
public class LoggingHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Log.Information("Health check test message");
            return HealthCheckResult.Healthy("Logging is working");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Logging failed", ex);
        }
    }
}
```

## Best Practices

### 1. Use Structured Logging
```csharp
// ✅ Good - Structured
_logger.LogInformation("Order {OrderId} processed in {ElapsedMs}ms", orderId, elapsed);

// ❌ Bad - String concatenation
_logger.LogInformation($"Order {orderId} processed in {elapsed}ms");
```

### 2. Use Appropriate Log Levels
```csharp
LogDebug    - Detailed diagnostic info
LogInformation - General flow of the application
LogWarning  - Abnormal but recoverable situations
LogError    - Errors that should be investigated
LogCritical - System is unusable
```

### 3. Avoid Logging Sensitive Data
```csharp
// ✅ Good
_logger.LogInformation("User {UserId} logged in", user.Id);

// ❌ Bad
_logger.LogInformation("User logged in: {@User}", user); // May log password
```

### 4. Use Scopes for Context
```csharp
using (_logger.BeginScope(new Dictionary<string, object>
{
    ["OrderId"] = orderId,
    ["CustomerId"] = customerId
}))
{
    // All logs within scope include OrderId and CustomerId
    await ProcessOrderSteps();
}
```

## Troubleshooting

### Enable Debug Logging
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug"
    }
  }
}
```

### Enable Self-Log
```csharp
Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));
```

### Check Sink Connectivity
```csharp
// Elasticsearch
curl -X GET "localhost:9200/_cluster/health?pretty"

// Seq
curl -X GET "http://localhost:5341/api/events?query=Application"
```

## Summary

The Serilog implementation is now complete with:
- ✅ All necessary packages installed
- ✅ Configuration extensions ready
- ✅ Enrichers for correlation and multi-tenancy
- ✅ Structured logging helpers
- ✅ Performance monitoring
- ✅ Multiple sink support
- ✅ Request/response logging middleware

The framework is ready for production-grade logging!