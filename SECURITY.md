# Security Policy

## Supported Versions

We release patches for security vulnerabilities in the following versions:

| Version | Supported          |
| ------- | ------------------ |
| 3.2.x   | :white_check_mark: |
| 3.1.x   | :white_check_mark: |
| 3.0.x   | :white_check_mark: |
| < 3.0   | :x:                |

## Reporting a Vulnerability

We take security vulnerabilities seriously. If you discover a security issue, please follow these steps:

### 1. Do Not Publicly Disclose

Please do not create a public GitHub issue for security vulnerabilities.

### 2. Report Privately

Send an email to **ademkinatas@gmail.com** with:

- **Subject**: `[SECURITY] Brief description of the vulnerability`
- **Description**: Detailed description of the vulnerability
- **Impact**: What could an attacker do with this vulnerability?
- **Steps to Reproduce**: How to reproduce the vulnerability
- **Affected Versions**: Which versions are affected
- **Suggested Fix**: If you have suggestions on how to fix it

### 3. Include

```markdown
## Vulnerability Details
- **Type**: [e.g., SQL Injection, XSS, CSRF, etc.]
- **Component**: [e.g., Repository, Authentication, etc.]
- **Severity**: [Critical/High/Medium/Low]

## Steps to Reproduce
1. Step 1
2. Step 2
3. Step 3

## Proof of Concept
```code or screenshots```

## Suggested Fix
How the vulnerability could be fixed
```

### 4. Response Timeline

- **Initial Response**: Within 48 hours
- **Assessment**: Within 7 days
- **Fix**: Depends on severity
  - Critical: Within 7 days
  - High: Within 14 days
  - Medium: Within 30 days
  - Low: Next release

### 5. Disclosure Process

1. We will acknowledge your report within 48 hours
2. We will investigate and provide an assessment
3. We will develop and test a fix
4. We will release a security patch
5. We will publicly disclose the vulnerability after users have had time to update (typically 30 days)

## Security Best Practices

When using Marventa Framework, follow these security best practices:

### 1. Authentication & Authorization

```csharp
// Always use JWT or API Key authentication
builder.Services.AddMarventaFramework(config, options =>
{
    options.EnableJWT = true;
    options.EnableApiKeys = true;
});
```

### 2. Input Validation

```csharp
// Always validate user input
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
```

### 3. SQL Injection Prevention

```csharp
// Use parameterized queries (automatically handled by EF Core)
var products = await _repository.Query()
    .Where(p => p.Category == category) // Safe
    .ToListAsync();

// Never use raw SQL with user input
// ❌ BAD: await _context.Database.ExecuteSqlRawAsync($"SELECT * FROM Products WHERE Name = '{userInput}'");
// ✅ GOOD: Use LINQ or parameterized queries
```

### 4. Secrets Management

```json
// Never commit secrets to source control
// Use User Secrets in development
{
  "Marventa": {
    "ApiKey": "use-user-secrets-or-environment-variables"
  }
}
```

```bash
# Store secrets securely
dotnet user-secrets set "Marventa:ApiKey" "your-secret-key"
```

### 5. HTTPS Only

```csharp
// Always use HTTPS in production
app.UseHttpsRedirection();

// Enable HSTS
app.UseHsts();
```

### 6. CORS Configuration

```csharp
// Restrict CORS to known origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("https://yourdomain.com")
            .AllowAnyMethod()
            .AllowAnyHeader());
});
```

### 7. Rate Limiting

```csharp
// Enable rate limiting to prevent abuse
builder.Services.AddMarventaFramework(config, options =>
{
    options.EnableRateLimiting = true;
});
```

### 8. Data Encryption

```csharp
// Encrypt sensitive data
builder.Services.AddMarventaFramework(config, options =>
{
    options.EnableEncryption = true;
});
```

## Known Security Features

Marventa Framework includes built-in security features:

- ✅ **JWT Authentication**: Token-based authentication
- ✅ **API Key Management**: API key generation and validation
- ✅ **Encryption Services**: AES encryption for sensitive data
- ✅ **Input Validation**: FluentValidation integration
- ✅ **SQL Injection Prevention**: Parameterized queries via EF Core
- ✅ **XSS Prevention**: Output encoding
- ✅ **CSRF Protection**: Anti-forgery tokens
- ✅ **Rate Limiting**: Request throttling
- ✅ **Audit Logging**: Track all operations
- ✅ **Tenant Isolation**: Multi-tenancy data isolation

## Security Updates

Subscribe to security updates:

- Watch the GitHub repository
- Enable notifications for releases
- Follow security advisories

## Security Checklist

Before deploying to production:

- [ ] All secrets stored securely (Azure Key Vault, AWS Secrets Manager, etc.)
- [ ] HTTPS enforced
- [ ] CORS configured properly
- [ ] Rate limiting enabled
- [ ] Input validation on all endpoints
- [ ] Authentication and authorization implemented
- [ ] Audit logging enabled
- [ ] Error messages don't expose sensitive information
- [ ] Dependencies up to date
- [ ] Security headers configured (HSTS, X-Frame-Options, etc.)

## Contact

For security concerns:
- **Email**: ademkinatas@gmail.com
- **Subject**: [SECURITY] Your issue

## Attribution

We recognize and thank security researchers who responsibly disclose vulnerabilities. With your permission, we will acknowledge you in:

- CHANGELOG.md
- Security advisories
- GitHub contributors

Thank you for helping keep Marventa Framework secure!