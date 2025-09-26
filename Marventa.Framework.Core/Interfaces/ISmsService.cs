using Marventa.Framework.Core.Models;

namespace Marventa.Framework.Core.Interfaces;

public interface ISmsService
{
    Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
    Task SendSmsAsync(SmsMessage smsMessage, CancellationToken cancellationToken = default);
    Task SendBulkSmsAsync(IEnumerable<SmsMessage> smsMessages, CancellationToken cancellationToken = default);
}