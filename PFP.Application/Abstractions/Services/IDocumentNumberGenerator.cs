using PFP.Domain.Enums;

namespace PFP.Application.Abstractions.Services
{
    public interface IDocumentNumberGenerator
    {
        Task<string> GenerateAsync(DocumentType documentType, CancellationToken cancellationToken);
    }
}
