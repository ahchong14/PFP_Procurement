using PFP.Host.PFP.Domain.Entities.Commons.Users;

namespace PFP.Host.PFP.Application.Abstractions.Persistence
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);   // Login、CreateUser查重
        Task<List<User>> GetAllAsync(CancellationToken cancellationToken);
        void Add(User user);
    }
}
