using PFP.Host.PFP.Domain.Enums;

namespace PFP.Host.PFP.Application.Abstractions.Services
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        int? RoleId { get; }
        Role? Role { get; }
    }
}
