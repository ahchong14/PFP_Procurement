using PFP.Application.Abstractions.Messaging;
using PFP.Application.Abstractions.Persistence;
using PFP.Application.Abstractions.Services;
using PFP.Application.Common.Exceptions;

namespace PFP.Application.Features.Settings.EmailSettings.Commands.UpdateEmailSettings
{
    public sealed class UpdateEmailSettingsCommandHandler(
        IEmailSettingsRepository emailSettingsRepository,
        ISecretProtector secretProtector,
        IUnitOfWork unitOfWork
    ) : IRequestHandler<UpdateEmailSettingsCommand, EmailSettingsDto>
    {
        public async Task<EmailSettingsDto> Handle(UpdateEmailSettingsCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.EmailSettings settings = await emailSettingsRepository.GetAsync(cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.EmailSettings), 1);

            settings.Host = request.Host.Trim();
            settings.Port = request.Port;
            settings.Username = request.Username.Trim();
            settings.FromEmail = request.FromEmail.Trim();
            settings.FromName = request.FromName.Trim();

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                settings.EncryptedPassword = secretProtector.Protect(request.Password);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new EmailSettingsDto(
                settings.Host,
                settings.Port,
                settings.Username,
                settings.FromEmail,
                settings.FromName);
        }
    }
}
