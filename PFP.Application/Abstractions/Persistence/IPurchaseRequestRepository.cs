using PFP.Domain.Entities.PurchaseRequests;

namespace PFP.Application.Abstractions.Persistence
{
    public interface IPurchaseRequestRepository
    {
        Task<PurchaseRequest?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<IReadOnlyList<PurchaseRequest>> GetAllAsync(CancellationToken cancellationToken);
        void Add(PurchaseRequest purchaseRequest);
    }
}
