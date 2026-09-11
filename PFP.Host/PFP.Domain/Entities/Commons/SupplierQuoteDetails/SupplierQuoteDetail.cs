using PFP.Host.PFP.Domain.Entities.Commons.PurchaseRequests;
using PFP.Host.PFP.Domain.Entities.Commons.SupplierQuoteCopys;
using PFP.Host.PFP.Domain.Interface;

namespace PFP.Host.PFP.Domain.Entities.Commons.SupplierQuoteDetails
{
    public class SupplierQuoteDetail : IBaseEntity, IExposableEntity, IBaseExposableEntity
    {
        public int Id { get; set; }

        public int SupplierQuoteCopyId { get; set; } = 0;

        public required SupplierQuoteCopy supplierQuoteCopy { get; set; } = default!;

        public int PurchaseRequestItemId { get; set; } = 0;

        public required PurchaseRequest purchaseRequest { get; set; } = default!;

        public decimal UnitPrice { get; set; } = 0;
    }
}
