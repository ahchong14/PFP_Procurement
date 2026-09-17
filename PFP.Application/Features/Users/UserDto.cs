using PFP.Domain.Enums;

namespace PFP.Application.Features.Users
{
    public sealed record UserDto(int Id, string Name, string Email, Role Role, string Department, bool Active);
}
