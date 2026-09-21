using Microsoft.EntityFrameworkCore;
using PFP.Application.Abstractions.Persistence;
using PFP.Domain.Entities.PurchaseOrders;
using PFP.Infrastructure.Persistence.Database;

namespace PFP.Infrastructure.Persistence.Repositories;

internal sealed class PurchaseOrderRepository(ApplicationDbContext dbContext)
    : IPurchaseOrderRepository
{
    // Get a purchase order with its detail items
    public Task<PurchaseOrder?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
        => dbContext.PurchaseOrders
            .Include(x => x.Items)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

    // Get all purchase orders for purchase order management
    public async Task<IReadOnlyList<PurchaseOrder>> GetAllAsync(
        CancellationToken cancellationToken)
        => await dbContext.PurchaseOrders
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

    // Add a new purchase order to the current unit of work
    public void Add(PurchaseOrder purchaseOrder)
        => dbContext.PurchaseOrders.Add(purchaseOrder);
}