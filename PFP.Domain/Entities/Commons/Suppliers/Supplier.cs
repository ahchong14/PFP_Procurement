using PFP.Domain.Entities.PurchaseOrders;
using PFP.Domain.Entities.RequestQuotations;
using PFP.Domain.Enums;
using PFP.Domain.Interface;

namespace PFP.Domain.Entities.Commons.Suppliers
{
    public class Supplier : IEntity, IExposableEntity
    {
        /// <summary>
        /// Unique identifier for the supplier.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Name of the supplier.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Email address of the supplier.
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// Contact information for the supplier.
        /// </summary>
        public string? Contact { get; set; }

        /// <summary>
        /// Value indicating whether the supplier is included in auto-counting.
        /// </summary>
        public bool InAutoCount { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public string? CreditorCode { get; set; }

        /// <summary>
        /// The account status of the supplier.
        /// </summary>
        public SupplierAccountStatus AccountStatus { get; set; } = SupplierAccountStatus.Invited;
        /// <summary>
        /// The registration token for the supplier.
        /// </summary>
        public string? RegistrationToken { get; set; }
        /// <summary>
        /// The date and time when the supplier registered.
        /// </summary>
        public DateTime? RegisteredAt { get; set; }
        /// <summary>
        /// The hashed password for the supplier's account.
        /// </summary>
        public string? PasswordHash { get; set; }

        public ICollection<RequestQuotation> RequestQuotations { get; set; } = [];

        public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = [];
    }
}
