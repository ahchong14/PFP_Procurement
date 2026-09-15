using PFP.Host.PFP.Domain.Entities;
using PFP.Host.PFP.Domain.Enums;

namespace PFP.Host.PFP.Application.Abstractions.Persistence
{
    public interface IApprovalSettingRepository
    {
        Task<ApprovalSetting?> GetByLevelAsync(ApprovalLevel level, CancellationToken cancellationToken);
        Task<List<ApprovalSetting>> GetAllAsync(CancellationToken cancellationToken);
    }
}
