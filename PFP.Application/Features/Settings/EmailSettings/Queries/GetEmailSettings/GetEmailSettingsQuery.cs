using PFP.Application.Abstractions.Messaging;
using PFP.Application.Common.Authorization;
using PFP.Domain.Enums;

namespace PFP.Application.Features.Settings.EmailSettings.Queries.GetEmailSettings
{
    [RequireRole(Role.HeadOfPurchase)]
    public sealed record GetEmailSettingsQuery : IRequest<EmailSettingsDto>;
}
