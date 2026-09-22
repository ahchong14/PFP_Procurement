using PFP.Domain.Entities;

namespace PFP.Application.Abstractions.Persistence
{
    public interface IEmailSettingsRepository
    {
        Task<EmailSettings?> GetAsync(CancellationToken cancellationToken);
    }
}
