namespace Marventa.Framework.Core.Models;

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