using PFP.Domain.Entities;
using PFP.Domain.Enums;

namespace PFP.Application.Abstractions.Persistence
{
    public interface IApprovalSettingRepository
    {
        Task<ApprovalSetting?> GetByLevelAsync(ApprovalLevel level, CancellationToken cancellationToken);
        Task<List<ApprovalSetting>> GetAllAsync(CancellationToken cancellationToken);
    }
}
