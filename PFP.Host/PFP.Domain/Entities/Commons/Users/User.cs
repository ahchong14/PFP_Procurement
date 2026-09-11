using PFP.Host.PFP.Domain.Entities.Commons.PurchaseRequests;
using PFP.Host.PFP.Domain.Enums;
// TODO: add the using for the namespace that contains PurchaseRequest, for example:
// using PFP.Host.PFP.Domain.Entities.Commons.PurchaseRequests;

namespace PFP.Host.PFP.Domain.Entities.Commons.Users
{
    public class User
    {
        public required string Id { get; set; }

        public required string Name { get; set; }

        public required string Email { get; set; }

        public Role Role { get; set; }

        public Department? Department { get; set; } = null;

        public bool IsActive { get; set; } = true;

        public required string PasswordHash { get; set; }

        public ICollection<PurchaseRequest> PurchaseRequests { get; set; } = new List<PurchaseRequest>();
    }
}
