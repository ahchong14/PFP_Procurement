using Microsoft.EntityFrameworkCore;
using PFP.Application.Abstractions.Services;
using PFP.Domain.Entities;
using PFP.Domain.Enums;

namespace PFP.Infrastructure.Persistence.Database.Seed;

internal static class ApplicationDbContextSeed
{
    public static async Task SeedAsync(
        ApplicationDbContext dbContext,
        ISecretProtector secretProtector,
        CancellationToken cancellationToken = default)
    {
        await SeedApprovalSettingsAsync(
            dbContext,
            cancellationToken);

        await SeedCountersAsync(
            dbContext,
            cancellationToken);

        await SeedEmailSettingsAsync(
            dbContext,
            secretProtector,
            cancellationToken);
    }

    // Placeholder thresholds - the Scope Document's Assumptions section notes the client
    // must provide the real per-level amounts; these just make the approval flow
    // functional out of the box (and give both levels a real Director approver role,
    // rather than both silently defaulting to Role.Requester) until that input arrives.
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
                Level = ApprovalLevel.L1,
                ApproverRole = Role.DirectorL1,
                MinAmount = 0m,
                MaxAmount = 10000m
            },
            new ApprovalSetting
            {
                Level = ApprovalLevel.L2,
                ApproverRole = Role.DirectorL2,
                MinAmount = 10000m,
                MaxAmount = null
            }
        };

        await dbContext.ApprovalSettings.AddRangeAsync(
            approvalSettings,
            cancellationToken);

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }

    // Empty placeholder - the client fills in real SMTP details through the settings page.
    // EncryptedPassword still has to go through ISecretProtector even for an empty value,
    // since SmtpEmailService always calls Unprotect() on whatever is stored.
    private static async Task SeedEmailSettingsAsync(
        ApplicationDbContext dbContext,
        ISecretProtector secretProtector,
        CancellationToken cancellationToken)
    {
        if (await dbContext.EmailSettings.AnyAsync(
                cancellationToken))
        {
            return;
        }

        await dbContext.EmailSettings.AddAsync(
            new EmailSettings
            {
                Id = 1,
                Host = string.Empty,
                Port = 587,
                Username = string.Empty,
                EncryptedPassword = secretProtector.Protect(string.Empty),
                FromEmail = "no-reply@example.com",
                FromName = "PFP Procurement System"
            },
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