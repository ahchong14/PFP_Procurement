using Microsoft.EntityFrameworkCore;
using PFP.Application.Abstractions.Persistence;
using PFP.Domain.Entities;
using PFP.Domain.Enums;
using PFP.Infrastructure.Persistence.Database;

namespace PFP.Infrastructure.Persistence.Repositories;

internal sealed class ApprovalSettingRepository(ApplicationDbContext dbContext)
    : IApprovalSettingRepository
{
    // Get approval settings by approval level
    public Task<ApprovalSetting?> GetByLevelAsync(
        ApprovalLevel level,
        CancellationToken cancellationToken)
        => dbContext.ApprovalSettings
            .FirstOrDefaultAsync(
                x => x.Level == level,
                cancellationToken);

    // Get all approval settings
    public async Task<IReadOnlyList<ApprovalSetting>> GetAllAsync(
        CancellationToken cancellationToken)
        => await dbContext.ApprovalSettings
            .AsNoTracking()
            .OrderBy(x => x.Level)
            .ToListAsync(cancellationToken);
}