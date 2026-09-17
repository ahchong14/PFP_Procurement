using PFP.Domain.Entities.PurchaseRequests;
using PFP.Domain.Entities.RequestQuotations;
using PFP.Domain.Enums;
using PFP.Domain.Interface;

namespace PFP.Domain.Entities.Commons.Users
{
    public class User : IEntity, IExposableEntity
    {
        /// <summary>
        /// Unique identifier for the user.
        /// </summary>
        public int Id { get; set; }
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
        /// Department of the user. Only meaningful when Role is Requester.
        /// </summary>
        public required string Department { get; set; }
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
        public ICollection<PurchaseRequest> PurchaseRequests { get; set; } = [];

        public ICollection<RQApproval> RQApprovals { get; set; } = [];
    }
}
