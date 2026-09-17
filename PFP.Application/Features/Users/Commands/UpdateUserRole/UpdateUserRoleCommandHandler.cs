using PFP.Application.Abstractions.Messaging;
using PFP.Application.Abstractions.Persistence;
using PFP.Application.Common.Exceptions;
using PFP.Domain.Entities.Commons.Users;

namespace PFP.Application.Features.Users.Commands.UpdateUserRole
{
    public sealed class UpdateUserRoleCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateUserRoleCommand, UserDto>
    {
        public async Task<UserDto> Handle(
            UpdateUserRoleCommand request,
            CancellationToken cancellationToken)
        {
            User user = await userRepository.GetByIdAsync(
            request.Id,
            cancellationToken)
            ?? throw new NotFoundException(nameof(User), request.Id);

            user.Role = request.Role;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new UserDto(
                user.Id,
                user.Name,
                user.Email,
                user.Role,
                user.Department,
                user.IsActive);
        }
    }
}