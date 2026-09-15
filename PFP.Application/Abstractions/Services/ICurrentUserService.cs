using PFP.Domain.Enums;

namespace PFP.Application.Abstractions.Services
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        int? SupplierId { get; }
        Role? Role { get; }
        bool IsAuthenticated { get; }
    }
}
