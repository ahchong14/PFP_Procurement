using Microsoft.EntityFrameworkCore;
using PFP.Application.Abstractions.Persistence;
using PFP.Domain.Entities.RequestQuotations;
using PFP.Infrastructure.Persistence.Database;

namespace PFP.Infrastructure.Persistence.Repositories;

internal sealed class RequestQuotationRepository(ApplicationDbContext dbContext)
    : IRequestQuotationRepository
{
    // Get a request quotation with items and approval records
    public Task<RequestQuotation?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
        => dbContext.RequestQuotations
            .Include(x => x.Items)
            .Include(x => x.Approvals)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

    // Get all request quotations for management
    public async Task<IReadOnlyList<RequestQuotation>> GetAllAsync(
        CancellationToken cancellationToken)
        => await dbContext.RequestQuotations
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

    // Add a new request quotation to the current unit of work
    public void Add(RequestQuotation requestQuotation)
        => dbContext.RequestQuotations.Add(requestQuotation);
}