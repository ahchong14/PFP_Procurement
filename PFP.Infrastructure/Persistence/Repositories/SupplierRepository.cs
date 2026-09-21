using Microsoft.EntityFrameworkCore;
using PFP.Application.Abstractions.Persistence;
using PFP.Domain.Entities.Commons.Suppliers;
using PFP.Infrastructure.Persistence.Database;

namespace PFP.Infrastructure.Persistence.Repositories;

internal sealed class SupplierRepository(ApplicationDbContext dbContext) : ISupplierRepository
{
    // Get a supplier by ID
    public Task<Supplier?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
        => dbContext.Suppliers
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

    // Get a supplier by email for authentication
    public Task<Supplier?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken)
        => dbContext.Suppliers
            .FirstOrDefaultAsync(
                x => x.Email == email,
                cancellationToken);

    // Get a supplier by registration token
    public Task<Supplier?> GetByRegistrationTokenAsync(
        string token,
        CancellationToken cancellationToken)
        => dbContext.Suppliers
            .FirstOrDefaultAsync(
                x => x.RegistrationToken == token,
                cancellationToken);

    // Get a supplier by creditor code for AutoCount synchronization
    public Task<Supplier?> GetByCreditorCodeAsync(
        string creditorCode,
        CancellationToken cancellationToken)
        => dbContext.Suppliers
            .FirstOrDefaultAsync(
                x => x.CreditorCode == creditorCode,
                cancellationToken);

    // Get all suppliers for supplier management
    public async Task<IReadOnlyList<Supplier>> GetAllAsync(
        CancellationToken cancellationToken)
        => await dbContext.Suppliers
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

    // Add a new supplier to the current unit of work
    public void Add(Supplier supplier)
        => dbContext.Suppliers.Add(supplier);
}