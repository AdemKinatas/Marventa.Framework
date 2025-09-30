# Marventa Framework Samples

Real-world example applications demonstrating different features and use cases of Marventa Framework.

## 🎯 Available Samples

### 1. [Basic Web API](BasicWebApi/)
**Complexity:** ⭐ Beginner

A simple CRUD API demonstrating core concepts:
- CQRS with MediatR
- Repository pattern
- Validation with FluentValidation
- API response patterns
- Basic caching

**Perfect for:** Learning the basics, starting new projects

**Features:** CQRS, Repository, Validation, Caching

---

### 2. [E-Commerce Platform](ECommerce/)
**Complexity:** ⭐⭐⭐ Advanced

A complete e-commerce application with:
- Product catalog management
- Shopping cart and checkout
- Order processing
- Payment integration
- Domain events
- Background jobs
- Redis caching
- Event-driven architecture

**Perfect for:** Understanding advanced patterns, building production apps

**Features:** CQRS, DDD, Events, Caching, Storage, Background Jobs, Specifications

---

### 3. [Multi-Tenant SaaS](MultiTenantSaaS/)
**Complexity:** ⭐⭐⭐⭐ Expert

A multi-tenant SaaS application featuring:
- Tenant isolation and provisioning
- Subscription management
- Usage metering and billing
- Custom domains per tenant
- White-labeling support
- Tenant-specific configuration
- Multi-database strategies

**Perfect for:** Building SaaS products, enterprise applications

**Features:** Multi-tenancy, Subscriptions, Tenant isolation, Advanced security

---

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 or .NET 9.0 SDK
- SQL Server (LocalDB or Express)
- Redis (optional, for caching samples)
- Visual Studio 2022 or VS Code

### Running a Sample

1. **Clone the repository**
```bash
git clone https://github.com/AdemKinatas/Marventa.Framework.git
cd Marventa.Framework/samples/{SampleName}
```

2. **Restore packages**
```bash
dotnet restore
```

3. **Update database** (if applicable)
```bash
dotnet ef database update
```

4. **Run the application**
```bash
dotnet run
```

5. **Open Swagger**
Navigate to `https://localhost:5001/swagger` in your browser

---

## 📚 Learning Path

### Beginner Path
1. Start with **Basic Web API**
   - Learn CQRS basics
   - Understand repository pattern
   - Practice validation

2. Explore **E-Commerce** (Orders module only)
   - Study domain events
   - Learn specifications
   - Understand aggregates

### Intermediate Path
1. Complete **E-Commerce** sample
   - Implement full features
   - Add custom features
   - Write tests

2. Study **Multi-Tenant SaaS** (Read only)
   - Understand tenant isolation
   - Learn multi-tenancy patterns

### Advanced Path
1. Build **Multi-Tenant SaaS** from scratch
   - Implement tenant provisioning
   - Add subscription management
   - Configure multiple databases

2. Combine patterns from all samples
   - Build your own application
   - Apply best practices
   - Optimize performance

---

## 🎓 What You'll Learn

### Architecture Patterns
- ✅ Clean Architecture
- ✅ CQRS (Command Query Responsibility Segregation)
- ✅ Domain-Driven Design (DDD)
- ✅ Repository Pattern
- ✅ Specification Pattern
- ✅ Event-Driven Architecture

### Framework Features
- ✅ MediatR integration
- ✅ FluentValidation
- ✅ Pipeline behaviors
- ✅ Domain events
- ✅ Multi-tenancy
- ✅ Caching strategies
- ✅ Background jobs
- ✅ API versioning

### Best Practices
- ✅ SOLID principles
- ✅ Dependency injection
- ✅ Error handling
- ✅ Logging and monitoring
- ✅ Testing strategies
- ✅ Performance optimization

---

## 🔧 Customization

Each sample can be customized by:

1. **Enabling/Disabling Features**
```csharp
builder.Services.AddMarventaFramework(configuration, options =>
{
    options.EnableCaching = true;        // Enable
    options.EnableStorage = false;       // Disable
});
```

2. **Switching Implementations**
```csharp
// Use Redis instead of Memory cache
{
  "Marventa": {
    "Caching": {
      "Provider": "Redis"  // or "Memory"
    }
  }
}
```

3. **Adding Custom Features**
- Add your own commands and queries
- Implement custom validators
- Create domain events
- Add background jobs

---

## 📊 Comparison Matrix

| Feature | Basic API | E-Commerce | Multi-Tenant SaaS |
|---------|-----------|------------|-------------------|
| **CQRS** | ✅ Basic | ✅ Advanced | ✅ Advanced |
| **Repository** | ✅ | ✅ | ✅ |
| **Validation** | ✅ | ✅ | ✅ |
| **Domain Events** | ❌ | ✅ | ✅ |
| **Specifications** | ❌ | ✅ | ✅ |
| **Caching** | ✅ Memory | ✅ Redis | ✅ Redis |
| **Background Jobs** | ❌ | ✅ | ✅ |
| **Multi-tenancy** | ❌ | ❌ | ✅ |
| **Event-Driven** | ❌ | ✅ | ✅ |
| **API Versioning** | ❌ | ✅ | ✅ |
| **Rate Limiting** | ❌ | ✅ | ✅ Per-tenant |
| **Health Checks** | ✅ | ✅ | ✅ |
| **Complexity** | ⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐ |
| **Lines of Code** | ~500 | ~3,000 | ~5,000 |

---

## 🧪 Testing

Each sample includes tests:

```bash
# Run all tests for a sample
cd samples/{SampleName}
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true

# Run specific category
dotnet test --filter Category=Unit
```

---

## 🤝 Contributing

Want to add a new sample or improve existing ones?

1. Fork the repository
2. Create your sample in `samples/YourSampleName/`
3. Follow the structure of existing samples
4. Add comprehensive README.md
5. Submit a pull request

### Sample Guidelines
- ✅ Include README.md with clear instructions
- ✅ Provide working code (no placeholders)
- ✅ Add comments explaining key concepts
- ✅ Include tests
- ✅ Follow framework best practices
- ✅ Keep it simple and focused

---

## 📖 Additional Resources

- [Main Documentation](../README.md)
- [Architecture Guide](../docs/architecture/)
- [Feature Guides](../docs/features/)
- [API Reference](../docs/api/)
- [Migration Guides](../docs/migration/)

---

## 💬 Support

Need help with samples?

- 💬 [GitHub Discussions](https://github.com/AdemKinatas/Marventa.Framework/discussions)
- 🐛 [Report Issues](https://github.com/AdemKinatas/Marventa.Framework/issues)
- 📧 Email: ademkinatas@gmail.com

---

<div align="center">

**Happy coding!** 🚀

[⬅️ Back to Main Documentation](../README.md)

</div>