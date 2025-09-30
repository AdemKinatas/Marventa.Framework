# Marventa Framework

> **Kurumsal .NET projeleri için hazır Clean Architecture çözümü**

[![NuGet](https://img.shields.io/nuget/v/Marventa.Framework.svg)](https://www.nuget.org/packages/Marventa.Framework/)
[![Downloads](https://img.shields.io/nuget/dt/Marventa.Framework.svg)](https://www.nuget.org/packages/Marventa.Framework/)

## ⚡ Hızlı Başlangıç

```bash
dotnet add package Marventa.Framework
```

**Hepsi bu kadar!** Tüm bağımlılıklar otomatik yüklenir.

---

## 🎯 Ne Sağlar?

| Özellik | Açıklama |
|---------|----------|
| 🏗️ BaseEntity | Otomatik ID, tarih, soft delete |
| 📦 Repository | Hazır CRUD operasyonları |
| 🎭 CQRS | MediatR ile command/query |
| ✅ Validation | FluentValidation otomatik |
| 📝 Logging | Serilog hazır |
| 💾 Cache | Redis + Memory cache |
| 🛡️ Exception | Merkezi hata yönetimi |
| 🔗 CorrelationId | Request tracking |

---

## 💻 Kod Örnekleri

### 1️⃣ Setup (Program.cs)
```csharp
using Marventa.Framework.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMarventaFramework(builder.Configuration, options =>
{
    options.EnableLogging = true;
    options.EnableCaching = true;
    options.EnableCQRS = true;
});

var app = builder.Build();
app.UseMarventaFramework(app.Configuration);
app.Run();
```

### 2️⃣ Entity Oluştur
```csharp
public class Product : BaseEntity  // Id, CreatedDate, IsDeleted otomatik
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

### 3️⃣ Repository Kullan
```csharp
public class ProductService
{
    private readonly IRepository<Product> _repo;

    public async Task<Product> GetAsync(Guid id)
        => await _repo.GetByIdAsync(id);
}
```

### 4️⃣ CQRS Command
```csharp
public record CreateProductCommand(string Name, decimal Price) : ICommand<Guid>;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IRepository<Product> _repo;

    public async Task<Guid> Handle(CreateProductCommand cmd, CancellationToken ct)
    {
        var product = new Product { Name = cmd.Name, Price = cmd.Price };
        await _repo.AddAsync(product, ct);
        return product.Id;
    }
}
```

### 5️⃣ Controller
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var product = await _mediator.Send(new GetProductQuery(id));
        return Ok(ApiResponse<Product>.SuccessResult(product));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return Ok(ApiResponse<Guid>.SuccessResult(id));
    }
}
```

---

## 📦 İçerik (v3.5.2)

### ✅ Hazır Özellikler (27)
- BaseEntity, Repository, Unit of Work
- CQRS, Validation, Logging, Transaction Behaviors
- Saga Pattern, Outbox/Inbox
- Redis Cache, Memory Cache
- Serilog Logging
- JWT Auth, API Key Auth
- Multi-Tenancy, Soft Delete, Audit
- CDN (Azure, AWS, CloudFlare)
- Storage (Local, Azure, S3)

### ⚠️ Test/Mock (6)
- Geliştirme için placeholder servisler

### 🚧 Yol Haritası (14)
- Event Sourcing, Background Jobs, E-commerce

---

## 📚 Kaynaklar

- **Tam Dokümantasyon**: [GitHub README](https://github.com/AdemKinatas/Marventa.Framework#readme)
- **5 Dakika Rehberi**: [QUICKSTART.md](https://github.com/AdemKinatas/Marventa.Framework/blob/master/QUICKSTART.md)
- **Issues**: [GitHub Issues](https://github.com/AdemKinatas/Marventa.Framework/issues)
- **Örnekler**: [Samples Klasörü](https://github.com/AdemKinatas/Marventa.Framework/tree/master/samples)

---

## ⚙️ Gereksinimler

- .NET 8.0 veya 9.0
- SQL Server, PostgreSQL veya MySQL (opsiyonel)
- Redis (cache için, opsiyonel)

---

## 📄 Lisans

MIT - Ticari ve kişisel kullanım için ücretsiz

**Yapımcı:** Adem Kinatas
