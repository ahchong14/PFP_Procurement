using PFP.Application.Abstractions.Messaging;

namespace PFP.Application.Features.Users.Commands.DeactivateUser
{
    public sealed record DeactivateUserCommand(int Id) : IRequest<UserDto>;
}
