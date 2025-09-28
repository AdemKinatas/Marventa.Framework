# 📚 Marventa Framework Documentation

> **Complete guide for using Marventa Framework's modular file management system**

---

## 📑 Table of Contents

1. [Introduction](#introduction)
2. [Quick Start](#quick-start)
3. [Modular Architecture](#modular-architecture)
4. [Feature Configuration](#feature-configuration)
5. [Feature Documentation](#feature-documentation)
   - [File Processing](#file-processing)
   - [Storage Management](#storage-management)
   - [CDN Integration](#cdn-integration)
   - [AI/ML Services](#aiml-services)
   - [Metadata Management](#metadata-management)
6. [Usage Examples](#usage-examples)
7. [API Reference](#api-reference)
8. [Best Practices](#best-practices)
9. [Troubleshooting](#troubleshooting)

---

## Introduction

Marventa Framework is a **modular, pay-as-you-use** file management system that allows you to enable only the features you need. Each module can be independently configured and scaled according to your requirements.

### 🎯 Core Philosophy

- **Modular Design**: Enable only what you need
- **Provider Agnostic**: Switch providers without code changes
- **Performance First**: Async operations and optimized processing
- **Enterprise Ready**: Production-tested with comprehensive error handling
- **Developer Friendly**: Clear APIs and extensive documentation

---

## Quick Start

### Installation

```bash
# Install via NuGet
dotnet add package Marventa.Framework

# Or specific packages
dotnet add package Marventa.Framework.Core
dotnet add package Marventa.Framework.Infrastructure
```

### Basic Setup

```csharp
// Program.cs
using Marventa.Framework.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add Marventa with minimal configuration
builder.Services.AddMarventaFramework(options =>
{
    // Enable only what you need
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

---

## Modular Architecture

Marventa Framework follows a **clean, modular architecture** that separates concerns and allows for independent scaling of features.

### 🏗️ Architecture Overview

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

### 📁 New Modular File Structure

**CDN Models** (8 files):
- `CDNUpload.cs` - Upload operations and results
- `CDNInvalidation.cs` - Cache invalidation models
- `CDNMetrics.cs` - Analytics and monitoring
- `CDNTransformation.cs` - Image/content transformations
- `CDNConfiguration.cs` - Settings and options
- `CDNDeletion.cs` - File deletion operations
- `CDNDistribution.cs` - Content distribution models
- `CDNEnums.cs` - All CDN-related enums

**Storage Models** (12 files):
- `StorageUpload.cs` - File upload operations
- `StorageDownload.cs` - File download operations
- `StorageFileInfo.cs` - File information models
- `StorageFileOperations.cs` - Copy, move, delete operations
- `StorageList.cs` - Directory listing models
- `StorageConfiguration.cs` - Provider settings
- `StorageBulkOperations.cs` - Batch operations
- `StorageAccessControl.cs` - Security and permissions
- `StorageAnalytics.cs` - Usage analytics
- `StorageHealth.cs` - Health monitoring
- `StorageProvider.cs` - Provider information
- `StorageEnums.cs` - All Storage-related enums

**ML Models** (6 files):
- `ImageAnalysis.cs` - Image recognition and analysis
- `TextAnalysis.cs` - Content analysis and NLP
- `GeometryAndColors.cs` - Visual analysis models
- `FaceAnalysis.cs` - Face detection and recognition
- `ContentOptimization.cs` - Content optimization
- `MLEnumerations.cs` - All ML-related enums

**File Metadata Models** (3 files):
- `CoreModels.cs` - File metadata and type-specific data
- `OperationResults.cs` - Metadata operation results
- `MetadataEnums.cs` - All Metadata-related enums

### 🏗️ Feature Toggle System

Each feature in Marventa Framework can be independently enabled or disabled:

```csharp
public class FileServiceOptions
{
    // Core Features - Toggle on/off
    public bool EnableFileProcessor { get; set; } = true;
    public bool EnableStorage { get; set; } = true;
    public bool EnableCDN { get; set; } = false;
    public bool EnableML { get; set; } = false;
    public bool EnableMetadata { get; set; } = true;

    // Feature-specific configurations
    public FileProcessorOptions FileProcessorOptions { get; set; }
    public StorageServiceOptions StorageOptions { get; set; }
    public CDNServiceOptions CDNOptions { get; set; }
    public MLServiceOptions MLOptions { get; set; }
    public MetadataServiceOptions MetadataOptions { get; set; }
}
```

### 🎛️ Feature Dependencies

| Feature | Dependencies | Optional | Use Case |
|---------|-------------|----------|----------|
| **FileProcessor** | None | ✅ | Image manipulation, optimization |
| **Storage** | None | ❌ Core | File upload/download |
| **CDN** | Storage | ✅ | Global content delivery |
| **ML** | Storage | ✅ | AI analysis, auto-tagging |
| **Metadata** | Storage | ✅ | Search, tagging, analytics |

---

## Feature Configuration

### Configuration via appsettings.json

```json
{
  "Marventa": {
    // Feature Toggles
    "EnableFileProcessor": true,
    "EnableStorage": true,
    "EnableCDN": true,
    "EnableML": false,
    "EnableMetadata": true,

    // File Processor Configuration
    "FileProcessorOptions": {
      "Provider": "ImageSharp",
      "MaxFileSizeBytes": 52428800,
      "DefaultImageQuality": 85,
      "SupportedImageFormats": ["jpg", "jpeg", "png", "webp"],
      "DefaultThumbnailSizes": [
        { "Name": "small", "Width": 150, "Height": 150 },
        { "Name": "medium", "Width": 300, "Height": 300 },
        { "Name": "large", "Width": 600, "Height": 600 }
      ],
      "PreserveMetadata": true
    },

    // Storage Configuration
    "StorageOptions": {
      "Provider": "AzureBlob",
      "ConnectionString": "${AZURE_STORAGE_CONNECTION}",
      "DefaultContainer": "files",
      "EnableEncryption": true,
      "EnableVersioning": false,
      "MaxFileSizeBytes": 104857600
    },

    // CDN Configuration
    "CDNOptions": {
      "Provider": "CloudFlare",
      "Endpoint": "https://cdn.example.com",
      "ApiKey": "${CLOUDFLARE_API_KEY}",
      "ZoneId": "${CLOUDFLARE_ZONE_ID}",
      "DefaultCacheTTL": 86400,
      "EnableCompression": true
    },

    // ML Configuration
    "MLOptions": {
      "Provider": "AzureAI",
      "ApiEndpoint": "https://ai.azure.com",
      "ApiKey": "${AZURE_AI_KEY}",
      "EnableImageAnalysis": true,
      "EnableTextAnalysis": false,
      "MinConfidenceThreshold": 0.7,
      "MaxConcurrentRequests": 10
    },

    // Metadata Configuration
    "MetadataOptions": {
      "Provider": "MongoDB",
      "ConnectionString": "${MONGODB_CONNECTION}",
      "DatabaseName": "FileMetadata",
      "EnableCaching": true,
      "CacheDuration": 3600
    }
  }
}
```

### Programmatic Configuration

```csharp
services.AddMarventaFramework(options =>
{
    // Load from configuration
    configuration.GetSection("Marventa").Bind(options);

    // Or configure programmatically
    if (environment.IsProduction())
    {
        options.EnableCDN = true;
        options.CDNOptions.Provider = CDNProvider.CloudFlare;
    }
    else
    {
        options.EnableCDN = false;
    }

    // Conditional ML enablement
    if (configuration.GetValue<bool>("Features:EnableAI"))
    {
        options.EnableML = true;
        options.MLOptions.Provider = MLProvider.AzureAI;
    }
});
```

---

## Feature Documentation

### File Processing

#### Overview
The file processing module handles image manipulation, optimization, and format conversion.

#### Enable the Feature
```csharp
options.EnableFileProcessor = true;
options.FileProcessorOptions = new FileProcessorOptions
{
    Provider = FileProcessorProvider.ImageSharp,
    MaxFileSizeBytes = 50 * 1024 * 1024, // 50MB
    DefaultImageQuality = 85
};
```

#### Available Operations

##### 1. Image Processing
```csharp
[ApiController]
public class ImageController : ControllerBase
{
    private readonly IMarventaFileProcessor _processor;

    [HttpPost("process")]
    public async Task<IActionResult> ProcessImage(IFormFile file)
    {
        var result = await _processor.ProcessImageAsync(
            file.OpenReadStream(),
            new ProcessingOptions
            {
                Width = 1920,
                Height = 1080,
                ResizeMode = ResizeMode.Crop,
                Quality = 90,
                MaintainAspectRatio = true
            });

        return File(result.ProcessedImage, "image/jpeg");
    }
}
```

##### 2. Thumbnail Generation
```csharp
// Generate multiple thumbnail sizes
var thumbnails = await _processor.GenerateThumbnailsAsync(
    imageStream,
    new[]
    {
        new ThumbnailSize { Name = "thumb", Width = 150, Height = 150 },
        new ThumbnailSize { Name = "medium", Width = 500, Height = 500 },
        new ThumbnailSize { Name = "large", Width = 1000, Height = 1000 }
    });

foreach (var thumbnail in thumbnails.Thumbnails)
{
    // Save each thumbnail
    await SaveThumbnail(thumbnail.Key, thumbnail.Value.ImageStream);
}
```

##### 3. Image Optimization
```csharp
// Optimize image for web
var optimized = await _processor.OptimizeImageAsync(
    imageStream,
    OptimizationLevel.High // Maximum compression
);

Console.WriteLine($"Original: {originalSize} bytes");
Console.WriteLine($"Optimized: {optimized.OptimizedSizeBytes} bytes");
Console.WriteLine($"Saved: {optimized.SpaceSavedPercentage}%");
```

##### 4. Format Conversion
```csharp
// Convert to WebP for better compression
var converted = await _processor.ConvertFormatAsync(
    imageStream,
    targetFormat: "webp",
    quality: 85
);
```

##### 5. Watermarking
```csharp
var watermarked = await _processor.ApplyWatermarkAsync(
    imageStream,
    new WatermarkOptions
    {
        Text = "© 2024 My Company",
        Position = WatermarkPosition.BottomRight,
        Opacity = 0.5f,
        FontSize = 24,
        TextColor = "#FFFFFF"
    });
```

---

### Storage Management

#### Overview
Multi-provider storage abstraction for file operations.

#### Enable the Feature
```csharp
options.EnableStorage = true;
options.StorageOptions = new StorageServiceOptions
{
    Provider = StorageProvider.AzureBlob,
    ConnectionString = "your-connection-string",
    DefaultContainer = "files",
    EnableVersioning = true
};
```

#### Storage Providers

| Provider | Configuration Required | Features |
|----------|----------------------|----------|
| **LocalFile** | Base path | Basic file operations |
| **AWS S3** | Access key, Secret key, Region | Versioning, Encryption, Lifecycle |
| **Azure Blob** | Connection string | Versioning, Tiers, Snapshots |
| **Google Cloud** | Service account JSON | Versioning, Lifecycle, Nearline |

#### Usage Examples

##### 1. File Upload
```csharp
var uploadResult = await _storage.UploadAsync(
    fileStream,
    "documents/report.pdf",
    "application/pdf",
    new StorageUploadOptions
    {
        Metadata = new Dictionary<string, string>
        {
            ["author"] = "John Doe",
            ["department"] = "Finance"
        },
        StorageClass = StorageClass.Standard,
        EnableEncryption = true
    });
```

##### 2. File Download
```csharp
var downloadResult = await _storage.DownloadAsync(fileId);
if (downloadResult.Success)
{
    return File(downloadResult.Content, downloadResult.ContentType);
}
```

##### 3. Bulk Operations
```csharp
// Bulk upload
var files = new Dictionary<string, Stream>
{
    ["file1.jpg"] = stream1,
    ["file2.jpg"] = stream2,
    ["file3.jpg"] = stream3
};

var bulkResult = await _storage.BulkUploadAsync(
    files,
    new StorageBulkOptions
    {
        MaxParallelism = 5,
        ContinueOnFailure = true
    });
```

##### 4. Signed URLs
```csharp
// Generate temporary access URL
var signedUrl = await _storage.GenerateSignedUrlAsync(
    fileId,
    expiry: TimeSpan.FromHours(24),
    permissions: StoragePermissions.Read
);
```

---

### CDN Integration

#### Overview
Content delivery network integration for global distribution.

#### Enable the Feature
```csharp
options.EnableCDN = true;
options.CDNOptions = new CDNServiceOptions
{
    Provider = CDNProvider.CloudFlare,
    Endpoint = "https://cdn.example.com",
    ApiKey = "your-api-key",
    DefaultCacheTTL = 86400 // 24 hours
};
```

#### CDN Providers

| Provider | Features | Regions |
|----------|----------|---------|
| **CloudFlare** | Free tier, DDoS protection | 200+ cities |
| **AWS CloudFront** | AWS integration, Lambda@Edge | 450+ POPs |
| **Azure CDN** | Azure integration, Rules engine | 130+ POPs |
| **Akamai** | Enterprise, Advanced security | 4000+ locations |

#### Usage Examples

##### 1. Upload to CDN
```csharp
var cdnResult = await _cdn.UploadToCDNAsync(
    fileId,
    fileStream,
    "image/jpeg",
    new CDNUploadOptions
    {
        CachingRules = new[]
        {
            new CachingRule
            {
                Path = "*.jpg",
                TTL = 604800, // 7 days
                BrowserTTL = 86400 // 1 day
            }
        }
    });
```

##### 2. Dynamic URL Transformations
```csharp
// Get CDN URL with on-the-fly transformations
var transformedUrl = await _cdn.GetCDNUrlAsync(
    fileId,
    new URLTransformations
    {
        Width = 400,
        Height = 300,
        Format = "webp",
        Quality = 85,
        Blur = 0,
        Sharpen = true
    });
// Result: https://cdn.example.com/image.jpg?w=400&h=300&f=webp&q=85&sharpen=1
```

##### 3. Cache Management
```csharp
// Invalidate cache
await _cdn.InvalidateCacheAsync(new[]
{
    "/images/product/*",
    "/assets/styles.css"
});

// Warm cache
await _cdn.WarmCacheAsync(new[]
{
    "https://cdn.example.com/critical/hero.jpg",
    "https://cdn.example.com/critical/logo.png"
});
```

---

### AI/ML Services

#### Overview
Artificial intelligence services for content analysis and generation.

#### Enable the Feature
```csharp
options.EnableML = true;
options.MLOptions = new MLServiceOptions
{
    Provider = MLProvider.AzureAI,
    ApiEndpoint = "https://ai.azure.com",
    ApiKey = "your-api-key",
    EnableImageAnalysis = true,
    EnableTextAnalysis = true,
    MinConfidenceThreshold = 0.7
};
```

#### ML Providers

| Provider | Capabilities | Pricing Model |
|----------|-------------|---------------|
| **Azure AI** | Vision, Text, Custom models | Pay-per-call |
| **AWS Rekognition** | Vision, Video, Custom labels | Pay-per-image |
| **Google Vision** | Vision, OCR, Safe search | Pay-per-unit |
| **OpenAI** | GPT, DALL-E, Embeddings | Token-based |

#### Image Analysis Examples

##### 1. Auto-Tagging
```csharp
var tags = await _ml.GenerateTagsAsync(imageStream);

foreach (var tag in tags.Tags.Where(t => t.Confidence > 0.8))
{
    Console.WriteLine($"{tag.Tag}: {tag.Confidence:P}");
}
```

##### 2. Content Moderation
```csharp
var moderation = await _ml.ModerateContentAsync(imageStream);

if (moderation.IsInappropriate)
{
    foreach (var issue in moderation.Issues)
    {
        Console.WriteLine($"Issue: {issue.Category} - {issue.Description}");
    }
    return BadRequest("Content violates policy");
}
```

##### 3. Object Detection
```csharp
var objects = await _ml.DetectObjectsAsync(imageStream);

foreach (var obj in objects.Objects)
{
    Console.WriteLine($"Found: {obj.Name} at ({obj.BoundingBox.X},{obj.BoundingBox.Y})");
    Console.WriteLine($"Confidence: {obj.Confidence:P}");
}
```

##### 4. Face Detection
```csharp
var faces = await _ml.DetectFacesAsync(imageStream);

foreach (var face in faces.Faces)
{
    var emotion = face.Emotions?.DominantEmotion;
    var age = face.Demographics?.EstimatedAge;

    Console.WriteLine($"Face detected:");
    Console.WriteLine($"  Emotion: {emotion}");
    Console.WriteLine($"  Age: {age?.MinAge}-{age?.MaxAge}");
    Console.WriteLine($"  Gender: {face.Demographics?.EstimatedGender}");
}
```

#### Text Analysis Examples

##### 1. Sentiment Analysis
```csharp
var sentiment = await _ml.AnalyzeSentimentAsync(reviewText);

Console.WriteLine($"Sentiment: {sentiment.Sentiment}");
Console.WriteLine($"Confidence: {sentiment.Confidence:P}");
Console.WriteLine($"Positive: {sentiment.Scores[SentimentType.Positive]:P}");
Console.WriteLine($"Negative: {sentiment.Scores[SentimentType.Negative]:P}");
```

##### 2. Entity Extraction
```csharp
var entities = await _ml.ExtractEntitiesAsync(documentText);

var organizations = entities.Entities
    .Where(e => e.Type == EntityType.Organization);

var people = entities.Entities
    .Where(e => e.Type == EntityType.Person);

var locations = entities.Entities
    .Where(e => e.Type == EntityType.Location);
```

---

### Metadata Management

#### Overview
Rich metadata system for search, tagging, and analytics.

#### Enable the Feature
```csharp
options.EnableMetadata = true;
options.MetadataOptions = new MetadataServiceOptions
{
    Provider = MetadataProvider.MongoDB,
    ConnectionString = "mongodb://localhost:27017",
    DatabaseName = "FileMetadata",
    EnableCaching = true
};
```

#### Metadata Examples

##### 1. Store Metadata
```csharp
await _metadata.StoreMetadataAsync(new FileMetadata
{
    FileId = uploadResult.FileId,
    FileName = "product-image.jpg",
    FileSizeBytes = 1048576,
    ContentType = "image/jpeg",
    Tags = new List<FileTag>
    {
        new() { Tag = "product", Source = TagSource.User },
        new() { Tag = "electronics", Source = TagSource.AI, Confidence = 0.95 }
    },
    Properties = new Dictionary<string, object>
    {
        ["productId"] = "PROD-12345",
        ["category"] = "Electronics",
        ["brand"] = "Samsung"
    }
});
```

##### 2. Search Files
```csharp
var searchResult = await _metadata.SearchAsync(new MetadataSearchCriteria
{
    Tags = new[] { "product", "electronics" },
    RequireAllTags = true,
    ContentTypes = new[] { "image/jpeg", "image/png" },
    SizeRange = new FileSizeRange
    {
        MinBytes = 100_000,   // 100KB
        MaxBytes = 10_000_000 // 10MB
    },
    CreatedRange = new TimeRange
    {
        Start = DateTime.UtcNow.AddDays(-30),
        End = DateTime.UtcNow
    },
    SortOrder = SearchSortOrder.CreatedDescending,
    MaxResults = 50,
    Skip = 0
});
```

##### 3. Tag Management
```csharp
// Add tags
await _metadata.AddTagsAsync(fileId, new[] { "featured", "bestseller" });

// Get popular tags
var popularTags = await _metadata.GetPopularTagsAsync(new TagFilters
{
    MinUsageCount = 10,
    Sources = new[] { TagSource.User, TagSource.AI },
    MaxResults = 20
});

// Get tag suggestions
var suggestions = await _metadata.SuggestTagsAsync(fileId);
var highConfidenceTags = suggestions.Suggestions
    .Where(s => s.Confidence > 0.8)
    .Select(s => s.Tag);
```

##### 4. Analytics
```csharp
var analytics = await _metadata.GetFileAnalyticsAsync(
    fileId,
    new TimeRange
    {
        Start = DateTime.UtcNow.AddDays(-30),
        End = DateTime.UtcNow
    });

Console.WriteLine($"Total views: {analytics.TotalAccesses}");
Console.WriteLine($"Unique users: {analytics.UniqueUsers}");
Console.WriteLine($"Peak time: {analytics.PeakAccessTime}");

// Access by type breakdown
foreach (var access in analytics.AccessByType)
{
    Console.WriteLine($"{access.Key}: {access.Value}");
}
```

---

## Usage Examples

### Example 1: E-Commerce Product Images

```csharp
public class ProductImageService
{
    private readonly IMarventaFileProcessor _processor;
    private readonly IMarventaStorage _storage;
    private readonly IMarventaCDN _cdn;
    private readonly IMarventaML _ml;
    private readonly IMarventaMetadata _metadata;

    public async Task<ProductImageResult> ProcessProductImage(
        IFormFile image,
        string productId)
    {
        // 1. Validate image
        var validation = await _processor.ValidateImageAsync(
            image.OpenReadStream());

        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        // 2. Optimize for web
        var optimized = await _processor.OptimizeImageAsync(
            image.OpenReadStream(),
            OptimizationLevel.High);

        // 3. Generate product thumbnails
        var thumbnails = await _processor.GenerateThumbnailsAsync(
            optimized.OptimizedImage,
            new[]
            {
                new ThumbnailSize { Name = "grid", Width = 300, Height = 300 },
                new ThumbnailSize { Name = "detail", Width = 800, Height = 800 },
                new ThumbnailSize { Name = "zoom", Width = 1600, Height = 1600 }
            });

        // 4. Upload to storage
        var uploadResult = await _storage.UploadAsync(
            optimized.OptimizedImage,
            $"products/{productId}/main.jpg",
            "image/jpeg");

        // 5. Deploy to CDN
        await _cdn.UploadToCDNAsync(
            uploadResult.FileId,
            optimized.OptimizedImage,
            "image/jpeg");

        // 6. AI analysis
        var tags = await _ml.GenerateTagsAsync(optimized.OptimizedImage);
        var colors = await _ml.AnalyzeColorsAsync(optimized.OptimizedImage);

        // 7. Store metadata
        await _metadata.StoreMetadataAsync(new FileMetadata
        {
            FileId = uploadResult.FileId,
            FileName = image.FileName,
            Properties = new Dictionary<string, object>
            {
                ["productId"] = productId,
                ["colors"] = colors.DominantColors.Take(5),
                ["aiTags"] = tags.Tags.Select(t => t.Tag)
            }
        });

        // 8. Return URLs
        return new ProductImageResult
        {
            MainImageUrl = await _cdn.GetCDNUrlAsync(uploadResult.FileId),
            ThumbnailUrls = thumbnails.Thumbnails.ToDictionary(
                t => t.Key,
                t => $"https://cdn.example.com/products/{productId}/{t.Key}.jpg"
            ),
            Tags = tags.Tags.Select(t => t.Tag),
            DominantColors = colors.DominantColors.Take(3)
        };
    }
}
```

### Example 2: Document Management with OCR

```csharp
public class DocumentService
{
    public async Task<DocumentResult> ProcessDocument(
        IFormFile document,
        DocumentMetadata metadata)
    {
        // Only storage and metadata features enabled
        // CDN disabled for sensitive documents

        // 1. Store with versioning
        var uploadResult = await _storage.UploadAsync(
            document.OpenReadStream(),
            $"documents/{metadata.Department}/{document.FileName}",
            document.ContentType,
            new StorageUploadOptions
            {
                EnableVersioning = true,
                EnableEncryption = true,
                Metadata = new Dictionary<string, string>
                {
                    ["author"] = metadata.Author,
                    ["confidentiality"] = metadata.ConfidentialityLevel,
                    ["department"] = metadata.Department
                }
            });

        // 2. Extract text (if ML enabled)
        string? extractedText = null;
        if (_ml != null)
        {
            var text = await _ml.ExtractTextAsync(document.OpenReadStream());
            extractedText = text.ExtractedText;

            // Analyze content
            var keywords = await _ml.ExtractKeywordsAsync(extractedText);
            var entities = await _ml.ExtractEntitiesAsync(extractedText);
        }

        // 3. Store searchable metadata
        await _metadata.StoreMetadataAsync(new FileMetadata
        {
            FileId = uploadResult.FileId,
            FileName = document.FileName,
            Properties = new Dictionary<string, object>
            {
                ["author"] = metadata.Author,
                ["department"] = metadata.Department,
                ["extractedText"] = extractedText,
                ["version"] = uploadResult.VersionId
            }
        });

        return new DocumentResult
        {
            DocumentId = uploadResult.FileId,
            Version = uploadResult.VersionId,
            StoragePath = uploadResult.StoragePath
        };
    }
}
```

### Example 3: Social Media Content Pipeline

```csharp
public class SocialMediaService
{
    public async Task<PostResult> CreatePost(PostCreateModel model)
    {
        // All features enabled for rich social media experience

        // 1. Content moderation
        var moderation = await _ml.ModerateContentAsync(
            model.Image.OpenReadStream());

        if (moderation.IsInappropriate)
            return new PostResult { Success = false, Reason = "Policy violation" };

        // 2. Process for social formats
        var processed = await _processor.ProcessImageAsync(
            model.Image.OpenReadStream(),
            new ProcessingOptions
            {
                Width = 1080,
                Height = 1080,
                ResizeMode = ResizeMode.Crop,
                Quality = 85
            });

        // 3. Create social media variations
        var variations = new Dictionary<string, Stream>();

        // Instagram feed
        variations["instagram_feed"] = await _processor.ProcessImageAsync(
            processed.ProcessedImage,
            new ProcessingOptions { Width = 1080, Height = 1080 });

        // Instagram story
        variations["instagram_story"] = await _processor.ProcessImageAsync(
            processed.ProcessedImage,
            new ProcessingOptions { Width = 1080, Height = 1920 });

        // Twitter
        variations["twitter"] = await _processor.ProcessImageAsync(
            processed.ProcessedImage,
            new ProcessingOptions { Width = 1200, Height = 675 });

        // 4. Upload all variations
        var uploadResults = await _storage.BulkUploadAsync(variations);

        // 5. Deploy to CDN for fast delivery
        foreach (var upload in uploadResults.Results)
        {
            await _cdn.UploadToCDNAsync(
                upload.Key,
                variations[upload.Key],
                "image/jpeg");
        }

        // 6. AI enrichment
        var tags = await _ml.GenerateTagsAsync(processed.ProcessedImage);
        var faces = await _ml.DetectFacesAsync(processed.ProcessedImage);
        var altText = await _ml.GenerateAltTextAsync(processed.ProcessedImage);

        // 7. Store post metadata
        await _metadata.StoreMetadataAsync(new FileMetadata
        {
            FileId = Guid.NewGuid().ToString(),
            Properties = new Dictionary<string, object>
            {
                ["userId"] = model.UserId,
                ["caption"] = model.Caption,
                ["variations"] = uploadResults.Results.Keys,
                ["aiTags"] = tags.Tags,
                ["faceCount"] = faces.FaceCount,
                ["altText"] = altText.AltText
            }
        });

        return new PostResult
        {
            Success = true,
            Urls = uploadResults.Results.ToDictionary(
                r => r.Key,
                r => $"https://cdn.example.com/{r.Value.FileId}"
            ),
            Tags = tags.Tags.Take(5).Select(t => t.Tag),
            AltText = altText.AltText
        };
    }
}
```

---

## API Reference

### Core Interfaces

```csharp
// File Processing
public interface IMarventaFileProcessor
{
    Task<ProcessingResult> ProcessImageAsync(Stream image, ProcessingOptions options);
    Task<ThumbnailResult> GenerateThumbnailsAsync(Stream image, ThumbnailSize[] sizes);
    Task<OptimizationResult> OptimizeImageAsync(Stream image, OptimizationLevel level);
    Task<ConversionResult> ConvertFormatAsync(Stream image, string targetFormat, int quality);
    Task<WatermarkResult> ApplyWatermarkAsync(Stream image, WatermarkOptions options);
    Task<ImageValidationResult> ValidateImageAsync(Stream file);
}

// Storage
public interface IMarventaStorage
{
    Task<StorageUploadResult> UploadAsync(Stream content, string fileName, string contentType);
    Task<StorageDownloadResult> DownloadAsync(string fileId);
    Task<StorageDeletionResult> DeleteAsync(string fileId);
    Task<StorageListResult> ListFilesAsync(string? prefix, int maxResults);
}

// CDN
public interface IMarventaCDN
{
    Task<CDNUploadResult> UploadToCDNAsync(string fileId, Stream content, string contentType);
    Task<string> GetCDNUrlAsync(string fileId, URLTransformations? transformations);
    Task<CDNInvalidationResult> InvalidateCacheAsync(string[] paths);
}

// Machine Learning
public interface IMarventaML
{
    Task<TagGenerationResult> GenerateTagsAsync(Stream image);
    Task<ModerationResult> ModerateContentAsync(Stream content);
    Task<ObjectDetectionResult> DetectObjectsAsync(Stream image);
}

// Metadata
public interface IMarventaMetadata
{
    Task<MetadataStorageResult> StoreMetadataAsync(FileMetadata metadata);
    Task<FileMetadata?> GetMetadataAsync(string fileId);
    Task<MetadataSearchResult> SearchAsync(MetadataSearchCriteria criteria);
}
```

---

## Best Practices

### 1. Feature Selection
- **Start minimal**: Enable only core features initially
- **Scale gradually**: Add features as requirements grow
- **Monitor usage**: Track which features provide value

### 2. Performance Optimization
```csharp
// Use parallel processing for bulk operations
options.FileProcessorOptions.MaxConcurrency = 10;
options.StorageOptions.EnableParallelUploads = true;

// Enable caching for frequently accessed content
options.MetadataOptions.EnableCaching = true;
options.CDNOptions.DefaultCacheTTL = 86400;

// Use appropriate storage classes
var uploadOptions = new StorageUploadOptions
{
    StorageClass = accessFrequency switch
    {
        "high" => StorageClass.Standard,
        "medium" => StorageClass.StandardIA,
        "low" => StorageClass.Archive,
        _ => StorageClass.Standard
    }
};
```

### 3. Error Handling
```csharp
// Implement retry policies
services.AddMarventaFramework(options =>
{
    options.StorageOptions.RetryPolicy = new RetryPolicyOptions
    {
        MaxRetries = 3,
        InitialDelay = TimeSpan.FromSeconds(1),
        MaxDelay = TimeSpan.FromSeconds(30),
        BackoffMultiplier = 2
    };
});

// Handle feature availability
if (options.EnableML)
{
    var tags = await _ml.GenerateTagsAsync(image);
    // Use tags
}
else
{
    // Fallback to manual tagging
}
```

### 4. Security Considerations
```csharp
// Always validate input
var validation = await _processor.ValidateImageAsync(file);
if (!validation.IsValid)
    throw new ValidationException(validation.Errors);

// Use encryption for sensitive data
options.StorageOptions.EnableEncryption = true;

// Implement access control
var signedUrl = await _storage.GenerateSignedUrlAsync(
    fileId,
    expiry: TimeSpan.FromHours(1),
    permissions: StoragePermissions.Read
);
```

---

## Troubleshooting

### Common Issues and Solutions

#### 1. Feature Not Available
```csharp
// Problem: Service is null
// Solution: Ensure feature is enabled

services.AddMarventaFramework(options =>
{
    options.EnableML = true; // Enable the feature
});
```

#### 2. Provider Configuration
```csharp
// Problem: Provider not working
// Solution: Check configuration

options.StorageOptions = new StorageServiceOptions
{
    Provider = StorageProvider.AzureBlob,
    ConnectionString = configuration.GetConnectionString("AzureStorage")
    // Ensure connection string is correct
};
```

#### 3. Performance Issues
```csharp
// Problem: Slow processing
// Solution: Enable parallel processing

options.FileProcessorOptions.MaxConcurrency = Environment.ProcessorCount;
options.StorageOptions.EnableParallelUploads = true;
```

#### 4. Memory Issues
```csharp
// Problem: Out of memory with large files
// Solution: Use streaming

// Instead of loading entire file
var bytes = File.ReadAllBytes(path);

// Use streaming
using var stream = File.OpenRead(path);
await _storage.UploadAsync(stream, fileName, contentType);
```

---

## Testing Framework

### 🧪 Mock Services for Testing

Marventa Framework includes comprehensive mock services for testing:

```csharp
// Test Setup
services.AddMarventaFramework(options =>
{
    // Use mock providers for testing
    options.StorageOptions.Provider = StorageProvider.Mock;
    options.CDNOptions.Provider = CDNProvider.Mock;
    options.MLOptions.Provider = MLProvider.Mock;
    options.FileProcessorOptions.Provider = FileProcessorProvider.Mock;
    options.MetadataOptions.Provider = MetadataProvider.Mock;
});
```

### Available Test Classes

**Core Mock Services:**
- `MockFileProcessorTests` - Image processing operations
- `MockStorageServiceTests` - File storage operations
- `MockCDNServiceTests` - CDN operations (coming soon)
- `MockMLServiceTests` - AI/ML services (coming soon)
- `MockMetadataServiceTests` - Metadata operations (coming soon)

**Sample Test Pattern:**
```csharp
[Fact]
public async Task UploadFile_Should_ReturnSuccess()
{
    // Arrange
    var fileContent = new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F };
    using var stream = new MemoryStream(fileContent);

    // Act
    var result = await _storageService.UploadFileAsync(stream, "test.txt", "text/plain");

    // Assert
    result.Should().NotBeNull();
    result.Success.Should().BeTrue();
    result.FileId.Should().NotBeNullOrEmpty();
    result.FileSizeBytes.Should().Be(fileContent.Length);
}
```

---

## New Features Added

### 📊 File Analytics System

Track file usage and generate insights:

```csharp
// Enable analytics
public class FileMetadata
{
    public FileAnalytics? Analytics { get; set; }
}

public class FileAnalytics
{
    public int TotalViews { get; set; }
    public int TotalDownloads { get; set; }
    public DateTime? LastAccessedAt { get; set; }
    public double AverageRating { get; set; }
    public int UniqueViewers { get; set; }
}

// Usage
var metadata = await _metadataService.GetFileMetadataAsync(fileId);
Console.WriteLine($"File viewed {metadata.Analytics.TotalViews} times");
```

### 🔍 Enhanced Validation

Improved validation with detailed results:

```csharp
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public string Format { get; set; } = string.Empty;
    public int ColorDepth { get; set; }
    public ImageDimensions? Dimensions { get; set; }
}

// Usage with new CheckFormat option
var options = new ValidationOptions
{
    CheckFormat = true,
    CheckDimensions = true,
    MaxWidth = 1920,
    MaxHeight = 1080
};

var result = await _fileProcessor.ValidateImageAsync(imageStream, options);
if (!result.IsValid)
{
    foreach (var error in result.Errors)
        Console.WriteLine($"Error: {error}");
}
```

### ⚡ Progressive JPEG Support

Enable progressive encoding for better user experience:

```csharp
var optimizationOptions = new OptimizationOptions
{
    Quality = 85,
    EnableProgressive = true,  // NEW: Progressive JPEG support
    PreserveMetadata = false
};

var result = await _fileProcessor.OptimizeImageAsync(imageStream, optimizationOptions);
```

### 📦 Bulk Processing Enhancements

Improved batch operations with detailed results:

```csharp
public class BulkProcessingResult
{
    public int TotalProcessed { get; set; }
    public int SuccessfullyProcessed { get; set; }
    public int Failed { get; set; }
    public Dictionary<string, ProcessingResult> Results { get; set; } = new();
    public TimeSpan TotalProcessingTime { get; set; }
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();
}

// Usage
var files = new Dictionary<string, Stream>
{
    ["image1.jpg"] = stream1,
    ["image2.png"] = stream2
};

var bulkResult = await _fileProcessor.ProcessBatchAsync(files, processingOptions);
Console.WriteLine($"Processed {bulkResult.SuccessfullyProcessed}/{bulkResult.TotalProcessed} files");
```

---

## Support

- 📧 **Email**: support@marventa.com
- 📖 **Documentation**: [docs.marventa.com](https://docs.marventa.com)
- 🐛 **Issues**: [GitHub Issues](https://github.com/marventa/framework/issues)
- 💬 **Community**: [Discord Server](https://discord.gg/marventa)

---

<div align="center">
  <strong>Built with ❤️ by the Marventa Team</strong>
  <br>
  <sub>Making file management simple, powerful, and intelligent</sub>
</div>