# 🚀 Marventa Framework

[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![NuGet](https://img.shields.io/badge/NuGet-v2.2.0-blue)](https://www.nuget.org/packages/Marventa.Framework)

> **Enterprise-ready, modular .NET framework for intelligent file management**

## ⚡ Quick Start

```bash
dotnet add package Marventa.Framework
```

```csharp
// Program.cs - Enable only what you need
builder.Services.AddMarventaFramework(options =>
{
    options.EnableStorage = true;        // File upload/download
    options.EnableFileProcessor = true;  // Image processing
    options.EnableCDN = false;          // Optional: Global delivery
    options.EnableML = false;           // Optional: AI analysis
    options.EnableMetadata = true;      // Optional: Search & analytics
});
```

```csharp
// Controller - Simple file upload with processing
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

    return Ok(new { FileId = uploadResult.FileId, Url = uploadResult.PublicUrl });
}
```

## 🎯 Key Features

- **🗄️ Storage**: Multi-provider (Azure Blob, AWS S3, Local) with unified API
- **🖼️ Processing**: Image resize, optimize, watermark, format conversion
- **🌐 CDN**: Global content delivery with caching
- **🤖 AI/ML**: Image analysis, face detection, content tagging
- **📊 Metadata**: Search, categorization, analytics

## 💡 Why Choose Marventa?

✅ **Modular** - Pay only for features you use
✅ **Production Ready** - Battle-tested in enterprise
✅ **Provider Agnostic** - Switch providers without code changes
✅ **Clean Architecture** - SOLID principles, maintainable
✅ **Developer Friendly** - Intuitive APIs, comprehensive docs

## 📚 Documentation

- **📖 [Complete Documentation](https://github.com/AdemKinatas/Marventa.Framework/blob/master/DOCUMENTATION.md)**
- **🔧 [Configuration Guide](https://github.com/AdemKinatas/Marventa.Framework)**
- **💡 [Usage Examples](https://github.com/AdemKinatas/Marventa.Framework)**

## 📄 License

MIT License - see [LICENSE](https://github.com/AdemKinatas/Marventa.Framework/blob/master/LICENSE) for details.

---

<div align="center">
  <strong>Built with ❤️ for the .NET Community</strong>
</div>