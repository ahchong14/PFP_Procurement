using PFP.Domain.Entities.RequestQuotations;

namespace PFP.Application.Abstractions.Persistence
{
    public interface IRequestQuotationRepository
    {
        Task<RequestQuotation?> GetByIdAsync(int id, CancellationToken cancellationToken);  // 实现时.Include(Items).Include(Approvals)
        Task<List<RequestQuotation>> GetAllAsync(CancellationToken cancellationToken);
        void Add(RequestQuotation requestQuotation);
    }
}
