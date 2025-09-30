# Marventa Framework Tests

Comprehensive test suite for Marventa Framework.

## Test Structure

```
Marventa.Framework.Tests/
├── Unit/                          # Unit tests (fast, isolated)
│   ├── Core/                      # Core entities and value objects
│   ├── Domain/                    # Domain logic and aggregates
│   └── Application/               # Application services and behaviors
├── Integration/                   # Integration tests (database, external services)
│   ├── Repository/                # Repository pattern tests
│   ├── MediatR/                   # CQRS pipeline tests
│   └── Caching/                   # Caching integration tests
├── Performance/                   # Performance and benchmark tests
│   └── Benchmarks/                # BenchmarkDotNet benchmarks
└── E2E/                          # End-to-end tests (full application flow)
```

## Running Tests

### All Tests
```bash
dotnet test
```

### Specific Category
```bash
# Unit tests only
dotnet test --filter Category=Unit

# Integration tests only
dotnet test --filter Category=Integration

# Performance tests only
dotnet test --filter Category=Performance
```

### With Coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Specific Test Class
```bash
dotnet test --filter FullyQualifiedName~BaseEntityTests
```

## Test Categories

### Unit Tests
- **Fast**: < 100ms per test
- **Isolated**: No external dependencies
- **Mocked**: All dependencies mocked
- **Coverage Target**: > 90%

**Examples:**
- `BaseEntityTests` - Entity base class functionality
- `ValidationBehaviorTests` - MediatR validation pipeline
- `ApiResponseTests` - Response model tests

### Integration Tests
- **Medium Speed**: < 1s per test
- **Real Dependencies**: Database, cache, etc.
- **In-Memory**: Uses in-memory databases
- **Coverage Target**: > 70%

**Examples:**
- `BaseRepositoryIntegrationTests` - Repository with real DbContext
- `CachingIntegrationTests` - Memory/Redis cache tests
- `UnitOfWorkTests` - Transaction management

### Performance Tests
- **Benchmarks**: BenchmarkDotNet tests
- **Load Tests**: k6 or Artillery scripts
- **Profiling**: Memory and CPU profiling

**Examples:**
- `CachingBenchmarks` - Cache performance comparison
- `RepositoryBenchmarks` - Query performance
- `SerializationBenchmarks` - JSON serialization

### E2E Tests
- **Slow**: > 1s per test
- **Full Stack**: Complete application flow
- **Real Services**: Optional real external services
- **Coverage Target**: > 50%

**Examples:**
- `OrderFlowE2ETests` - Complete order creation flow
- `AuthenticationE2ETests` - Login and authorization
- `TenantIsolationE2ETests` - Multi-tenancy validation

## Writing Tests

### Unit Test Example

```csharp
using Xunit;

namespace Marventa.Framework.Tests.Unit.Core;

[Trait("Category", "Unit")]
public class BaseEntityTests
{
    [Fact]
    public void BaseEntity_NewInstance_ShouldHaveId()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        Assert.NotEqual(Guid.Empty, entity.Id);
    }
}
```

### Integration Test Example

```csharp
using Xunit;

namespace Marventa.Framework.Tests.Integration.Repository;

[Trait("Category", "Integration")]
public class BaseRepositoryIntegrationTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly BaseRepository<TestEntity> _repository;

    public BaseRepositoryIntegrationTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        _repository = new BaseRepository<TestEntity>(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntityToDatabase()
    {
        // Arrange
        var entity = new TestEntity { Name = "Test" };

        // Act
        await _repository.AddAsync(entity);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _repository.GetByIdAsync(entity.Id);
        Assert.NotNull(result);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
```

### Performance Test Example

```csharp
using BenchmarkDotNet.Attributes;

namespace Marventa.Framework.Tests.Performance.Benchmarks;

[MemoryDiagnoser]
[Trait("Category", "Performance")]
public class CachingBenchmarks
{
    [Benchmark]
    public void MemoryCache_Set()
    {
        _cache.Set("key", "value", TimeSpan.FromMinutes(5));
    }
}
```

## Test Guidelines

### DO
✅ Use descriptive test names (MethodName_Scenario_ExpectedResult)
✅ Follow Arrange-Act-Assert pattern
✅ Keep tests small and focused (one assert per test)
✅ Use test categories (`[Trait("Category", "Unit")]`)
✅ Clean up resources in Dispose/DisposeAsync
✅ Mock external dependencies in unit tests
✅ Use realistic test data

### DON'T
❌ Test framework code (Entity Framework, MediatR, etc.)
❌ Have tests depend on each other
❌ Use Thread.Sleep (use Task.Delay if needed)
❌ Hard-code connection strings (use in-memory)
❌ Test private methods directly
❌ Create flaky tests (random failures)

## Code Coverage

### Viewing Coverage
```bash
# Generate coverage report
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=html

# Open report
start ./coverage/index.html
```

### Coverage Targets
- **Overall**: > 80%
- **Core**: > 90%
- **Domain**: > 85%
- **Application**: > 80%
- **Infrastructure**: > 70%
- **Web**: > 60%

## Continuous Integration

Tests run automatically on:
- Every push to any branch
- Every pull request
- Nightly builds (full test suite + performance)

## Troubleshooting

### Tests Failing Locally

**Issue**: "Cannot connect to database"
**Solution**: Use in-memory database for integration tests

**Issue**: "Test timeout"
**Solution**: Increase timeout: `[Fact(Timeout = 5000)]`

**Issue**: "Flaky test failures"
**Solution**: Avoid shared state, use unique test data

### Performance Tests

**Issue**: "Benchmark results vary widely"
**Solution**: Run benchmarks in Release mode:
```bash
dotnet run -c Release --project Benchmarks
```

## Tools

- **xUnit**: Test framework
- **Moq**: Mocking library
- **FluentAssertions**: Assertion library (optional)
- **BenchmarkDotNet**: Performance benchmarking
- **Coverlet**: Code coverage
- **ReportGenerator**: Coverage reports

## Contributing

When adding new features:
1. Write unit tests first (TDD)
2. Add integration tests for infrastructure
3. Update this README if adding new test categories
4. Ensure coverage meets targets

## Learn More

- [xUnit Documentation](https://xunit.net/)
- [BenchmarkDotNet](https://benchmarkdotnet.org/)
- [Moq Documentation](https://github.com/moq/moq4)
- [Testing Best Practices](../docs/testing/best-practices.md)