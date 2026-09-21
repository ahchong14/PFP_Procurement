using Microsoft.EntityFrameworkCore;
using PFP.Application.Abstractions.Persistence;
using PFP.Domain.Entities.SupplierQuoteCopys;
using PFP.Infrastructure.Persistence.Database;

namespace PFP.Infrastructure.Persistence.Repositories;

internal sealed class SupplierQuoteRepository(ApplicationDbContext dbContext)
    : ISupplierQuoteRepository
{
    // Get a supplier quote copy by token with its purchase request
    public Task<SupplierQuoteCopy?> GetByTokenAsync(
        string token,
        CancellationToken cancellationToken)
        => dbContext.SupplierQuoteCopies.Include(x => x.PurchaseRequest).FirstOrDefaultAsync(x => x.Token == token, cancellationToken);

    // Get a supplier quote copy by ID
    public Task<SupplierQuoteCopy?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
        => dbContext.SupplierQuoteCopies
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

    // Get supplier quote history by supplier
    public async Task<IReadOnlyList<SupplierQuoteCopy>> GetBySupplierIdAsync(
        int supplierId,
        CancellationToken cancellationToken)
        => await dbContext.SupplierQuoteCopies
            .AsNoTracking()
            .Where(x => x.SupplierId == supplierId)
            .OrderByDescending(x => x.SentAt)
            .ToListAsync(cancellationToken);

    // Add a new supplier quote copy to the current unit of work
    public void Add(SupplierQuoteCopy supplierQuoteCopy)
        => dbContext.SupplierQuoteCopies.Add(supplierQuoteCopy);
}