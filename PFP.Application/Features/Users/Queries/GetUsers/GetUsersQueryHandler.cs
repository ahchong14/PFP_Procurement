using PFP.Application.Abstractions.Messaging;
using PFP.Application.Abstractions.Persistence;
using PFP.Domain.Entities.Commons.Users;

namespace PFP.Application.Features.Users.Queries.GetUsers
{
    public sealed class GetUsersQueryHandler(IUserRepository userRepository)
        : IRequestHandler<GetUsersQuery, List<UserDto>>
    {
        public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            List<User> users = await userRepository.GetAllAsync(cancellationToken);
            return users.Select(u => new UserDto(u.Id, u.Name, u.Email, u.Role, u.Department, u.IsActive)).ToList();
        }
    }
}
