using PFP.Application.Abstractions.Messaging;
using PFP.Application.Abstractions.Persistence;
using PFP.Application.Common.Exceptions;
using PFP.Domain.Entities.Commons.Users;

namespace PFP.Application.Features.Users.Commands.DeactivateUser
{
    public sealed class DeactivateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork) : IRequestHandler<DeactivateUserCommand, UserDto>
    {
        public async Task<UserDto> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
        {
            User user = await userRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(User), request.Id);

            user.IsActive = false;
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new UserDto(user.Id, user.Name, user.Email, user.Role, user.Department, user.IsActive);
        }
    }
}
