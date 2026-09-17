using PFP.Application.Abstractions.Messaging;
using PFP.Application.Abstractions.Persistence;
using PFP.Application.Common.Exceptions;
using PFP.Domain.Entities.Commons.Users;

namespace PFP.Application.Features.Users.Commands.ActivateUser
{
    public sealed class ActivateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork) : IRequestHandler<ActivateUserCommand, UserDto>
    {
        public async Task<UserDto> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
        {
            User user = await userRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(User), request.Id);

            user.IsActive = true;
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new UserDto(user.Id, user.Name, user.Email, user.Role, user.Department, user.IsActive);
        }
    }
}


