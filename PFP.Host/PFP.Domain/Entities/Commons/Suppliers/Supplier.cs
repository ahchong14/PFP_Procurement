using PFP.Host.PFP.Domain.Enums;
using PFP.Host.PFP.Domain.Interface;

namespace PFP.Host.PFP.Domain.Entities.Commons.Suppliers
{
    public class Supplier : IEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the supplier.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the name of the supplier.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the email address of the supplier.
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// Gets or sets the contact information for the supplier.
        /// </summary>
        public string? Contact { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the supplier is included in auto-counting.
        /// </summary>
        public bool InAutoCount { get; set; }

        /// <summary>
        /// Gets or sets the account status of the supplier.
        /// </summary>
        public SupplierAccountStatus AccountStatus { get; set; } = SupplierAccountStatus.Invited;
        /// <summary>
        /// Gets or sets the registration token for the supplier.
        /// </summary>
        public string? RegistrationToken { get; set; }
        /// <summary>
        /// Gets or sets the date and time when the supplier registered.
        /// </summary>
        public DateTime? RegisteredAt { get; set; }
        /// <summary>
        /// Gets or sets the hashed password for the supplier's account.
        /// </summary>
        public string? PasswordHash { get; set; }


    }
}
