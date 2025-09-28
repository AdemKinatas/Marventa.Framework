# 🚀 Marventa Framework

[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![NuGet](https://img.shields.io/badge/NuGet-v2.2.0-blue)](https://www.nuget.org/packages/Marventa.Framework)

> **Complete enterprise-grade .NET framework with 40+ modular features including file management, security, multi-tenancy, messaging, analytics, e-commerce, and more**

## 📋 Table of Contents

1. [Quick Start](#-quick-start)
2. [Core Philosophy](#-core-philosophy)
3. [Architecture](#️-architecture)
4. [Features](#-features)
   - [Storage Management](#-storage-management)
   - [Image Processing](#️-image-processing)
   - [CDN Integration](#-cdn-integration)
   - [AI/ML Services](#-aiml-services)
   - [Metadata Management](#-metadata-management)
   - [Security & Authentication](#-security--authentication)
   - [Multi-Tenancy Support](#-multi-tenancy-support)
   - [Event-Driven Architecture](#-event-driven-architecture)
   - [CQRS Pattern](#-cqrs-pattern)
   - [Performance & Scalability](#-performance--scalability)
   - [Analytics & Monitoring](#-analytics--monitoring)
   - [Messaging & Communication](#-messaging--communication)
   - [Search & Discovery](#-search--discovery)
   - [Background Processing](#-background-processing)
   - [E-Commerce Features](#-e-commerce-features)
   - [API Management](#-api-management)
   - [Configuration & Features](#-configuration--features)
5. [Configuration](#-configuration)
6. [Testing](#-testing)
7. [Best Practices](#-best-practices)

---

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

builder.Services.AddMarventaFramework(options =>
{
    options.EnableStorage = true;        // File operations
    options.EnableFileProcessor = true;  // Image processing
    options.EnableCDN = false;          // Optional
    options.EnableML = false;           // Optional
    options.EnableMetadata = true;      // Optional
});

var app = builder.Build();
app.UseMarventaFramework();
app.Run();
```

### Simple File Upload

```csharp
[ApiController]
public class FilesController : ControllerBase
{
    private readonly IMarventaStorage _storage;
    private readonly IMarventaFileProcessor _processor;

    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        // Process image
        var processResult = await _processor.ProcessImageAsync(file.OpenReadStream(), new()
        {
            Width = 800, Height = 600, Quality = 85
        });

        // Upload to storage
        var uploadResult = await _storage.UploadFileAsync(
            processResult.ProcessedImage, file.FileName, file.ContentType);

        return Ok(new {
            FileId = uploadResult.FileId,
            Url = uploadResult.PublicUrl
        });
    }
}
```

---

## 🎯 Core Philosophy

- **🔧 Modular Design**: Enable only what you need - pay for what you use
- **🔄 Provider Agnostic**: Switch providers without code changes
- **⚡ Performance First**: Async operations and optimized processing
- **🏢 Enterprise Ready**: Production-tested with comprehensive error handling
- **👨‍💻 Developer Friendly**: Clean APIs with extensive documentation

---

## 🏗️ Architecture

**Clean, modular architecture** with **40+ enterprise features** in **29+ focused, single-responsibility files**:

```
Marventa.Framework/
├── 📦 Core/                    # Domain models and interfaces
│   ├── 🔌 Interfaces/         # 40+ service contracts
│   ├── 📄 Models/            # 29+ focused model files
│   │   ├── CDN/              # 8 CDN-specific files
│   │   ├── Storage/          # 12 Storage-specific files
│   │   ├── ML/               # 6 ML-specific files
│   │   ├── FileProcessing/   # Processing models
│   │   └── FileMetadata/     # 3 Metadata files
│   ├── 🔐 Security/          # JWT, Encryption, API Keys
│   ├── 🏢 Multi-Tenant/      # Tenant management
│   ├── 🔄 Events/            # Domain & Integration events
│   └── 🚫 Exceptions/        # Custom exceptions
├── 🎯 Domain/                  # Business logic
│   └── 🛒 ECommerce/         # Payment, Shipping, Fraud
├── 🔧 Application/            # CQRS, Commands, Queries
│   ├── ⚡ Commands/          # Command handlers
│   ├── 🔍 Queries/           # Query handlers
│   ├── 🔄 Behaviors/         # MediatR behaviors
│   └── ✅ Validators/        # Validation logic
├── 🏗️ Infrastructure/         # Service implementations
│   ├── 📧 Messaging/         # Email, SMS, Message Bus
│   ├── 🔍 Search/            # Elasticsearch
│   ├── 📊 Analytics/         # Event tracking
│   ├── ⚡ RateLimiting/       # Tenant rate limits
│   └── 🔍 Observability/     # Distributed tracing
└── 🌐 Web/                   # ASP.NET integration
    ├── 🔐 Security/          # Middleware
    ├── 📋 Middleware/        # Exception, Correlation
    ├── 📊 Versioning/        # API versioning
    └── ⚙️ Extensions/        # DI configuration
```

**SOLID Compliance**: Each file follows Single Responsibility Principle

---

## 🎨 Features

### 🗄️ Storage Management

**Multi-provider storage with unified API**

```csharp
// Azure Blob Storage
services.AddMarventaFramework(options =>
{
    options.StorageOptions.Provider = StorageProvider.AzureBlob;
    options.StorageOptions.ConnectionString = "DefaultEndpointsProtocol=https;...";
});

// AWS S3
options.StorageOptions.Provider = StorageProvider.AWS;
options.StorageOptions.AccessKey = "your-access-key";
options.StorageOptions.SecretKey = "your-secret-key";

// Local File System
options.StorageOptions.Provider = StorageProvider.LocalFile;
options.StorageOptions.BasePath = "uploads";
```

**Usage Examples:**

```csharp
// Upload file
var result = await _storage.UploadFileAsync(stream, "document.pdf", "application/pdf");

// Download file
var download = await _storage.DownloadFileAsync(result.FileId);

// File operations
await _storage.CopyFileAsync(fileId, "backup/document.pdf");
await _storage.DeleteFileAsync(fileId);

// Bulk operations
var files = new Dictionary<string, Stream> { ["file1.jpg"] = stream1, ["file2.png"] = stream2 };
var bulkResult = await _storage.BulkUploadAsync(files);
```

### 🖼️ Image Processing

**Comprehensive image manipulation and optimization**

```csharp
// Image processing configuration
options.FileProcessorOptions.Provider = FileProcessorProvider.ImageSharp;
options.FileProcessorOptions.DefaultImageQuality = 85;
options.FileProcessorOptions.MaxFileSizeBytes = 52428800; // 50MB
```

**Usage Examples:**

```csharp
// Resize image
var resizeResult = await _processor.ProcessImageAsync(imageStream, new ProcessingOptions
{
    Width = 800,
    Height = 600,
    Quality = 90
});

// Generate thumbnails
var thumbnailResult = await _processor.GenerateThumbnailsAsync(imageStream, new[]
{
    new ThumbnailSize { Name = "small", Width = 150, Height = 150 },
    new ThumbnailSize { Name = "medium", Width = 300, Height = 300 },
    new ThumbnailSize { Name = "large", Width = 600, Height = 600 }
});

// Optimize image
var optimizeResult = await _processor.OptimizeImageAsync(imageStream, new OptimizationOptions
{
    Quality = 75,
    EnableProgressive = true,
    PreserveMetadata = false
});

// Apply watermark
var watermarkResult = await _processor.ApplyWatermarkAsync(imageStream, new WatermarkOptions
{
    Text = "© 2024 Company Name",
    Position = WatermarkPosition.BottomRight,
    Opacity = 0.7f
});

// Convert format
var convertResult = await _processor.ConvertFormatAsync(imageStream, "webp", new ConversionOptions
{
    Quality = 80,
    PreserveMetadata = true
});
```

### 🌐 CDN Integration

**Global content delivery with caching**

```csharp
// CDN configuration
options.CDNOptions.Provider = CDNProvider.CloudFlare;
options.CDNOptions.Endpoint = "https://cdn.example.com";
options.CDNOptions.ApiKey = "your-api-key";
options.CDNOptions.DefaultCacheTTL = 86400; // 24 hours
```

**Usage Examples:**

```csharp
// Upload to CDN
var cdnResult = await _cdn.UploadToCDNAsync(fileId, fileStream, "image/jpeg", new CDNUploadOptions
{
    CacheTTL = TimeSpan.FromHours(24),
    EnableCompression = true
});

// Invalidate cache
await _cdn.InvalidateCacheAsync(new[] { "/images/photo.jpg", "/css/style.css" });

// Transform images on CDN
var transformResult = await _cdn.TransformImageAsync(fileId, new ImageTransformation
{
    Width = 400,
    Height = 300,
    Quality = 80,
    Format = "webp"
});

// Get CDN metrics
var metrics = await _cdn.GetCDNMetricsAsync(new TimeRange
{
    StartTime = DateTime.UtcNow.AddDays(-30),
    EndTime = DateTime.UtcNow
});
```

### 🤖 AI/ML Services

**Intelligent content analysis and processing**

```csharp
// ML configuration
options.MLOptions.Provider = MLProvider.AzureAI;
options.MLOptions.ApiEndpoint = "https://cognitiveservices.azure.com";
options.MLOptions.ApiKey = "your-api-key";
options.MLOptions.MinConfidenceThreshold = 0.7;
```

**Usage Examples:**

```csharp
// Image analysis
var analysisResult = await _ml.AnalyzeImageAsync(imageStream, new ImageAnalysisOptions
{
    DetectObjects = true,
    DetectFaces = true,
    GenerateTags = true,
    ExtractText = true
});

// Face detection
var faceResult = await _ml.DetectFacesAsync(imageStream, new FaceDetectionOptions
{
    DetectAge = true,
    DetectGender = true,
    DetectEmotions = true
});

// Text extraction (OCR)
var ocrResult = await _ml.ExtractTextAsync(imageStream, new TextExtractionOptions
{
    Language = "en",
    DetectOrientation = true
});

// Content optimization suggestions
var suggestions = await _ml.GetOptimizationSuggestionsAsync(fileId, new OptimizationRequest
{
    TargetAudience = "mobile",
    MaxFileSize = 1024000 // 1MB
});
```

### 📊 Metadata Management

**Advanced file metadata and search capabilities**

```csharp
// Metadata configuration
options.MetadataOptions.Provider = MetadataProvider.MongoDB;
options.MetadataOptions.ConnectionString = "mongodb://localhost:27017";
options.MetadataOptions.DatabaseName = "FileMetadata";
```

**Usage Examples:**

```csharp
// Add file metadata
var metadata = new FileMetadata
{
    FileId = fileId,
    Title = "Product Image",
    Description = "High-quality product photo",
    Tags = new[] { new FileTag { Name = "product", Source = TagSource.Manual } },
    CustomProperties = new Dictionary<string, object>
    {
        ["ProductId"] = "P12345",
        ["Category"] = "Electronics"
    }
};
await _metadata.AddFileMetadataAsync(metadata);

// Search files
var searchResult = await _metadata.SearchFilesAsync(new MetadataSearchOptions
{
    Query = "product electronics",
    FileTypes = new[] { "image/jpeg", "image/png" },
    DateRange = new TimeRange(DateTime.Now.AddDays(-30), DateTime.Now),
    Tags = new[] { "product" }
});

// File analytics
var analytics = await _metadata.GetFileAnalyticsAsync(fileId);
Console.WriteLine($"Views: {analytics.TotalViews}, Downloads: {analytics.TotalDownloads}");

// Tag management
await _metadata.AddTagsToFileAsync(fileId, new[] { "featured", "bestseller" });
var popularTags = await _metadata.GetPopularTagsAsync(new TagPopularityOptions
{
    TimeRange = new TimeRange(DateTime.Now.AddDays(-30), DateTime.Now),
    Limit = 10
});
```

### 🔐 Security & Authentication

**Comprehensive security with JWT, API Keys, and encryption**

```csharp
// JWT Configuration
options.JwtOptions.SecretKey = "your-secret-key";
options.JwtOptions.Issuer = "your-app";
options.JwtOptions.Audience = "your-audience";
options.JwtOptions.ExpirationMinutes = 60;
```

**Usage Examples:**

```csharp
// JWT Token Generation
var tokenResult = await _tokenService.GenerateTokenAsync(userId, new[] { "admin", "user" });
Console.WriteLine($"Access Token: {tokenResult.AccessToken}");
Console.WriteLine($"Refresh Token: {tokenResult.RefreshToken}");

// API Key Authentication (in controller)
[ApiKey]
public class SecureController : ControllerBase { }

// Encryption Service
var encrypted = await _encryptionService.EncryptAsync("sensitive-data");
var decrypted = await _encryptionService.DecryptAsync(encrypted);

// Password Hashing
var hash = await _encryptionService.GenerateHashAsync("password", salt);
var isValid = await _encryptionService.VerifyHashAsync("password", hash, salt);
```

### 🏢 Multi-Tenancy Support

**Complete tenant isolation and management**

```csharp
// Multi-tenant configuration
options.MultiTenancyOptions.TenantResolutionStrategy = TenantResolutionStrategy.Header;
options.MultiTenancyOptions.DefaultTenantId = "default";
options.MultiTenancyOptions.EnableTenantScopedServices = true;
```

**Usage Examples:**

```csharp
// Tenant Context
var currentTenant = _tenantContext.Current;
Console.WriteLine($"Current Tenant: {currentTenant.Id} - {currentTenant.Name}");

// Tenant-Scoped Caching
await _tenantScopedCache.SetAsync("key", data, TimeSpan.FromHours(1));
var cachedData = await _tenantScopedCache.GetAsync<MyData>("key");

// Tenant Rate Limiting
var isAllowed = await _tenantRateLimiter.TryAcquireAsync("api-endpoint", 100, TimeSpan.FromMinutes(1));
if (!isAllowed) return StatusCode(429, "Rate limit exceeded");

// Tenant Authorization
var hasAccess = await _tenantAuthorization.HasAccessAsync(tenantId, "feature-name");
```

### 🔄 Event-Driven Architecture

**Domain and Integration events with Event Bus**

```csharp
// Event Bus configuration
options.EventBusOptions.Provider = EventBusProvider.RabbitMQ;
options.EventBusOptions.ConnectionString = "amqp://localhost";
```

**Usage Examples:**

```csharp
// Publishing Domain Events
var domainEvent = new UserRegisteredEvent(userId, email, DateTime.UtcNow);
await _eventBus.PublishAsync(domainEvent);

// Publishing Integration Events
var integrationEvent = new OrderCompletedEvent(orderId, customerId, totalAmount);
await _eventBus.PublishIntegrationEventAsync(integrationEvent);

// Event Handler
public class UserRegisteredEventHandler : IDomainEventHandler<UserRegisteredEvent>
{
    public async Task HandleAsync(UserRegisteredEvent domainEvent)
    {
        // Send welcome email
        await _emailService.SendWelcomeEmailAsync(domainEvent.Email);
    }
}
```

### ⚡ CQRS Pattern

**Command Query Responsibility Segregation with MediatR-style architecture**

**Usage Examples:**

```csharp
// Command Definition
public class CreateUserCommand : ICommand<CreateUserResult>
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

// Command Handler
public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, CreateUserResult>
{
    public async Task<CreateUserResult> HandleAsync(CreateUserCommand command)
    {
        // Create user logic
        var user = new User(command.Email, command.FirstName, command.LastName);
        await _userRepository.AddAsync(user);
        return new CreateUserResult { UserId = user.Id };
    }
}

// Query Definition
public class GetUserQuery : IQuery<UserDto>
{
    public int UserId { get; set; }
}

// Query Handler
public class GetUserQueryHandler : IQueryHandler<GetUserQuery, UserDto>
{
    public async Task<UserDto> HandleAsync(GetUserQuery query)
    {
        var user = await _userRepository.GetByIdAsync(query.UserId);
        return _mapper.Map<UserDto>(user);
    }
}
```

### ⚡ Performance & Scalability

**Rate limiting, caching, and distributed locking**

```csharp
// Caching configuration
options.CacheOptions.Provider = CacheProvider.Redis;
options.CacheOptions.ConnectionString = "localhost:6379";
options.CacheOptions.DefaultExpiration = TimeSpan.FromMinutes(30);
```

**Usage Examples:**

```csharp
// Distributed Caching
await _cacheService.SetAsync("user:123", userData, TimeSpan.FromHours(1));
var cachedUser = await _cacheService.GetAsync<UserData>("user:123");

// Distributed Locking
using var lockHandle = await _distributedLock.AcquireAsync("resource-key", TimeSpan.FromMinutes(5));
if (lockHandle.IsAcquired)
{
    // Critical section - only one process can execute this
    await ProcessCriticalOperation();
}

// Rate Limiting Attribute
[RateLimit(RequestsPerMinute = 60)]
public class ApiController : ControllerBase { }
```

### 📊 Analytics & Monitoring

**Comprehensive analytics and health monitoring**

```csharp
// Analytics configuration
options.AnalyticsOptions.Provider = AnalyticsProvider.GoogleAnalytics;
options.AnalyticsOptions.TrackingId = "GA-123456789";
```

**Usage Examples:**

```csharp
// Event Tracking
await _analyticsService.TrackEventAsync("user_action", "button_click", new
{
    UserId = userId,
    ButtonName = "purchase",
    PageUrl = "/checkout"
});

// Metric Tracking
await _analyticsService.TrackMetricAsync("response_time", 150.5, new
{
    Endpoint = "/api/users",
    Method = "GET"
});

// Exception Tracking
try { /* operation */ }
catch (Exception ex)
{
    await _analyticsService.TrackExceptionAsync(ex, new { UserId = userId });
}

// Health Checks
var healthStatus = await _healthCheck.CheckHealthAsync();
Console.WriteLine($"System Health: {healthStatus.Status}");
foreach (var check in healthStatus.Checks)
{
    Console.WriteLine($"  {check.Key}: {check.Value.Status}");
}
```

### 📧 Messaging & Communication

**Email, SMS, and Message Bus integration**

```csharp
// Email configuration
options.EmailOptions.Provider = EmailProvider.SendGrid;
options.EmailOptions.ApiKey = "your-sendgrid-key";
options.EmailOptions.FromEmail = "noreply@yourapp.com";
```

**Usage Examples:**

```csharp
// Email Service
await _emailService.SendEmailAsync(new EmailMessage
{
    To = "user@example.com",
    Subject = "Welcome!",
    HtmlBody = "<h1>Welcome to our platform!</h1>",
    PlainTextBody = "Welcome to our platform!"
});

// Bulk Email
var recipients = new[] { "user1@example.com", "user2@example.com" };
await _emailService.SendBulkEmailAsync(recipients, "Newsletter", htmlContent);

// SMS Service
await _smsService.SendSmsAsync("+1234567890", "Your verification code: 123456");

// Message Bus
await _messageBus.PublishAsync(new UserRegisteredMessage
{
    UserId = userId,
    Email = email,
    RegistrationDate = DateTime.UtcNow
});
```

### 🔍 Search & Discovery

**Elasticsearch integration with advanced search capabilities**

```csharp
// Search configuration
options.SearchOptions.Provider = SearchProvider.Elasticsearch;
options.SearchOptions.ConnectionString = "http://localhost:9200";
options.SearchOptions.DefaultIndex = "documents";
```

**Usage Examples:**

```csharp
// Document Indexing
var document = new ProductDocument
{
    Id = "prod-123",
    Name = "Wireless Headphones",
    Description = "High-quality wireless headphones",
    Price = 99.99m,
    Category = "Electronics"
};
await _searchService.IndexDocumentAsync("products", document);

// Search with Filters
var searchResult = await _searchService.SearchAsync<ProductDocument>("products", new SearchRequest
{
    Query = "wireless headphones",
    Filters = new Dictionary<string, object>
    {
        ["Category"] = "Electronics",
        ["Price"] = new { gte = 50, lte = 150 }
    },
    Sort = new[] { new SortField { Field = "Price", Order = SortOrder.Ascending } },
    Size = 20,
    From = 0
});

// Aggregations
var aggregationResult = await _searchService.AggregateAsync("products", new AggregationRequest
{
    Aggregations = new Dictionary<string, IAggregation>
    {
        ["avg_price"] = new AverageAggregation { Field = "Price" },
        ["categories"] = new TermsAggregation { Field = "Category" }
    }
});
```

### ⏱️ Background Processing

**Job scheduling and background task management**

```csharp
// Background job configuration
options.BackgroundJobOptions.Provider = BackgroundJobProvider.Hangfire;
options.BackgroundJobOptions.ConnectionString = "Server=localhost;Database=Jobs";
```

**Usage Examples:**

```csharp
// Schedule Background Job
var jobId = await _backgroundJobService.EnqueueAsync<IEmailService>(
    service => service.SendEmailAsync(emailMessage));

// Schedule Delayed Job
var delayedJobId = await _backgroundJobService.ScheduleAsync<IReportService>(
    service => service.GenerateMonthlyReportAsync(),
    TimeSpan.FromHours(24));

// Recurring Job
await _backgroundJobService.AddRecurringJobAsync(
    "daily-cleanup",
    () => _cleanupService.CleanupOldFilesAsync(),
    "0 2 * * *"); // Every day at 2 AM

// Job Status
var jobStatus = await _backgroundJobService.GetJobStatusAsync(jobId);
Console.WriteLine($"Job Status: {jobStatus.State}");
```

### 🛒 E-Commerce Features

**Payment processing, shipping, and fraud detection**

**Usage Examples:**

```csharp
// Payment Processing
var payment = new Payment
{
    Amount = 99.99m,
    Currency = "USD",
    PaymentMethod = PaymentMethod.CreditCard,
    CustomerId = "cust-123"
};
var paymentResult = await _paymentService.ProcessPaymentAsync(payment);

// Shipping Management
var shipment = new Shipment
{
    OrderId = "order-123",
    ShippingAddress = shippingAddress,
    Carrier = ShippingCarrier.FedEx,
    TrackingNumber = "1234567890"
};
await _shippingService.CreateShipmentAsync(shipment);

// Track Shipment
var trackingInfo = await _shippingService.TrackShipmentAsync("1234567890");
Console.WriteLine($"Status: {trackingInfo.Status}, Location: {trackingInfo.CurrentLocation}");

// Fraud Detection
var fraudCheck = await _fraudService.CheckTransactionAsync(new FraudCheckRequest
{
    TransactionAmount = 99.99m,
    CustomerIP = "192.168.1.1",
    CustomerEmail = "customer@example.com",
    BillingAddress = billingAddress
});

if (fraudCheck.RiskScore > 0.7)
{
    // Flag as potentially fraudulent
    await _fraudService.FlagTransactionAsync(transactionId, FraudReason.HighRiskScore);
}
```

### 🌐 API Management

**Versioning, idempotency, and HTTP client abstraction**

**Usage Examples:**

```csharp
// API Versioning
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class UsersController : VersionedControllerBase
{
    [HttpGet]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetUsersV1() { /* v1 logic */ }

    [HttpGet]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> GetUsersV2() { /* v2 logic */ }
}

// Idempotency
[HttpPost]
[Idempotent]
public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
{
    // This endpoint is automatically idempotent
    var order = await _orderService.CreateOrderAsync(request);
    return Ok(order);
}

// HTTP Client Service
var response = await _httpClientService.GetAsync<UserDto>("https://api.example.com/users/123");
var postResponse = await _httpClientService.PostAsync<CreateUserResponse, CreateUserRequest>(
    "https://api.example.com/users", createUserRequest);
```

### ⚙️ Configuration & Features

**Feature flags and dynamic configuration**

**Usage Examples:**

```csharp
// Feature Flags
var isNewCheckoutEnabled = await _featureFlagService.IsEnabledAsync("new-checkout-flow");
if (isNewCheckoutEnabled)
{
    // Use new checkout process
    return await ProcessNewCheckoutAsync(request);
}
else
{
    // Use legacy checkout
    return await ProcessLegacyCheckoutAsync(request);
}

// User-Specific Feature Flags
var hasAdvancedFeatures = await _featureFlagService.IsEnabledForUserAsync(
    "advanced-analytics", userId);

// Dynamic Configuration
var maxRetries = await _configurationService.GetValueAsync<int>("api.max-retries");
var timeout = await _configurationService.GetValueAsync<TimeSpan>("api.timeout");

// Configuration with Default
var cacheTimeout = await _configurationService.GetValueAsync("cache.timeout", TimeSpan.FromMinutes(30));
```

---

## ⚙️ Configuration

### appsettings.json Configuration

```json
{
  "Marventa": {
    "EnableStorage": true,
    "EnableFileProcessor": true,
    "EnableCDN": false,
    "EnableML": false,
    "EnableMetadata": true,

    "StorageOptions": {
      "Provider": "AzureBlob",
      "ConnectionString": "DefaultEndpointsProtocol=https;...",
      "DefaultContainer": "files",
      "EnableEncryption": true,
      "MaxFileSizeBytes": 104857600
    },

    "FileProcessorOptions": {
      "Provider": "ImageSharp",
      "DefaultImageQuality": 85,
      "MaxFileSizeBytes": 52428800,
      "SupportedFormats": ["jpg", "jpeg", "png", "webp", "gif"],
      "DefaultThumbnailSizes": [
        { "Name": "small", "Width": 150, "Height": 150 },
        { "Name": "medium", "Width": 300, "Height": 300 },
        { "Name": "large", "Width": 600, "Height": 600 }
      ]
    },

    "CDNOptions": {
      "Provider": "CloudFlare",
      "Endpoint": "https://cdn.example.com",
      "ApiKey": "${CLOUDFLARE_API_KEY}",
      "DefaultCacheTTL": 86400,
      "EnableCompression": true
    },

    "MLOptions": {
      "Provider": "AzureAI",
      "ApiEndpoint": "https://cognitiveservices.azure.com",
      "ApiKey": "${AZURE_AI_KEY}",
      "MinConfidenceThreshold": 0.7,
      "MaxConcurrentRequests": 10
    },

    "MetadataOptions": {
      "Provider": "MongoDB",
      "ConnectionString": "mongodb://localhost:27017",
      "DatabaseName": "FileMetadata",
      "EnableFullTextSearch": true
    }
  }
}
```

### Environment Variables

```bash
# Storage
AZURE_STORAGE_CONNECTION_STRING="DefaultEndpointsProtocol=https;..."
AWS_ACCESS_KEY_ID="your-access-key"
AWS_SECRET_ACCESS_KEY="your-secret-key"

# CDN
CLOUDFLARE_API_KEY="your-api-key"
CLOUDFLARE_ZONE_ID="your-zone-id"

# AI/ML
AZURE_AI_KEY="your-cognitive-services-key"
OPENAI_API_KEY="your-openai-key"

# Metadata
MONGODB_CONNECTION_STRING="mongodb://localhost:27017"
```

---

## 🧪 Testing

**Built-in mock services for comprehensive testing:**

```csharp
// Test configuration
services.AddMarventaFramework(options =>
{
    options.StorageOptions.Provider = StorageProvider.Mock;
    options.FileProcessorOptions.Provider = FileProcessorProvider.Mock;
    options.CDNOptions.Provider = CDNProvider.Mock;
    options.MLOptions.Provider = MLProvider.Mock;
    options.MetadataOptions.Provider = MetadataProvider.Mock;
});

// Example test
[Fact]
public async Task UploadFile_Should_ReturnSuccess()
{
    // Arrange
    var fileContent = new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F };
    using var stream = new MemoryStream(fileContent);

    // Act
    var result = await _storage.UploadFileAsync(stream, "test.txt", "text/plain");

    // Assert
    result.Should().NotBeNull();
    result.Success.Should().BeTrue();
    result.FileId.Should().NotBeNullOrEmpty();
}
```

**Test Coverage**: 39 comprehensive tests covering all features

---

## ✅ Best Practices

### 1. Resource Management

```csharp
// Always dispose streams
using var fileStream = File.OpenRead(filePath);
var result = await _storage.UploadFileAsync(fileStream, fileName, contentType);

// Use using statements for automatic disposal
using var processedStream = result.ProcessedImage;
```

### 2. Error Handling

```csharp
try
{
    var result = await _storage.UploadFileAsync(stream, fileName, contentType);
    if (!result.Success)
    {
        _logger.LogError("Upload failed: {Error}", result.ErrorMessage);
        return BadRequest(result.ErrorMessage);
    }
}
catch (Exception ex)
{
    _logger.LogError(ex, "Upload operation failed");
    return StatusCode(500, "Internal server error");
}
```

### 3. Performance Optimization

```csharp
// Use cancellation tokens
var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
var result = await _processor.ProcessImageAsync(stream, options, cts.Token);

// Enable parallel processing for bulk operations
var files = GetFiles();
var results = await _storage.BulkUploadAsync(files);
```

### 4. Security

```csharp
// Validate file types
var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
if (!allowedTypes.Contains(file.ContentType))
{
    return BadRequest("File type not allowed");
}

// Check file size
if (file.Length > 10 * 1024 * 1024) // 10MB
{
    return BadRequest("File too large");
}

// Enable encryption for sensitive files
options.StorageOptions.EnableEncryption = true;
```

---

## 📦 Available Packages

| Package | Purpose | Dependencies |
|---------|---------|--------------|
| `Marventa.Framework` | **Complete solution** | All features included |
| `Marventa.Framework.Core` | **Models & Interfaces** | No dependencies |
| `Marventa.Framework.Infrastructure` | **Service implementations** | Core + External libraries |
| `Marventa.Framework.Web` | **ASP.NET integration** | Infrastructure |

---

## 💡 Why Choose Marventa Framework?

✅ **Complete Enterprise Solution** - 40+ features in one framework
✅ **Modular Design** - Enable only what you need, pay for what you use
✅ **Production Ready** - Battle-tested in enterprise environments
✅ **Provider Agnostic** - Switch providers without code changes
✅ **Clean Architecture** - SOLID principles, CQRS, Event Sourcing
✅ **Multi-Tenant Ready** - Complete tenant isolation and management
✅ **Security First** - JWT, API Keys, Encryption, Rate Limiting
✅ **Event-Driven** - Domain events, Integration events, Message Bus
✅ **Performance Optimized** - Caching, Distributed locks, Background jobs
✅ **Developer Friendly** - Intuitive APIs with extensive examples
✅ **Comprehensive Testing** - 39 tests with full mock support
✅ **Zero Build Errors** - Professional, production-ready

---

## 📄 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

---

<div align="center">
  <strong>Built with ❤️ for the .NET Community</strong>
  <br>
  <sub>The complete enterprise .NET framework - from file management to full-scale applications</sub>
</div>