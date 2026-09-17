using FluentValidation;

namespace PFP.Application.Features.Users.Commands.UpdateUserRole
{
    public sealed class UpdateUserRoleCommandValidator : AbstractValidator<UpdateUserRoleCommand>
    {
        public UpdateUserRoleCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Role).IsInEnum();
        }
    }
}
