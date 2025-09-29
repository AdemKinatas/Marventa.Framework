using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Marventa.Framework.Core.Interfaces.Data;

public interface IConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
    Task<IDbConnection> CreateConnectionAsync(string connectionString, CancellationToken cancellationToken = default);
    string GetConnectionString();
    string GetConnectionString(string name);
}