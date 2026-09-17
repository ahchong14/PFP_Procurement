using PFP.Application.Abstractions.Messaging;
using PFP.Application.Abstractions.Persistence;
using PFP.Application.Common.Exceptions;
using PFP.Domain.Entities.Commons.Users;

namespace PFP.Application.Features.Users.Queries.GetUsersById
{
    public sealed class GetUserByIdQueryHandler(IUserRepository userRepository)
        : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            User user = await userRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(User), request.Id);

            return new UserDto(user.Id, user.Name, user.Email, user.Role, user.Department, user.IsActive);
        }
    }
}

