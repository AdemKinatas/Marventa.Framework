using System.Net.Http;
using System.Text;
using System.Text.Json;
using Marventa.Framework.Core.Interfaces;
using Marventa.Framework.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Infrastructure.Services;

public class SmsService : ISmsService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmsService> _logger;
    private readonly string _apiUrl;
    private readonly string _apiKey;
    private readonly string _fromNumber;

    public SmsService(HttpClient httpClient, IConfiguration configuration, ILogger<SmsService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;

        _apiUrl = _configuration["Sms:ApiUrl"] ?? throw new ArgumentNullException("Sms:ApiUrl is required");
        _apiKey = _configuration["Sms:ApiKey"] ?? throw new ArgumentNullException("Sms:ApiKey is required");
        _fromNumber = _configuration["Sms:FromNumber"] ?? throw new ArgumentNullException("Sms:FromNumber is required");

        _httpClient.BaseAddress = new Uri(_apiUrl);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    public Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        var smsMessage = new SmsMessage
        {
            PhoneNumber = phoneNumber,
            Message = message
        };

        return SendSmsAsync(smsMessage, cancellationToken);
    }

    public async Task SendSmsAsync(SmsMessage smsMessage, CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = new
            {
                to = smsMessage.PhoneNumber,
                from = smsMessage.From ?? _fromNumber,
                message = smsMessage.Message,
                parameters = smsMessage.Parameters
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogDebug("Sending SMS to {PhoneNumber}", smsMessage.PhoneNumber);

            var response = await _httpClient.PostAsync("/sms/send", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            _logger.LogInformation("SMS sent successfully to {PhoneNumber}", smsMessage.PhoneNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send SMS to {PhoneNumber}", smsMessage.PhoneNumber);
            throw;
        }
    }

    public async Task SendBulkSmsAsync(IEnumerable<SmsMessage> smsMessages, CancellationToken cancellationToken = default)
    {
        var messages = smsMessages.ToList();
        _logger.LogInformation("Sending {Count} bulk SMS messages", messages.Count);

        var tasks = messages.Select(async sms =>
        {
            try
            {
                await SendSmsAsync(sms, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send bulk SMS to {PhoneNumber}", sms.PhoneNumber);
            }
        });

        await Task.WhenAll(tasks);
        _logger.LogInformation("Bulk SMS sending completed");
    }
}