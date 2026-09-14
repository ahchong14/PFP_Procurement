using PFP.Host.PFP.Domain.Entities.PurchaseRequests;
using PFP.Host.PFP.Domain.Entities.RequestQuotations;
using PFP.Host.PFP.Domain.Enums;
using PFP.Host.PFP.Domain.Interface;
// TODO: add the using for the namespace that contains PurchaseRequest, for example:
// using PFP.Host.PFP.Domain.Entities.Commons.PurchaseRequests;

namespace PFP.Host.PFP.Domain.Entities.Commons.Users
{
    public class User : IEntity, IExposableEntity
    {
        /// <summary>
        /// Unique identifier for the user.
        /// </summary>
        public required int Id { get; set; }
        /// <summary>
        /// Name of the user.
        /// </summary>
        public required string Name { get; set; }
        /// <summary>
        /// Email address of the user.
        /// </summary>
        public required string Email { get; set; }
        /// <summary>
        /// Role of the user.
        /// </summary>
        public Role Role { get; set; }
        /// <summary>
        /// Department of the user.   
        /// </summary>
        public Department? Department { get; set; } = null;
        /// <summary>
        /// Value indicating whether the user is active or not.
        /// </summary>
        public bool IsActive { get; set; } = true;
        /// <summary>
        /// Hashed password for the user.
        /// </summary>
        public required string PasswordHash { get; set; }
        /// <summary>
        /// The collection of purchase requests associated with the user.
        /// </summary>
        public ICollection<PurchaseRequest> PurchaseRequests { get; set; } = new List<PurchaseRequest>();

        public ICollection<RQApproval> RQApprovals { get; set; } = [];
    }
}
