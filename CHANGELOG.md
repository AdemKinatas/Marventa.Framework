# Changelog

All notable changes to Marventa.Framework will be documented in this file.

## [4.0.0] - 2025-10-01

### 🎯 Major Release - Breaking Changes

This is a complete architectural redesign of Marventa.Framework. Version 4.0.0 introduces a unified single-package approach, replacing the previous multi-project structure.

### ⚠️ Breaking Changes

- **Architecture**: Complete restructure from multi-project to single-project architecture
- **Package**: All features consolidated into one package: `Marventa.Framework`
- **Namespaces**: Updated namespace structure for better organization
  - `Marventa.Framework.Core.Domain` - Domain entities and patterns
  - `Marventa.Framework.Core.Application` - Application layer
  - `Marventa.Framework.CQRS` - CQRS implementation
  - `Marventa.Framework.EventBus` - Event-driven architecture
  - `Marventa.Framework.Caching` - Caching services
  - `Marventa.Framework.Security` - Authentication & authorization
  - `Marventa.Framework.Infrastructure` - Data access
  - And more...
- **.NET Version**: Now requires .NET 9.0 (upgraded from .NET 8.0)
- **Dependencies**: Updated all NuGet packages to latest versions
- **Migration**: Users upgrading from v3.x will need to update namespace references

### ✨ New Features

#### Core Domain-Driven Design
- `Entity<TId>` - Base entity with domain events
- `AuditableEntity<TId>` - Entity with audit fields (CreatedAt, UpdatedAt, etc.)
- `AggregateRoot<TId>` - Aggregate root pattern
- `ValueObject` - Value object pattern
- `DomainEvent` - Domain event base class

#### CQRS & MediatR
- `ICommand` and `ICommand<TResponse>` - Command interfaces
- `IQuery<TResponse>` - Query interface
- `ValidationBehavior` - Automatic FluentValidation integration
- `LoggingBehavior` - Request/response logging
- `PerformanceBehavior` - Performance monitoring

#### Event-Driven Architecture
- **RabbitMQ Integration**
  - `RabbitMqEventBus` - Complete event bus implementation
  - `RabbitMqConnection` - Connection management with auto-reconnect
- **MassTransit Support**
  - `MassTransitEventBus` - MassTransit-based event bus
  - `MassTransitConfiguration` - Easy configuration
- **Integration Events**
  - `IIntegrationEvent` - Integration event interface
  - `IIntegrationEventHandler<T>` - Event handler interface

#### Multi-Level Caching
- **In-Memory Cache** - `MemoryCacheService`
- **Distributed Redis Cache** - `RedisCache`
- **Hybrid Cache** - `HybridCacheService` (Memory + Redis)
- Unified `ICacheService` interface
- Configurable cache options (absolute/sliding expiration)

#### Security & Authentication
- **JWT Authentication**
  - `JwtTokenGenerator` - Token generation and validation
  - `JwtConfiguration` - Configuration options
- **Authorization**
  - `PermissionHandler` - Permission-based authorization
  - `PermissionPolicyProvider` - Dynamic policy provider
- **Encryption**
  - `AesEncryption` - AES encryption/decryption
  - `PasswordHasher` - BCrypt password hashing
- **Rate Limiting**
  - `RateLimiterMiddleware` - IP-based rate limiting

#### Infrastructure & Data Access
- `BaseDbContext` - EF Core DbContext with domain events
- `GenericRepository<TEntity, TId>` - Generic repository pattern
- `UnitOfWork` - Unit of Work pattern with transactions
- `EfCoreExtensions` - Easy service registration
- Support for SQL Server, PostgreSQL, MongoDB

#### Search & Storage
- **Elasticsearch**
  - `ElasticsearchService` - Full CRUD operations
  - Full-text search capabilities
- **Cloud Storage**
  - `AzureBlobStorage` - Azure Blob Storage integration
  - `S3Storage` - AWS S3 integration

#### Resilience & Fault Tolerance
- **Polly Policies**
  - `RetryPolicy` - Retry with exponential backoff
  - `CircuitBreakerPolicy` - Circuit breaker pattern
  - `TimeoutPolicy` - Request timeout handling

