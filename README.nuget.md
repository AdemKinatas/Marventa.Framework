# Marventa Framework

[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/AdemKinatas/Marventa.Framework/blob/master/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Marventa.Framework.svg)](https://www.nuget.org/packages/Marventa.Framework)

> **Enterprise-grade .NET framework implementing Clean Architecture and SOLID principles with CQRS, MediatR Behaviors, and 47+ modular features**

## ⚡ Quick Start

### Installation
```bash
dotnet add package Marventa.Framework
```

### Basic Setup
```csharp
using Marventa.Framework.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add Marventa Framework
builder.Services.AddMarventaFramework(builder.Configuration, options =>
{
    options.EnableLogging = true;
    options.EnableCaching = true;
    options.EnableCQRS = true;
    options.EnableHealthChecks = true;

    // CQRS Configuration
    options.CqrsOptions.Assemblies.Add(typeof(Program).Assembly);
});

var app = builder.Build();
app.Run();
```

## ✨ Key Features

🎯 **Clean Architecture** - Separation of concerns with Core, Domain, Application, Infrastructure, and Web layers
⚡ **CQRS + MediatR** - Command Query Responsibility Segregation with automatic validation, logging, and transactions
🔐 **Enterprise Security** - JWT, API Keys, Encryption, Multi-tenancy support
💾 **Advanced Data Access** - Repository pattern, Unit of Work, Specifications, Soft delete
🚀 **Performance** - Redis caching, CDN integration, Circuit breaker, Distributed locking
📊 **Observability** - OpenTelemetry, Structured logging, Health checks, Analytics
🔄 **Event-Driven** - Domain events, Message queuing, Saga orchestration
🧩 **47+ Modular Features** - Enable only what you need

## 📚 Documentation

- [Complete Documentation](https://github.com/AdemKinatas/Marventa.Framework#readme)
- [Getting Started](https://github.com/AdemKinatas/Marventa.Framework/blob/master/docs/getting-started/)
- [Code Samples](https://github.com/AdemKinatas/Marventa.Framework/tree/master/samples)
- [API Reference](https://github.com/AdemKinatas/Marventa.Framework/blob/master/docs/api-reference/)

## 🤝 Support

- 💬 [GitHub Discussions](https://github.com/AdemKinatas/Marventa.Framework/discussions)
- 🐛 [Report Issues](https://github.com/AdemKinatas/Marventa.Framework/issues)
- 📧 Email: ademkinatas@gmail.com

## 📄 License

MIT License - see [LICENSE](https://github.com/AdemKinatas/Marventa.Framework/blob/master/LICENSE) for details.