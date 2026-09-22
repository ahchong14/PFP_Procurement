using System.Net;
using System.Net.Mail;
using PFP.Application.Abstractions.Persistence;
using PFP.Application.Abstractions.Services;
using PFP.Domain.Entities;

namespace PFP.Infrastructure.Email;

internal sealed class SmtpEmailService(
    IEmailSettingsRepository emailSettingsRepository,
    ISecretProtector secretProtector) : IEmailService
{
    public async Task SendAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(to);
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);

        // Read fresh on every send, not cached at startup - so a change made through the
        // settings page takes effect immediately, without an app restart.
        EmailSettings settings = await emailSettingsRepository.GetAsync(cancellationToken)
            ?? throw new InvalidOperationException(
                "Email settings have not been configured yet. Configure them through the settings page first.");

        using var message = new MailMessage
        {
            From = new MailAddress(settings.FromEmail, settings.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };

        message.To.Add(to);

        using var client = new SmtpClient(settings.Host, settings.Port)
        {
            Credentials = new NetworkCredential(
                settings.Username,
                secretProtector.Unprotect(settings.EncryptedPassword)),
            EnableSsl = true,
        };

        await client.SendMailAsync(message, cancellationToken);
    }
}
