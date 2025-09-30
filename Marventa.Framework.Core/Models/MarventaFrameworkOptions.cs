using Marventa.Framework.Core.Configuration;

namespace Marventa.Framework.Core.Models;

/// <summary>
/// Modular framework configuration options
/// </summary>
public class MarventaFrameworkOptions
{
    // Core Infrastructure (6 features)
    public bool EnableLogging { get; set; } = false;
    public bool EnableCaching { get; set; } = false;
    public bool EnableRepository { get; set; } = false;
    public bool EnableHealthChecks { get; set; } = false;
    public bool EnableValidation { get; set; } = false;
    public bool EnableExceptionHandling { get; set; } = false;

    // Security & Authentication (4 features)
    public bool EnableSecurity { get; set; } = false;
    public bool EnableJWT { get; set; } = false;
    public bool EnableApiKeys { get; set; } = false;
    public bool EnableEncryption { get; set; } = false;

    // Communication Services (3 features)
    public bool EnableEmail { get; set; } = false;
    public bool EnableSMS { get; set; } = false;
    public bool EnableHttpClient { get; set; } = false;

    // Data & Storage (5 features)
    public bool EnableStorage { get; set; } = false;
    public bool EnableFileProcessor { get; set; } = false;
    public bool EnableMetadata { get; set; } = false;
    public bool EnableDatabaseSeeding { get; set; } = false;
    public bool EnableSeeding { get; set; } = false;

    // API Management (4 features)
    public bool EnableVersioning { get; set; } = false;
    public bool EnableRateLimiting { get; set; } = false;
    public bool EnableCompression { get; set; } = false;
    public bool EnableIdempotency { get; set; } = false;

    // Performance & Scalability (5 features)
    public bool EnableDistributedLocking { get; set; } = false;
    public bool EnableCircuitBreaker { get; set; } = false;
    public bool EnableBatchOperations { get; set; } = false;
    public bool EnableAdvancedCaching { get; set; } = false;
    public bool EnableCDN { get; set; } = false;

    // Monitoring & Analytics (4 features)
    public bool EnableAnalytics { get; set; } = false;
    public bool EnableObservability { get; set; } = false;
    public bool EnableTracking { get; set; } = false;
    public bool EnableFeatureFlags { get; set; } = false;

    // Background Processing (3 features)
    public bool EnableBackgroundJobs { get; set; } = false;
    public bool EnableMessaging { get; set; } = false;
    public bool EnableDeadLetterQueue { get; set; } = false;

    // Enterprise Architecture (5 features)
    public bool EnableMultiTenancy { get; set; } = false;
    public bool EnableEventDriven { get; set; } = false;
    public bool EnableCQRS { get; set; } = false;
    public bool EnableSagas { get; set; } = false;
    public bool EnableProjections { get; set; } = false;

    // Search & AI (3 features)
    public bool EnableSearch { get; set; } = false;
    public bool EnableML { get; set; } = false;
    public bool EnableRealTimeProjections { get; set; } = false;

    // Business Features (5 features)
    public bool EnableECommerce { get; set; } = false;
    public bool EnablePayments { get; set; } = false;
    public bool EnableShipping { get; set; } = false;
    public bool EnableFraudDetection { get; set; } = false;
    public bool EnableInternationalization { get; set; } = false;

    // Configuration Options
    public LoggingOptions LoggingOptions { get; set; } = new();
    public CachingOptions CachingOptions { get; set; } = new();
    public SecurityOptions SecurityOptions { get; set; } = new();
    public StorageOptions StorageOptions { get; set; } = new();
    public MiddlewareOptions MiddlewareOptions { get; set; } = new();
    public CqrsOptions CqrsOptions { get; set; } = new();
}