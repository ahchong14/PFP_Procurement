using PFP.Application.Abstractions.Messaging;
using PFP.Application.Common.Authorization;
using PFP.Domain.Enums;

namespace PFP.Application.Features.Users.Queries.GetUsers
{
    [RequireRole(Role.HeadOfPurchase)]
    public sealed record GetUsersQuery : IRequest<List<UserDto>>;
}
