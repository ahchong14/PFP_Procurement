using PFP.Application.Abstractions.Messaging;
using PFP.Domain.Enums;

namespace PFP.Application.Features.Users.Commands.CreateUser
{
    public sealed record CreateUserCommand(string Name, string Email, Role Role, string Department, string Password) : IRequest<UserDto>;
}
