using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Marventa.Framework.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly bool _enableSsl;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        _smtpHost = _configuration["Email:SmtpHost"] ?? throw new ArgumentNullException("Email:SmtpHost is required");
        _smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
        _smtpUsername = _configuration["Email:Username"] ?? throw new ArgumentNullException("Email:Username is required");
        _smtpPassword = _configuration["Email:Password"] ?? throw new ArgumentNullException("Email:Password is required");
        _fromEmail = _configuration["Email:FromEmail"] ?? _smtpUsername;
        _fromName = _configuration["Email:FromName"] ?? "Marventa Framework";
        _enableSsl = bool.Parse(_configuration["Email:EnableSsl"] ?? "true");
    }

    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        return SendEmailAsync(to, subject, body, false, cancellationToken);
    }

    public Task SendEmailAsync(string to, string subject, string body, bool isHtml, CancellationToken cancellationToken = default)
    {
        var emailMessage = new EmailMessage
        {
            To = to,
            Subject = subject,
            Body = body,
            IsHtml = isHtml
        };

        return SendEmailAsync(emailMessage, cancellationToken);
    }

    public async Task SendEmailAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = CreateSmtpClient();
            using var message = CreateMailMessage(emailMessage);

            _logger.LogDebug("Sending email to {To} with subject: {Subject}", emailMessage.To, emailMessage.Subject);
            await client.SendMailAsync(message, cancellationToken);
            _logger.LogInformation("Email sent successfully to {To}", emailMessage.To);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", emailMessage.To);
            throw;
        }
    }

    public async Task SendBulkEmailAsync(IEnumerable<EmailMessage> emailMessages, CancellationToken cancellationToken = default)
    {
        var messages = emailMessages.ToList();
        _logger.LogInformation("Sending {Count} bulk emails", messages.Count);

        var tasks = messages.Select(async email =>
        {
            try
            {
                await SendEmailAsync(email, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send bulk email to {To}", email.To);
            }
        });

        await Task.WhenAll(tasks);
        _logger.LogInformation("Bulk email sending completed");
    }

    private SmtpClient CreateSmtpClient()
    {
        return new SmtpClient(_smtpHost)
        {
            Port = _smtpPort,
            Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
            EnableSsl = _enableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network
        };
    }

    private MailMessage CreateMailMessage(EmailMessage emailMessage)
    {
        var message = new MailMessage
        {
            From = new MailAddress(emailMessage.From ?? _fromEmail, emailMessage.FromName ?? _fromName),
            Subject = emailMessage.Subject,
            Body = emailMessage.Body,
            IsBodyHtml = emailMessage.IsHtml,
            Priority = (MailPriority)emailMessage.Priority
        };

        // Add recipients
        message.To.Add(emailMessage.To);

        if (!string.IsNullOrEmpty(emailMessage.Cc))
        {
            message.CC.Add(emailMessage.Cc);
        }

        if (!string.IsNullOrEmpty(emailMessage.Bcc))
        {
            message.Bcc.Add(emailMessage.Bcc);
        }

        // Add attachments
        foreach (var attachment in emailMessage.Attachments)
        {
            var stream = new MemoryStream(attachment.Content);
            var mailAttachment = new Attachment(stream, attachment.FileName, attachment.ContentType);
            message.Attachments.Add(mailAttachment);
        }

        // Add custom headers
        foreach (var header in emailMessage.Headers)
        {
            message.Headers.Add(header.Key, header.Value);
        }

        return message;
    }
}