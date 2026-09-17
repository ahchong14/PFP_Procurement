using PFP.Application.Abstractions.Messaging;
using PFP.Application.Common.Authorization;
using PFP.Domain.Enums;

namespace PFP.Application.Features.Users.Commands.UpdateUserRole;

[RequireRole(Role.Admin)]
public sealed record UpdateUserRoleCommand(
    int Id,
    Role Role
) : IRequest<UserDto>;