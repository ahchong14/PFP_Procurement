using PFP.Host.PFP.Domain.Entities.Commons.Suppliers;
using PFP.Host.PFP.Domain.Entities.PurchaseRequests;
using PFP.Host.PFP.Domain.Enums;
using PFP.Host.PFP.Domain.Interface;

namespace PFP.Host.PFP.Domain.Entities.SupplierQuoteCopys
{
    public class SupplierQuoteCopy : IEntity, IExposableEntity
    {
        /// <summary>
        /// The unique identifier for the SupplierQuoteCopy entity.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The identifier of the associated PurchaseRequest entity.
        /// </summary>
        public required int PurchaseRequestId { get; set; } = 0;
        /// <summary>
        /// The associated PurchaseRequest entity.
        /// </summary>
        public required PurchaseRequest PurchaseRequest { get; set; } = default!;
        /// <summary>
        /// The identifier of the associated Supplier entity.
        /// </summary>
        public required int SupplierId { get; set; } = 0;
        /// <summary>
        /// The associated Supplier entity.
        /// </summary>
        public required Supplier Supplier { get; set; } = default!;
        /// <summary>
        /// The unique token associated with the SupplierQuoteCopy entity.
        /// </summary>
        public required string Token { get; set; }
        /// <summary>
        /// The status of the SupplierQuoteCopy entity, indicating its current state in the workflow.
        /// </summary>
        public Copystatus Status { get; set; } = Copystatus.Pending;
        /// <summary>
        /// Any additional remarks or comments related to the SupplierQuoteCopy entity.
        /// </summary>
        public string? Remarks { get; set; }
        /// <summary>
        /// The total amount quoted by the supplier in the SupplierQuoteCopy entity.
        /// </summary>
        public decimal TotalAmount { get; set; } = 0m;
        /// <summary>
        /// The date and time when the SupplierQuoteCopy entity was sent to the supplier.
        /// </summary>
        public DateTime SentAt { get; set; }
        /// <summary>
        /// The date and time when the SupplierQuoteCopy entity was submitted by the supplier.
        /// </summary>
        public DateTime? SubmittedAt { get; set; }
        /// <summary>
        /// Gets or sets the collection of SupplierQuoteDetail entities associated with this SupplierQuoteCopy. This collection represents the details of the supplier's quote, including individual items and their respective prices.
        /// </summary>
        public ICollection<SupplierQuoteDetail> QuotedDetail { get; set; } = [];
    }
}
