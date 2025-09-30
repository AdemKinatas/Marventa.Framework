# Multi-Tenant SaaS Sample

A complete multi-tenant SaaS application built with Marventa Framework demonstrating tenant isolation, subscription management, and enterprise features.

## Features

✅ **Tenant Isolation** - Complete data isolation per tenant
✅ **Subscription Management** - Plans, billing, and usage tracking
✅ **Tenant Provisioning** - Automatic tenant setup and configuration
✅ **Custom Domains** - Each tenant can use their own domain
✅ **White-labeling** - Tenant-specific branding
✅ **Usage Metering** - Track API calls, storage, users per tenant
✅ **Tenant Administration** - Manage tenants from admin portal
✅ **Audit Logging** - Per-tenant audit trails
✅ **Multi-database** - Separate database per tenant (optional)
✅ **Tenant-specific Settings** - Custom configuration per tenant

## Architecture

### Tenant Resolution Strategies

1. **Header-based** - `X-Tenant-Id` header
2. **Subdomain-based** - `tenant1.yourapp.com`
3. **Domain-based** - Custom domains per tenant
4. **Route-based** - `/tenants/{tenantId}/...`

### Database Strategies

1. **Shared Database, Shared Schema** - All tenants in one database with TenantId column
2. **Shared Database, Separate Schema** - Separate schema per tenant
3. **Separate Database** - Dedicated database per tenant (enterprise tier)

## Project Structure

```
MultiTenantSaaS/
├── src/
│   ├── Tenants/
│   │   ├── Commands/
│   │   │   ├── CreateTenantCommand.cs
│   │   │   ├── UpdateTenantCommand.cs
│   │   │   └── DeleteTenantCommand.cs
│   │   ├── Queries/
│   │   ├── Entities/
│   │   │   ├── Tenant.cs
│   │   │   └── TenantSettings.cs
│   │   └── Services/
│   │       ├── TenantProvisioningService.cs
│   │       └── TenantResolver.cs
│   ├── Subscriptions/
│   │   ├── Entities/
│   │   │   ├── Subscription.cs
│   │   │   ├── Plan.cs
│   │   │   └── Usage.cs
│   │   └── Services/
│   │       ├── BillingService.cs
│   │       └── UsageTrackingService.cs
│   ├── Users/
│   │   ├── Entities/
│   │   └── Services/
│   └── Shared/
│       ├── Middleware/
│       │   └── TenantResolutionMiddleware.cs
│       └── Filters/
│           └── TenantAuthorizationFilter.cs
└── tests/
```

## Domain Models

### Tenant Entity

```csharp
public class Tenant : AuditableEntity
{
    public string Name { get; private set; }
    public string Subdomain { get; private set; }
    public string? CustomDomain { get; private set; }
    public TenantStatus Status { get; private set; }
    public Guid SubscriptionId { get; private set; }
    public TenantSettings Settings { get; private set; }

    public void Activate()
    {
        if (Status == TenantStatus.Active)
            throw new InvalidOperationException("Tenant is already active");

        Status = TenantStatus.Active;
        AddDomainEvent(new TenantActivatedEvent(Id));
    }

    public void Suspend(string reason)
    {
        Status = TenantStatus.Suspended;
        AddDomainEvent(new TenantSuspendedEvent(Id, reason));
    }

    public void UpdateSettings(TenantSettings newSettings)
    {
        Settings = newSettings;
        AddDomainEvent(new TenantSettingsUpdatedEvent(Id, Settings));
    }
}
```

### Subscription Entity

```csharp
public class Subscription : AuditableEntity
{
    public Guid TenantId { get; private set; }
    public Guid PlanId { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public Money MonthlyPrice { get; private set; }
    public Usage CurrentUsage { get; private set; }

    public void Renew()
    {
        if (Status != SubscriptionStatus.Active)
            throw new InvalidOperationException("Only active subscriptions can be renewed");

        StartDate = DateTime.UtcNow;
        EndDate = StartDate.AddMonths(1);
        CurrentUsage = new Usage();
        AddDomainEvent(new SubscriptionRenewedEvent(Id, TenantId));
    }

    public void TrackUsage(UsageType type, int amount)
    {
        CurrentUsage.Track(type, amount);

        if (CurrentUsage.HasExceededLimit(PlanId))
            AddDomainEvent(new UsageLimitExceededEvent(Id, TenantId, type));
    }
}
```

