using Microsoft.EntityFrameworkCore;
using PFP.Application.Abstractions.Persistence;
using PFP.Domain.Entities.PurchaseRequests;
using PFP.Infrastructure.Persistence.Database;

namespace PFP.Infrastructure.Persistence.Repositories;

internal sealed class PurchaseRequestRepository(ApplicationDbContext dbContext)
    : IPurchaseRequestRepository
{
    // Get a purchase request with items and supplier quote copies
    public Task<PurchaseRequest?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
        => dbContext.PurchaseRequests
            .Include(x => x.Items)
            .Include(x => x.SupplierQuoteCopies)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

    // Get all purchase requests for purchase request management
    public async Task<IReadOnlyList<PurchaseRequest>> GetAllAsync(
        CancellationToken cancellationToken)
        => await dbContext.PurchaseRequests
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

    // Add a new purchase request to the current unit of work
    public void Add(PurchaseRequest purchaseRequest)
        => dbContext.PurchaseRequests.Add(purchaseRequest);
}