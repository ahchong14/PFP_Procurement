using PFP.Domain.Entities.PurchaseOrders;

namespace PFP.Application.Abstractions.Persistence
{
    public interface IPurchaseOrderRepository
    {
        Task<PurchaseOrder?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<IReadOnlyList<PurchaseOrder>> GetAllAsync(CancellationToken cancellationToken);
        void Add(PurchaseOrder purchaseOrder);
    }
}
