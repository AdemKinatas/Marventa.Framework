using Marventa.Framework.Core.Models;

namespace Marventa.Framework.Core.Interfaces.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    Task SendEmailAsync(string to, string subject, string body, bool isHtml, CancellationToken cancellationToken = default);
    Task SendEmailAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default);
    Task SendBulkEmailAsync(IEnumerable<EmailMessage> emailMessages, CancellationToken cancellationToken = default);
}