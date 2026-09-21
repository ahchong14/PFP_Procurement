using Microsoft.EntityFrameworkCore;
using PFP.Application.Abstractions.Persistence;
using PFP.Domain.Entities.Commons.Users;
using PFP.Infrastructure.Persistence.Database;

namespace PFP.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository(ApplicationDbContext dbContext) : IUserRepository
{
    // Get a user by ID
    public Task<User?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
        => dbContext.Users
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

    // Get a user by email for authentication and email validation
    public Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken)
        => dbContext.Users
            .FirstOrDefaultAsync(
                x => x.Email == email,
                cancellationToken);

    // Get all users for user management
    public async Task<IReadOnlyList<User>> GetAllAsync(
        CancellationToken cancellationToken)
        => await dbContext.Users
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

    // Add a new user to the current unit of work
    public void Add(User user)
        => dbContext.Users.Add(user);
}
