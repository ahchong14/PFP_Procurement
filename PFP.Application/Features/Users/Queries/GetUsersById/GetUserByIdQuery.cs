using PFP.Application.Abstractions.Messaging;
using PFP.Application.Common.Authorization;

namespace PFP.Application.Features.Users.Queries.GetUsersById
{
    [RequireRole(Domain.Enums.Role.HeadOfPurchase)]
    public sealed record GetUserByIdQuery(int Id) : IRequest<UserDto>;
}
