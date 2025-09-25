# Marventa Framework

Modern .NET 9.0 enterprise framework with clean architecture.

## Quick Start

```csharp
// Add Elasticsearch
services.AddElasticsearch(configuration);

// Use in controller
[HttpGet("search")]
public async Task<IActionResult> Search([FromQuery] string query)
{
    var result = await _searchService.SearchAsync<Product>(new SearchRequest
    {
        Query = query,
        Page = 1,
        PageSize = 20
    });

    return Ok(result);
}
```

## Configuration

```json
{
  "Elasticsearch": {
    "ConnectionString": "http://localhost:9200",
    "IndexPrefix": "myapp-",
    "TimeoutSeconds": 60
  }
}
```

## Architecture

- **Core**: Interfaces and domain models
- **Infrastructure**: External services (Elasticsearch, etc.)
- **Application**: Business logic and validation
- **Web**: Controllers and middleware

## Features

- **Elasticsearch**: Full-text search with HTTP client
- **Clean Architecture**: Proper layer separation
- **Validation**: FluentValidation integration
- **Logging**: Structured logging support
- **Testing**: Unit and integration tests

Built with .NET 9.0 for high performance and modern development.