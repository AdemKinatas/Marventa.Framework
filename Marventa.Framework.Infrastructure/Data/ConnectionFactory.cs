using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Marventa.Framework.Core.Interfaces.Data;
using Microsoft.Extensions.Configuration;

namespace Marventa.Framework.Infrastructure.Data;

public class ConnectionFactory : IConnectionFactory
{
    private readonly IConfiguration _configuration;
    private readonly string _defaultConnectionString;

    public ConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
        _defaultConnectionString = _configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException("DefaultConnection is required");
    }

    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqlConnection(_defaultConnectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }

    public async Task<IDbConnection> CreateConnectionAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }

    public string GetConnectionString()
    {
        return _defaultConnectionString;
    }

    public string GetConnectionString(string name)
    {
        return _configuration.GetConnectionString(name)
            ?? throw new ArgumentException($"Connection string '{name}' not found");
    }
}