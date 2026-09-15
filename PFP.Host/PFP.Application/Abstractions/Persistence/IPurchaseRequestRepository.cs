using PFP.Host.PFP.Domain.Entities.PurchaseRequests;

namespace PFP.Host.PFP.Application.Abstractions.Persistence
{
    public interface IPurchaseRequestRepository
    {
        Task<PurchaseRequest?> GetByIdAsync(int id, CancellationToken cancellationToken);  // 实现时.Include(Items).Include(SupplierQuoteCopies)
        Task<List<PurchaseRequest>> GetAllAsync(CancellationToken cancellationToken);
        void Add(PurchaseRequest purchaseRequest);
    }
}