## Tenant Resolution

### 1. Subdomain-based Resolution

```csharp
public class SubdomainTenantResolver : ITenantResolver
{
    private readonly IRepository<Tenant> _tenantRepository;
    private readonly IMemoryCache _cache;

    public async Task<Tenant?> ResolveAsync(HttpContext context)
    {
        var host = context.Request.Host.Host;
        var subdomain = ExtractSubdomain(host);

        if (string.IsNullOrEmpty(subdomain))
            return null;

        // Check cache first
        var cacheKey = $"tenant:subdomain:{subdomain}";
        if (_cache.TryGetValue(cacheKey, out Tenant? cachedTenant))
            return cachedTenant;

        // Query database
        var tenant = await _tenantRepository
            .Query()
            .FirstOrDefaultAsync(t => t.Subdomain == subdomain && t.Status == TenantStatus.Active);

        if (tenant != null)
            _cache.Set(cacheKey, tenant, TimeSpan.FromMinutes(30));

        return tenant;
    }

    private string ExtractSubdomain(string host)
    {
        // Extract subdomain from host (e.g., "tenant1.yourapp.com" -> "tenant1")
        var parts = host.Split('.');
        return parts.Length > 2 ? parts[0] : string.Empty;
    }
}
```

### 2. Header-based Resolution

```csharp
public class HeaderTenantResolver : ITenantResolver
{
    private readonly IRepository<Tenant> _tenantRepository;

    public async Task<Tenant?> ResolveAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantIdValue))
            return null;

        if (!Guid.TryParse(tenantIdValue, out var tenantId))
            return null;

        return await _tenantRepository.GetByIdAsync(tenantId);
    }
}
```

## Tenant Provisioning

