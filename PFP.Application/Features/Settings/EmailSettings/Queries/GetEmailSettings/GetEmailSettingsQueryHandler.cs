using PFP.Application.Abstractions.Messaging;
using PFP.Application.Abstractions.Persistence;
using PFP.Application.Common.Exceptions;

namespace PFP.Application.Features.Settings.EmailSettings.Queries.GetEmailSettings
{
    public sealed class GetEmailSettingsQueryHandler(IEmailSettingsRepository emailSettingsRepository)
        : IRequestHandler<GetEmailSettingsQuery, EmailSettingsDto>
    {
        public async Task<EmailSettingsDto> Handle(GetEmailSettingsQuery request, CancellationToken cancellationToken)
        {
            Domain.Entities.EmailSettings settings = await emailSettingsRepository.GetAsync(cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.EmailSettings), 1);

            return new EmailSettingsDto(
                settings.Host,
                settings.Port,
                settings.Username,
                settings.FromEmail,
                settings.FromName);
        }
    }
}
