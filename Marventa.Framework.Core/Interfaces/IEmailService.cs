using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Marventa.Framework.Core.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    Task SendEmailAsync(string to, string subject, string body, bool isHtml, CancellationToken cancellationToken = default);
    Task SendEmailAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default);
    Task SendBulkEmailAsync(IEnumerable<EmailMessage> emailMessages, CancellationToken cancellationToken = default);
}

public interface ISmsService
{
    Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
    Task SendSmsAsync(SmsMessage smsMessage, CancellationToken cancellationToken = default);
    Task SendBulkSmsAsync(IEnumerable<SmsMessage> smsMessages, CancellationToken cancellationToken = default);
}

public class EmailMessage
{
    public string To { get; set; } = string.Empty;
    public string? Cc { get; set; }
    public string? Bcc { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; } = false;
    public string? From { get; set; }
    public string? FromName { get; set; }
    public List<EmailAttachment> Attachments { get; set; } = new();
    public Dictionary<string, string> Headers { get; set; } = new();
    public int Priority { get; set; } = 3; // 1=High, 3=Normal, 5=Low
}

public class EmailAttachment
{
    public string FileName { get; set; } = string.Empty;
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = "application/octet-stream";
}

public class SmsMessage
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? From { get; set; }
    public Dictionary<string, string> Parameters { get; set; } = new();
}