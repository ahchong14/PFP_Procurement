using PFP.Application.Abstractions.Persistence;
using PFP.Infrastructure.Persistence.Database;

namespace PFP.Infrastructure.Persistence
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken)
        {
            return _db.SaveChangesAsync(cancellationToken);
        }
    }
}
