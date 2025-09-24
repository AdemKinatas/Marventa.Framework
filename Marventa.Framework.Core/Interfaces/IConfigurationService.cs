using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Marventa.Framework.Core.Interfaces;

public interface IConfigurationService
{
    T GetValue<T>(string key);
    T GetValue<T>(string key, T defaultValue);
    Task<T> GetValueAsync<T>(string key, CancellationToken cancellationToken = default);
    Task<T> GetValueAsync<T>(string key, T defaultValue, CancellationToken cancellationToken = default);
    Task SetValueAsync<T>(string key, T value, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
    Task<Dictionary<string, string>> GetSectionAsync(string sectionKey, CancellationToken cancellationToken = default);
    string GetConnectionString(string name);
    bool IsEnvironment(string environment);
    string GetEnvironmentName();
}

public interface IFeatureFlagService
{
    Task<bool> IsEnabledAsync(string featureName, CancellationToken cancellationToken = default);
    Task<bool> IsEnabledForUserAsync(string featureName, string userId, CancellationToken cancellationToken = default);
    Task<T> GetFeatureValueAsync<T>(string featureName, T defaultValue, CancellationToken cancellationToken = default);
    Task SetFeatureFlagAsync(string featureName, bool enabled, CancellationToken cancellationToken = default);
}