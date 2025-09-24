using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Marventa.Framework.Core.Interfaces;

public interface IHttpClientService
{
    Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);
    Task<T?> PostAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default);
    Task<T?> PutAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default);
    Task<T?> DeleteAsync<T>(string endpoint, CancellationToken cancellationToken = default);
    Task<string> GetStringAsync(string endpoint, CancellationToken cancellationToken = default);
    Task<string> PostStringAsync(string endpoint, object data, CancellationToken cancellationToken = default);
    void AddDefaultHeader(string name, string value);
    void SetBaseAddress(string baseAddress);
    void SetTimeout(int timeoutSeconds);
}