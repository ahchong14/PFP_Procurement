using PFP.Domain.Entities.Commons.Suppliers;

namespace PFP.Application.Abstractions.Persistence
{
    public interface ISupplierRepository
    {
        Task<Supplier?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<Supplier?> GetByEmailAsync(string email, CancellationToken cancellationToken);              // Login（供应商也走这张表）
        Task<Supplier?> GetByRegistrationTokenAsync(string token, CancellationToken cancellationToken);   // CompleteSupplierRegistration
        Task<Supplier?> GetByCreditorCodeAsync(string creditorCode, CancellationToken cancellationToken); // SyncSuppliersFromAutoCount的upsert匹配键
        Task<IReadOnlyList<Supplier>> GetAllAsync(CancellationToken cancellationToken);
        void Add(Supplier supplier);
    }
}
