using PFP.Application.Abstractions.Messaging;
using PFP.Application.Common.Authorization;
using PFP.Domain.Enums;

namespace PFP.Application.Features.Settings.EmailSettings.Commands.UpdateEmailSettings
{
    // Password is optional: the settings page never shows the current one back to the
    // caller (see EmailSettingsDto), so omitting it here means "keep the existing password".
    [RequireRole(Role.HeadOfPurchase)]
    public sealed record UpdateEmailSettingsCommand(
        string Host,
        int Port,
        string Username,
        string? Password,
        string FromEmail,
        string FromName
    ) : IRequest<EmailSettingsDto>;
}
