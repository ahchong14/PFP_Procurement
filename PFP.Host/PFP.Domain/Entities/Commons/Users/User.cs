using PFP.Host.PFP.Domain.Entities.Commons.PurchaseRequests;
using PFP.Host.PFP.Domain.Enums;
using PFP.Host.PFP.Domain.Interface;
// TODO: add the using for the namespace that contains PurchaseRequest, for example:
// using PFP.Host.PFP.Domain.Entities.Commons.PurchaseRequests;

namespace PFP.Host.PFP.Domain.Entities.Commons.Users
{
    public class User : IBaseEntity, IBaseExposableEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public required string Id { get; set; }
        /// <summary>
        /// gets or sets the name of the user.
        /// </summary>
        public required string Name { get; set; }
        /// <summary>
        /// gets or sets the email address of the user.
        /// </summary>
        public required string Email { get; set; }
        /// <summary>
        /// gets or sets the role of the user.
        /// </summary>
        public Role Role { get; set; }
        /// <summary>
        ///  gets or sets the department of the user.   
        /// </summary>
        public Department? Department { get; set; } = null;
        /// <summary>
        /// gets or sets a value indicating whether the user is active or not.
        /// </summary>
        public bool IsActive { get; set; } = true;
        /// <summary>
        /// gets or sets the hashed password for the user.
        /// </summary>
        public required string PasswordHash { get; set; }
        /// <summary>
        /// gets or sets the collection of purchase requests associated with the user.
        /// </summary>
        public ICollection<PurchaseRequest> PurchaseRequests { get; set; } = new List<PurchaseRequest>();
    }
}
