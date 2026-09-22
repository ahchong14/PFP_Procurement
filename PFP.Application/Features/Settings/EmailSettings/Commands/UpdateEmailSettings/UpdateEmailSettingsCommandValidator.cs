using FluentValidation;

namespace PFP.Application.Features.Settings.EmailSettings.Commands.UpdateEmailSettings
{
    public sealed class UpdateEmailSettingsCommandValidator : AbstractValidator<UpdateEmailSettingsCommand>
    {
        public UpdateEmailSettingsCommandValidator()
        {
            RuleFor(x => x.Host).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Port).InclusiveBetween(1, 65535);
            RuleFor(x => x.Username).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Password).MinimumLength(1).When(x => x.Password is not null);
            RuleFor(x => x.FromEmail).NotEmpty().EmailAddress().MaximumLength(255);
            RuleFor(x => x.FromName).NotEmpty().MaximumLength(150);
        }
    }
}
