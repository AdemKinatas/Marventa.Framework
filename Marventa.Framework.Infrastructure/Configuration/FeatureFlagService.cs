using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Marventa.Framework.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Infrastructure.Configuration;

public class FeatureFlagService : IFeatureFlagService
{
    private readonly IConfiguration _configuration;
    private readonly ICacheService _cacheService;
    private readonly ILogger<FeatureFlagService> _logger;
    private const string FeatureFlagSection = "FeatureFlags";
    private const string CachePrefix = "feature_flag:";

    public FeatureFlagService(IConfiguration configuration, ICacheService cacheService, ILogger<FeatureFlagService> logger)
    {
        _configuration = configuration;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<bool> IsEnabledAsync(string featureName, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CachePrefix}{featureName}";

        // Try to get from cache first
        var cachedValue = await _cacheService.GetAsync<bool?>(cacheKey, cancellationToken);
        if (cachedValue.HasValue)
        {
            _logger.LogDebug("Feature flag {FeatureName} retrieved from cache: {Value}", featureName, cachedValue.Value);
            return cachedValue.Value;
        }

        // Get from configuration
        var configKey = $"{FeatureFlagSection}:{featureName}";
        var isEnabled = _configuration.GetValue<bool>(configKey, false);

        // Cache the result for 5 minutes
        await _cacheService.SetAsync(cacheKey, isEnabled, TimeSpan.FromMinutes(5), cancellationToken);

        _logger.LogDebug("Feature flag {FeatureName} retrieved from configuration: {Value}", featureName, isEnabled);
        return isEnabled;
    }

    public async Task<bool> IsEnabledForUserAsync(string featureName, string userId, CancellationToken cancellationToken = default)
    {
        // Check global feature flag first
        var globalFlag = await IsEnabledAsync(featureName, cancellationToken);
        if (!globalFlag)
            return false;

        // Check user-specific feature flags
        var userCacheKey = $"{CachePrefix}{featureName}:user:{userId}";
        var cachedValue = await _cacheService.GetAsync<bool?>(userCacheKey, cancellationToken);
        if (cachedValue.HasValue)
        {
            return cachedValue.Value;
        }

        // Check configuration for user-specific flags
        var configKey = $"{FeatureFlagSection}:{featureName}:Users:{userId}";
        var userFlag = _configuration.GetValue<bool?>(configKey);

        if (userFlag.HasValue)
        {
            await _cacheService.SetAsync(userCacheKey, userFlag.Value, TimeSpan.FromMinutes(5), cancellationToken);
            _logger.LogDebug("User-specific feature flag {FeatureName} for user {UserId}: {Value}", featureName, userId, userFlag.Value);
            return userFlag.Value;
        }

        // Check percentage rollout
        var percentageKey = $"{FeatureFlagSection}:{featureName}:Percentage";
        var percentage = _configuration.GetValue<int?>(percentageKey);
        if (percentage.HasValue)
        {
            var userHash = Math.Abs(userId.GetHashCode()) % 100;
            var isEnabledForUser = userHash < percentage.Value;

            await _cacheService.SetAsync(userCacheKey, isEnabledForUser, TimeSpan.FromMinutes(5), cancellationToken);
            _logger.LogDebug("Percentage-based feature flag {FeatureName} for user {UserId}: {Value} (percentage: {Percentage})",
                featureName, userId, isEnabledForUser, percentage.Value);

            return isEnabledForUser;
        }

        // Default to global flag
        return globalFlag;
    }

    public async Task<T> GetFeatureValueAsync<T>(string featureName, T defaultValue, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CachePrefix}value:{featureName}";

        var cachedValue = await _cacheService.GetAsync<T>(cacheKey, cancellationToken);
        if (cachedValue != null)
        {
            return cachedValue;
        }

        var configKey = $"{FeatureFlagSection}:{featureName}:Value";
        var value = _configuration.GetValue(configKey, defaultValue);

        await _cacheService.SetAsync(cacheKey, value, TimeSpan.FromMinutes(5), cancellationToken);

        return value;
    }

    public async Task SetFeatureFlagAsync(string featureName, bool enabled, CancellationToken cancellationToken = default)
    {
        // This would typically integrate with external configuration systems
        // For now, we'll just update the cache
        var cacheKey = $"{CachePrefix}{featureName}";
        await _cacheService.SetAsync(cacheKey, enabled, TimeSpan.FromHours(1), cancellationToken);

        _logger.LogInformation("Feature flag {FeatureName} set to {Value} in cache", featureName, enabled);
    }
}