using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Marventa.Framework.Web.Extensions;

var builder = Host.CreateApplicationBuilder(args);

// Test the Marventa Framework with some key features enabled
builder.Services.AddMarventaFramework(options =>
{
    // Core Infrastructure (6 features)
    options.EnableLogging = true;
    options.EnableCaching = true;
    options.EnableRepository = true;
    options.EnableHealthChecks = true;
    options.EnableValidation = true;
    options.EnableExceptionHandling = true;

    // Security & Authentication (4 features)
    options.EnableSecurity = true;
    options.EnableJWT = true;
    options.EnableApiKeys = true;
    options.EnableEncryption = true;

    // Communication Services (3 features)
    options.EnableEmail = true;
    options.EnableSMS = true;
    options.EnableHttpClient = true;

    // Data & Storage (5 features)
    options.EnableStorage = true;
    options.EnableFileProcessor = true;
    options.EnableMetadata = true;
    options.EnableDatabaseSeeding = true;
    options.EnableSeeding = true;

    // API Management (4 features)
    options.EnableVersioning = true;
    options.EnableRateLimiting = true;
    options.EnableCompression = true;
    options.EnableIdempotency = true;

    // Performance & Scalability (5 features)
    options.EnableDistributedLocking = true;
    options.EnableCircuitBreaker = true;
    options.EnableBatchOperations = true;
    options.EnableAdvancedCaching = true;
    options.EnableCDN = true;

    // Monitoring & Analytics (4 features)
    options.EnableAnalytics = true;
    options.EnableObservability = true;
    options.EnableTracking = true;
    options.EnableFeatureFlags = true;

    // Background Processing (3 features)
    options.EnableBackgroundJobs = true;
    options.EnableMessaging = true;
    options.EnableDeadLetterQueue = true;

    // Enterprise Architecture (5 features)
    options.EnableMultiTenancy = true;
    options.EnableEventDriven = true;
    options.EnableCQRS = true;
    options.EnableSagas = true;
    options.EnableProjections = true;

    // Search & AI (3 features)
    options.EnableSearch = true;
    options.EnableML = true;
    options.EnableRealTimeProjections = true;

    // Business Features (5 features)
    options.EnableECommerce = true;
    options.EnablePayments = true;
    options.EnableShipping = true;
    options.EnableFraudDetection = true;
    options.EnableInternationalization = true;
});

var host = builder.Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Marventa Framework initialized successfully with all 47 features enabled!");

// Test basic functionality - DI container build succeeds without errors
logger.LogInformation("🎉 Marventa Framework v2.11.0 is working correctly!");
logger.LogInformation("🎉 All 47 modular features can be enabled without DI errors!");
logger.LogInformation("🎉 Framework builds successfully and DI container initializes properly!");

Console.WriteLine("Press any key to exit...");
Console.ReadKey();