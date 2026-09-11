using PFP.Host.PFP.Domain.Entities.Commons.PurchaseRequests;
using PFP.Host.PFP.Domain.Entities.Commons.SupplierQuoteDetails;
using PFP.Host.PFP.Domain.Interface;

namespace PFP.Host.PFP.Domain.Entities.Commons.PurchaseRequestDetails
{
    public class PurchaseRequestDetail : IBaseEntity
    {
        public int Id { get; set; }

        public required int PurchaseRequestId { get; set; }

        public required PurchaseRequest PurchaseRequest { get; set; } = default!;

        public string ItemCode { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string? Location { get; set; }

        public required string Uom { get; set; }

        public decimal Qty { get; set; } = 0;

        public required ICollection<SupplierQuoteDetail> supplierQuoteDetails { get; set; } = new List<SupplierQuoteDetail>();
    }
}