```csharp
public class TenantProvisioningService
{
    private readonly IRepository<Tenant> _tenantRepository;
    private readonly IRepository<Subscription> _subscriptionRepository;
    private readonly IDatabaseSeeder _seeder;
    private readonly IEmailService _emailService;

    public async Task<Tenant> ProvisionTenantAsync(CreateTenantCommand command)
    {
        // 1. Create tenant
        var tenant = new Tenant
        {
            Name = command.Name,
            Subdomain = command.Subdomain,
            Status = TenantStatus.Provisioning,
            Settings = TenantSettings.Default()
        };

        await _tenantRepository.AddAsync(tenant);

        // 2. Create subscription
        var subscription = new Subscription
        {
            TenantId = tenant.Id,
            PlanId = command.PlanId,
            Status = SubscriptionStatus.Trial,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(14) // 14-day trial
        };

        await _subscriptionRepository.AddAsync(subscription);

        // 3. Seed tenant-specific data
        await _seeder.SeedTenantDataAsync(tenant.Id);

        // 4. Send welcome email
        await _emailService.SendWelcomeEmailAsync(tenant.Id, command.AdminEmail);

        // 5. Activate tenant
        tenant.Activate();

        return tenant;
    }
}
```

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SaaSDb;",
    "Redis": "localhost:6379"
  },
  "Marventa": {
    "MultiTenancy": {
      "Strategy": "Subdomain",
      "DatabaseStrategy": "SharedDatabase",
      "EnableTenantCache": true,
      "CacheDurationMinutes": 30
    }
  },
  "TenantDefaults": {
    "MaxUsers": 10,
    "MaxStorageMB": 1000,
    "MaxApiCallsPerMonth": 10000
  }
}
```

### Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add DbContext with multi-tenancy
builder.Services.AddDbContext<SaaSDbContext>((sp, options) =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add Marventa Framework with multi-tenancy
builder.Services.AddMarventaFramework(builder.Configuration, options =>
{
    // Enable multi-tenancy features
    options.EnableMultiTenancy = true;
    options.EnableCaching = true;
    options.EnableRepository = true;
    options.EnableCQRS = true;
    options.EnableEventDriven = true;
    options.EnableBackgroundJobs = true;

    options.CqrsOptions.Assemblies.Add(typeof(Program).Assembly);
});

// Register tenant resolver
builder.Services.AddScoped<ITenantResolver, SubdomainTenantResolver>();

// Add tenant provisioning service
builder.Services.AddScoped<TenantProvisioningService>();

var app = builder.Build();

// Add tenant resolution middleware
app.UseMiddleware<TenantResolutionMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

## Middleware

```csharp
public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public async Task InvokeAsync(HttpContext context, ITenantResolver resolver, ITenantContext tenantContext)
    {
        var tenant = await resolver.ResolveAsync(context);

        if (tenant == null)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("Tenant not found");
            return;
        }

        tenantContext.SetTenant(tenant);

        await _next(context);
    }
}
```

## API Endpoints

### Tenant Management (Admin)

```http
POST   /api/admin/tenants
GET    /api/admin/tenants
GET    /api/admin/tenants/{id}
PUT    /api/admin/tenants/{id}
DELETE /api/admin/tenants/{id}
POST   /api/admin/tenants/{id}/activate
POST   /api/admin/tenants/{id}/suspend
```

### Subscription Management

```http
GET    /api/subscriptions/current
PUT    /api/subscriptions/upgrade
PUT    /api/subscriptions/cancel
GET    /api/subscriptions/usage
GET    /api/subscriptions/invoices
```

### Tenant Settings

```http
GET    /api/settings
PUT    /api/settings
PUT    /api/settings/branding
PUT    /api/settings/domain
```

## Testing Multi-tenancy

### Unit Tests

```csharp
public class TenantProvisioningServiceTests
{
    [Fact]
    public async Task ProvisionTenant_ShouldCreateTenantAndSubscription()
    {
        // Arrange
        var command = new CreateTenantCommand
        {
            Name = "Test Tenant",
            Subdomain = "test",
            PlanId = Guid.NewGuid(),
            AdminEmail = "admin@test.com"
        };

        // Act
        var tenant = await _service.ProvisionTenantAsync(command);

        // Assert
        Assert.NotNull(tenant);
        Assert.Equal(TenantStatus.Active, tenant.Status);
    }
}
```

### Integration Tests

```csharp
public class MultiTenantIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Request_WithValidTenantHeader_ShouldSucceed()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant-Id", _tenant1Id.ToString());

        // Act
        var response = await client.GetAsync("/api/products");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Request_WithDifferentTenant_ShouldNotSeeOtherTenantsData()
    {
        // Arrange
        var client1 = _factory.CreateClient();
        client1.DefaultRequestHeaders.Add("X-Tenant-Id", _tenant1Id.ToString());

        var client2 = _factory.CreateClient();
        client2.DefaultRequestHeaders.Add("X-Tenant-Id", _tenant2Id.ToString());

        // Act
        var response1 = await client1.GetAsync("/api/products");
        var response2 = await client2.GetAsync("/api/products");

        var products1 = await response1.Content.ReadAsAsync<List<Product>>();
        var products2 = await response2.Content.ReadAsAsync<List<Product>>();

        // Assert
        Assert.NotEqual(products1.Count, products2.Count);
        Assert.DoesNotContain(products1, p => products2.Any(p2 => p2.Id == p.Id));
    }
}
```

## Performance Considerations

- **Tenant caching**: Cache tenant resolution for 30 minutes
- **Connection pooling**: Reuse database connections per tenant
- **Query optimization**: Automatic tenant filtering at database level
- **Handles**: **5,000+ tenants** per instance

## Security Best Practices

1. **Tenant Isolation**: Always filter queries by TenantId
2. **Authorization**: Check user belongs to tenant before operations
3. **Rate Limiting**: Per-tenant rate limits
4. **Data Encryption**: Tenant-specific encryption keys
5. **Audit Logging**: Per-tenant audit trails

## Learn More

- [Main Documentation](../../README.md)
- [Multi-tenancy Guide](../../docs/features/multi-tenancy.md)
- [Tenant Isolation Patterns](../../docs/architecture/tenant-isolation.md)
- [SaaS Metrics & Analytics](../../docs/features/analytics.md)