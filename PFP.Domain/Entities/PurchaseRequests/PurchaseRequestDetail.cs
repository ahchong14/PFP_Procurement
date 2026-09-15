using PFP.Domain.Entities.SupplierQuoteCopys;

namespace PFP.Domain.Entities.PurchaseRequests
{
    public class PurchaseRequestDetail
    {
        public int Id { get; set; }

        public required int PurchaseRequestId { get; set; }

        public required PurchaseRequest PurchaseRequest { get; set; } = default!;

        public required string ItemCode { get; set; }

        public string? Description { get; set; }

        public string? Location { get; set; }

        public required string Uom { get; set; }

        public decimal Qty { get; set; } = 0;

        public required ICollection<SupplierQuoteDetail> QuotedBy { get; set; } = [];
    }
}
