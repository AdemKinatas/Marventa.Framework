# 🚀 Marventa Framework - 5 Dakikada Başla

## Adım 1: Paketi Yükle (30 saniye)

```bash
dotnet add package Marventa.Framework
```

✅ Hepsi bu kadar! Tüm bağımlılıklar otomatik yüklenir.

---

## Adım 2: Program.cs'i Ayarla (2 dakika)

```csharp
using Marventa.Framework.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Marventa Framework'ü ekle
builder.Services.AddMarventaFramework(builder.Configuration, options =>
{
    options.EnableLogging = true;              // Serilog ile loglama
    options.EnableCaching = true;              // Redis cache
    options.EnableExceptionHandling = true;    // Otomatik hata yönetimi
    options.EnableCQRS = true;                 // MediatR ile CQRS
    options.EnableRepository = true;           // Repository pattern
});

var app = builder.Build();

// Middleware'i aktif et
app.UseMarventaFramework(app.Configuration);

app.MapControllers();
app.Run();
```

---

## Adım 3: İlk Entity'ni Oluştur (1 dakika)

```csharp
using Marventa.Framework.Core.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
```

✅ Otomatik olarak gelir:
- `Id` (Guid)
- `CreatedDate`, `UpdatedDate`
- `CreatedBy`, `UpdatedBy`
- `IsDeleted` (Soft delete)

---

## Adım 4: DbContext Oluştur (1 dakika)

```csharp
using Marventa.Framework.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : BaseDbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ITenantContext tenantContext)
        : base(options, tenantContext)
    {
    }

    public DbSet<Product> Products { get; set; }
}
```

---

## Adım 5: appsettings.json'u Yapılandır (30 saniye)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyApp;Trusted_Connection=True;"
  },
  "Marventa": {
    "EnableLogging": true,
    "EnableCaching": false,
    "Database": {
      "Provider": "SqlServer"
    }
  }
}
```

---

## ✅ Hazırsın!

Artık kullanabilirsin:

### Repository Pattern
```csharp
public class ProductService
{
    private readonly IRepository<Product> _repository;

    public async Task<Product> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task CreateAsync(Product product)
    {
        await _repository.AddAsync(product);
    }
}
```

### CQRS - Command
```csharp
public record CreateProductCommand(string Name, decimal Price) : ICommand<Guid>;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IRepository<Product> _repository;

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = new Product
        {
            Name = request.Name,
            Price = request.Price
        };

        await _repository.AddAsync(product, ct);
        return product.Id;
    }
}
```

### API Response
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult Get(Guid id)
    {
        var product = new Product { Id = id, Name = "Test", Price = 100 };
        return Ok(ApiResponse<Product>.SuccessResult(product));
    }
}
```

---

## 📚 Daha Fazlası

- [Tam Dokümantasyon](README.md)
- [Örnekler](examples/)
- [GitHub](https://github.com/AdemKinatas/Marventa.Framework)

## ❓ Sorun mu Var?

1. `dotnet clean && dotnet restore` çalıştır
2. NuGet cache'i temizle: `dotnet nuget locals all --clear`
3. Issue aç: https://github.com/AdemKinatas/Marventa.Framework/issues
