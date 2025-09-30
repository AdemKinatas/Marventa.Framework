# Installation Guide

This guide will help you install and set up Marventa Framework in your .NET project.

## Prerequisites

Before installing Marventa Framework, ensure you have:

- **.NET SDK** 8.0 or 9.0 ([Download](https://dotnet.microsoft.com/download))
- **IDE** (Visual Studio 2022, VS Code, or Rider)
- **SQL Server** (LocalDB, Express, or Full version) - Optional
- **Redis** - Optional, for distributed caching

## Installation Methods

### Method 1: NuGet Package Manager (Recommended)

#### Via .NET CLI
```bash
dotnet add package Marventa.Framework
```

#### Via Package Manager Console (Visual Studio)
```powershell
Install-Package Marventa.Framework
```

#### Via Visual Studio UI
1. Right-click on your project
2. Select "Manage NuGet Packages"
3. Search for "Marventa.Framework"
4. Click "Install"

### Method 2: Package Reference

Add this to your `.csproj` file:

```xml
<ItemGroup>
  <PackageReference Include="Marventa.Framework" Version="3.2.0" />
</ItemGroup>
```

Then restore packages:
```bash
dotnet restore
```

## Verifying Installation

After installation, verify the framework is correctly installed:

```bash
dotnet list package | findstr Marventa
```

You should see:
```
> Marventa.Framework    3.2.0
```

## Project Setup

### 1. Create a New Project

```bash
# Create a new Web API project
dotnet new webapi -n MyProject
cd MyProject

# Add Marventa Framework
dotnet add package Marventa.Framework
```

### 2. Update Program.cs

```csharp
using Marventa.Framework.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add Marventa Framework
builder.Services.AddMarventaFramework(builder.Configuration, options =>
{
    options.EnableLogging = true;
    options.EnableCaching = true;
    options.EnableHealthChecks = true;
    options.EnableExceptionHandling = true;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
```

### 3. Configure appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Marventa": {
    "ApiKey": "your-secret-api-key-here",
    "Caching": {
      "Provider": "Memory"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyProjectDb;Trusted_Connection=True;"
  }
}
```

### 4. Run the Application

```bash
dotnet run
```

Visit `https://localhost:5001/health` to verify the framework is working.

## Optional Dependencies

Depending on which features you enable, you may need additional packages:

### Entity Framework Core (for Repository pattern)
```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
```

### Redis (for distributed caching)
```bash
dotnet add package StackExchange.Redis
```

### FluentValidation (for CQRS validation)
```bash
dotnet add package FluentValidation.DependencyInjectionExtensions
```

### MediatR (for CQRS)
```bash
dotnet add package MediatR
```

> **Note**: Most dependencies are already included in Marventa.Framework package. Only install additional packages if you're using specific features.

## Troubleshooting

### Issue: Package Not Found

**Error**: `Unable to find package 'Marventa.Framework'`

**Solution**: Ensure your NuGet package source includes nuget.org:
```bash
dotnet nuget list source
dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org
```

### Issue: Version Conflict

**Error**: `Version conflict detected for Marventa.Framework`

**Solution**: Update all Marventa packages to the same version:
```bash
dotnet add package Marventa.Framework --version 3.2.0
```

### Issue: Missing Dependencies

**Error**: `Could not load file or assembly 'Marventa.Framework.Core'`

**Solution**: Clean and restore:
```bash
dotnet clean
dotnet restore
dotnet build
```

## Next Steps

Now that you have Marventa Framework installed:

1. [Quick Start Tutorial](quick-start.md) - Build your first API
2. [Configuration Guide](configuration.md) - Learn about configuration options
3. [First Project](first-project.md) - Create a complete CRUD application

## Upgrading

To upgrade to the latest version:

```bash
dotnet add package Marventa.Framework
```

Or specify a specific version:
```bash
dotnet add package Marventa.Framework --version 3.2.0
```

See the [Migration Guide](../migration/v2-to-v3.md) for breaking changes.

## Support

Need help with installation?

- 💬 [GitHub Discussions](https://github.com/AdemKinatas/Marventa.Framework/discussions)
- 🐛 [Report Issues](https://github.com/AdemKinatas/Marventa.Framework/issues)
- 📧 Email: ademkinatas@gmail.com