using PFP.Host.PFP.Domain.Entities.PurchaseRequests;
using PFP.Host.PFP.Domain.Interface;

namespace PFP.Host.PFP.Domain.Entities.SupplierQuoteCopys
{
    public class SupplierQuoteDetail : IEntity, IExposableEntity
    {
        /// <summary>
        /// Unique identifier for the SupplierQuoteDetail entity.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The identifier of the associated SupplierQuoteCopy entity.
        /// </summary>
        public int SupplierQuoteCopyId { get; set; }
        /// <summary>
        /// The associated SupplierQuoteCopy entity.
        /// </summary>
        public required SupplierQuoteCopy supplierQuoteCopy { get; set; } = default!;
        /// <summary>   
        /// The identifier of the associated PurchaseRequestItem entity.
        /// </summary>
        public int PurchaseRequestItemId { get; set; }
        /// <summary>
        /// The associated PurchaseRequest entity.
        /// </summary>
        public required PurchaseRequest purchaseRequest { get; set; } = default!;
        /// <summary>
        /// The unit price of the item in the supplier quote.
        /// </summary>
        public decimal UnitPrice { get; set; }
    }
}
