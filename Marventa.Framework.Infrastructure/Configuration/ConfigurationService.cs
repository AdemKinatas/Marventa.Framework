using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Marventa.Framework.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Marventa.Framework.Infrastructure.Configuration;

public class ConfigurationService : IConfigurationService
{
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;

    public ConfigurationService(IConfiguration configuration, IHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }

    public T GetValue<T>(string key)
    {
        return _configuration.GetValue<T>(key) ?? throw new ArgumentException($"Configuration key '{key}' not found");
    }

    public T GetValue<T>(string key, T defaultValue)
    {
        return _configuration.GetValue(key, defaultValue);
    }

    public Task<T> GetValueAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var value = GetValue<T>(key);
        return Task.FromResult(value);
    }

    public Task<T> GetValueAsync<T>(string key, T defaultValue, CancellationToken cancellationToken = default)
    {
        var value = GetValue(key, defaultValue);
        return Task.FromResult(value);
    }

    public Task SetValueAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        // Note: IConfiguration is read-only by design
        // For dynamic configuration, you would need to implement a custom configuration provider
        // or use external configuration services like Azure App Configuration, AWS Parameter Store, etc.
        throw new NotSupportedException("Setting configuration values is not supported with standard IConfiguration. Use external configuration providers for dynamic configuration.");
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var exists = _configuration.GetSection(key).Exists();
        return Task.FromResult(exists);
    }

    public Task<Dictionary<string, string>> GetSectionAsync(string sectionKey, CancellationToken cancellationToken = default)
    {
        var section = _configuration.GetSection(sectionKey);
        var result = section.AsEnumerable()
            .Where(kvp => kvp.Value != null)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value!);

        return Task.FromResult(result);
    }

    public string GetConnectionString(string name)
    {
        return _configuration.GetConnectionString(name)
            ?? throw new ArgumentException($"Connection string '{name}' not found");
    }

    public bool IsEnvironment(string environment)
    {
        return _environment.IsEnvironment(environment);
    }

    public string GetEnvironmentName()
    {
        return _environment.EnvironmentName;
    }
}