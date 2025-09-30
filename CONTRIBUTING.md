# Contributing to Marventa Framework

Thank you for your interest in contributing to Marventa Framework! This document provides guidelines and instructions for contributing.

## Code of Conduct

By participating in this project, you agree to maintain a respectful and inclusive environment for all contributors.

## How to Contribute

### Reporting Bugs

1. Check if the bug has already been reported in [Issues](https://github.com/AdemKinatas/Marventa.Framework/issues)
2. If not, create a new issue using the Bug Report template
3. Provide detailed information:
   - Framework version
   - .NET version
   - Operating system
   - Steps to reproduce
   - Expected vs actual behavior
   - Code samples and stack traces

### Suggesting Features

1. Check if the feature has been suggested in [Issues](https://github.com/AdemKinatas/Marventa.Framework/issues)
2. Create a new issue using the Feature Request template
3. Clearly describe:
   - The problem it solves
   - Proposed API/usage
   - Benefits to users
   - Any breaking changes

### Asking Questions

- Use [GitHub Discussions](https://github.com/AdemKinatas/Marventa.Framework/discussions) for questions
- Check existing discussions first
- Provide context and code samples

## Development Setup

### Prerequisites

- .NET SDK 8.0 or 9.0
- Visual Studio 2022 or Rider
- Git
- SQL Server (LocalDB for testing)

### Getting Started

1. **Fork the repository**
   ```bash
   # Click "Fork" on GitHub, then clone your fork
   git clone https://github.com/YOUR_USERNAME/Marventa.Framework.git
   cd Marventa.Framework
   ```

2. **Create a branch**
   ```bash
   git checkout -b feature/my-new-feature
   # or
   git checkout -b fix/issue-123
   ```

3. **Restore packages**
   ```bash
   dotnet restore
   ```

4. **Build the solution**
   ```bash
   dotnet build
   ```

5. **Run tests**
   ```bash
   dotnet test
   ```

## Coding Guidelines

### Code Style

- Use C# 12 features where appropriate
- Follow [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use meaningful variable and method names
- Add XML documentation comments to public APIs
- Keep methods small and focused (SOLID principles)

### Example

```csharp
/// <summary>
/// Retrieves an entity by its unique identifier
/// </summary>
/// <param name="id">The entity identifier</param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>The entity if found; otherwise null</returns>
public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
{
    return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
}
```

### Project Structure

```
Marventa.Framework/
├── Marventa.Framework.Core/          # Core entities, interfaces, enums
├── Marventa.Framework.Domain/        # Domain models, value objects, aggregates
├── Marventa.Framework.Application/   # CQRS, MediatR, behaviors
├── Marventa.Framework.Infrastructure/# Implementations, data access
├── Marventa.Framework.Web/           # ASP.NET Core extensions, middleware
└── Marventa.Framework.Tests/         # Unit, integration, and E2E tests
```

### Testing

- Write unit tests for all new features
- Aim for >80% code coverage
- Use descriptive test names: `MethodName_Scenario_ExpectedResult`
- Follow Arrange-Act-Assert pattern

```csharp
[Fact]
public async Task GetByIdAsync_ExistingEntity_ReturnsEntity()
{
    // Arrange
    var entity = new TestEntity { Id = Guid.NewGuid() };
    await _repository.AddAsync(entity);

    // Act
    var result = await _repository.GetByIdAsync(entity.Id);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(entity.Id, result.Id);
}
```

## Pull Request Process

1. **Update documentation**
   - Update README.md if adding features
   - Add XML comments to public APIs
   - Update CHANGELOG.md

2. **Ensure tests pass**
   ```bash
   dotnet test
   ```

3. **Check code quality**
   ```bash
   dotnet build /p:TreatWarningsAsErrors=true
   ```

4. **Commit your changes**
   ```bash
   git add .
   git commit -m "Add feature: description"
   ```

   Follow conventional commits:
   - `feat: add new feature`
   - `fix: resolve bug`
   - `docs: update documentation`
   - `test: add tests`
   - `refactor: improve code structure`

5. **Push to your fork**
   ```bash
   git push origin feature/my-new-feature
   ```

6. **Create Pull Request**
   - Go to the original repository
   - Click "New Pull Request"
   - Select your branch
   - Fill out the PR template
   - Link related issues

7. **Code Review**
   - Address reviewer feedback
   - Make requested changes
   - Push updates to your branch

8. **Merge**
   - Once approved, your PR will be merged
   - Delete your branch after merge

## Commit Message Guidelines

Use clear, descriptive commit messages:

```bash
# Good
feat: add Redis caching support
fix: resolve null reference in TenantContext
docs: update CQRS documentation

# Bad
update stuff
fix bug
changes
```

## Documentation

When adding new features:

1. Update README.md
2. Add XML documentation comments
3. Create/update docs in `/docs` folder
4. Add examples in `/samples` folder
5. Update CHANGELOG.md

## Version Management

We follow [Semantic Versioning](https://semver.org/):

- **MAJOR** (3.x.x): Breaking changes
- **MINOR** (x.2.x): New features, backward compatible
- **PATCH** (x.x.0): Bug fixes, backward compatible

## Release Process

1. Update version in `.csproj` files
2. Update CHANGELOG.md
3. Create git tag: `v3.2.0`
4. Push tag to trigger GitHub Actions
5. NuGet package published automatically

## Community

- **GitHub Discussions**: For questions and ideas
- **GitHub Issues**: For bugs and feature requests
- **Pull Requests**: For code contributions

## Recognition

Contributors will be recognized in:
- CHANGELOG.md
- README.md (major contributions)
- GitHub contributors page

## Questions?

- 💬 [GitHub Discussions](https://github.com/AdemKinatas/Marventa.Framework/discussions)
- 📧 Email: ademkinatas@gmail.com

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

**Thank you for contributing to Marventa Framework!** 🚀