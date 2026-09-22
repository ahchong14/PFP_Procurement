using Microsoft.EntityFrameworkCore;
using PFP.Domain.Entities;
using PFP.Domain.Enums;

namespace PFP.Infrastructure.Persistence.Database.Seed;

internal static class ApplicationDbContextSeed
{
    public static async Task SeedAsync(
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        await SeedApprovalSettingsAsync(
            dbContext,
            cancellationToken);

        await SeedCountersAsync(
            dbContext,
            cancellationToken);
    }

    private static async Task SeedApprovalSettingsAsync(
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (await dbContext.ApprovalSettings.AnyAsync(
                cancellationToken))
        {
            return;
        }

        ApprovalSetting[] approvalSettings = new[]
        {
            new ApprovalSetting
            {
                Level = ApprovalLevel.L1
            },
            new ApprovalSetting
            {
                Level = ApprovalLevel.L2
            }
        };

        await dbContext.ApprovalSettings.AddRangeAsync(
            approvalSettings,
            cancellationToken);

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }

    private static async Task SeedCountersAsync(
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (await dbContext.Counters.AnyAsync(
                cancellationToken))
        {
            return;
        }

        Counter[] counters = new[]
        {
            new Counter
            {
                Name = "PR",
                Seq = 0
            },
            new Counter
            {
                Name = "RQ",
                Seq = 0
            },
            new Counter
            {
                Name = "PO",
                Seq = 0
            }
        };

        await dbContext.Counters.AddRangeAsync(
            counters,
            cancellationToken);

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }
}