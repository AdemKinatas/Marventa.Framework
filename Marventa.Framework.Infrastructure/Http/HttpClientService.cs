using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Marventa.Framework.Core.Interfaces.Http;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Infrastructure.Http;

public class HttpClientService : IHttpClientService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpClientService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public HttpClientService(HttpClient httpClient, ILogger<HttpClientService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("HTTP GET request to: {Endpoint}", endpoint);
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during HTTP GET request to: {Endpoint}", endpoint);
            throw;
        }
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("HTTP POST request to: {Endpoint}", endpoint);
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during HTTP POST request to: {Endpoint}", endpoint);
            throw;
        }
    }

    public async Task<T?> PutAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("HTTP PUT request to: {Endpoint}", endpoint);
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(endpoint, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during HTTP PUT request to: {Endpoint}", endpoint);
            throw;
        }
    }

    public async Task<T?> DeleteAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("HTTP DELETE request to: {Endpoint}", endpoint);
            var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during HTTP DELETE request to: {Endpoint}", endpoint);
            throw;
        }
    }

    public async Task<string> GetStringAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("HTTP GET string request to: {Endpoint}", endpoint);
            return await _httpClient.GetStringAsync(endpoint, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during HTTP GET string request to: {Endpoint}", endpoint);
            throw;
        }
    }

    public async Task<string> PostStringAsync(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("HTTP POST string request to: {Endpoint}", endpoint);
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during HTTP POST string request to: {Endpoint}", endpoint);
            throw;
        }
    }

    public void AddDefaultHeader(string name, string value)
    {
        _httpClient.DefaultRequestHeaders.Add(name, value);
        _logger.LogDebug("Added default header: {Name} = {Value}", name, value);
    }

    public void SetBaseAddress(string baseAddress)
    {
        _httpClient.BaseAddress = new Uri(baseAddress);
        _logger.LogDebug("Set base address: {BaseAddress}", baseAddress);
    }

    public void SetTimeout(int timeoutSeconds)
    {
        _httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        _logger.LogDebug("Set timeout: {Timeout} seconds", timeoutSeconds);
    }
}