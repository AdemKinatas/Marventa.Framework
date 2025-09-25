using System.Threading;
using System.Threading.Tasks;

namespace Marventa.Framework.Core.Interfaces;

public interface IFeatureFlagService
{
    Task<bool> IsEnabledAsync(string featureName, CancellationToken cancellationToken = default);
    Task<bool> IsEnabledForUserAsync(string featureName, string userId, CancellationToken cancellationToken = default);
    Task<T> GetFeatureValueAsync<T>(string featureName, T defaultValue, CancellationToken cancellationToken = default);
    Task SetFeatureFlagAsync(string featureName, bool enabled, CancellationToken cancellationToken = default);
}