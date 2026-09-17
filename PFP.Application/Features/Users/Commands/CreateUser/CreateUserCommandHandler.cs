using PFP.Application.Abstractions.Messaging;
using PFP.Application.Abstractions.Persistence;
using PFP.Application.Abstractions.Services;
using PFP.Application.Common.Exceptions;
using PFP.Domain.Entities.Commons.Users;

namespace PFP.Application.Features.Users.Commands.CreateUser;

public sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateUserCommand, UserDto>
{
    public async Task<UserDto> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        string name = request.Name.Trim();
        string email = request.Email.Trim().ToLowerInvariant();
        string deaprtment = request.Department.Trim();

        User? existingUser =
            await userRepository.GetByEmailAsync(
                request.Email,
                cancellationToken);

        if (existingUser is not null)
        {
            throw new BusinessRuleException(
                $"Email \"{request.Email}\" is already registered.");
        }

        User user = new()
        {
            Name = request.Name,
            Email = request.Email,
            Role = request.Role,
            Department = request.Department,
            IsActive = true,
            PasswordHash = passwordHasher.Hash(request.Password)
        };

        userRepository.Add(user);

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