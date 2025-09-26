namespace Marventa.Framework.Core.Models;

public class SmsMessage
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? From { get; set; }
    public Dictionary<string, string> Parameters { get; set; } = new();
}