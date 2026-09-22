using Microsoft.EntityFrameworkCore;
using PFP.Application.Abstractions.Persistence;
using PFP.Domain.Entities;
using PFP.Infrastructure.Persistence.Database;

namespace PFP.Infrastructure.Persistence.Repositories;

internal sealed class EmailSettingsRepository(ApplicationDbContext dbContext) : IEmailSettingsRepository
{
    // Single-row configuration - tracked, since the only caller that needs it untracked
    // (GetEmailSettingsQueryHandler) reads exactly one row and the overhead is negligible.
    public Task<EmailSettings?> GetAsync(CancellationToken cancellationToken)
        => dbContext.EmailSettings.FirstOrDefaultAsync(cancellationToken);
}
