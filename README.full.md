# 🚀 Marventa Framework

[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![NuGet](https://img.shields.io/badge/NuGet-v2.2.0-blue)](https://www.nuget.org/packages/Marventa.Framework)

> **Enterprise-ready, modular .NET framework for intelligent file management and content processing**

## ✨ What is Marventa Framework?

A comprehensive, **pay-as-you-use** file management framework that provides everything from basic storage to AI-powered content analysis. Built with **Clean Architecture** and **SOLID principles** for maximum flexibility and maintainability.

## 🎯 Key Features

| Feature | Description |
|---------|-------------|
| **🗄️ Storage Management** | Multi-provider storage (Azure Blob, AWS S3, Local) with unified API |
| **🖼️ Image Processing** | Resize, optimize, watermark, format conversion with quality control |
| **🌐 CDN Integration** | Global content delivery with caching and edge optimization |
| **🤖 AI/ML Services** | Image analysis, face detection, content tagging, OCR |
| **📊 Metadata Management** | Search, categorization, analytics with advanced tagging |

## ⚡ Quick Start

### Installation
```bash
dotnet add package Marventa.Framework
```

### Basic Setup
```csharp
// Program.cs
builder.Services.AddMarventaFramework(options =>
{
    // Enable only what you need - pay for what you use!
    options.EnableStorage = true;        // File upload/download
    options.EnableFileProcessor = true;  // Image processing
    options.EnableCDN = false;          // Optional: Global delivery
    options.EnableML = false;           // Optional: AI analysis
    options.EnableMetadata = true;      // Optional: Search & analytics
});
```

### Simple Usage
```csharp
// Upload and process an image
[ApiController]
public class FilesController : ControllerBase
{
    private readonly IStorageService _storage;
    private readonly IFileProcessor _processor;

    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        // 1. Process image
        var processResult = await _processor.ProcessImageAsync(file.OpenReadStream(), new()
        {
            Width = 800,
            Height = 600,
            Quality = 85
        });

        // 2. Upload to storage
        var uploadResult = await _storage.UploadFileAsync(
            processResult.ProcessedImage,
            file.FileName,
            file.ContentType
        );

        return Ok(new {
            FileId = uploadResult.FileId,
            Url = uploadResult.PublicUrl,
            Size = uploadResult.FileSizeBytes
        });
    }
}
```

## 🔧 Configuration Options

### Storage Providers
```json
{
  "Marventa": {
    "StorageOptions": {
      "Provider": "AzureBlob",           // AzureBlob | AWS | Local | Mock
      "ConnectionString": "...",
      "EnableEncryption": true,
      "MaxFileSizeBytes": 104857600
    }
  }
}
```

### Image Processing
```json
{
  "FileProcessorOptions": {
    "Provider": "ImageSharp",           // ImageSharp | Mock
    "DefaultImageQuality": 85,
    "MaxFileSizeBytes": 52428800,
    "SupportedFormats": ["jpg", "png", "webp"]
  }
}
```

## 🏗️ Modular Architecture

Enable only the features you need:

```csharp
builder.Services.AddMarventaFramework(options =>
{
    // Core (Always enabled)
    options.EnableStorage = true;        // File operations

    // Optional Features (Pay-per-use)
    options.EnableFileProcessor = true;  // Image processing
    options.EnableCDN = true;           // Global delivery
    options.EnableML = true;            // AI analysis
    options.EnableMetadata = true;      // Search & analytics
});
```

## 📦 Available Packages

| Package | Purpose | Dependencies |
|---------|---------|--------------|
| `Marventa.Framework` | **All features** | Complete solution |
| `Marventa.Framework.Core` | **Models & Interfaces** | No dependencies |
| `Marventa.Framework.Infrastructure` | **Implementations** | Core + External libs |
| `Marventa.Framework.Web` | **ASP.NET Integration** | Infrastructure |

## 🌟 Advanced Features

### AI-Powered Content Analysis
```csharp
var analysis = await _mlService.AnalyzeImageAsync(imageStream, new()
{
    DetectObjects = true,
    DetectFaces = true,
    GenerateTags = true,
    ExtractText = true
});

Console.WriteLine($"Found {analysis.DetectedObjects.Count} objects");
Console.WriteLine($"Confidence: {analysis.OverallConfidence:P}");
```

### Smart Caching with CDN
```csharp
var cdnResult = await _cdnService.UploadWithTransformationAsync(fileStream, new()
{
    Transformations = new[]
    {
        new ImageTransformation { Width = 800, Height = 600 },
        new ImageTransformation { Width = 400, Height = 300 },
        new ImageTransformation { Width = 150, Height = 150 }
    },
    CacheTTL = TimeSpan.FromHours(24)
});
```

### File Analytics & Search
```csharp
// Add metadata for searchability
await _metadataService.AddFileMetadataAsync(fileId, new()
{
    Title = "Product Image",
    Description = "High-quality product photo",
    Tags = new[] { "product", "ecommerce", "high-res" },
    CustomProperties = new Dictionary<string, object>
    {
        ["ProductId"] = "P12345",
        ["Category"] = "Electronics"
    }
});

// Search files
var searchResults = await _metadataService.SearchFilesAsync(new()
{
    Query = "product electronics",
    FileTypes = new[] { "image/jpeg", "image/png" },
    DateRange = new(DateTime.Now.AddDays(-30), DateTime.Now)
});
```

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

## 📚 Documentation

- **📖 Complete Documentation**: [View Docs](./DOCUMENTATION.md)
- **🚀 Quick Start Guide**: [Getting Started](./docs/quick-start.md)
- **🔧 Configuration**: [Configuration Guide](./docs/configuration.md)
- **💡 Examples**: [Usage Examples](./docs/examples.md)

## 💡 Why Choose Marventa Framework?

✅ **Modular Design** - Pay only for features you use
✅ **Production Ready** - Battle-tested in enterprise environments
✅ **Provider Agnostic** - Switch storage/CDN providers without code changes
✅ **Clean Architecture** - Maintainable, testable, scalable
✅ **Comprehensive** - File management, processing, AI, analytics in one package
✅ **Developer Friendly** - Intuitive APIs with extensive documentation

## 📄 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

---

<div align="center">
  <strong>Built with ❤️ for the .NET Community</strong>
  <br>
  <sub>Making enterprise file management simple, powerful, and intelligent</sub>
</div>