using Microsoft.Extensions.Configuration;

namespace Marventa.Framework.Web.Options;

public class MarventaCoreOptions
{
    private readonly IConfiguration _configuration;
    private readonly string _serviceName;
    private readonly string? _serviceVersion;

    internal bool MultiTenancyEnabled { get; private set; }
    internal bool IdempotencyEnabled { get; private set; }
    internal bool ObservabilityEnabled { get; private set; }
    internal bool ValidationEnabled { get; private set; }
    internal bool JwtAuthenticationEnabled { get; private set; }
    internal bool JwtKeyRotationEnabled { get; private set; }
    internal bool AuditingEnabled { get; private set; }

    public MarventaCoreOptions(IConfiguration configuration, string serviceName, string? serviceVersion = null)
    {
        _configuration = configuration;
        _serviceName = serviceName;
        _serviceVersion = serviceVersion;
    }

    public MarventaCoreOptions UseMultiTenancy()
    {
        MultiTenancyEnabled = true;
        return this;
    }

    public MarventaCoreOptions UseIdempotency()
    {
        IdempotencyEnabled = true;
        return this;
    }

    public MarventaCoreOptions UseObservability()
    {
        ObservabilityEnabled = true;
        return this;
    }

    public MarventaCoreOptions UseValidation()
    {
        ValidationEnabled = true;
        return this;
    }

    public MarventaCoreOptions UseJwtAuthentication()
    {
        JwtAuthenticationEnabled = true;
        return this;
    }

    public MarventaCoreOptions UseJwtKeyRotation()
    {
        JwtKeyRotationEnabled = true;
        return this;
    }

    public MarventaCoreOptions UseAuditing()
    {
        AuditingEnabled = true;
        return this;
    }

    public MarventaCoreOptions UseAll()
    {
        return UseMultiTenancy()
            .UseIdempotency()
            .UseObservability()
            .UseValidation()
            .UseJwtAuthentication()
            .UseJwtKeyRotation()
            .UseAuditing();
    }

    internal IConfiguration Configuration => _configuration;
    internal string ServiceName => _serviceName;
    internal string? ServiceVersion => _serviceVersion;
}