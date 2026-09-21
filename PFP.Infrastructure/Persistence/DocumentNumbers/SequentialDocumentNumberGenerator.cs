using Microsoft.EntityFrameworkCore;
using PFP.Application.Abstractions.Services;
using PFP.Domain.Enums;
using PFP.Infrastructure.Persistence.Database;

namespace PFP.Infrastructure.Persistence.DocumentNumbers;

internal sealed class SequentialDocumentNumberGenerator(ApplicationDbContext dbContext)
    : IDocumentNumberGenerator
{
    public async Task<string> GenerateAsync(
        DocumentType documentType,
        CancellationToken cancellationToken)
    {
        var prefix = GetPrefix(documentType);
        var seq = await NextSeqAsync(prefix, cancellationToken);

        return $"{prefix}-{DateTime.UtcNow:yyyy}-{seq:D5}";
    }

    // Uses raw SQL rather than the change tracker: dbContext is shared with the calling
    // Handler for the rest of the request, so this must not flush any of the Handler's own
    // pending inserts early, and the increment itself has to be atomic under concurrent callers.
    private async Task<int> NextSeqAsync(string prefix, CancellationToken cancellationToken)
    {
        var seq = await dbContext.Database
            .SqlQuery<int>(
                $"UPDATE counters SET Seq = Seq + 1 OUTPUT INSERTED.Seq WHERE Name = {prefix}")
            .SingleOrDefaultAsync(cancellationToken);

        if (seq > 0)
        {
            return seq;
        }

        // First document of this type in the system's lifetime - seed the counter row.
        // ApplicationDbContextSeed should ideally pre-create the PR/RQ/PO rows at startup
        // so this branch is never hit in practice; kept here as a safety net.
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO counters (Name, Seq) VALUES ({prefix}, 1)",
            cancellationToken);

        return 1;
    }

    private static string GetPrefix(DocumentType documentType) => documentType switch
    {
        DocumentType.PurchaseRequest => "PR",
        DocumentType.RequestQuotation => "RQ",
        DocumentType.PurchaseOrder => "PO",
        _ => throw new ArgumentOutOfRangeException(nameof(documentType), documentType, null),
    };
}
