namespace PFP.Application.Abstractions.Services
{
    public interface IDocumentNumberGenerator
    {
        Task<string> NextAsync(string prefix, CancellationToken cancellationToken);
    }
}