#### Observability & Monitoring
- **Logging**
  - `SerilogConfiguration` - Structured logging setup
  - Elasticsearch sink for centralized logs
  - Enrichers (Machine, Thread, Environment)
- **Health Checks**
  - `DatabaseHealthCheck` - Database connectivity
  - `RedisHealthCheck` - Redis connectivity
  - `RabbitMqHealthCheck` - RabbitMQ connectivity

#### Multi-Tenancy
- `ITenantContext` - Tenant context interface
- `TenantResolver` - Header/Claim-based tenant resolution
- `TenantMiddleware` - Automatic tenant detection

#### Validation & Error Handling
- FluentValidation integration
- `ValidationException` - Validation error handling
- `GlobalExceptionHandler` - Centralized exception handling
- `ExceptionMiddleware` - Exception middleware
- Custom exceptions: `BusinessException`, `NotFoundException`, `UnauthorizedException`

#### API Features
- `ApiResponse<T>` - Standardized API responses
- `ApiResponseFactory` - Factory for Result pattern conversion
- Swagger/OpenAPI support
- API versioning support

#### Extension Methods
- `AddMarventaFramework()` - Core framework setup
- `AddMarventaJwtAuthentication()` - JWT authentication
- `AddMarventaCaching()` - Caching services
- `AddMarventaRabbitMq()` - RabbitMQ event bus
- `AddMarventaElasticsearch()` - Elasticsearch
- `AddMarventaMultiTenancy()` - Multi-tenancy
- `AddMarventaDbContext<T>()` - Database context
- `UseMarventaFramework()` - Middleware setup
- `UseMarventaMultiTenancy()` - Multi-tenancy middleware

### 📦 Dependencies

All dependencies updated to latest stable versions compatible with .NET 9.0:
- Entity Framework Core 9.0.0
- MediatR 12.2.0
- MassTransit 8.1.3
- FluentValidation 11.9.0
- Serilog 3.1.1
- Polly 8.2.1
- And many more...

### 🔧 Improvements

- **Performance**: Optimized for .NET 9.0 performance improvements
- **Code Quality**: Complete rewrite with best practices
- **Documentation**: Comprehensive README with examples
- **Type Safety**: Full nullable reference types support
- **Maintainability**: Single-project structure for easier maintenance

### 📚 Documentation

- Added comprehensive README.md with usage examples
- Added CHANGELOG.md for version tracking
- XML documentation for all public APIs
- GitHub Actions workflow for automated NuGet publishing

### 🚀 Migration Guide from v3.x

1. **Update Package Reference**:
   ```bash
   # Remove old packages
   dotnet remove package Marventa.Application
   dotnet remove package Marventa.Domain
   dotnet remove package Marventa.Infrastructure
   # ... (remove all old Marventa packages)

   # Add new unified package
   dotnet add package Marventa.Framework --version 4.0.0
   ```

2. **Update Namespaces**:
   ```csharp
   // Old (v3.x)
   using Marventa.Application;
   using Marventa.Domain;

   // New (v4.0.0)
   using Marventa.Framework.Core.Domain;
   using Marventa.Framework.Core.Application;
   ```

3. **Update Service Registration**:
   ```csharp
   // Old (v3.x)
   services.AddMarventaApplication();
   services.AddMarventaDomain();

   // New (v4.0.0)
   services.AddMarventaFramework(configuration);
   services.AddMarventaMediatR(typeof(Program).Assembly);
   ```

### ⚡ Quick Start

```bash
dotnet add package Marventa.Framework --version 4.0.0
```

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add Marventa Framework
builder.Services.AddMarventaFramework(builder.Configuration);
builder.Services.AddMarventaMediatR(typeof(Program).Assembly);
builder.Services.AddMarventaJwtAuthentication(builder.Configuration);
builder.Services.AddMarventaCaching(builder.Configuration, CacheType.Hybrid);

var app = builder.Build();
app.UseMarventaFramework(app.Environment);
app.Run();
```

---

## [3.5.2] - Previous Version

For previous versions, please refer to the git history.

---

## Version History Summary

- **v4.0.0** - Complete architectural redesign, single-package approach, .NET 9.0
- **v3.5.2** - Last multi-project version, .NET 8.0
- **v3.x.x** - Multi-project architecture versions
- **v2.x.x** - Legacy versions
- **v1.x.x** - Initial releases
