using System;
using System.Threading;
using System.Threading.Tasks;

namespace Marventa.Framework.Core.Interfaces;

public interface IDistributedLock
{
    Task<ILockHandle?> AcquireAsync(string resource, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
    Task<T> ExecuteWithLockAsync<T>(string resource, Func<Task<T>> action, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
    Task ExecuteWithLockAsync(string resource, Func<Task> action, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
}