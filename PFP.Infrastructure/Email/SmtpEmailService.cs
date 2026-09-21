using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using PFP.Application.Abstractions.Services;
using PFP.Infrastructure.Options;

namespace PFP.Infrastructure.Email;

internal sealed class SmtpEmailService(IOptions<EmailOptions> options) : IEmailService
{
    private readonly EmailOptions _options = options.Value;

    public async Task SendAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(to);
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);

        using var message = new MailMessage
        {
            From = new MailAddress(_options.FromEmail, _options.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };

        message.To.Add(to);

        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            Credentials = new NetworkCredential(_options.Username, _options.Password),
            EnableSsl = true,
        };

        await client.SendMailAsync(message, cancellationToken);
    }
}
