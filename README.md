# Marventa.Framework

A comprehensive .NET framework following Clean Architecture principles with JWT authentication, CQRS, caching, rate limiting, health checks, and more.

## Features

- ✅ **Clean Architecture** - Proper separation of concerns with Core, Domain, Application, Infrastructure, and Web layers
- ✅ **JWT Authentication** - Complete token-based authentication and authorization
- ✅ **CQRS Pattern** - Command Query Responsibility Segregation implementation
- ✅ **Caching** - Memory caching with Redis interface support
- ✅ **Rate Limiting** - Advanced rate limiting middleware
- ✅ **Health Checks** - Database and cache health monitoring
- ✅ **API Versioning** - Multiple versioning strategies support
- ✅ **Exception Handling** - Global exception handling middleware
- ✅ **Repository Pattern** - Generic repository with Unit of Work
- ✅ **Security** - Encryption services and secure token management
- ✅ **Communication** - Email and SMS services
- ✅ **HTTP Client** - Circuit breaker pattern implementation
- ✅ **Feature Flags** - Dynamic feature toggle support
- ✅ **Logging** - Comprehensive logging infrastructure

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package Marventa.Framework
```

Or via Package Manager Console:

```powershell
Install-Package Marventa.Framework
```

## Quick Start

Add Marventa Framework to your ASP.NET Core application:

```csharp
// Program.cs
using Marventa.Framework;

var builder = WebApplication.CreateBuilder(args);

// Add Marventa Framework services
builder.Services.AddMarventa();

var app = builder.Build();

// Use Marventa Framework middleware
app.UseMarventa();

app.Run();
```

## Configuration

Configure framework options in your `appsettings.json`:

```json
{
  "JWT": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "your-issuer",
    "Audience": "your-audience",
    "ExpiryInMinutes": 60
  },
  "RateLimit": {
    "EnableRateLimiting": true,
    "MaxRequests": 100,
    "WindowSizeInMinutes": 1
  },
  "ApiVersioning": {
    "DefaultVersion": "1.0",
    "Strategy": "Header"
  }
}
```

## Usage Examples

### JWT Authentication
```csharp
// Inject ITokenService
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;

    public AuthController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var token = await _tokenService.GenerateAccessTokenAsync(claims);
        return Ok(new { Token = token });
    }
}
```

### Repository Pattern
```csharp
// Inject repository
public class UserService
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IRepository<User> userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<User> CreateUserAsync(User user)
    {
        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return user;
    }
}
```

### CQRS Pattern
```csharp
// Command
public record CreateUserCommand(string Name, string Email) : ICommand<User>;

// Handler
public class CreateUserHandler : ICommandHandler<CreateUserCommand, User>
{
    public async Task<User> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        // Implementation
    }
}

// Usage
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserCommand command)
    {
        var user = await _mediator.Send(command);
        return Ok(user);
    }
}
```

## Documentation

For detailed documentation and advanced usage examples, visit our [GitHub repository](https://github.com/AdemKinatas/Marventa.Framework).

## Contributing

Contributions are welcome! Please read our contributing guidelines and submit pull requests to our GitHub repository.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

If you encounter any issues or have questions:

- 🐛 [Report bugs](https://github.com/AdemKinatas/Marventa.Framework/issues)
- 💬 [Ask questions](https://github.com/AdemKinatas/Marventa.Framework/discussions)
- 📖 [Read documentation](https://github.com/AdemKinatas/Marventa.Framework)

---

Made with ❤️ by the Adem Kınataş
