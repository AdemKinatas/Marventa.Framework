# 🚀 Marventa Framework

[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![NuGet](https://img.shields.io/badge/NuGet-v2.2.0-blue)](https://www.nuget.org/packages/Marventa.Framework)

> **Enterprise-ready, modular .NET framework for intelligent file management and content processing**

Marventa Framework is a **modular, pay-as-you-use** file management system that allows you to enable only the features you need. Built with **Clean Architecture** and **SOLID principles** for maximum flexibility and maintainability.

## 🎯 Core Philosophy

- **Modular Design**: Enable only what you need
- **Provider Agnostic**: Switch providers without code changes
- **Performance First**: Async operations and optimized processing
- **Enterprise Ready**: Production-tested with comprehensive error handling
- **Developer Friendly**: Clear APIs and extensive documentation

## ⚡ Quick Start

### Installation

```bash
dotnet add package Marventa.Framework
```

### Basic Setup

```csharp
// Program.cs
using Marventa.Framework.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add Marventa with minimal configuration
builder.Services.AddMarventaFramework(options =>
{
    // Enable only what you need - pay for what you use!
    options.EnableFileProcessor = true;
    options.EnableStorage = true;
    options.EnableCDN = false;      // Optional
    options.EnableML = false;       // Optional
    options.EnableMetadata = true;
});

var app = builder.Build();
app.UseMarventaFramework();
app.Run();
```

### Simple Usage

```csharp
// Upload and process an image
[ApiController]
public class FilesController : ControllerBase
{
    private readonly IMarventaStorage _storage;
    private readonly IMarventaFileProcessor _processor;

    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        // 1. Process image
        var processResult = await _processor.ProcessImageAsync(file.OpenReadStream(), new()
        {
            Width = 800, Height = 600, Quality = 85
        });

        // 2. Upload to storage
        var uploadResult = await _storage.UploadFileAsync(
            processResult.ProcessedImage, file.FileName, file.ContentType);

        return Ok(new {
            FileId = uploadResult.FileId,
            Url = uploadResult.PublicUrl,
            Size = uploadResult.FileSizeBytes
        });
    }
}
```

## 🏗️ Modular Architecture

Marventa Framework follows a **clean, modular architecture** with 29+ focused, single-responsibility files:

```
Marventa.Framework/
├── Core/                    # Domain models and interfaces
│   ├── Interfaces/         # Service contracts
│   └── Models/            # Organized by feature domain
│       ├── CDN/           # 8 focused CDN model files
│       ├── Storage/       # 12 focused Storage model files
│       ├── ML/            # 6 focused ML model files
│       ├── FileProcessing/# Processing options and results
│       └── FileMetadata/  # 3 focused Metadata model files
├── Infrastructure/         # External service implementations
│   └── Services/          # Concrete implementations
└── Web/                   # Web-specific features
    └── Extensions/        # DI container extensions
```

## 🎯 Key Features

| Feature | Description | Dependencies |
|---------|-------------|--------------|
| **🗄️ Storage Management** | Multi-provider storage (Azure Blob, AWS S3, Local) with unified API | None |
| **🖼️ Image Processing** | Resize, optimize, watermark, format conversion with quality control | None |
| **🌐 CDN Integration** | Global content delivery with caching and edge optimization | Storage |
| **🤖 AI/ML Services** | Image analysis, face detection, content tagging, OCR | Storage |
| **📊 Metadata Management** | Search, categorization, analytics with advanced tagging | Storage |

## 💡 Why Choose Marventa Framework?

✅ **Modular Design** - Pay only for features you use
✅ **Production Ready** - Battle-tested in enterprise environments
✅ **Provider Agnostic** - Switch storage/CDN providers without code changes
✅ **Clean Architecture** - Maintainable, testable, scalable
✅ **SOLID Compliance** - 29+ single-responsibility files
✅ **Comprehensive Testing** - 39 tests with MockFileProcessorTests & MockStorageServiceTests
✅ **Zero Build Errors** - Professional, production-ready codebase

## 🧪 Testing Support

Built-in mock services for comprehensive testing:

```csharp
// Use mock providers in tests
services.AddMarventaFramework(options =>
{
    options.StorageOptions.Provider = StorageProvider.Mock;
    options.FileProcessorOptions.Provider = FileProcessorProvider.Mock;
    // All operations work in-memory for fast testing
});
```

## 📦 Available Packages

| Package | Purpose | Dependencies |
|---------|---------|--------------|
| `Marventa.Framework` | **All features** | Complete solution |
| `Marventa.Framework.Core` | **Models & Interfaces** | No dependencies |
| `Marventa.Framework.Infrastructure` | **Implementations** | Core + External libs |
| `Marventa.Framework.Web` | **ASP.NET Integration** | Infrastructure |

## 📚 Complete Documentation

**📖 [DOCUMENTATION.md](./DOCUMENTATION.md)** - Comprehensive guide with:
- Feature configuration examples
- Provider setup instructions
- Usage scenarios and best practices
- API reference and troubleshooting
- 1350+ lines of detailed documentation

## 📄 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

---

<div align="center">
  <strong>Built with ❤️ for the .NET Community</strong>
  <br>
  <sub>Making enterprise file management simple, powerful, and intelligent</sub>
</div>