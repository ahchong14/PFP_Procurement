using PFP.Domain.Entities.SupplierQuoteCopys;

namespace PFP.Application.Abstractions.Persistence
{
    public interface ISupplierQuoteRepository
    {
        Task<SupplierQuoteCopy?> GetByTokenAsync(string token, CancellationToken cancellationToken);  // 免登录供应商访问入口
        Task<SupplierQuoteCopy?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<SupplierQuoteCopy>> GetBySupplierIdAsync(int supplierId, CancellationToken cancellationToken); // /suppliers/me/quotes
        void Add(SupplierQuoteCopy supplierQuoteCopy);
    }
}
