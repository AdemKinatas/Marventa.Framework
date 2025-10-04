using Marventa.Framework.Configuration;
using Marventa.Framework.Extensions;
using Marventa.Framework.Middleware;
using Marventa.Framework.TestApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<TestDbContext>(options =>
    options.UseInMemoryDatabase("MarventaFrameworkTest"));

// Register TestDbContext as DbContext for Outbox
builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<TestDbContext>());

// Add Outbox Pattern
builder.Services.AddOutbox();

// Add Idempotency Pattern (requires distributed cache)
builder.Services.AddDistributedMemoryCache(); // Using in-memory cache for testing
builder.Services.AddIdempotency();

// Add API Key Authentication
builder.Services.AddApiKeyAuthentication();

// Add Request/Response Logging
builder.Services.Configure<LoggingOptions>(
    builder.Configuration.GetSection("LoggingOptions"));

// Marventa Framework
builder.Services.AddMarventa(
    builder.Configuration,
    typeof(Program).Assembly);

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

// Request/Response Logging Middleware (must be early in pipeline)
app.UseMiddleware<RequestResponseLoggingMiddleware>();

// Idempotency Middleware (after routing, before endpoints)
app.UseRouting();
app.UseIdempotency();

// Marventa Framework Middleware
app.UseMarventa(builder.Configuration, app.Environment);

app.Run();
