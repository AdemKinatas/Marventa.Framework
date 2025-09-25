using System;
using System.Threading.Tasks;

namespace Marventa.Framework.Infrastructure.Locking;

public interface ILockHandle : IAsyncDisposable
{
    string Resource { get; }
    string LockId { get; }
    bool IsAcquired { get; }
    Task ReleaseAsync();
}