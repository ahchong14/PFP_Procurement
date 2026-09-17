using PFP.Application.Abstractions.Messaging;

namespace PFP.Application.Features.Users.Commands.ActivateUser
{
    public sealed record ActivateUserCommand(int Id) : IRequest<UserDto>;
}
