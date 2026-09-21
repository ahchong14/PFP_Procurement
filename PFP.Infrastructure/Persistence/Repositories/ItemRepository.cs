using Microsoft.EntityFrameworkCore;
using PFP.Application.Abstractions.Persistence;
using PFP.Domain.Entities.Items;
using PFP.Infrastructure.Persistence.Database;

namespace PFP.Infrastructure.Persistence.Repositories;

internal sealed class ItemRepository(ApplicationDbContext dbContext) : IItemRepository
{
    // Get an item by ID
    public Task<Item?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
        => dbContext.Items
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

    // Get an item by code for AutoCount synchronization
    public Task<Item?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken)
        => dbContext.Items
            .FirstOrDefaultAsync(
                x => x.Code == code,
                cancellationToken);

    // Get all items for item selection
    public async Task<IReadOnlyList<Item>> GetAllAsync(
        CancellationToken cancellationToken)
        => await dbContext.Items
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

    // Add a new item to the current unit of work
    public void Add(Item item)
        => dbContext.Items.Add(item);
}