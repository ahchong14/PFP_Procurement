using PFP.Domain.Entities.Items;

namespace PFP.Application.Abstractions.Persistence
{
    public interface IItemRepository
    {
        Task<Item?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<Item?> GetByCodeAsync(string code, CancellationToken cancellationToken);
        Task<List<Item>> GetAllAsync(CancellationToken cancellationToken);
        void Add(Item item);
    }
}
