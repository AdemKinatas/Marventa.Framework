using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Marventa.Framework.Core.Interfaces.Security;
using Marventa.Framework.Infrastructure.Services.Security;

namespace Marventa.Framework.Infrastructure.Services;

public class JwtKeyRotationHostedService : BackgroundService
{
    private readonly IJwtKeyRotationService _keyRotationService;
    private readonly JwtKeyRotationOptions _options;
    private readonly ILogger<JwtKeyRotationHostedService> _logger;

    public JwtKeyRotationHostedService(
        IJwtKeyRotationService keyRotationService,
        IOptions<JwtKeyRotationOptions> options,
        ILogger<JwtKeyRotationHostedService> logger)
    {
        _keyRotationService = keyRotationService;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.EnableAutomaticRotation)
        {
            _logger.LogInformation("JWT key rotation is disabled");
            return;
        }

        // Initial rotation to ensure we have a key
        try
        {
            await _keyRotationService.RotateKeysAsync();
            _logger.LogInformation("Initial JWT key rotation completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to perform initial JWT key rotation");
        }

        // Periodic rotation
        using var timer = new PeriodicTimer(_options.RotationInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await timer.WaitForNextTickAsync(stoppingToken);
                await _keyRotationService.RotateKeysAsync();
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("JWT key rotation service is stopping");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during scheduled JWT key rotation");
            }
        }
    }
}